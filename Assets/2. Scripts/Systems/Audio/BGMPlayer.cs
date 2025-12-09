using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioClip bgmClip;

    private bool isPlayed = false;

    private void Start()
    {
        GameEvents.OnLoadCompleted += PlayBGM;
    }

    private void OnDestroy()
    {
        StopBGM();
        GameEvents.OnLoadCompleted -= PlayBGM;
    }

    void PlayBGM()
    {
        if (isPlayed && bgmClip == null) return;
        isPlayed = true;

        SoundManager.Instance.PlaySound(bgmClip.name);
    }

    void StopBGM()
    {
        if (!isPlayed && bgmClip == null) return;

        SoundManager.Instance.StopSound(bgmClip.name);      // 재생된 경우 중지
    }
}
