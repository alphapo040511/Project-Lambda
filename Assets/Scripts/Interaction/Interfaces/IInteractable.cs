using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    // 오브젝트 탐색 및 활성화 메서드
    void OnActivate();                          // 발견 범위 진입
    void OnDeactivate();                        // 발견 범위 이탈
    void OnTargeted();                          // 상호작용 가능 범위 진입
    void OnUntargeted();                        // 상호작용 가능 범위 이탈

    // 상호작용 기능 메서드
    void OnInteractStart();                     // 상호작용 토글시 호출
    void OnInteractHold(float deltaTime);       // 홀드 시작시 호출
    void OnInteractEnd();                       // 홀드 종료시 호출
}
