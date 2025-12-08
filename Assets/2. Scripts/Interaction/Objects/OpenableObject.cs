using UnityEngine;

public class OpenableObject : Interactable
{
    public OpenableBase openable;

    public override void SetObjectState(string id, ObjectState state)
    {
        this.state = state;
    }

    public override void OnInteractHold(float deltaTime)
    {
        if (openable != null)
        {
           if(!openable.isMoving)
                base.OnInteractHold(deltaTime);
        }
        else
            base.OnInteractHold(deltaTime);
    }

    protected override void Complete()
    {
        base.Complete();

        if (openable != null && !openable.isMoving)
        {
            openable.Move();
        }
    }
}
