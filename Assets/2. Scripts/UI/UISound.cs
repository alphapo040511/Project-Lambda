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

    private void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip != null && audioSource != null)
            audioSource.PlayOneShot(hoverClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameObject.name == "LevelSceneLoad")
        {
            if (startClip != null)
                audioSource.PlayOneShot(startClip);
            return;
        }

        if (clickClip != null)
            audioSource.PlayOneShot(clickClip);
    }

}
