using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WordPuzzleActiveEvent : MonoBehaviour
{
    public CinemachineVirtualCamera roomCamera;             // 동면실 카메라
    public CinemachineVirtualCamera computerCamera;         // 컴퓨터 카메라
    public UnityEvent onPCEvent;

    public void StartEvent()
    {
        if (roomCamera == null || computerCamera == null) return;

        StartCoroutine(EventCoroutine());
    }

    IEnumerator EventCoroutine()
    {
        EventSystem.Instance.StartEvent();

        DirectionManager.Instance.EnterDangerZone();
        roomCamera.enabled = true;

        yield return new WaitForSeconds(0.1f);
        DirectionManager.Instance.ExitDangerZone();

        yield return new WaitForSeconds(3f);

        roomCamera.enabled = false;
        computerCamera.enabled = true;

        yield return new WaitForSeconds(1.5f);

        onPCEvent?.Invoke();

        yield return new WaitForSeconds(3f);

        DirectionManager.Instance.EnterDangerZone();
        computerCamera.enabled = false;

        yield return new WaitForSeconds(0.1f);
        DirectionManager.Instance.ExitDangerZone();

        EventSystem.Instance.EndEvent();
    }
}
