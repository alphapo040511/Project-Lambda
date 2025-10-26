using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new QuestData", menuName = "Quest/new QusetData")]
public class QuestDataSO :ScriptableObject
{
    public string questId;
    public string titleKey;
    public string descriptionKey;
    public int targetCount;
}

public class QuestData
{
    public string questId;
    public string titleKey;
    public string descriptionKey;
    public int targetCount;
    public int currentCount;
    public bool IsCompleted => targetCount <= currentCount;

    public QuestData(QuestDataSO dataSO)
    {
        this.questId = dataSO.questId;
        this.titleKey = dataSO.titleKey;
        this.descriptionKey = dataSO.descriptionKey;
        this.targetCount = dataSO.targetCount;
        currentCount = 0;
    }
}
