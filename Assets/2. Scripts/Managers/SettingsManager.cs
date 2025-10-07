using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameSettings
{
    [Header("Display Settings")]
    public int resolutionIndex = 0;     // 해상도 인덱스
    public bool isFullscreen = true;    // 전체화면 여부
    public int targetFrameRate = 0;    // 목표 프레임 인덱스
    public bool vsyncEnabled = true;    // 수직동기화 여부

    [Header("Audio Settings")]
    public float masterVolume = 1f;     // 마스터 볼륨 (0~1)
    public float sfxVolume = 1f;        // 효과음 볼륨 (0~1)
    public float musicVolume = 1f;      // 배경음 볼륨 (0~1)

    [Header("UI Setting")]
    public float uiScale = 1f;          // UI 스케일

    [Header("Graphics Settings")]
    public int qualityLevel = 2;        // 그래픽 품질 레벨 (사용 안)

    //그림자
    public int shadowQuality = 2;       // 0 꺼짐 / 1 Hard / 2 Soft
    public int shadowDistance = 100;    // 그림자 거리 ~까지 그림자가 나타남
    public int shadowCascadeCount = 3;  // 그림자 품질 0 낮음 / 1 중간 / 2 높음 / 3 매우 높음
    public int shadowResolution = 3;    // 그림자 품질 0 낮음 / 1 중간 / 2 높음 / 3 매우 높음

    public int textureQuality;          // 0~2
    public int aaLevel;                 // 0, 2, 4, 8
    public float lodBias;               // 0.5~2.0
    //public bool postProcessing;       글로벌 볼륨을 건드려야해서 나중에 설정

    [Header("Language Setting")]
    public Language language = Language.en; // 언어 설정

    // Clone 메서드
    public GameSettings Clone()
    {
        GameSettings clone = new GameSettings();
        clone.resolutionIndex = this.resolutionIndex;
        clone.isFullscreen = this.isFullscreen;
        clone.targetFrameRate = this.targetFrameRate;
        clone.vsyncEnabled = this.vsyncEnabled;

        clone.masterVolume = this.masterVolume;
        clone.sfxVolume = this.sfxVolume;
        clone.musicVolume = this.musicVolume;

        clone.uiScale = this.uiScale;

        clone.shadowQuality = this.shadowQuality;
        clone.qualityLevel = this.qualityLevel;
        clone.shadowDistance = this.shadowDistance;
        clone.shadowCascadeCount = this.shadowCascadeCount;
        clone.shadowResolution = this.shadowResolution;
        clone.textureQuality = this.textureQuality;
        clone.aaLevel = this.aaLevel;
        clone.lodBias = this.lodBias;

        clone.language = this.language;


        return clone;
    }

    // 변경 여부 확인
    public bool Equals(GameSettings other)
    {
        if (other == null) return false;

        return resolutionIndex == other.resolutionIndex &&
               isFullscreen == other.isFullscreen &&
               targetFrameRate == other.targetFrameRate &&
                vsyncEnabled == other.vsyncEnabled &&

               //masterVolume == other.masterVolume &&                      // 사운드 같은 경우는 실시간으로 저장 되도록
               //sfxVolume == other.sfxVolume &&
               //musicVolume == other.musicVolume &&

               uiScale == other.uiScale &&

               qualityLevel == other.qualityLevel &&

               shadowQuality == other.shadowQuality &&
               shadowDistance == other.shadowDistance &&
               shadowCascadeCount == other.shadowCascadeCount &&
               shadowResolution == other.shadowResolution &&
               textureQuality == other.textureQuality &&
               aaLevel == other.aaLevel &&
               lodBias == other.lodBias &&

               language == other.language;
    }
}


public class SettingsManager : SingletonMonoBehaviour<SettingsManager>
{
    [Header("Audio References")]
    public AudioMixer masterMixer;

    [Header("UI Scale References")]
    public Canvas mainCanvas;

    [Header("Settings")]
    public GameSettings currentSettings = new GameSettings();

    private Resolution[] availableResolutions;
    private int[] availableFrameRates;
    private const string SETTINGS_KEY = "GameSettings";

    // 오디오 믹서 파라미터 이름
    private const string MASTER_VOLUME_PARAM = "MasterVolume";
    private const string SFX_VOLUME_PARAM = "SFXVolume";
    private const string MUSIC_VOLUME_PARAM = "MusicVolume";

    // 현재 적용중인 urpAsset;
    private UniversalRenderPipelineAsset urpAsset;

    // 라이팅 세팅용
    private LightSettingManager lightSetting;

