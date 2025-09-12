using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : Actor
{
    //상태 변화 이벤트 선언
    public event Action<MoveState> OnMoveStateChanged;

    public Posture currentPosture { get; private set; } = Posture.Standing;

    private IPlayerState currentState;

    [Header("Speed Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float acceleration = 3f;
    public float crouchSpeedMultiplier = 0.6f;
    [HideInInspector] public float targetSpeed;

    private float currenSpeed;
    private Vector3 moveDirection;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public FirstPersonCamera cameraController;
    [HideInInspector] public CameraHeightController cameraHeightController;
    [HideInInspector] public InteractionFinder interactionFinder;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cameraController = GetComponentInChildren<FirstPersonCamera>();
        cameraHeightController = GetComponentInChildren<CameraHeightController>();
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
