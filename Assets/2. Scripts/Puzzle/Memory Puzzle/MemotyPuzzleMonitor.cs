using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MemotyPuzzleMonitor : MonoBehaviour, IPointerClickHandler
{
    public MemoryPuzzleController controller;           // 일단 겹합도가 올라가도 이렇게 할게요 ㅠㅠ
    public int monitorIndex;
    public SpriteRenderer iconRenderer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || !controller.isFocused || !controller.CanInteractMonitor()) return;        // 포커싱 되어 있지 않거나, 모니터 상호작용이 불가능 하면

        // 해당 모니터 선택
    }

    public void SetIcon(Sprite icon)
    {
        iconRenderer.sprite = icon;
        iconRenderer.material.color = Color.white;
    }

    public void SetColor(Color color)
    {
        iconRenderer.color = color;
    }
}
