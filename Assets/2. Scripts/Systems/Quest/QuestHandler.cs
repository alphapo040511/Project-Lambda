using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestHandler : MonoBehaviour
{
    [Header("Quest Data")]
    public QuestDataSO targetQuest;

    [Header("Events")]
    public UnityEvent onActivated;              // 퀘스트가 활성화 될 때
    public UnityEvent onDeactivated;            // 퀘스트가 비활성화 될 때
    public UnityEvent onUsed;                   // 해당 객체를 사용 했을 때
    public UnityEvent onCleared;                // 퀘스트가 클리어 될 때


    private void Awake()
    {
        QuestManager.Instance.onRegistQuest += RegistQuestHandler;
        QuestManager.Instance.onCompleteQuest += CompleteQuestHander;
    }

    private void OnDestroy()
    {
        QuestManager.Instance.onRegistQuest -= RegistQuestHandler;
        QuestManager.Instance.onCompleteQuest -= CompleteQuestHander;
    }

    // 외부에서 퀘스트를 등록할 때 호출
    public void RegistQuest()
    {
        QuestManager.Instance.RegistQuest(targetQuest.questId);
        SoundManager.Instance.PlaySound("sfx_sleepCapsule_denied");     // 퀘스트 등록 사운드
    }

    // 외부에서 퀘스트를 사용할 때 호출
    public void ProgressQuest()
    {
        QuestManager.Instance.ProgressQuest(targetQuest.questId);
        onUsed?.Invoke();       // 사용됨 이벤트 호출
    }


    // 퀘스트 등록 시 호출
    void RegistQuestHandler(string id)
    {
        if (targetQuest == null) return;        // 빈 데이터의 핸들러의 경우 무시

        if (id == targetQuest.questId)          // 해당 퀘스트 라면 활성화
            onActivated?.Invoke();
        else
            onDeactivated?.Invoke();            // 아니라면 비활성화 이벤트 호출
    }

    // 퀘스트 완료시 호출
    void CompleteQuestHander(string id)
    {
        if (targetQuest == null) return;        // 빈 데이터의 핸들러의 경우 무시

        if (id != targetQuest.questId) return;
        onCleared?.Invoke();
    }
}
