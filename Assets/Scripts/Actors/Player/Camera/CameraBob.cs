using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//카메라 흔들림, 호흡 연출
public class CameraBob : Actor
{
    public PlayerController playerController;

    [Header("Idle Bob Settings")]
    public float idleAmplitude = 0.03f;         // 흔들림 정도
    public float idleFrequency = 1.0f;          // 흔들림 간격

    [Header("Walk Bob Settings")]
    public float walkAmplitude = 0.05f;         // 흔들림 정도
    public float walkFrequency = 1.0f;          // 흔들림 간격

    [Header("Run Bob Settings")]
    public float runAmplitude = 0.05f;          // 흔들림 정도
    public float runFrequency = 0.75f;          // 흔들림 간격

    [Header("Blend Settings")]
    public float smoothSpeed = 5f;              // 상태 전환 강도

    private Vector3 initialPos;                 // 초기 위치
    private float timer = 0f;
    private float targetAmplitude;
    private float targetFrequency;
    

    private void Awake()
    {
        if(playerController != null )
        {
            playerController.OnMoveStateChanged += HandleMoveStateChanged;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnMoveStateChanged -= HandleMoveStateChanged;
        }
    }

    void Start()
    {
        initialPos = transform.localPosition;
    }

    protected override void ActorUpdate() 
    {
        timer += Time.deltaTime / targetFrequency;

        // 흔들림 계산 (삼각 함수 사용으로 진동) 
        float bobOffsetY = Mathf.Sin(timer) * targetAmplitude;                                 // 상하 움직임
        float bobOffsetX = Mathf.Cos(timer * 0.5f) * targetAmplitude * 0.3f;                   // 좌우 움직임 (약하게)

        // 최종 위치 적용
        Vector3 targetPos = initialPos + new Vector3(bobOffsetX, bobOffsetY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothSpeed);
    }

    // 플레이어의 상태가 변경 되면 목표 흔들림 강도 변경
    private void HandleMoveStateChanged(MoveState newState)
    {
        switch (newState)
        {
            case MoveState.Idle:
            case MoveState.Interacting:
                targetAmplitude = idleAmplitude;
                targetFrequency = idleFrequency;
                break;
            case MoveState.Walking:
                targetAmplitude = walkAmplitude;
                targetFrequency = walkFrequency;
                break;
            case MoveState.Running:
                targetAmplitude = runAmplitude;
                targetFrequency = runFrequency;
                break;
        }
    }
}
