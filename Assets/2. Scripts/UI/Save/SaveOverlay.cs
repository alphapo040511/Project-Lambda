using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveOverlay : ScreenBase
{
    public CanvasGroup overlayCanvas;
    public float fadeDuration = 1.5f;

    Coroutine fadingRoutine;

    public override void Hide()
    {
        if (fadingRoutine != null)
            StopCoroutine(fadingRoutine);

        fadingRoutine = StartCoroutine(Fading(false));
    }

    public override void Init()
    {
        if (overlayCanvas != null)
        { 
            overlayCanvas.gameObject.SetActive(false);
            overlayCanvas.alpha = 0;
        }
    }

    public override void Show()
    {
        if (fadingRoutine != null)
            StopCoroutine(fadingRoutine);

        fadingRoutine = StartCoroutine(Fading(true));
    }

    IEnumerator Fading(bool on)
    {
        if (overlayCanvas == null) yield break;

        if (on)     // 켜지는 화면 이라면
        overlayCanvas.gameObject.SetActive(true);

        float start = overlayCanvas.alpha;
        float end = on ? 1 : 0;

        float t = 0;
        while(t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            overlayCanvas.alpha = Mathf.Clamp(start, end, t);

            yield return null;
        }

        if (on == false)     // 꺼지는 화면 이라면
            overlayCanvas.gameObject.SetActive(false);

        fadingRoutine = null;
    }
}
