using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Valve : Interactable
{
    public float releaseDelay = 2f;             // 밸브가 다시 풀리기까지 대기 시간

    private float timeRemaining;

    public override void OnInteractEnd()
    {
        base.OnInteractEnd();
        timeRemaining = releaseDelay;
        used = false;                           // 재사용이 가능하도록
    }

    protected override void ActorUpdate()
    {
        if (interacting) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
        }
        else if(currentHoldTime > 0f)
        {
            currentHoldTime -= Time.deltaTime;
            currentHoldTime = Mathf.Max(currentHoldTime, 0f);
        }

        float progress = Mathf.Clamp01(currentHoldTime / interactionHoldTime);          // 0 ~ 1 값으로 설정

        if(targetUI != null)
            targetUI.OnInteractHold(progress);                                          // 진행도 표시 활성화
    }

    protected override void Complete()
    {
        base.Complete();
        ToastMessageSystem.Instance.EnqueueMessage(new ToastMessege("밸브 개방이 완료 되었습니다.", 1.5f));
    }
}
