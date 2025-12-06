using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DirectionManager : MonoBehaviour
{
    public static DirectionManager Instance { get; private set; }

    [Header("스크린 글리치 셰이더 머테리얼")]
    public Material screenGlitchShader;

    [Header("노이즈 이미지")]
    public Image noiseImage;

    [Header("페이드 이미지")]
    public Image fadeImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

    }

    private void OnDestroy()
    {
        screenGlitchShader.SetFloat("_NoiseAmount", 0);
        screenGlitchShader.SetFloat("_GlitchStrength", 0);
        screenGlitchShader.SetFloat("_ScanLinesStrength", 0);
        screenGlitchShader.SetColor("_MainColor", Color.white);
    }

    public void EnterDangerZone()
    {
        screenGlitchShader.DOKill();
        screenGlitchShader.DOFloat(25f, "_NoiseAmount", 8f);
        screenGlitchShader.DOFloat(25f, "_GlitchStrength", 8f);
        screenGlitchShader.DOFloat(1f, "_ScanLinesStrength", 8f);
        screenGlitchShader.DOColor(Color.gray, "_MainColor", 15f);
    }

    public void ExitDangerZone()
    {
        screenGlitchShader.DOKill();
        screenGlitchShader.DOFloat(0f, "_NoiseAmount", 1f);
        screenGlitchShader.DOFloat(0f, "_GlitchStrength", 1f);
        screenGlitchShader.DOFloat(0f, "_ScanLinesStrength", 1f);
        screenGlitchShader.DOColor(Color.white, "_MainColor", 1f);
    }

    public void GameOver()
    {
        screenGlitchShader.DOKill();
    }

}
