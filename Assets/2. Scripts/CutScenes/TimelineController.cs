using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;


public class TimelineController : MonoBehaviour
{
    public PlayableDirector director;
    public string dialogID = "AI_HibernationWake_001";                  // 사용할 대사 ID

    public UnityEvent onPlayed;

    public bool playOnAwake = false;
    private bool played = false;



    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnTimelineStopped;

        GameEvents.OnChangeGameState += ChangedGameState;
        GameEvents.OnLoadCompleted += LoadCompletedHandler;         // 데이터 로드 완료 이벤트
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnTimelineStopped;

        GameEvents.OnChangeGameState -= ChangedGameState;
        GameEvents.OnLoadCompleted -= LoadCompletedHandler;         // 데이터 로드 완료 이벤트
    }

    void LoadCompletedHandler()
    {
        if (playOnAwake && !played)
            Play();
    }

    public void Play()
    {
        if (director != null)
            director.Play();

        if (dialogID != null)
            DialogueManager.Instance.PlayingDialog(dialogID);

        EventSystem.Instance.StartEvent();
    }

    public void Played()
    {
        Debug.Log("재생 완료");
        played = true;
        director.time = director.duration;
        director.Evaluate();                    // 즉시 반영
    }


    public void Stop()
    {
        Played();
        onPlayed?.Invoke();
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
