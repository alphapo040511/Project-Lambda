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
        if(!body.isOpen)
            body.Move();

        isLocked = false;

        interactable = false;

        onUnlocked?.Invoke();
    }
}
