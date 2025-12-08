using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeObject : OpenableBase
{
    public Vector3 closeRotation;
    public Vector3 openRotation;
    private Quaternion targetRotation
    {
        get
        {
            return isOpen ? Quaternion.Euler(openRotation) : Quaternion.Euler(closeRotation);
        }
    }

    public float rotateSpeed = 2f;



    private void Update()
    {
        if(isMoving)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * rotateSpeed);

            if(Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
            {
                transform.localRotation = targetRotation;
                isMoving = false;
            }
        }
    }
}
