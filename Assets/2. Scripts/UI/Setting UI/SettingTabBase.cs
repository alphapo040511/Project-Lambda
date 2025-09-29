using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SettingTabBase : MonoBehaviour, IScreen
{
    public SettingsUI settingsUI;
    protected bool isInitializing = false;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Init()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);

        RefreshUI();
    }

    private void Start()
    {
        isInitializing = true;

        InitializeUI();
        SetupEventListeners();

        settingsUI.onRefreshUI += RefreshUI;

        isInitializing = false;
    }

    protected abstract void InitializeUI();                 // UI 초기 설정

    protected abstract void SetupEventListeners();          // UI 이벤트 설정

    public abstract void RefreshUI();                       // UI에 현재 세팅 적용

}
