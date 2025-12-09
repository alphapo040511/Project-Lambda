using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OperationRoomEvent : MonoBehaviour
{
    [Header("깜빡이 라이트")]
    public GameObject normalLight;
    public GameObject brokenLight;

    [Header("깜빡임 간격")]
    public float minInterval = 1f;
    public float maxInterval = 5f;

    [Header("지직 확률")]
    [Range(0, 100)]
    public int glitchChance = 70;

    [Header("마네킹")]
    public GameObject mannequin;
    public Camera playerCamera;

    [Header("오디오")]
    public AudioSource audioSource;
    public AudioClip noise;
    public AudioClip spooky;

    [Header("수술실 볼륨")]
    public Volume operationVolume;
    public VolumeProfile normalVolume;

    private CullingGroup cullingGroup;
    private BoundingSphere[] boundingSphere;
    private bool mannequinHidden = false;
    private bool flickerStopped = false;

    private void Start()
    {
        StartCoroutine(FlickerRoutine());

        if (playerCamera == null)
            playerCamera = Camera.main;

        cullingGroup = new CullingGroup();
        cullingGroup.targetCamera = playerCamera;

        boundingSphere = new BoundingSphere[1];
        boundingSphere[0] = new BoundingSphere(mannequin.transform.position, 1.0f);

        cullingGroup.SetBoundingSpheres(boundingSphere);
        cullingGroup.SetBoundingSphereCount(1);
        cullingGroup.onStateChanged = OnMannequinSeen;
    }

    private IEnumerator FlickerRoutine()
    {
        normalLight.SetActive(true);
        brokenLight.SetActive(false);

        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(SingleFlicker());

            if (Random.Range(0, 100) < glitchChance)
                yield return StartCoroutine(QuickFlicker());

            normalLight.SetActive(true);
            brokenLight.SetActive(false);
        }
    }

    private IEnumerator SingleFlicker()
    {
        normalLight.SetActive(false);
        brokenLight.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        normalLight.SetActive(true);
        brokenLight.SetActive(false);
    }

    private IEnumerator QuickFlicker()
    {
        int count = Random.Range(2, 4);

        for (int i = 0; i < count; i++)
        {
            bool isOn = normalLight.activeSelf;
            normalLight.SetActive(!isOn);
            brokenLight.SetActive(isOn);

            yield return new WaitForSeconds(0.1f);
        }

        normalLight.SetActive(true);
        brokenLight.SetActive(false);
    }

    void SwitchProfile()
    {
        if (operationVolume != null)
        {
            operationVolume.profile = normalVolume;
        }
    }

    public void StopFlicker()
    {
        if (flickerStopped) return;
        flickerStopped = true;

        StopAllCoroutines();

        normalLight.SetActive(false);
        brokenLight.SetActive(true);

        mannequinHidden = false;
        mannequin.SetActive(true);
    }

    public void OnMannequinSeen(CullingGroupEvent sphere)
    {
        if (!mannequin.activeSelf) return;
        if (mannequinHidden) return;

        if (sphere.isVisible)
        {
            Invoke(nameof(HideMannequin), 2f);
            audioSource.PlayOneShot(spooky);
            DirectionManager.Instance.screenGlitchShader.DOKill();
            DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_NoiseAmount", 1f);
            DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_GlitchStrength", 1f);
            DirectionManager.Instance.screenGlitchShader.DOFloat(1f, "_ScanLinesStrength", 1f);
        }

    }

    private void HideMannequin()
    {
        if (mannequinHidden) return;

        //노이즈 이미지 알파값 설정
        var noiseImageColor = DirectionManager.Instance.noiseImage.color;
        noiseImageColor.a = 0f;
        DirectionManager.Instance.noiseImage.color = noiseImageColor;

        Sequence noiseSeq = DOTween.Sequence();

        audioSource.PlayOneShot(noise);

        //노이즈 이미지 깜빡임
        noiseSeq.Append(DirectionManager.Instance.noiseImage.DOFade(1f, 0.1f))
           .AppendInterval(0.5f)            //0.5초 동안 유지
           .Append(DirectionManager.Instance.noiseImage.DOFade(0f, 0.1f));

        SwitchProfile();

        DirectionManager.Instance.screenGlitchShader.DOKill();
        DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_NoiseAmount", 0.1f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_GlitchStrength", 0.1f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_ScanLinesStrength", 0.1f);

        mannequinHidden = true;
        mannequin.SetActive(false);

        normalLight.SetActive(true);
        brokenLight.SetActive(false);
    }

    private void OnDisable()
    {
        cullingGroup?.Dispose();
    }
}
