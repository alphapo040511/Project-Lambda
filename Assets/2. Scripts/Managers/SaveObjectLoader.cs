using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveObjectLoader : SingletonMonoBehaviour<SaveObjectLoader>
{
    private SaveData targetSave;
    private Dictionary<string, ISaveObject> saveObjs = new Dictionary<string, ISaveObject>();

    private void OnEnable()
    {
        GameEvents.OnSceneChanged += SceneChangeHandler;
    }

    private void OnDisable()
    {
        GameEvents.OnSceneChanged -= SceneChangeHandler;
    }

    void SceneChangeHandler(string sceneName)
    {
        if (targetSave == null || targetSave.saveSceneName != sceneName) return;      // 타겟이 없거나(기본값 또는 저장 X) 잘못된 씬 인 경우 로딩 안함
        StartCoroutine(ObjectInitialize());
    }

    IEnumerator ObjectInitialize()
    {
        yield return new WaitForSeconds(0.1f);      // 씬 안정화

        saveObjs = FindAllSaveObj();

        // 세이브 내용 적용
        int total = targetSave.objectDatas.Count;
        int count = 0;
        foreach(var data in targetSave.objectDatas)
        {
            if(saveObjs.ContainsKey(data.uniqueId))
            {
                saveObjs[data.uniqueId].SetObjectState(data.uniqueId, data.objectState);
            }
            count++;
            Debug.Log($"오브젝트 데이터 로드 ({count}/{total})");

            // 10개마다 잠시 쉬기
            if (count % 10 == 0)
                yield return null;
        }

        
        PlayerController player = FindObjectOfType<PlayerController>(true);
        if (player != null)
        {
            player.transform.localPosition = targetSave.savePosition;
            player.transform.localRotation = targetSave.saveRotation;
            Debug.Log("플레이어 위치 설정 완료");
        }

        yield return StartCoroutine(QuestManager.Instance.LoadQuestData(targetSave.questDatas));                // 저장된 퀘스트 정보를 불러오기
        QuestManager.Instance.RegistQuest(targetSave.questId);
        Debug.Log("퀘스트 설정 완료");

        GameEvents.LoadCompleted();

        yield return new WaitForSeconds(0.1f);      // 안정화

        targetSave = null;
    }

    Dictionary<string, ISaveObject> FindAllSaveObj()
    {
        var tempDatas = new Dictionary<string, ISaveObject>();       // 딕셔너리 초기화

        // 일반 오브젝트들 불러오기
        SaveObject[] targets = FindObjectsOfType<SaveObject>(true);
        foreach (var target in targets)
        {
            if(string.IsNullOrEmpty(target.uniqueId))      // 아이디가 없는경우 넘어감
            {
                Debug.LogWarning($"{target.gameObject.name}의 Unique ID가 설정되지 않았습니다.");
                continue;
            }

            tempDatas[target.UniqueId] = target;
        }

        // 상호작용 오브젝트 불러오기
        Interactable[] interactableTarget = FindObjectsOfType<Interactable>(true);
        foreach (var target in interactableTarget)
        {
            if (string.IsNullOrEmpty(target.uniqueId))      // 아이디가 없는경우 넘어감
            {
                Debug.LogWarning($"{target.gameObject.name}의 Unique ID가 설정되지 않았습니다.");
                continue;
            }

            tempDatas[target.UniqueId] = target;
        }

        return tempDatas;
    }

    public void SetSaveData(SaveData save)
    {
        targetSave = save;
    }

    public List<ObjectSaveData> GetObjectDatas()
    {
        var objectSaveData = new List<ObjectSaveData>();
        var tempDatas = FindAllSaveObj();                       // 현재 씬의 모드 오브젝트 반환

        foreach(var data in tempDatas.Values)
        {
            ObjectSaveData objData = new ObjectSaveData();      // id와 state 값을 전달
            objData.uniqueId = data.UniqueId;
            objData.objectState = data.State;

            objectSaveData.Add(objData);
        }

        return objectSaveData;
    }

    public SaveData CreateNewSaveData()
    {
        SaveData save = new SaveData();

        // 씬 저장
        save.saveSceneName = SceneManager.Instance.GetCurrentSceneName();

        // 플레이어 위치 저장
        PlayerController player = FindObjectOfType<PlayerController>(true);
        if (player != null)
        {
            save.savePosition = player.transform.localPosition;
            save.saveRotation = player.transform.localRotation;
        }

        save.questDatas = QuestManager.Instance.GetQuestDatas();

        // 오브젝트 상태 저장
        save.objectDatas = GetObjectDatas();

        return save;
    }
}
