using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : SettingTabBase
{
    [Header("Audio Settings")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider voiceVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI voiceVolumeText;

    protected override void InitializeUI() { }  // RefreshUI로 충분

    protected override void SetupEventListeners()
    {
        // 오디오 설정
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (voiceVolumeSlider != null)
            voiceVolumeSlider.onValueChanged.AddListener(OnVoiceVolumeChanged);
    }

    public override void RefreshUI()
    {

        // 오디오 설정 UI 업데이트
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = settingsUI.tempSettings.masterVolume;
            if (masterVolumeText != null)
                masterVolumeText.text = $"{settingsUI.tempSettings.masterVolume * 100:F0}%";
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = settingsUI.tempSettings.sfxVolume;
            if (sfxVolumeText != null)
                sfxVolumeText.text = $"{settingsUI.tempSettings.sfxVolume * 100:F0}%";
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = settingsUI.tempSettings.musicVolume;
            if (musicVolumeText != null)
                musicVolumeText.text = $"{settingsUI.tempSettings.musicVolume * 100:F0}%";
        }

        if (voiceVolumeSlider != null)
        {
            voiceVolumeSlider.value = settingsUI.tempSettings.voiceVolume;
            if (voiceVolumeText != null)
                voiceVolumeText.text = $"{settingsUI.tempSettings.voiceVolume * 100:F0}%";
        }
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (isInitializing) return;

        settingsUI.tempSettings.masterVolume = value;
        if (masterVolumeText != null)
            masterVolumeText.text = $"{value * 100:F0}%";

        // 실시간 볼륨 적용
        SettingsManager.Instance.SetMasterVolume(value);
    }

    #region Event Handlers
    private void OnSFXVolumeChanged(float value)
    {
        if (isInitializing) return;

        settingsUI.tempSettings.sfxVolume = value;
        if (sfxVolumeText != null)
            sfxVolumeText.text = $"{value * 100:F0}%";

        // 실시간 볼륨 적용
        SettingsManager.Instance.SetSFXVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (isInitializing) return;

        settingsUI.tempSettings.musicVolume = value;
        if (musicVolumeText != null)
            musicVolumeText.text = $"{value * 100:F0}%";

        // 실시간 볼륨 적용
        SettingsManager.Instance.SetMusicVolume(value);
    }

    private void OnVoiceVolumeChanged(float value)
    {
        if (isInitializing) return;

        settingsUI.tempSettings.voiceVolume = value;
        if (voiceVolumeText != null)
            voiceVolumeText.text = $"{value * 100:F0}%";

        // 실시간 볼륨 적용
        SettingsManager.Instance.SetVoiceVolume(value);
    }
    #endregion
}
