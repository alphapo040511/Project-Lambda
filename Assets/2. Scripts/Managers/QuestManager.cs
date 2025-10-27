using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : SingletonMonoBehaviour<QuestManager>
{
    [Header("Quest List")]
    public List<QuestDataSO> questDatas = new List<QuestDataSO>();
    private Dictionary<string, QuestData> questList = new Dictionary<string, QuestData>();

    [Header("UI References")]
    public QuestView view;

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

    private string currentQuestId;

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;                  // this(이 객체)를 T 형식으로 변환
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        QuestIndexing();
        // 세이브 파일이 있는 경우 퀘스트 개수 정리
    }

    private void Start()
    {
        RegistQuest("Quest1");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            RegistQuest("Quest2");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ProgressQuest("Quest2");
        }
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

    public void RegistQuest(string questId)
    {
        UIManager.Instance.ShowOverlay(OverlayType.Quest);

        if (!questList.ContainsKey(questId))
        {
            Debug.LogError($"QuestId에 해당하는 퀘스트가 없습니다. (QuestID : {questId})");
            return;
        }

        currentQuestId = questId;
        onRegistQuest?.Invoke(currentQuestId);
        view.Show(questList[currentQuestId].titleKey, questList[currentQuestId].descriptionKey);

        if (questList[currentQuestId].targetCount > 1)
            view.UpdateProgress(questList[currentQuestId].targetCount, questList[currentQuestId].currentCount);
    }

    public void ProgressQuest(string questId, int amount = 1)
    {
        if (currentQuestId != questId || !questList.ContainsKey(questId)) return;

        questList[questId].currentCount += amount;

        view.UpdateProgress(questList[questId].targetCount, questList[questId].currentCount);

        if(questList[questId].IsCompleted)
        {
            Complete();
        }
    }

    void Complete()
    {
        onCompleteQuest?.Invoke(currentQuestId);
        currentQuestId = null;

        view.Hide();
    }
}
