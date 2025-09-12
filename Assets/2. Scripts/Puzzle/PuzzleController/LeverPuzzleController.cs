using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LeverTarget
{
    public Lever lever;                                                 // 타겟이 되는 레버
    [Tooltip("해당 레버가 퍼즐 완료를 위해 켜져 있어야 하는지")]
    public bool shouldBeOn;                                             // 해당 레버가 퍼즐 완료를 위해 켜져 있어야 하는지
}

public class LeverPuzzleController : MonoBehaviour
{
    [Tooltip("사용할 레버 정보")]
    public List<LeverTarget> targets;
    [Tooltip("퍼즐 해결 시 동작할 대상")]
    public List<InteractionReceiver> interactionReceivers = new List<InteractionReceiver>();

    void Start()
    {
        foreach (var target in targets)
        {
            target.lever.OnLeverToggled += CheckPuzzle;
        }
    }

    void OnDestroy()
    {
        foreach (var target in targets)
        {
            if (target.lever != null)
                target.lever.OnLeverToggled -= CheckPuzzle;
        }
    }

    public void CheckPuzzle()
    {
        foreach(var target in targets)
        {
            if (target.lever.IsOn != target.shouldBeOn)     // 타겟의 현재 상태와 목표 상태가 같은지 비교
            {
                foreach (var receiver in interactionReceivers)
                {
                    receiver.OnInteractionComplete(false);
                }
                return;                                     // 하나라도 다르다면 취소
            }
        }

        // 모두 목표 상태일 경우
        foreach (var receiver in interactionReceivers)
        {
            receiver.OnInteractionComplete(true);
        }

    }
}
