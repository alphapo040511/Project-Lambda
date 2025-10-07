using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GraphicSettings : SettingTabBase
{
    [Header("Display Settings")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public CustomSelector frameRateSelector;

    [Header("Graphics Settings")]
    public TMP_Dropdown qualityDropdown;
    public Toggle vsyncToggle;

    protected override void InitializeUI()
    {
        // 해상도 드롭다운 설정
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(new System.Collections.Generic.List<string>(
                SettingsManager.Instance.GetResolutionStrings()));
        }

        // 프레임레이트 드롭다운 설정
        if (frameRateSelector != null)
        {
            frameRateSelector.ClearOptions();
            frameRateSelector.AddOptions(new System.Collections.Generic.List<string>(
                SettingsManager.Instance.GetFrameRates()));
        }

        // 품질 드롭다운 설정
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(
                SettingsManager.Instance.GetQualityLevelStrings()));
        }
    }

    protected override void SetupEventListeners()
    {
        // 디스플레이 설정
        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        if (frameRateSelector != null)
            frameRateSelector.onValueChanged += OnFrameRateChanged;


        // 그래픽 설정
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);

        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(OnVSyncChanged);
    }

    public override void RefreshUI()
    {
        // 디스플레이 설정 UI 업데이트
        if (resolutionDropdown != null)
            resolutionDropdown.value = settingsUI.tempSettings.resolutionIndex;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = settingsUI.tempSettings.isFullscreen;

        if (frameRateSelector != null)
        {
            //          switch문의 간략화
            //          변수 = 값 switch
            //          {
            //              패턴 => 결과식
            //              _ => 기본값
            //          }

            frameRateSelector.SetOption(settingsUI.tempSettings.targetFrameRate);
        }

        // 그래픽 설정 UI 업데이트
        if (qualityDropdown != null)
            qualityDropdown.value = settingsUI.tempSettings.qualityLevel;

        if (vsyncToggle != null)
            vsyncToggle.isOn = settingsUI.tempSettings.vsyncEnabled;
    }

    #region Event Handlers
    private void OnResolutionChanged(int value)
    {
        if (isInitializing) return;
        Debug.Log($"해상도 변경 {value}");
        settingsUI.tempSettings.resolutionIndex = value;
    }

    private void OnFullscreenChanged(bool value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.isFullscreen = value;
    }

    private void OnFrameRateChanged(int value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.targetFrameRate = value;
    }


    private void OnQualityChanged(int value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.qualityLevel = value;
    }

    private void OnVSyncChanged(bool value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.vsyncEnabled = value;
    }

    #endregion
}
