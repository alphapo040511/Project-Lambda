using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DirectionManager : MonoBehaviour
{
    public static DirectionManager Instance { get; private set; }

    [Header("스크린 글리치 셰이더 머테리얼")]
    public Material screenGlitchShader;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void EnterDangerZone()
    {
        screenGlitchShader.DOKill();
        screenGlitchShader.DOFloat(25f, "_NoiseAmount", 8f);
        screenGlitchShader.DOFloat(25f, "_GlitchStrength", 8f);
        screenGlitchShader.DOFloat(1f, "_ScanLinesStrength", 8f);
        screenGlitchShader.DOColor(Color.gray, "_MainColor", 8f);
    }

    public void ExitDangerZone()
    {
        screenGlitchShader.DOKill();
        screenGlitchShader.DOFloat(0f, "_NoiseAmount", 1f);
        screenGlitchShader.DOFloat(0f, "_GlitchStrength", 1f);
        screenGlitchShader.DOFloat(0f, "_ScanLinesStrength", 1f);
        screenGlitchShader.DOColor(Color.white, "_MainColor", 1f);
    }
}
