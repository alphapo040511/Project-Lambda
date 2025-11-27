using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class VolumeManager : SingletonMonoBehaviour<VolumeManager>
{
    public Volume mainVolume;
    [HideInInspector] public DepthOfField dof;
    [HideInInspector] public Vignette vignette;
    [HideInInspector] public FilmGrain filmGrain;

    // 코루틴 저장
    Coroutine focusCor;
    Coroutine vignetteCor;

    // Start is called before the first frame update
    void Start()
    {
        InitVolumeReference();
    }

    void InitVolumeReference()
    {
        if (mainVolume == null)
        {
            Debug.LogWarning("Volume을 할당하지 않아 Volume을 찾습니다.");
            mainVolume = GetComponent<Volume>();
            if(mainVolume == null)
            {
                Debug.LogError("Volume을 할당하지 않았습니다.");
                return;
            }
        }

        if(!mainVolume.profile.TryGet(out dof))
        {
            Debug.LogError("Dof를 찾을 수 없습니다.");
        }

        if (!mainVolume.profile.TryGet(out vignette))
        {
            Debug.LogError("Vignette를 찾을 수 없습니다.");
        }

        if (!mainVolume.profile.TryGet(out filmGrain))
        {
            Debug.LogError("FilmGrain를 찾을 수 없습니다.");
        }
    }

    #region Quick Method

    // 포커싱 거리 변경 메서드
    public void ChangeFocusDistance(float distance, float duration = 0.75f)
    {
        if (dof == null) return;

        if (duration <= 0) dof.focusDistance.value = distance;

        if(focusCor != null)
        {
            StopCoroutine(focusCor);
        }

        focusCor = StartCoroutine(Focusing(distance, duration));
    }

    // 포커싱 거리 전환 보간
    IEnumerator Focusing(float distance, float duration)
    {
        float startDis = dof.focusDistance.value;

        float timer = 0;

        while(timer < 1)
        {
            timer += Time.deltaTime / duration;

            float currentDis = Mathf.Lerp(startDis, distance, timer);

            dof.focusDistance.value = currentDis;

            yield return null;
        }
    }

    // 비네트 강도 변경 메서드
    public void ChangeVignette(float intensity, float duration = 0.75f)
    {
        if (vignette == null) return;

        if (duration <= 0) vignette.intensity.value = intensity;

        if (vignetteCor != null)
        {
            StopCoroutine(vignetteCor);
        }

        vignetteCor = StartCoroutine(VignetteRoutine(intensity, duration));
    }


    // 비네트 강도 전환 보간
    IEnumerator VignetteRoutine(float intensity, float duration)
    {
        float startIntensity = vignette.intensity.value;

        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / duration;

            float currentIntensity = Mathf.Lerp(startIntensity, intensity, timer);

            vignette.intensity.value = currentIntensity;

            yield return null;
        }
    }

    public void Blink(float minIntensity, float maxIntensity, float period = 1f, int repeat = 3)
    {
        if (vignetteCor != null)
        {
            StopCoroutine(vignetteCor);
        }

        vignetteCor = StartCoroutine(BlinkRoutine(minIntensity, maxIntensity, period, repeat));
    }

    IEnumerator BlinkRoutine(float minIntensity, float maxIntensity, float period, int repeat)
    {
        float startIntensity = vignette.intensity.value;
        while(repeat-- > 0)
        {
            yield return StartCoroutine(VignetteRoutine(maxIntensity, period / 2));     // 주기의 절반씩
            yield return StartCoroutine(VignetteRoutine(minIntensity, period / 2));
        }

        yield return StartCoroutine(VignetteRoutine(startIntensity, period));
    }

    #endregion
}
