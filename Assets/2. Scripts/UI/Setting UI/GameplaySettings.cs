using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettings : SettingTabBase
{
    [Header("Language Settings")]
    public TMP_Dropdown languageDropdown;
    public Slider mouseSensitivitySlider;
    public TextMeshProUGUI sensitivityValue;

    protected override void InitializeUI()
    {

        // 언어 드롭다운 설정
        if (languageDropdown != null)
        {
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(new System.Collections.Generic.List<string>(
                SettingsManager.Instance.GetLanguageStrings()));
        }

        if(mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = settingsUI.tempSettings.mouseSentivity;
        }

        if (sensitivityValue != null)
        {
            sensitivityValue.text = settingsUI.tempSettings.mouseSentivity.ToString("F1");
        }
    }

    protected override void SetupEventListeners()
    {

        // 언어 설정
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        if(mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

    }

    #region Event Handlers
    public override void RefreshUI()
    {

        // 언어 설정 UI

        if (languageDropdown != null)
        {
            int language = settingsUI.tempSettings.language switch
            {
                Language.en => 0,
                Language.ko => 1,
                _ => 2
            };
            languageDropdown.value = language;
        }

        isInitializing = false;
    }

    private void OnLanguageChanged(int value)
    {
        if (isInitializing) return;

        Language language = value switch
        {
            0 => Language.en,
            1 => Language.ko,
            _ => Language.ko
        };

        settingsUI.tempSettings.language = language;
    }

    private void OnSensitivityChanged(float value)
    {
        if(sensitivityValue != null)
            sensitivityValue.text = value.ToString("F1");

        settingsUI.tempSettings.mouseSentivity = value;
    }
    #endregion
}

