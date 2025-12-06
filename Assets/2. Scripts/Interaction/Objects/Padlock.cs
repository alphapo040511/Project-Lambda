using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Events;

public class Padlock : Interactable
{
    public ItemDataSO keyItem;
    public SlideObject body;

    public bool isLocked = true;

    public UnityEvent onFailed;
    public UnityEvent onUnlocked;

    public override void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;
        this.state = state;

        switch (state)
        {
            case ObjectState.Used:          // 일단 사용 완료만 표시
                Unlock();
                break;
        }
    }

    protected override void Complete()
    {
        base.Complete();

        if (keyItem != null && InventoryManager.ContainItem(keyItem.uniqueID))
        {
            Unlock();
        }
        else
        {
            onFailed?.Invoke();
        }
    }

    void Unlock()
    {
        state = ObjectState.Used;       // 사용 완료 상태로 변경

        if (body != null && !body.isOpen)
            body.Move();

        isLocked = false;

        interactable = false;

        onUnlocked?.Invoke();
    }
}
