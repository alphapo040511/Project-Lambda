using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveState
{
    Idle,
    Walking,
    Running,
    Interacting
}

// 자세 상태 관리
public enum Posture
{
    Standing,               // 서기
    Crouching               // 앉기
}

public interface IPlayerState
{
    MoveState stateType { get; }

    void Enter();               // 상태 시작시
    void Update();              // 매 프레임
    void FixedUpdate();         // 매 프레임
    void Exit();                // 상태 종료시
    void HandleUpdate();        // 입력 처리
}
