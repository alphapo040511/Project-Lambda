using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenableBase : MonoBehaviour
{
    public virtual void Move()
    {

    }
    public bool isMoving { get; protected set; } = true;
}
