using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 앉기 상태에 따른 변화 관리
public class PlayerCrouchController : Actor
{
    [Header("Camera Settings")]
    public Transform cameraPivot;
    public float standingHeight = 0.8f;
    public float crouchHeight = 0;
    public float smoothSpeed = 5f;

    private float targetHeight;

    [Header("Collider Settings")]
    public CapsuleCollider playerCollider;
    public float crouchSize = 1.4f;
    public float standingSize = 2f;                         // 서 있을 때 사이즈 (2 고정)
    private float targetSize;

    private bool isInterpolating;                           // 보간 진행중인지 확인용

    private void Awake()
    {
        if (playerCollider == null)
            playerCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        targetHeight = standingHeight;
    }

    protected override void ActorUpdate()
    {
        if (!isInterpolating) return;                   // 보간이 완료 되었다면 return;

        if(cameraPivot == null || playerCollider == null)
        {
            Debug.LogWarning("PlayerCrouchController에서 Pivot 또는 Collier를 찾을 수 없습니다.");
            return;
        }

        // 카메라 높이 변경
        Vector3 currentPos = cameraPivot.localPosition;
        Vector3 targetPos = new Vector3(currentPos.x, targetHeight, currentPos.z);

        cameraPivot.localPosition = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * smoothSpeed);


        // 콜라이더 크기 변경
        float currentSize = playerCollider.height;
        playerCollider.height = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * smoothSpeed);

        Vector3 targetCenter = Vector3.up * (targetSize - standingSize) * 0.5f;                                        // 높이 2 기준 -변화량/2 만큼 이동
        playerCollider.center = Vector3.Lerp(playerCollider.center, targetCenter, Time.deltaTime * smoothSpeed);

        if(Vector3.Distance(cameraPivot.localPosition, targetPos) <= 0.01f)
        {
            SnapToTarget();
            isInterpolating = false;
        }
    }

    public void PostureChange(Posture posture)
    {
        isInterpolating = true;
        if (posture == Posture.Crouching)
        {
            targetHeight = crouchHeight;
            targetSize = crouchSize;
        }
        else
        {
            targetHeight = standingHeight;
            targetSize = standingSize;
        }
    }

    // 목표 값으로 스냅
    private void SnapToTarget()
    {
        Vector3 currentPos = cameraPivot.localPosition;
        cameraPivot.localPosition = new Vector3(currentPos.x, targetHeight, currentPos.z);

        playerCollider.height = targetSize;

        playerCollider.center = Vector3.up * (targetSize - standingSize) * 0.5f;
    }
}
