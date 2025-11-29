using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

public class QuestManager : SingletonMonoBehaviour<QuestManager>
{
    [Header("Quest List")]
    public List<QuestDataSO> questDatas = new List<QuestDataSO>();
    private Dictionary<string, QuestData> questList = new Dictionary<string, QuestData>();

    [Header("UI References")]
    public QuestView view;

    
    public string currentQuestId;

    private Queue<string> questQueue = new Queue<string>();

    // 이벤트
    public event Action<string> onRegistQuest;                  // 새 퀘스트 등록시 실행
    public event Action<string> onCompleteQuest;                // 퀘스트 완료시 실행

    #region Events
    private void OnEnable()
    {
        GameEvents.OnChangeGameState += HandleStateChange;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= HandleStateChange;
    }


    private void HandleStateChange(GameState state)
    {
        if(state == GameState.Cutscene)
            UIManager.Instance.HideOverlay(OverlayType.Quest);
        else
            UIManager.Instance.ShowOverlay(OverlayType.Quest);
    }
    #endregion


    protected override void Awake()
    {
        base.Awake();
        QuestIndexing();
        // 세이브 파일이 있는 경우 퀘스트 개수 정리
    }

    public void ResetQuest()
    {
        view.Hide();
        QuestIndexing();
    }

    void QuestIndexing()
    {
        questList.Clear();

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

    public void RegistQuest(string questId)
    {
        if (!questList.ContainsKey(questId))
        {
            Debug.LogError($"{questId}에 해당하는 퀘스트가 없습니다.");
            return;
        }

        if (questList[questId].IsCompleted)
        {
            Debug.Log("이미 완료된 퀘스트 입니다.");
            return;
        }

        UIManager.Instance.ShowOverlay(OverlayType.Quest);

        questQueue.Enqueue(questId);                        // 꼬임 방지를 위해 퀘스트를 큐에 저장

        if (questQueue.Count == 0) return;

        currentQuestId = questQueue.Dequeue();

        onRegistQuest?.Invoke(currentQuestId);
        view.Show(questList[currentQuestId].titleKey, questList[currentQuestId].descriptionKey);

        if (questList[currentQuestId].targetCount > 1)
            view.UpdateProgress(questList[currentQuestId].targetCount, questList[currentQuestId].currentCount);

        Debug.Log($"{currentQuestId}를 현재 퀘스트로 등록 했습니다.");
    }

    public void ProgressQuest(string questId, int amount = 1)
    {
        if (currentQuestId != questId || !questList.ContainsKey(questId))
        {
            Debug.LogWarning($"{questId}에 해당하는 퀘스트가 존재하지 않습니다.");
            return;
        }

        questList[questId].currentCount += amount;

        view.UpdateProgress(questList[questId].targetCount, questList[questId].currentCount);

        if(questList[questId].IsCompleted)
        {
            Complete(questId);
        }
    }

    void Complete(string questId)
    {
        Debug.Log($"{currentQuestId} 퀘스트 완료");
        onCompleteQuest?.Invoke(currentQuestId);
    }

    public IEnumerator LoadQuestData(List<QuestSaveData> datas)
    {
        foreach (QuestSaveData data in datas)
        {
            if(questList.ContainsKey(data.questId))
            {
                questList[data.questId].currentCount = data.currentProgress;
            }

            yield return null;
        }
    }

    public List<QuestSaveData> GetQuestDatas()
    {
        List<QuestSaveData> list = new List<QuestSaveData>();

        foreach(var quest in questList.Values)
        {
            QuestSaveData data = new QuestSaveData();
            data.questId = quest.questId;
            data.currentProgress = quest.currentCount;
            list.Add(data);
        }

        return list;
    }

    public string GetQuestTitle(string questId)
    {
        if(questList.ContainsKey(questId))
        {
            return questList[questId].titleKey;
        }

        return null;
    }

    public string GetQuestDescriptionKey(string questId)
    {
        if (questList.ContainsKey(questId))
        {
            return questList[questId].descriptionKey;
        }

        return null;
    }
}
