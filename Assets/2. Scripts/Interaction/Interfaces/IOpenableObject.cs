using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOpenableObject
{
    public void Move();
    public bool IsMoving { get; }
}
