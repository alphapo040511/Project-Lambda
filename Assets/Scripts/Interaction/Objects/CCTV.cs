using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : Interactable
{
    public List<GameObject> hologramObjects = new List<GameObject>();

    public float releaseDelay = 10f;                                         // 홀로그램 표시 시간

    private float timeRemaining;


    protected override void ActorUpdate()
    {
        if (interacting) return;

        if (timeRemaining > 0f)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                Reset();
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
        timeRemaining = releaseDelay;

        DialogueManager.Instance.StopAllDialog();
        DialogueManager.Instance.EnqueueDialog("AI_Hologram_Reconstruct");

        foreach (var obj in hologramObjects)
        {
            obj.gameObject.SetActive(true);
        }
    }

    protected override void Reset()
    {
        base.Reset();

        foreach (var obj in hologramObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
