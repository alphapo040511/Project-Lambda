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
            tempDatas[target.UniqueId] = target;
        }

        // 상호작용 오브젝트 불러오기
        Interactable[] interactableTarget = FindObjectsOfType<Interactable>(true);
        foreach (var target in interactableTarget)
        {
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
}
