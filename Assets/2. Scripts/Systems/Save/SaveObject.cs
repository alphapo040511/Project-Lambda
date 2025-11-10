using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System;

public class SaveObject : MonoBehaviour, ISaveObject
{
    protected string uniqueId;
    public string UniqueId => uniqueId;

    protected ObjectState state = ObjectState.Default;
    public ObjectState State => state;

    public StateEvents stateEvents;

    [System.Serializable]
    public class StateEvents
    {
        public UnityEvent defaultState;
        public UnityEvent offState;
        public UnityEvent onState;
        public UnityEvent usedState;
        public UnityEvent disableState;
    }

    // 한번에 여러 오브젝트를 관리할 경우 유니티 이벤트를 통해 변경하는것도 가능
    public virtual void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;

        switch(state)
        {
            case ObjectState.Default:
                stateEvents.defaultState?.Invoke();
                break;
            case ObjectState.Off:
                stateEvents.offState?.Invoke();
                break;
            case ObjectState.On:
                stateEvents.onState?.Invoke();
                break;
            case ObjectState.Used:
                stateEvents.usedState?.Invoke();
                break;
            case ObjectState.Disable:
                stateEvents.disableState?.Invoke();
                break;
        }
    }

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
}
