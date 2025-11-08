using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 세이브 표시용 메타데이터
[System.Serializable]
public class SaveMetadata
{
    public string thumbnailPath;        // 썸네일 표시
    public string saveLocationName;     // 저장한 장소 이름 표시
    public long saveTime;               // 저장시간
    public float playTime;              // 플레이 시간
    public string currentQuest;         // 현제 퀘스트 이름
}

// 세이브 데이터
[System.Serializable]
public class SaveData
{
    public string saveSceneName;
    public Vector3 savePosition;
    public Quaternion saveRotation;
    public List<QuestSaveData> questDatas;
    public List<InteractionSaveData> interactionDatas;
}

// 퀘스트 상태 저장
[System.Serializable]
public class QuestSaveData
{
    public string questId;
    public int currentProgress;
}


// 상호작용 (또는 퍼즐) 저장 상태
[System.Serializable]
public class InteractionSaveData
{
    // 나중에 구조를 생각해볼게요..ㅠ
    public string interactionId;
    public InteractionState interactionState;
}

public enum InteractionState
{ 
    Default,
    Used,           // 사용된
    Disable         // 비활성화(또는 획득)
}
