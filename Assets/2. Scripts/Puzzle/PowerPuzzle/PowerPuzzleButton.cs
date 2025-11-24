using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerPuzzleButton : MonoBehaviour, IPointerClickHandler
{
    public PowerPuzzleController controller;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || !controller.isFocused || !controller.CanPressAnswerButton()) return;        // 포커싱 되어 있지 않거나, 아직 쿨다운 중일때

        controller.AnswerButtonPressed();
    }
}
