using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class QusetRegistTrigger : SaveObject
{
    public QuestDataSO targetQuest;
    [Header("재사용 여부")] public bool reuseable = false;

    public override void SetObjectState(string id, ObjectState state)
    {
        if( id ==  uniqueId)
        {
            this.state = state;
            if(state == ObjectState.Disable)
            {
                this.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && State == ObjectState.Default)
        {
            QuestManager.Instance.RegistQuest(targetQuest.questId);

            if(!reuseable)
            {
                state = ObjectState.Disable;
                this.enabled = false;
            }
        }
    }
}
