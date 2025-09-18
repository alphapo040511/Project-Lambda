using UnityEngine;
using UnityEngine.EventSystems;

public class WindowDrag : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public RectTransform window; // 이동할 전체 창
    private Vector2 offset;

    public void OnPointerDown(PointerEventData eventData)
    {
        window.SetAsLastSibling();

        // 클릭 위치와 창 좌측 상단 차이 계산
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window, eventData.position, eventData.pressEventCamera, out offset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            window.parent as RectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            window.localPosition = (Vector3)(localPoint - offset) + Vector3.back;                                       // 배경과 겹치지 않도록 z축 -1
        }
    }
}
