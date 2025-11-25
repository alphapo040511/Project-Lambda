using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QusetRegistTrigger : MonoBehaviour
{
    public QuestDataSO targetQuest;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            QuestManager.Instance.RegistQuest(targetQuest.questId);
        }
    }
}
