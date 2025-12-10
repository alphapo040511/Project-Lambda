using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 단순 페이드만 사용하는 스크린
public class FadeScreen : ScreenBase
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.75f;

    private Coroutine fadeCoroutine;

    private bool isActive = false;

    public override void Init()
    {
        isActive = false;
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        if (!isActive) return;
        isActive = false;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(Fading(false));
    }

    public override void Show()
    {
        if (isActive) return;
        isActive = true;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(Fading(true));
    }

    IEnumerator Fading(bool isFadeIn)
    {
        float t = 0f;
        float start = isFadeIn ? 0f : 1f;
        float end = isFadeIn ? 1f : 0f;

        canvasGroup.alpha = start;

        if (isFadeIn)
        {
            canvasGroup.gameObject.SetActive(true);
        }

        canvasGroup.gameObject.SetActive(true);

        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            float target = Mathf.Lerp(start, end, t);
            canvasGroup.alpha = target;
            yield return null;
        }

        if (!isFadeIn)
        {
            canvasGroup.gameObject.SetActive(false);
        }

        fadeCoroutine = null;
    }
}
