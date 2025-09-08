using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TempCutSceneController : MonoBehaviour
{
    public PlayableDirector director;

    // Start is called before the first frame update
    void Start()
    {
        director.Play();

        DialogueManager.Instance.PlayingDialog("AI_HibernationWake_001");
    }

    private void OnEnable()
    {
        GameEvents.OnChangeGameState += ChangedGameState;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= ChangedGameState;
    }

    private void ChangedGameState(GameState state)
    {
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
