using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour, IInteractable
{
    [Header("Interact Settings")]
    public float interactionHoldTime = 3f;                          // 상호작용을 위해 누르고 있어야 하는 시간 (초)

    protected float currentHoldTime = 0f;

    // 상호작용이 가능한지 확인용
    public bool interactable { get; private set; } = true;
    private bool interacting = false;

    private InteractionUIView targetUI;

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

    public virtual void OnInteractStart()
    {
        // 상호작용 시작 (키 다운)
        interacting = true;
    }

    public virtual void OnInteractHold(float deltaTime)
    {
        // 상호작용 홀드 진행
        currentHoldTime += deltaTime;
        float progress = Mathf.Clamp01(currentHoldTime / interactionHoldTime);         // 0 ~ 1 값으로 설정
        targetUI.OnInteractHold(progress);
    }

    public virtual void OnInteractEnd()
    {
        // 상호작용 종료 (키 업)
        interacting = false;
    }
}
