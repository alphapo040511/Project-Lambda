using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableBase : SaveObject
{
    // 한번에 여러 오브젝트를 관리할 경우 유니티 이벤트를 통해 변경하는것도 가능
    public override void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;
        this.state = state;

        if(state == ObjectState.On)
        {
            Move(true);
        }
        else if(state == ObjectState.Off)
        {
            Move(false);
        }
    }

    public virtual void Move()
    {
        Move(!isOpen);
    }

    public virtual void Move(bool open)
    {
        if (open)
            state = ObjectState.On;
        else
            state = ObjectState.Off;

        isOpen = open;
        isMoving = true;
    }

    public bool isMoving { get; protected set; } = true;

    public bool isOpen { get; protected set; } = false;
}
