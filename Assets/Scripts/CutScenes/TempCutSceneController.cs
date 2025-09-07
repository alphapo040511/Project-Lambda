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

        DialogueManager.Instance.EnqueueDialog("AI_HibernationWake_001");
        DialogueManager.Instance.EnqueueDialog("AI_HibernationWake_002");
        DialogueManager.Instance.EnqueueDialog("AI_HibernationWake_003");
        DialogueManager.Instance.EnqueueDialog("AI_HibernationWake_004");
        DialogueManager.Instance.EnqueueDialog("AI_HibernationWake_005");
    }

}
