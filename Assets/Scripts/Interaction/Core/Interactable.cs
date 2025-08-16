using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : Actor, IInteractable
{
    [Header("Interact Settings")]
    public float interactionHoldTime = 3f;                          // 상호작용을 위해 누르고 있어야 하는 시간 (초)

    protected float currentHoldTime = 0f;

    // 상호작용이 가능한지 확인용
    public bool interactable { get; private set; } = true;
    protected bool used = false;
    protected bool interacting = false;

    protected InteractionUIView targetUI;

    // 상호작용 범위 진입
    public void OnActivate()
    {
        if(targetUI != null)
        {
            targetUI.ActivateInteractionUI();
        }
        else
        {
            targetUI = InteractionUIManager.Instance.CreatingInteractionUI(this);
            targetUI.ActivateInteractionUI();
        }
    }

    // 상호작용 범위 벗어남
    public void OnDeactivate()
    {
        if (targetUI != null)
        {
            targetUI.gameObject.SetActive(false);
        }
    }

    // 대상으로 지정
    public virtual void OnTargeted()
    {
        if(targetUI != null)
        {
            targetUI.OnSelected();
        }
    }

    // 대상 지정 해제
    public virtual void OnUntargeted()
    {
        if (targetUI != null)
        {
            targetUI.OnDeselected();
        }
    }

    // 상호작용 시작 (키 다운)
    public virtual void OnInteractStart() 
    {
        interacting = true;
    }

    public virtual void OnInteractHold(float deltaTime)
    {
        if (used) return;

        // 상호작용 홀드 진행
        currentHoldTime += deltaTime;

        if(!used && currentHoldTime >= interactionHoldTime)                             // 사용되지 않았고, 상호작용 시간을 충족 하였을 경우
        {
            Complete();
        }

        float progress = Mathf.Clamp01(currentHoldTime / interactionHoldTime);         // 0 ~ 1 값으로 설정
        targetUI.OnInteractHold(progress);
    }

    // 상호작용 종료 (키 업)
    public virtual void OnInteractEnd()
    {
        interacting = false;
    }

    protected virtual void Complete()
    {
        Debug.Log("상호작용 완료");
        used = true;
    }
}
