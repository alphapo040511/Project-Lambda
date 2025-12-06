using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElevatorEndingEvent : MonoBehaviour
{
    [Header("오디오 설정")]
    public AudioSource elevatorEndingAudioSource;
    public AudioClip elevatorMovingClip;
    public float fadeDuration = 3f;

    [Header("연출 소스")]
    public AudioClip direction_noise;
    public AudioClip direction_scary_riser;
    public AudioClip direction1_end;
    public GameObject normalElevator;
    public GameObject glitchElevator1;
    public GameObject glitchElevator2;

    [Header("Event Camera")]
    public CinemachineVirtualCamera eventCamera;

    public void OnTriggerEnter(Collider other)
    {
        StartCoroutine(ElevatorEndingScene());
    }
    public void CompleteAudioPlay()
    {
        elevatorEndingAudioSource.clip = elevatorMovingClip;
        elevatorEndingAudioSource.volume = 0f;
        elevatorEndingAudioSource.loop = true;
        elevatorEndingAudioSource.Play();

        elevatorEndingAudioSource.DOFade(1f, fadeDuration);
    }

    IEnumerator ElevatorEndingScene()
    {
        EventSystem.Instance.StartEvent();      // 이벤트 상태 시작

        // 카메라 전환
        if (eventCamera != null)
            eventCamera.enabled = true;

        CompleteAudioPlay();

        eventCamera.transform.DOShakeRotation(
            duration: 100f,
            strength: new Vector3(0.25f, 0.25f, 0.25f),
            vibrato: 2,
            randomness: 90f
        );

        yield return new WaitForSeconds(5f);


        //노이즈 이미지 알파값 설정
        var noiseImageColor = DirectionManager.Instance.noiseImage.color;
        noiseImageColor.a = 0f;
        DirectionManager.Instance.noiseImage.color = noiseImageColor;

        Sequence noiseSeq = DOTween.Sequence();

        elevatorEndingAudioSource.PlayOneShot(direction_noise);

        //노이즈 이미지 깜빡임
        noiseSeq.Append(DirectionManager.Instance.noiseImage.DOFade(1f, 0.1f))
           .AppendInterval(0.5f)            //0.5초 동안 유지
           .Append(DirectionManager.Instance.noiseImage.DOFade(0f, 0.1f));

        elevatorEndingAudioSource.PlayOneShot(direction_scary_riser);

        //글리치 셰이더, 비네트 on
        DirectionManager.Instance.screenGlitchShader.DOKill();
        DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_NoiseAmount", 10f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_GlitchStrength", 10f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(1f, "_ScanLinesStrength", 10f);

        VolumeManager.Instance.ChangeVignette(0.4f, 6f);

        normalElevator.SetActive(false);
        glitchElevator1.SetActive(true);

        yield return new WaitForSeconds(4f);

        //노이즈 이미지 알파값 설정
        noiseImageColor.a = 0f;
        DirectionManager.Instance.noiseImage.color = noiseImageColor;

        elevatorEndingAudioSource.PlayOneShot(direction_noise);

        //노이즈 이미지 깜빡임
        noiseSeq.Append(DirectionManager.Instance.noiseImage.DOFade(1f, 0.1f))
           .AppendInterval(0.5f)            //0.5초 동안 유지
           .Append(DirectionManager.Instance.noiseImage.DOFade(0f, 0.1f));

        eventCamera.transform.DOKill();
        eventCamera.transform.DOShakeRotation(
            duration: 100f,
            strength: new Vector3(1f, 1f, 1f),
            vibrato: 2,
            randomness: 90f
        );

        glitchElevator1.SetActive(false);
        glitchElevator2.SetActive(true);

        yield return new WaitForSeconds(4f);

        DirectionManager.Instance.screenGlitchShader.DOKill();
        eventCamera.transform.DOKill();

        var fadeImageColor = DirectionManager.Instance.fadeImage.color;
        fadeImageColor.a = 1f;
        DirectionManager.Instance.fadeImage.color = fadeImageColor;

        elevatorEndingAudioSource.Stop();

        yield return null;
    }

}
