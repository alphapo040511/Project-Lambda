using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LetterBoxOverlay : MonoBehaviour, IScreen
{
    public CanvasGroup canvasGroup;
    public RectTransform topLetterbox;
    public RectTransform bottomLetterbox;

    public float fadeDuration = 1.5f;

    private Coroutine fadeCoroutine;

    public void Show()
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ShowLetterBox());
    }

    public void Hide()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(HideLetterBox());
    }

    public void Init()
    {
        topLetterbox.anchoredPosition = new Vector2(0, 668.5f);
        bottomLetterbox.anchoredPosition = new Vector2(0, -668.5f);
        canvasGroup.gameObject.SetActive(false);
    }

    private IEnumerator ShowLetterBox()
    {
        float t = 0f;
        Vector2 start = topLetterbox.anchoredPosition;
        Vector2 end = new Vector2(0, 540f);


        canvasGroup.gameObject.SetActive(true);

        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            Vector2 target = Vector2.Lerp(start, end, t);

            topLetterbox.anchoredPosition = target;
            bottomLetterbox.anchoredPosition = -target;

            yield return null;
        }
    }

    private IEnumerator HideLetterBox()
    {
        float t = 0f;
        Vector2 start = topLetterbox.anchoredPosition;
        Vector2 end = new Vector2(0, 668.5f);

        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            Vector2 target = Vector2.Lerp(start, end, t);

            topLetterbox.anchoredPosition = target;
            bottomLetterbox.anchoredPosition = -target;

            yield return null;
        }

        canvasGroup.gameObject.SetActive(false);
    }
}
