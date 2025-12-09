using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerController : Actor
{
    //상태 변화 이벤트 선언
    public event Action<MoveState> OnMoveStateChanged;

    public Posture currentPosture { get; private set; } = Posture.Standing;

    public IPlayerState currentState { get; private set; }

    [Header("Speed Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float acceleration = 3f;
    public float crouchSpeedMultiplier = 0.6f;
    [HideInInspector] public float targetSpeed;

    [Header("Footstep Settings")]
    public AudioSource footstepSource;
    public AudioClip[] footstepClips;

    [Tooltip("최소 발소리 간격 (달릴 때)")]
    public float minStepInterval = 0.25f;

    [Tooltip("최대 발소리 간격 (걸을 때)")]
    public float maxStepInterval = 0.5f;

    private float stepTimer = 0f;

    private float currenSpeed;
    private Vector3 moveDirection;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public FirstPersonCamera cameraController;
    [HideInInspector] public PlayerCrouchController cameraHeightController;
    [HideInInspector] public InteractionFinder interactionFinder;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraController = GetComponentInChildren<FirstPersonCamera>();
        cameraHeightController = GetComponentInChildren<PlayerCrouchController>();
        interactionFinder = GetComponentInChildren<InteractionFinder>();
    }

    void Start()
    {
        SetState(new IdleState(this));
    }

    protected override void ActorUpdate()
    {
        if(currentState != null)
        {
            currentState.HandleUpdate();
            currentState.Update();
            HandlePostureInput();

            // 사운드 추가
            HandleFootsteps();
        }
    }

    protected override void ActorFixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    // 새로운 상태 적용
    public void SetState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();                                // 기존 상태 종료

        currentState = newState;
        currentState.Enter();                                   // 새로운 상태 등록

        if (OnMoveStateChanged != null)
            OnMoveStateChanged.Invoke(newState.stateType);      // 상태 변경 이벤트 호출
    }

    public void Move()
    {
        currenSpeed = Mathf.Lerp(currenSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 velocity = moveDirection * currenSpeed;
        if(currentPosture == Posture.Crouching)
            velocity *= crouchSpeedMultiplier;
        velocity.y = rb.velocity.y;

        if (rb != null)
        {
            rb.velocity = velocity;
        }
    }

    void HandleFootsteps()
    {
        // 실제 이동 속도
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        // 멈춰있으면 카운터 리셋
        if (speed < 0.1f)
        {
            stepTimer = 0f;
            return;
        }

        // 속도를 0~1 비율로 정규화
        float speed01 = Mathf.InverseLerp(0f, runSpeed, speed);

        // 속도에 따라 발소리 간격 변경
        float currentInterval = Mathf.Lerp(maxStepInterval, minStepInterval, speed01);

        stepTimer += Time.deltaTime;

        if (stepTimer >= currentInterval)
        {
            PlayFootstep();
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        if (footstepClips == null || footstepClips.Length == 0) return;
        if (footstepSource == null) return;

        int index = UnityEngine.Random.Range(0, footstepClips.Length);

        footstepSource.pitch = UnityEngine.Random.Range(0.95f, 1.05f); // 살짝 랜덤 피치
        footstepSource.PlayOneShot(footstepClips[index]);
    }

    public bool CanInteract()
    {
        if (interactionFinder != null)
        {
            return interactionFinder.CanInteract();
        }
        else
        {
            Debug.LogWarning("InteractionFinder를 찾을 수 없습니다.");
            return false;
        }
    }

    // 상태가 사용할 입력 받는 함수
    public void GetInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = (transform.forward * v + transform.right * h).normalized;
    }

    public Vector3 GetInputDirection() => moveDirection;

    void HandlePostureInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentPosture == Posture.Standing)
            {
                currentPosture = Posture.Crouching;
                OnCrouch();                                                     // 카메라 등 상태 변경
            }
            else if (currentPosture == Posture.Crouching && CanStandUp())
            {
                currentPosture = Posture.Standing;
                OnStandUp();
            }
        }
    }

    void OnCrouch()
    {
        cameraHeightController.PostureChange(Posture.Crouching);
    }
    
    void OnStandUp()
    {
        cameraHeightController.PostureChange(Posture.Standing);
    }

    bool CanStandUp()
    {
        // 머리 위에 장애물이 없는지 확인하는 코드
        return true;
    }
}