    protected override void Awake()
    {
        base.Awake();

        // 현재 퀄리티 에셋 참조
        urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        lightSetting = gameObject.AddComponent<LightSettingManager>();

        InitializeSettings();           //설정 초기화
        LoadSettings();                 //설정 데이터 로드
        ApplyAllSettings();             //모든 설정 적용
    }

    private void InitializeSettings()
    {
        // 사용 가능한 해당도 목록 가져오기(해상도 + 주사율 정보 모두 나옴)
        //availableResolutions = Screen.resolutions;      // Screen.resolutions 현재 모니터에서 지원하는 해상도 배열을 반환

        availableResolutions = Screen.resolutions           // 해상도 + 주사율 정보 모두 나옴
                .GroupBy(r => new { r.width, r.height })    // 해상도만 기준으로 그룹화
                .Select(g => g.First())                     // 같은 해상도 중 첫 번째만 사용
    .           ToArray();

        availableFrameRates = Screen.resolutions
            .Select(f => (int)Math.Round(f.refreshRateRatio.value))     // 주사율만 추출
            .Distinct()                                                 // 중복 제거
            .OrderBy(f => f)                                            // 낮은 순 정렬
            .ToArray();

        // 현재 해상도를 기본(제일 높은 해상도)로 설정
        currentSettings.resolutionIndex = availableResolutions.Length - 1;

        // 현재 품질 설정 가져오기
        currentSettings.qualityLevel = QualitySettings.GetQualityLevel();
        currentSettings.vsyncEnabled = QualitySettings.vSyncCount > 0;
        // vSyncCount = 0 (없음)/ 1 (1번의 Vertical Blank마다 동기화, 60에 60FPS 고정)/ 2 (2번 마다, 60에 30FPS 고정)

        Debug.Log("SettingManager 초기화 완료");
    }

    #region Save/Load Settings (세이브 / 로드 설정)
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(currentSettings, true);
        PlayerPrefs.SetString(SETTINGS_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("설정 저장됨");
    }

    public void LoadSettings()
    {
        if(PlayerPrefs.HasKey(SETTINGS_KEY))
        {
            string json = PlayerPrefs.GetString(SETTINGS_KEY);
            JsonUtility.FromJsonOverwrite(json, currentSettings);

            Debug.Log("설정 로드됨");
        }
        else
        {
            Debug.Log(" 저장된 설정이 없어 기본값 사용");
        }
    }

    public void ResetToDefault()
    {
        currentSettings = new GameSettings();
        currentSettings.resolutionIndex = availableResolutions.Length - 1;          // 해상도는 가장 높게 설정
        ApplyAllSettings();
        SaveSettings();

        Debug.Log("설정이 기본값으로 초기화됨");
    }
    #endregion

    #region Apply Settings (설정 적용 메서드)
    public void ApplyAllSettings()
    {
        ApplyDisplaySettings();
        ApplyAudioSettings();
        ApplyUISettings();
        ApplyGraphicsSettings();
        ApplyLanguageSetting();

        lightSetting.LightSetting();        // 라이트는 따로 설정
    }

    private void ApplyDisplaySettings()
    {
        // 해상도 (및 전체화면) 적용
        if(availableResolutions != null && currentSettings.resolutionIndex < availableResolutions.Length)
        {
            Resolution targetRes = availableResolutions[currentSettings.resolutionIndex];
            Screen.SetResolution(targetRes.width, targetRes.height, currentSettings.isFullscreen);

            Debug.LogFormat("index {0} : {1} x {2}", currentSettings.resolutionIndex, targetRes.width, targetRes.height);
        }

        // 프레임레이트 설정
        Application.targetFrameRate = currentSettings.targetFrameRate;

        // 수직동기화 설정
        QualitySettings.vSyncCount = currentSettings.vsyncEnabled ? 1 : 0;
    }

    private void ApplyAudioSettings()
    {
        if(masterMixer != null)
        {
            // 볼륨을 dB로 변환 (0~1 범위를 -80dB ~ 80dB로)
            float masterDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;
            float sfxDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;
            float musicDB = currentSettings.masterVolume > 0 ?
                Mathf.Log10(currentSettings.masterVolume) * 20 : -80f;

            masterMixer.SetFloat(MASTER_VOLUME_PARAM, masterDB);
            masterMixer.SetFloat(SFX_VOLUME_PARAM, sfxDB);
            masterMixer.SetFloat(MUSIC_VOLUME_PARAM, musicDB);
        }
        else
        {
            // 오디오 믹서가 없으면 AudioListener 볼륨 조정
            AudioListener.volume = currentSettings.masterVolume;
        }
    }

