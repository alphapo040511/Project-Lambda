using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenableObject : Interactable
{
    public GameObject targetObject;
    private IOpenableObject openable;

    private void Start()
    {
        if(targetObject != null)
            openable = targetObject.GetComponent<IOpenableObject>();
    }

    public override void OnInteractHold(float deltaTime)
    {
        if (openable != null && !openable.IsMoving)
            base.OnInteractHold(deltaTime);
    }

    protected override void Complete()
    {
        base.Complete();
        if(openable != null && !openable.IsMoving)
        {
            openable.Move();
        }
    }
}
