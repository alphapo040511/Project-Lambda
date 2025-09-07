using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//[System.Serializable]
//public class ToastMessege
//{
//    public string text { get; private set; }
//    public float duration { get; private set; }

//    public ToastMessege(string text, float duration)
//    {
//        this.text = text;
//        this.duration = duration;
//    }
//}

public class ToastMessageSystem : SingletonMonoBehaviour<ToastMessageSystem>
{
    public ToastMessageView viewPrefab;                 // 초기화를 위한 view prefab (다른 방식으로 변경 가능성 있음)
    private ToastMessageView view;                      // 실사용할 view

    protected override void Awake()
    {
        base.Awake();
        EnsureCanvas();
        InitializeView();
    }

    // 캔버스가 아닐 경우 캔버스로 변경
    private void EnsureCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;        // 비율 유지
            scaler.referenceResolution = new Vector2(1920, 1080);                   // FHD 기준
            scaler.matchWidthOrHeight = 1.0f;                                       // 세로 기준 맞춤

            // GraphicRaycaster 추가 (UI 클릭 가능하게)
            gameObject.AddComponent<GraphicRaycaster>();
        }
    }

    // view 초기화
    private void InitializeView()
    {
        view = Instantiate(viewPrefab, transform);

        RectTransform viewTransform = view.GetComponent<RectTransform>();

        viewTransform.anchorMin = new Vector2(0.5f, 0f);                        // 앵커 기준 위치 설정
        viewTransform.anchorMax = new Vector2(0.5f, 0f);
        viewTransform.pivot = new Vector2(0.5f, 0f);
        viewTransform.anchoredPosition = new Vector2(0f, 100f);

        view.Hide();
    }

    // 새로운 메세지 표시
    public void ShowMessage(string message)
    {
        view.SetText(message);
        view.Show();
    }

    // 메세지 표시 비활성화
    public void ClearMessage()
    {
        view.Hide();
    }

}
