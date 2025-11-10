using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : Actor, IInteractable, ISaveObject
{
    [Header("Interact Settings")]
    public float interactionHoldTime = 3f;                          // 상호작용을 위해 누르고 있어야 하는 시간 (초)

    protected float currentHoldTime = 0f;

    // 상호작용이 가능한지 확인용
    public bool interactable = true;
    protected bool used = false;
    protected bool interacting = false;

    public UnityEvent onCompleted;

    [Header("UI Setting")]
    [Tooltip("플로팅 UI를 띄울 위치 Transform (필수 아님)")]
    public Transform floatingUITransform;
    public Vector3 FloatingUIPosition                     // UI에서 받아갈 위치
    {
        get
        {
            if(floatingUITransform != null)
            {
                return floatingUITransform.position;  
            }
            else
            {
                return transform.position;
            }
        }
    }

    protected InteractionUIView targetUI;

    #region 저장 관련 인터페이스 구현

    protected string uniqueId;
    public string UniqueId => uniqueId;

    protected ObjectState state = ObjectState.Default;
    public ObjectState State => state;



    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            // 프리팹 에셋에는 ID 생성 금지
            if (PrefabUtility.IsPartOfPrefabAsset(this))
                return;

            if (string.IsNullOrEmpty(uniqueId))
            {
                uniqueId = Guid.NewGuid().ToString();               // 새로운 아이디 생성
                EditorUtility.SetDirty(this);                       // id 에디터에 반영
            }
        }
    }
    #endif

    public virtual void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;

        Debug.Log($"{gameObject.name} 상태 : {state}로 설정");
    }

    #endregion

    private void OnDestroy()
    {
        if (targetUI != null)
        {
            targetUI.gameObject.SetActive(false);
        }
    }

    #region 상호작용 탐색

    // 상호작용 범위 진입
    public void OnActivate()
    {
        if(targetUI == null)
        {
            targetUI = InteractionUIManager.Instance.CreatingInteractionUI(this);
        }

        if(interactable)
        {
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
        if(targetUI != null && !used && interactable)
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

    #endregion


    #region 상호작용 진행
    // 상호작용 시작 (키 다운)
    public virtual void OnInteractStart() 
    {
        interacting = true;
        currentHoldTime = 0f;
    }

    public virtual void OnInteractHold(float deltaTime)
    {
        if (used || !interactable) return;

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
        currentHoldTime = 0f;


        float progress = Mathf.Clamp01(currentHoldTime / interactionHoldTime);         // 0 ~ 1 값으로 설정
        targetUI.OnInteractHold(progress);
    }

    #endregion

    protected virtual void Complete()
    {
        Debug.Log("상호작용 완료");
        used = true;                    // 기본적으론 1회용
        state = ObjectState.Used;
        currentHoldTime = 0f;
        onCompleted?.Invoke();
    }

    protected virtual void Reset()
    {
        used = false;
        state = ObjectState.Default;
        currentHoldTime = 0f;
    }

    // 상호작용 활성화
    public void EnableInteraction()
    {
        interactable = true;           // 사용 가능 상태로 변경
    }

    // 상호작용 비활성화
    public void DisableInteraction()
    {
        interactable = false;                   // 사용 불가 상태로 변경
        state = ObjectState.Disable;
        OnDeactivate();                         // 상호작용 UI 비활성화
    }


}
