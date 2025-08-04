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

    public void Update()
    {
        if (player.GetInputDirection().magnitude > 0.1f)
        {
            player.SetState(new WalkState(player));
        }
    }


    public void HandleUpdate() { }

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
    }

    public void Update() { }
  

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

    public void Exit() { }
}

