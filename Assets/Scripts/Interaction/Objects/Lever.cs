using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    [Header("레버 설정")]
    public int leverId;                                         // 각 레버 식별용
    [SerializeField] private bool isOn = false;
    public bool IsOn => isOn;                                   // 읽기 전용

    [Header("임시 애니메이션 설정")]
    public Transform leverTransform;                            // 직접적으로 움직일 레버 트랜스폼
    public float rotateSpeed = 2f;                              // 레버 회전 속도
    public Vector3 onEuler;                                     // 켜졌을 때 각도 (로컬 기준)
    public Vector3 offEuler;                                    // 꺼졌을 때 각도

    private float timer = 0f;

    private Quaternion targetRotation
    {
        get
        {
            return Quaternion.Euler(isOn ? onEuler : offEuler);
        }
    }

    public event Action OnLeverToggled;                       // 레버 상태 변경시 호출

    private void Start()
    {
        leverTransform.localRotation = targetRotation;
    }

    protected override void Complete()
    {
        base.Complete();

        isOn = !isOn;                               // 상태 변경
        OnLeverToggled?.Invoke();                   // 이벤트 호출
        Debug.Log("레버 내림");
    }

    protected override void ActorUpdate()
    {
        if (!used) return;                                  // 사용 되지 않았다면 return

        if(leverTransform == null)
        {
            Debug.LogWarning($"[ID:{leverId}] {gameObject.name}의 손잡이가 지정되지 않았습니다.");
            return;
        }

        timer += Time.deltaTime * rotateSpeed;

        // 각도에 맞춰 회전
        leverTransform.localRotation = Quaternion.Slerp(
            Quaternion.Euler(!isOn ? onEuler : offEuler),           // 반대 위치 기준으로 움직임
            targetRotation,
            timer
            );

        if (timer >= 1)                                             // 목표까지 도달하면
        {
            leverTransform.localRotation = targetRotation;
            Reset();                                                // 스냅 및 사용 가능하도록 초기화
        }
    }

    protected override void Reset()
    {
        base.Reset();
        timer = 0f;
    }
}
