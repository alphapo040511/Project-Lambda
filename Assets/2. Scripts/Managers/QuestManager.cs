using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : SingletonMonoBehaviour<QuestManager>
{
    [Header("Quest List")]
    public List<QuestDataSO> questDatas = new List<QuestDataSO>();
    private Dictionary<string, QuestData> questList = new Dictionary<string, QuestData>();

    // 이벤트
    public event Action<string> onNewQuest;                 // 새 퀘스트 등록시 실행
    public event Action<string> onCompleteQuest;            // 퀘스트 완료시 실행

    private string currentQuestId;

    protected override void Awake()
    {
        base.Awake();
        QuestIndexing();
        // 세이브 파일이 있는 경우 퀘스트 개수 정리
    }

    void QuestIndexing()
    {
        int count = 0;
        foreach(QuestDataSO quest in questDatas)
        {
            if(questList.ContainsKey(quest.questId))
            {
                Debug.LogError($"중복된 QuestId가 있습니다. (QuestID : {quest.questId})");
                continue;
            }

            questList.Add(quest.questId, new QuestData(quest));
            count++;
        }

        Debug.Log($"퀘스트 {count}개 정리 완료");
    }

    public void SetQuest(string questId)
    {
        if (!questList.ContainsKey(questId))
        {
            Debug.LogError($"QuestId에 해당하는 퀘스트가 없습니다. (QuestID : {questId})");
            return;
        }

        currentQuestId = questId;
        onNewQuest?.Invoke(currentQuestId);
    }

}