    private void ApplyUISettings()
    {
        if(mainCanvas != null)
        {
            var canvasScaler = mainCanvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if(canvasScaler != null)
            {
                canvasScaler.scaleFactor = currentSettings.uiScale;
            }
        }
    }

    private void ApplyGraphicsSettings()
    {
        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);

        // 그림자 거리/카스케이드
        urpAsset.shadowDistance = (currentSettings.shadowQuality == 0) ? currentSettings.shadowDistance : 0;   // 꺼져있으면 0
        urpAsset.shadowCascadeCount = currentSettings.shadowCascadeCount + 1;
        
        // MSAA
        urpAsset.msaaSampleCount = currentSettings.aaLevel;

        QualitySettings.lodBias = currentSettings.lodBias;
    }

    private void ApplyLanguageSetting()
    {
        GameEvents.LanguageChanged(currentSettings.language);

        LocalizationSettings.SelectedLocale =
            LocalizationSettings.AvailableLocales.Locales[(int)currentSettings.language];
    }


    #endregion

    // 개별 설정은 잘 사용 안할듯 합니다..?
    #region Individual Setting Methods (개별 설정 메서드)
    // 해상도 설정
    public void SetResoultion(int resoultionIndex)
    {
        if (resoultionIndex > 0 && resoultionIndex < availableResolutions.Length)
        { 
            currentSettings.resolutionIndex = resoultionIndex;
            ApplyDisplaySettings();
        }
    }

    // 전체화면 설정
    public void SetFullscreen(bool fullscreen)
    {
        currentSettings.isFullscreen = fullscreen;
        ApplyDisplaySettings();
    }

    // 프레임레이트 설정
    public void SetTargetFrameRate(int frameRateIndex)
    {
        if (frameRateIndex > availableFrameRates.Length) return;        //  범위 밖이면 return

        currentSettings.targetFrameRate = frameRateIndex;
        Application.targetFrameRate = availableFrameRates[frameRateIndex];
    }

    // 수직동기화 설정
    public void SetVSync(bool ebabled)
    {
        currentSettings.vsyncEnabled = ebabled;
        ApplyGraphicsSettings();
    }

    // MasterVolume 설정
    public void SetMasterVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
        GameEvents.VolumeChanged(currentSettings.masterVolume);
    }

    // SFXVolume 설정
    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
    }

    // MusicVolume 설정
    public void SetMusicVolume(float volume)
    {
        currentSettings.masterVolume = Mathf.Clamp01(volume);
        ApplyAudioSettings();
    }

    // 그래픽 퀄리티 설정
    public void SetQualityLevel(int qualityLevel)
    {
        currentSettings.qualityLevel = Mathf.Clamp(qualityLevel, 0, QualitySettings.names.Length - 1);
                                                                    //Project Settings → Quality 탭에서 설정한 모든 품질 레벨의 이름 리스트
        ApplyGraphicsSettings();
    }


    #endregion

    #region Getters
    // 지원 해상도 배열 반환
    public Resolution[] GetAvailableResolutions()
    {
        return availableResolutions;
    }

    // 지원 해상도 문자열 반환
    public string[] GetResolutionStrings()
    {
        if(availableResolutions == null) return new string[0];

        string[] resStrings = new string[availableResolutions.Length];
        for(int i = 0; i < resStrings.Length; i++)
        {
            resStrings[i] = $"{availableResolutions[i].width} x {availableResolutions[i].height}";
        }
        return resStrings;
    }

    public string[] GetFrameRates()
    {
        if (availableFrameRates == null) return new string[1] { "Unlimited" };

        string[] resStrings = new string[availableFrameRates.Length + 1];
        for (int i = 0; i < resStrings.Length - 1; i++)
        {
            resStrings[i] = $"{availableFrameRates[i]} FPS";
            Debug.Log(resStrings[i] + "FPS");
        }

        resStrings[availableFrameRates.Length] = "Unlimited";

        return resStrings;
    }

    // 그래픽 퀄리티 레벨(문자열) 반환
    public string[] GetQualityLevelStrings()
    {
        return QualitySettings.names;
    }

    // 언어 설정 문자열 반환
    public string[] GetLanguageStrings()
    {
        Language[] langs = (Language[])Enum.GetValues(typeof(Language));

        string[] names = new string[langs.Length];

        for(int i = 0; i < langs.Length; i++)
        {
            string name = langs[i] switch
            {
                Language.en => "English",
                Language.ko => "한국어",
                _ => null
            };
            names[i] = name;
        }

        return names;
    }
    #endregion
}
