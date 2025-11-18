using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SlideOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform rect;
    Vector2 originalPos;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOAnchorPos(originalPos + new Vector2(40f, 0), 0.5f).SetEase(Ease.OutExpo).From();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOAnchorPos(originalPos, 0.2f);
    }
}
