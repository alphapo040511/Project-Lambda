using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string dialogID = "AI_HibernationWake_001";                  // 사용할 대사 ID

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Play), 0.5f);
    }

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnTimelineStopped;

        GameEvents.OnChangeGameState += ChangedGameState;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnTimelineStopped;

        GameEvents.OnChangeGameState -= ChangedGameState;
    }

    public void Play()
    {
        if (director != null)
            director.Play();

        if (dialogID != null)
            DialogueManager.Instance.PlayingDialog(dialogID);

        EventSystem.Instance.StartEvent();
    }

    public void Stop()
    {
        EventSystem.Instance.EndEvent();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        Stop();
    }

    private void ChangedGameState(GameState state)
    {
        if (director == null) return;

        if (state == GameState.Paused || state == GameState.CutscenePause)
        {
            director.Pause();
        }
        else
        {
            director.Play();
        }
    }

}
