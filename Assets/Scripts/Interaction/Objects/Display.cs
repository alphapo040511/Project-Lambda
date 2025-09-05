using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Display : Interactable
{
    public CinemachineVirtualCamera VirtualCamera;

    protected override void Complete()
    {
        base.Complete();
        Reset();

        GameManager.Instance.ChangeGameState(GameState.Menu);               // 임시로 메뉴로 지정

        if(VirtualCamera != null)
            VirtualCamera.Priority = 11;
    }

    public void ExitDisplay()
    {
        GameManager.Instance.ChangeGameState(GameState.Playing);

        if (VirtualCamera != null)
                VirtualCamera.Priority = 9;
    }
}
