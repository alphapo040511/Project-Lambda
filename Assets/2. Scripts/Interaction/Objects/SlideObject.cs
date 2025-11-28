using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideObject : MonoBehaviour, IOpenableObject
{
    public Vector3 openPosition;
    public Vector3 closePosition;
    private Vector3 targetPosition
    {
        get
        {
            return isOpen ? openPosition : closePosition;
        }
    }

    public float moveSpeed = 3f;

    public bool isOpen = false;

    public bool IsMoving => isMoving;
    private bool isMoving = true;

    private void Update()
    {
        if(IsMoving)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * moveSpeed);
            if(Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                isMoving = false;
                transform.localPosition = targetPosition;
            }
        }
    }

    public void Move()
    {
        isOpen = !isOpen;
        isMoving = true;
    }
}
