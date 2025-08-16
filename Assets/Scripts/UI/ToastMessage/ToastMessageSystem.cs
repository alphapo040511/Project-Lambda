using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToastMessege
{
    public string text { get; private set; }
    public float duration { get; private set; }

    public ToastMessege(string text, float duration)
    {
        this.text = text;
        this.duration = duration;
    }
}

public class ToastMessageSystem : SingletonMonoBehaviour<ToastMessageSystem>
{
    public ToastMessageView viewPrefab;                 // 초기화를 위한 view prefab (다른 방식으로 변경 가능성 있음)
    private ToastMessageView view;                      // 실사용할 view

    private Queue<ToastMessege> messageQueue = new Queue<ToastMessege> ();           // 대화문을 저장할 큐

    private Coroutine hideRoutine;                      // 표시 시간을 측정할 코루틴

    private bool isShowing = false;                // 현재 대화 표시중인지 확인

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

    // 새로운 메세지 추가
    public void EnqueueMessage(ToastMessege message)
    {
        messageQueue.Enqueue(message);
        if (!isShowing)
        {
            StartCoroutine(ProcessQueue());
        }
    }

    public void ClearMessage()
    {
        StopCoroutine(ProcessQueue());
        messageQueue.Clear();
        isShowing = false;
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();

            view.SetText(message.text);
            view.Show();

            yield return new WaitForSeconds(message.duration);

            view.Hide();
        }

        isShowing = false;
    }
}
