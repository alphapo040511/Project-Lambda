using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Valve : Interactable
{
    public override void OnInteractStart()
    {
        base.OnInteractStart();

        currentHoldTime = 0f;                           // 임시로 시간 초기화
    }

    public override void OnInteractEnd()
    {
        base.OnInteractEnd();

        currentHoldTime = 0f;
    }
}
