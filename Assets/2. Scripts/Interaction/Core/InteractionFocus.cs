using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class InteractionFocus : Interactable
{
    [Header("Focus Settings")]
    public CinemachineVirtualCamera VirtualCamera;

    public bool isFocused { get; private set; } = false;

    private void Start()
    {
        if (VirtualCamera != null)
        {
            VirtualCamera.Priority = 20;
            VirtualCamera.enabled = false;
        }
    }

    protected override void ActorUpdate()
    {
        if(isFocused && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitFocus();
        }
    }

    protected override void Complete()
    {
        base.Complete();
        EnterFocus();
    }

    protected virtual void EnterFocus()
    {
        isFocused = true;

        GameManager.Instance.ChangeGameState(GameState.Focus);

        if (VirtualCamera != null)
            VirtualCamera.enabled = true;
    }

    public virtual void ExitFocus()
    {
        Reset();

        isFocused = false;
        
        GameManager.Instance.ChangeGameState(GameState.Playing);

        if (VirtualCamera != null)
            VirtualCamera.enabled = false;

        state = ObjectState.Off;
    }


}
