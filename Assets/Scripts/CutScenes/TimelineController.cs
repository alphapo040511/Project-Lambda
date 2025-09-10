using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string dialogID = "AI_HibernationWake_001";                  // 사용할 대사 ID

    private bool played = false;

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
        played = true;
        EventSystem.Instance.EndEvent();
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (played) return;         // 이미 플레이 된 경우 무시

        Stop();
    }

    private void ChangedGameState(GameState state)
    {
        if (director == null || played) return;                                 // 이미 플레이 된 경우 무시

        if (state == GameState.Paused)
        {
            director.Pause();
        }
        else
        {
            director.Play();
        }
    }

}
