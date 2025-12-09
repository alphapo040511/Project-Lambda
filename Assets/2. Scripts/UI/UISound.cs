using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip hoverClip;
    [SerializeField]
    private AudioClip clickClip;
    [SerializeField]
    private AudioClip startClip;

    private string hoverClipName = "sfx_UIButton_Hover";
    private string clickClipName = "sfx_UIButton_Click";
    private string startClipName = "sfx_UIButton_Start";

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null)
            SoundManager.Instance.PlaySound(hoverClip.name);
        else
            SoundManager.Instance.PlaySound(hoverClipName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "LevelSceneLoad")
        {
            if (startClip != null)
                SoundManager.Instance.PlaySound(startClip.name);
            else
                SoundManager.Instance.PlaySound(startClipName);
            return;
        }

        if (clickClip != null)
            SoundManager.Instance.PlaySound(clickClip.name);
        else
            SoundManager.Instance.PlaySound(clickClipName);
    }

}
