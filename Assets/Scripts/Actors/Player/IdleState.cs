using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerController player;                  // 대상이 되는 플레이어

    public MoveState stateType => MoveState.Idle;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.targetSpeed = 0f;
    }

    public void HandleUpdate() 
    {
        player.GetInput();

        if (player.GetInputDirection().magnitude > 0.1f)
        {
            player.SetState(new WalkState(player));
            return;
        }

        if (Input.GetKey(KeyCode.E) && player.CanInteract())     // E 키다운 & 상호작용이 가능한 경우
        {
            player.SetState(new InteractState(player, player.interactionFinder.GetTarget()));
        }
    }
    public void Update() { }

    public void FixedUpdate() 
    {
        player.Move();
    }

    public void Exit() { }

}

public class WalkState : IPlayerState
{
    private PlayerController player;

    public MoveState stateType => MoveState.Walking;

    public WalkState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.targetSpeed = player.walkSpeed;
    }

    public void HandleUpdate()
    {
        player.GetInput();

        var speed = player.GetInputDirection().magnitude;

        if (speed < 0.1f)
        {
            player.SetState(new IdleState(player));
            return;
        }

        // Shift 누르면 달리기 상태로 전환
        if (Input.GetKey(KeyCode.LeftShift))            // 토글/홀드 등은 추가
        {
            player.SetState(new RunState(player));
            return;
        }

        if(Input.GetKey(KeyCode.E) && player.CanInteract())     // E 키다운 & 상호작용이 가능한 경우
        {
            player.SetState(new InteractState(player, player.interactionFinder.GetTarget()));
        }
    }

    public void Update() { }

    public void FixedUpdate()
    {
        player.Move();
    }

    public void Exit() { }
}

public class RunState : IPlayerState
{
    private PlayerController player;

    public MoveState stateType => MoveState.Running;

    public RunState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.targetSpeed = player.runSpeed;
    }

    public void HandleUpdate()
    {
        player.GetInput();

        var speed = player.GetInputDirection().magnitude;

        if (speed < 0.1f)
        {
            player.SetState(new IdleState(player));
            return;
        }

        // Shift 떼면 걷기 상태로 전환
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            player.SetState(new WalkState(player));
            return;
        }
    }

    public void Update()
    {
        // 스테미너 감소등 달리기 로직
    }

    public void FixedUpdate() 
    {
        player.Move();
    }

    public void Exit() { }
}

public class InteractState : IPlayerState
{
    private PlayerController player;
    private IInteractable interactable;

    public MoveState stateType => MoveState.Interacting;

    public InteractState(PlayerController player, IInteractable interactable)
    {
        this.player = player;
        this.interactable = interactable;
    }

    public void Enter()
    {
        player.targetSpeed = 0f;
        interactable.OnInteractStart();
        player.cameraController.enabled = false;        // 카메라 조작 비활성화
    }

    public void HandleUpdate()
    {
        // 상호작용 키 해제 시 상태 전이
        if(!Input.GetKey(KeyCode.E))
        {
            player.SetState(new IdleState(player));
            return;
        }
    }

    public void Update()
    {
        if (player.CanInteract())
        {
            interactable.OnInteractHold(Time.deltaTime);
        }
        else
        {
            player.SetState(new IdleState(player));
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        interactable.OnInteractEnd();
        player.cameraController.enabled = true;         // 카메라 활성화
    }
}

