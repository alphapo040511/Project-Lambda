using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionReceiver : Actor, IInteractionReceiver
{
    public abstract void OnInteractionComplete(bool shouldActivate);
}
