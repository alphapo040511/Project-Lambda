using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class SettingsUI : ScreenBase
{
    public PopupView popupView;         // 임시 팝업 뷰 (바꿀 예정)
    public Canvas canvas;

    [Header("Tab Settings")]
    public SettingTabBase gameplayTab;
    public SettingTabBase audioTab;
    public SettingTabBase graphicTab;
    public Button gameplayButton;
    public Button audioButton;
    public Button graphicButton;
    private SettingTabBase currenTab;


    [Header("Control Buttons")]
    public Button applyButton;
    public Button resetButton;
    public Button closeButton;
    public Button exitButtton;

    // 설정 변경 이벤트
    public event Action onRefreshUI;                            // 설정 초기화 등 모든 UI의 새로고침이 필요할 때

    [HideInInspector]public GameSettings tempSettings;

    public override void Show()
    {
        canvas.gameObject.SetActive(true);

        LoadingSetting();

        ShowTab(gameplayTab);                               // 첫 번째 탭 활성화
    }

    public override void Hide()
    {
        canvas.gameObject.SetActive(false);
    }

    public override void Init()
    {
        canvas.gameObject.SetActive(false);
        LoadingSetting();
    }

    private void Start()
    {
        SetupEventListeners();
        InitializeTabs();
    }

    private void InitializeTabs()
    {
        ShowTab(gameplayTab);
        audioTab.Hide();
        graphicTab.Hide();
    }

    private void SetupEventListeners()
    {
        // 탭 설정
        gameplayButton.onClick.AddListener(() => ShowTab(gameplayTab));
        audioButton.onClick.AddListener(() => ShowTab(audioTab));
        graphicButton.onClick.AddListener(() => ShowTab(graphicTab));

        // 버튼 설정
        if (applyButton != null)
            applyButton.onClick.AddListener(OnApplyClicked);

        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClicked);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        if(exitButtton != null)
            exitButtton.onClick.AddListener(OnExitClicked);
    }

    private void LoadingSetting()
    {
        if (SettingsManager.Instance == null) return;
            tempSettings = SettingsManager.Instance.currentSettings.Clone();

        onRefreshUI?.Invoke();
    }

    #region Tab Change
    public void ShowTab(SettingTabBase target)
    {
        if (target == null || currenTab == target) return;

        if (SettingsManager.Instance.currentSettings.Equals(tempSettings))      // 설정 값이 변하지 않았다면
        {
            SettingsManager.Instance.SaveSettings();        // 오디오 같이 실시간 저장 요소 적용을 위해

            ChangeTab(target);
        }
        else
        {
            popupView.Show(
                "변경된 설정이 저장되지 않았습니다.\n" +
                "저장하시겠습니까?",
                "Apply",
                "Close",
                () =>
                {
                    OnApplyClicked();
                    ChangeTab(target);
                },
                () => {
                    LoadingSetting();
                    ChangeTab(target);
                });
        }
    }

    void ChangeTab(SettingTabBase target)
    {
        CloseTab(currenTab);
        target.Show();
        currenTab = target;
    }


    private void CloseTab(SettingTabBase tab)
    {
        if(tab != null)
        {
            tab.Hide();
            currenTab = null;
        }
    }

    #endregion

    #region Event Handlers


    private void OnApplyClicked()
    {
        // 임시 설정을 실제 설정에 적용
        SettingsManager.Instance.currentSettings = tempSettings.Clone();
        SettingsManager.Instance.ApplyAllSettings();
        SettingsManager.Instance.SaveSettings();

        Debug.Log("설정이 적용되었습니다");
    }

    private void OnResetClicked()
    {
        SettingsManager.Instance.ResetToDefault();
        LoadingSetting();
        
        Debug.Log("설정이 초기화되었습니다");
    }

    private void OnCloseClicked()
    {
        if (SettingsManager.Instance.currentSettings.Equals(tempSettings))      // 설정 값이 변하지 않았다면
        {
            SettingsManager.Instance.SaveSettings();        // 오디오 같이 실시간 저장 요소 적용을 위해

            // 설정 창 닫기
            GameManager.Instance.ResumeGame();
        }
        else
        {
            popupView.Show(
                "변경된 설정이 저장되지 않았습니다.\n" +
                "저장하시겠습니까?",
                "Apply",
                "Close",
                () =>
                {
                    OnApplyClicked();
                    GameManager.Instance.ResumeGame();
                },
                () => {
                    LoadingSetting();
                    GameManager.Instance.ResumeGame();
                    });
        }
    }

    private void OnExitClicked()
    {
        GameManager.Instance.ChangeGameState(GameState.Menu);
        SceneManager.Instance.LoadMainMenu();
    }

    #endregion
}

