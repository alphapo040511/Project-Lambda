using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialStart : SaveObject
{
    public PlayableDirector wakeUpDirector;

    private void Start()
    {
        wakeUpDirector.stopped += OnWakeUpStopped;
    }

    void OnDestroy()
    {
        wakeUpDirector.stopped -= OnWakeUpStopped;
    }

    private void OnWakeUpStopped(PlayableDirector director)
    {
        if (state == ObjectState.Used) return;

        StartCoroutine(ShowTutorial());
        state = ObjectState.Used;
    }


    IEnumerator ShowTutorial()
    {
        yield return new WaitForSeconds(1f);
        TutorialManager.Instance.Show(TutorialType.Move);
        TutorialManager.Instance.Show(TutorialType.Look);

        yield return new WaitForSeconds(3f);
        TutorialManager.Instance.Show(TutorialType.Interaction);
    }

}
