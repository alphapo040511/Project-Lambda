using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class DeathZoneEvent : MonoBehaviour
{
    [Header("카메라 설정")]
    public Camera deathCamera;

    [Header("사망 오디오 연출")]
    public AudioSource deathAudioSource;
    public AudioClip deathClip;
    public AudioClip deathAfterClip;
    public AudioMixerGroup SFX;

    [Header("사망 시 페이드 이미지")]
    public Image deathFadeImage;

    Coroutine deathRoutine;

    private void OnEnable()
    {
        GameEvents.OnChangeGameState += OnChangeState;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= OnChangeState;
    }

    void OnChangeState(GameState gameState)
    {
        if(gameState == GameState.Menu)
        {
            if(deathRoutine != null)
                StopCoroutine(deathRoutine);

            if(Camera.main != null)
                Camera.main.gameObject.SetActive(true);

            if (deathCamera != null)
                deathCamera.gameObject.SetActive(false);

            if (deathFadeImage != null)
                deathFadeImage.color = Color.clear;
        }

        if(gameState == GameState.GameOver)
        {
            PlayerDie();
        }
    }

    public void PlayerDie()
    {
        if(deathRoutine != null)
        {
            Debug.LogWarning("이미 사망 연출이 진행중 입니다.");
            return;
        }

        deathRoutine = StartCoroutine(DeathDirection());
    }

    public IEnumerator FadeIn(float duration)
    {
        float time = 0f;
        Color color = deathFadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, time / duration);
            deathFadeImage.color = color;
            yield return null;
        }
    }
    public IEnumerator FadeOut(float duration)
    {
        float time = 0f;
        Color color = deathFadeImage.color;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, time / duration);
            deathFadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator DeathDirection()
    {
        CoroutineRunner.Instance.Run(FadeIn(0.1f));         // 페이드 인

        deathAudioSource.PlayOneShot(deathClip);            // 음산한 효과음 재생

        yield return new WaitForSeconds(deathClip.length + 1f);     //효과음 끝날때 + n초까지 대기

        Camera.main.gameObject.SetActive(false);             // 메인 카메라 비활성화
        deathCamera.gameObject.SetActive(true);             // 데스존 카메라 활성화

        VolumeManager.Instance.filmGrain.intensity.value = 0.275f;      // 필름그레인 값 증가
        CoroutineRunner.Instance.Run(FadeOut(1f));          // 페이드 아웃
        deathAudioSource.PlayOneShot(deathAfterClip);       // 사망 이후 효과음

        deathRoutine = null;
    }
}
