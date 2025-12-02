using UnityEngine;

public class OpenableObject : Interactable
{
    public GameObject targetObject;
    private IOpenableObject openable;

    private void Start()
    {
        if(targetObject != null)
            openable = targetObject.GetComponent(typeof(IOpenableObject)) as IOpenableObject;

        if (openable == null)
            Debug.Log("없음");
    }

    public override void OnInteractHold(float deltaTime)
    {
        if (openable != null)
        {
           if(!openable.IsMoving)
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

        if (openable != null && !openable.IsMoving)
        {
            openable.Move();
        }
    }
}
