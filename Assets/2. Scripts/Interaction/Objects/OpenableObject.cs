using UnityEngine;

public class OpenableObject : Interactable
{
    public OpenableBase openable;

    private void Start()
    {

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


        if (openable == null)
            Debug.Log("사라짐");

        if (openable != null && !openable.isMoving)
        {
            openable.Move();
        }
    }
}
