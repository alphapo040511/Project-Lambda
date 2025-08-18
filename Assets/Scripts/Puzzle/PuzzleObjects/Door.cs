using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : InteractionReceiver
{
    [Header("문 설정")]
    public Vector3 openPosition;
    public Vector3 closePosition;

    [Header("임시 애니메이션 설정")]
    public float moveSpeed = 2f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    protected override void ActorUpdate()
    {
        if (!isMoving) return;


        if(Vector3.Distance(transform.localPosition, targetPosition) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
        }
        else
        {
            transform.localPosition = targetPosition;
            isMoving = false;
        }
    }

    public void OpenDoor()
    {
        ToastMessageSystem.Instance.EnqueueMessage(new ToastMessege("연구실 입구의 문 잠금 해제, 문이 열립니다.", 2f));
        targetPosition = openPosition;
        isMoving = true;
    }

    public void CloseDoor()
    {
        targetPosition = closePosition;
        isMoving = true;
    }

    public override void OnInteractionComplete(bool shouldActivate)
    {
        if(shouldActivate)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    #region 인스펙터 기능
    [ContextMenu("열림 위치 저장")]
    public void SaveOpenPosition()
    {
        openPosition = transform.localPosition;
    }

    [ContextMenu("닫힘 위치 저장")]
    public void SaveClosePosition()
    {
        closePosition = transform.localPosition;
    }
    #endregion
}
