using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionReceiver
{
    public void OnInteractionComplete(bool shouldActivate);
}
