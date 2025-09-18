using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowBase : MonoBehaviour
{
    [Header("Window Settings")]
    public string windowName;
    public Sprite windowIcon;

    [Header("Compoenet Settings")]
    public RectTransform windowTransform;
    public TextMeshProUGUI nameText;
    public Image iconImage;
    public Button closeButton;
    public float fadeSpeed = 10f;

    private ComputerSystem targetComputer;
    private Coroutine fadeCoroutine;

    public bool isActive { get; private set; } = false;
    private Vector3 originSize;
    private Vector2 lastPos;

    protected virtual void Awake()
    {
        if(windowTransform == null)
            windowTransform = GetComponent<RectTransform>();

        if(closeButton != null )
            closeButton.onClick.AddListener(() => CloseWindow());
    }



    protected virtual void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(() => CloseWindow());
    }

    public void Initialize(ComputerSystem pc)
    {
        targetComputer = pc;

        // 탭 크기 설정
        originSize = windowTransform.localScale;                        // 초기 크기 저장
        windowTransform.localScale = Vector3.zero;                      // 최소화

        lastPos = windowTransform.anchoredPosition;                     // 초기 위치 저장
        windowTransform.anchoredPosition = new Vector2(lastPos.x, 0);   // 하단으로 이동

        if (nameText != null)
            nameText.text = windowName;                                 // 이름 표시 설정

        if (iconImage != null)
            iconImage.sprite = windowIcon;                              // 아이콘 표시 설정
    }

    #region Window
    // 창 열기
    public virtual void OpenWindow()
    {
        if (windowTransform == null || isActive) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ScaleChange(true));
    }


    // 창 닫기
    public virtual void CloseWindow()
    {
        if (windowTransform == null || !isActive) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ScaleChange(false));
    }

    #endregion

    // 캔버스 크기 변화 연출
    private IEnumerator ScaleChange(bool scaleIn)
    {
        // 크기 조절
        Vector3 startSize = scaleIn ? Vector3.zero : originSize;
        Vector3 endSize = scaleIn ? originSize : Vector3.zero;

        windowTransform.transform.localScale = startSize;                          // 스케일 초기화

        // 위치 조절
        Vector2 startPos = windowTransform.anchoredPosition;
        Vector2 endPos = scaleIn ? lastPos : new Vector2(startPos.x, 0);

        if(!scaleIn)
            lastPos = startPos;             // 화면 축소의 경우 마지막 위치 기억


        float t = 0;

        while(t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            windowTransform.localScale = Vector3.Lerp(startSize, endSize, t);

            windowTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        windowTransform.transform.localScale = endSize;
        windowTransform.anchoredPosition = endPos;

        isActive = scaleIn;
    }
}
