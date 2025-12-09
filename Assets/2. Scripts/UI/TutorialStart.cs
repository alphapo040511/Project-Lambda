using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TutorialStart : MonoBehaviour
{
    public PlayableDirector wakeUpDirector;

    private void Start()
    {
        wakeUpDirector.stopped += OnWakeUpStopped;
    }

    private void OnWakeUpStopped(PlayableDirector director)
    {
        StartCoroutine(ShowTutorial());
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
