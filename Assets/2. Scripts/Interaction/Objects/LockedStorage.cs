using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedStorage : OpenableObject
{
    public List<Padlock> padlocks = new List<Padlock>();

    public void UnlockPadlock()
    {
        CheckAllPadlock();
    }

    void CheckAllPadlock()
    {
        int unlock = 0;

        for (int i = 0; i < padlocks.Count; i++)
        {
            if (padlocks[i].isLocked == false)
                unlock++;
        }

        if(unlock == padlocks.Count)
        {
            interactable = true;
        }
        else
        {
            interactable = false;
        }
    }
}
