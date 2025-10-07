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
    public Toggle vsyncToggle;

    [Header("Graphics Settings")]
    public TMP_Dropdown qualityDropdown;
    public CustomSelector shadowQualitySelector;
    public CustomSelector shadowDistanceSelector;
    public CustomSelector shadowCascadeCountSelector;
    public CustomSelector shadowResolutionSelector;


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

        // 그림자 설정
        if (shadowQualitySelector != null)
            shadowQualitySelector.onValueChanged += OnShadowQualityChanged;

        if (shadowDistanceSelector != null)
            shadowDistanceSelector.onValueChanged += OnShadowDistancehanged;

        if (shadowCascadeCountSelector != null)
            shadowCascadeCountSelector.onValueChanged += OnShadowCascadeCountChanged;

        if (shadowResolutionSelector != null)
            shadowResolutionSelector.onValueChanged += OnShadowResolutionhanged;
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

        // 그림자 설정
        if (shadowQualitySelector != null)
            shadowQualitySelector.SetOption(settingsUI.tempSettings.shadowQuality);

        if (shadowDistanceSelector != null)
            shadowDistanceSelector.SetOption(settingsUI.tempSettings.shadowDistance);

        if (shadowCascadeCountSelector != null)
            shadowCascadeCountSelector.SetOption(settingsUI.tempSettings.shadowCascadeCount);

        if (shadowResolutionSelector != null)
            shadowResolutionSelector.SetOption(settingsUI.tempSettings.shadowResolution);
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

    private void OnShadowQualityChanged(int value)
    {
        if (isInitializing) return;

        // 그림자가 꺼져있는 경우 아래의 옵션 가리기
        shadowDistanceSelector.gameObject.SetActive(value != 0);
        shadowCascadeCountSelector.gameObject.SetActive(value != 0);
        shadowResolutionSelector.gameObject.SetActive(value != 0);

        settingsUI.tempSettings.shadowQuality = value;
    }

    private void OnShadowDistancehanged(int value)
    {
        if (isInitializing) return;

        int distance = value switch
        {
            0 => 50,
            1 => 100,
            2 => 150,
            3 => 200,
            _ => 100
        };

        settingsUI.tempSettings.shadowDistance = distance;
    }

    private void OnShadowCascadeCountChanged(int value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.shadowCascadeCount = value;
    }

    private void OnShadowResolutionhanged(int value)
    {
        if (isInitializing) return;
        settingsUI.tempSettings.shadowResolution = value;
    }

    #endregion
}
