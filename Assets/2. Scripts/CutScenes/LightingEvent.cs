using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingEvent : MonoBehaviour
{
    [Header("조명을 포함한 대상 오브젝트들")]
    public List<GameObject> normalProps = new List<GameObject>();
    public List<GameObject> glitchProps = new List<GameObject>();

    [Header("Event Camera")]
    public CinemachineVirtualCamera eventCamera;

    public void ToggleLight(bool on)
    {
        StartCoroutine(ToggleRoutine(on));
    }

    IEnumerator ToggleRoutine(bool on)
    {
        EventSystem.Instance.StartEvent();      // 이벤트 상태 시작

        // 카메라 전환
        if (eventCamera != null)
            eventCamera.enabled = true;

        yield return new WaitForSeconds(0.5f);

        // 노이즈 및 오브젝트 전환
        DirectionManager.Instance.EnterDangerZone();
        yield return new WaitForSeconds(0.5f);

        ToggleProps(on);

        yield return new WaitForSeconds(1f);

        // 노이즈 및 오브젝트 전환
        DirectionManager.Instance.ExitDangerZone();
        yield return new WaitForSeconds(1.5f);

        // 카메라 전환
        if (eventCamera != null)
            eventCamera.enabled = false;

        // 이벤트 종료
        EventSystem.Instance.EndEvent();
    }

    void ToggleProps(bool on)
    {
        for(int i = 0; i < normalProps.Count; i++)
        {
            normalProps[i].SetActive(on);
        }

        for (int i = 0; i < glitchProps.Count; i++)
        {
            glitchProps[i].SetActive(!on);
        }
    }
}
