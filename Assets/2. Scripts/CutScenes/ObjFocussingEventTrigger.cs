using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ObjFocussingEventTrigger :SaveObject
{
    public UnityEvent onStartAction;
    public UnityEvent onEndAction;

    [Header("Event Settings")]
    public QuestDataSO needQuest;
    public CinemachineVirtualCamera actionCamera;
    public float eventDuration = 3f;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip findSound;

    public override void SetObjectState(string id, ObjectState state)
    {
        if (id != uniqueId) return;
        this.state = state;

        switch(state)
        {
            case ObjectState.Disable:
            break;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (state == ObjectState.Disable) return;
        if (needQuest != null && QuestManager.Instance.currentQuestId != needQuest.questId) return;       // 목표 퀘스트 현재 퀘스트가 아닌경우 무시

        if (actionCamera != null)
        {
            StartCoroutine(EventAction());
        }
        else
        {
            onStartAction?.Invoke();
            onEndAction?.Invoke();
            state = ObjectState.Disable;                // 사용 불가 상태로 변경
        }
    }

    IEnumerator EventAction()
    {
        EventSystem.Instance.StartEvent();
        onStartAction?.Invoke();
        actionCamera.enabled = true;
        
        if(audioSource != null && findSound != null)
        {
            audioSource.PlayOneShot(findSound);
        }

        yield return new WaitForSeconds(eventDuration);

        actionCamera.enabled = false;
        EventSystem.Instance.EndEvent();
        onEndAction?.Invoke();
        state = ObjectState.Disable;                // 사용 불가 상태로 변경
    }
}
