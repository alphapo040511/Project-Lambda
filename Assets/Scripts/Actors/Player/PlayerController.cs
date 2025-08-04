using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Actor
{
    //상태 변화 이벤트 선언
    public event Action<MoveState> OnMoveStateChanged;

    private IPlayerState currentState;

    [Header("Speed Settings")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float targetSpeed;

    private float currenSpeed;
    private Vector3 moveDirection;

    void Start()
    {
        SetState(new IdleState(this));
    }

    protected override void ActorUpdate()
    {
        GetInput();

        if(currentState != null)
        {
            currentState.Update();
            currentState.HandleUpdate();
        }

        Move();
    }

    // 새로운 상태 적용
    public void SetState(IPlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();                                // 기존 상태 종료

        currentState = newState;
        currentState.Enter();                                   // 새로운 상태 등록

        if (OnMoveStateChanged != null)
            OnMoveStateChanged.Invoke(newState.stateType);      // 상태 변경 이벤트 호출
    }

    private void Move()
    {
        currenSpeed = Mathf.Lerp(currenSpeed, targetSpeed, Time.deltaTime);

        Vector3 move = moveDirection * currenSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }

    // 상태가 사용할 입력 받는 함수
    public void GetInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection = (transform.forward * v + transform.right * h).normalized;
    }

    public Vector3 GetInputDirection() => moveDirection;
}
