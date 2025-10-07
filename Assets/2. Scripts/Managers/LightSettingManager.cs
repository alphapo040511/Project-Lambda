using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightSettingManager : MonoBehaviour
{
    private void Start()
    {
        LightSetting();

        // 씬 변경시 라이팅 설정
        GameEvents.OnSceneChanged += LightSetting;
    }

    public void LightSetting(string sceneName = "")
    {
        Light[] lights = FindObjectsOfType<Light>();

        if(lights != null)
        {
            // 라이트에 의한 그림자 퀄리티 설정
            LightShadows shadowType = SettingsManager.Instance.currentSettings.shadowQuality switch
            {
                0 => LightShadows.None,
                1 => LightShadows.Hard,
                2 => LightShadows.Soft,
                _ => LightShadows.Hard
            };

            // 그림자 해상도 설정
            LightShadowResolution resolution = SettingsManager.Instance.currentSettings.shadowResolution switch
            {
                0 => LightShadowResolution.Low,
                1 => LightShadowResolution.Medium,
                2 => LightShadowResolution.High,
                3 => LightShadowResolution.VeryHigh,
                _ => LightShadowResolution.Medium
            };

            SetShadow(lights, shadowType, resolution);
        }
    }

    void SetShadow(Light[] lights, LightShadows shadowType, LightShadowResolution resolution)
    {
        foreach (var l in lights)
        {
            l.shadows = shadowType;
            l.shadowResolution = resolution;
        }
    }
}
