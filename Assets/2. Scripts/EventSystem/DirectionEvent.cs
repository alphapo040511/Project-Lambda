using UnityEngine;
using DG.Tweening;

public class DirectionEvent : MonoBehaviour
{
    [Header("오디오 소스")]
    public AudioSource directionAudioSource;
    public AudioSource endAudioSource;

    [Header("1번 연출 소스")]
    public AudioClip direction1_noise;
    public AudioClip direction1_scary;
    public AudioClip direction1_end;

    public GameObject normalWalls;
    public GameObject glitchWalls;

    private bool firstDirectionTriggered = false;

    private void OnTriggerEnter(Collider other)
    {

        PlayerController player = other.GetComponent<PlayerController>();

        if (!other.CompareTag("Player")) return;

        Debug.Log($"{gameObject.name} 실행");

        switch (gameObject.name)
        {
            case "FirstDirectionCollider":
                if (firstDirectionTriggered == false)
                {
                    PlayDirection1();
                }
                break;

            case "SecondDirectionCollider":
                PlayDirection2();
                break;

        }
    }

    void PlayDirection1()
    {
        firstDirectionTriggered = true;
        Debug.Log("1번 연출 시작");

        //노이즈 이미지 알파값 설정
        var noiseImageColor = DirectionManager.Instance.noiseImage.color;
        noiseImageColor.a = 0f;
        DirectionManager.Instance.noiseImage.color = noiseImageColor;

        Sequence noiseSeq = DOTween.Sequence();

        directionAudioSource.PlayOneShot(direction1_noise);

        //노이즈 이미지 깜빡임
        noiseSeq.Append(DirectionManager.Instance.noiseImage.DOFade(1f, 0.1f))
           .AppendInterval(0.5f)            //0.5초 동안 유지
           .Append(DirectionManager.Instance.noiseImage.DOFade(0f, 0.1f));

        glitchWalls.SetActive(true);
        normalWalls.SetActive(false);

        //글리치 셰이더, 비네트 on
        DirectionManager.Instance.screenGlitchShader.DOKill();
        DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_NoiseAmount", 6f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(25f, "_GlitchStrength", 6f);
        DirectionManager.Instance.screenGlitchShader.DOFloat(1f, "_ScanLinesStrength", 6f);

        VolumeManager.Instance.ChangeVignette(0.8f, 6f);

        directionAudioSource.PlayOneShot(direction1_scary);

        DOVirtual.DelayedCall(direction1_scary.length, () =>
        {
            float endAudioLength = direction1_end.length;

            directionAudioSource.PlayOneShot(direction1_end);

            //노이즈 이미지 깜빡임
            noiseSeq.Append(DirectionManager.Instance.noiseImage.DOFade(1f, 0.1f))
               .AppendInterval(1f)            //1초 동안 유지
               .Append(DirectionManager.Instance.noiseImage.DOFade(0f, 0.1f));

            //글리치 셰이더, 비네트 off
            DirectionManager.Instance.screenGlitchShader.DOKill();
            DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_NoiseAmount", 0.1f);
            DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_GlitchStrength", 0.1f);
            DirectionManager.Instance.screenGlitchShader.DOFloat(0f, "_ScanLinesStrength", 0.1f);
            VolumeManager.Instance.ChangeVignette(0.07f, 0.1f);

            glitchWalls.SetActive(false);
            normalWalls.SetActive(true);

            DOVirtual.DelayedCall(endAudioLength + 2f, () =>
            {
                DialogueManager.Instance.PlayingDialog("AI_LowSanity");
            });

            Debug.Log("1번 연출 종료");
        });

    }

    void PlayDirection2()
    {
        Debug.Log("2번 연출 시작");

    }
}
