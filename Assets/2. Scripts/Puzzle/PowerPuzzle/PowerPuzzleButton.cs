using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerPuzzleButton : MonoBehaviour, IPointerClickHandler
{
    public PowerPuzzleController controller;

    [Header("전력 퍼즐 오디오")]
    public AudioSource audioSource;
    public AudioClip buttonClip;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || !controller.isFocused) return;        // 포커싱 되어 있지 않으면 넘기기

        controller.AnswerButtonPressed();
        audioSource.PlayOneShot(buttonClip);
    }
}
