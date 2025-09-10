using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Display : Interactable
{
    public CinemachineVirtualCamera VirtualCamera;

    private void Start()
    {
        if (VirtualCamera != null)
        {
            VirtualCamera.Priority = 20;
            VirtualCamera.enabled = false;
        }
    }

    protected override void Complete()
    {
        base.Complete();
        Reset();

        GameManager.Instance.ChangeGameState(GameState.Display);

        if(VirtualCamera != null)
            VirtualCamera.enabled = true;
    }

    public void ExitDisplay()
    {
        GameManager.Instance.ChangeGameState(GameState.Playing);

        if (VirtualCamera != null)
            VirtualCamera.enabled = false;
    }
}
