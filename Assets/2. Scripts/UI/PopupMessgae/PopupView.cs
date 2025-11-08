using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization;  // LocalizeStringEvent


public class PopupView : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI confirmText;
    public TextMeshProUGUI cancelText;
    public Button confirmButton;
    public Button cancelButton;

    // 콜백 저장용 이벤트
    public Action OnConfirmClicked;
    public Action OnCancelClicked;

    private TextLocalizer content;
    private TextLocalizer confirm;
    private TextLocalizer cancel;

    private const string tableName = "Settings Table";

    public void Init()
    {
        ButtenEventSetting();
        LocalizerSetting();
    }

    void ButtenEventSetting()
    {
        // 버튼 리스너 등록
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(() =>
            {
                Debug.Log("view 에서 적용 확인");
                OnConfirmClicked?.Invoke();
            });
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(() =>
            {
                Debug.Log("view 에서 닫기 확인");
                OnCancelClicked?.Invoke();
            });
        }
    }

    void LocalizerSetting()
    {
        content = new TextLocalizer(messageText, tableName);
        confirm = new TextLocalizer(confirmText, tableName);
        cancel = new TextLocalizer(cancelText, tableName);
    }

    public void Show()
    {
        //GameManager.Instance.ChangeGameState(GameState.Menu);
        UIManager.Instance.ShowOverlay(OverlayType.Popup);
    }

    public void Hide()
    {
        //GameManager.Instance.ResumeGame();
        UIManager.Instance.HideOverlay(OverlayType.Popup);
    }

    public void SetMessage(string key)
    {
        content.SetKey(key);
    }

    public void SetConfirm(string key)
    {
        confirm.SetKey(key);
    }

    public void SetCancel(string key)
    {
        cancel.SetKey(key);
    }

    #region Visible Settings
    public void SetConfirmButtonVisible(bool isVisible)
    {
        confirmButton.gameObject.SetActive(isVisible);
    }

    public void SetCancelButtonVisible(bool isVisible)
    {
        cancelButton.gameObject.SetActive(isVisible);
    }

    public void SetButtonsVisible(bool confirmIsVisible, bool cancleIsVisible)
    {
        confirmButton.gameObject.SetActive(confirmIsVisible);
        cancelButton.gameObject.SetActive(cancleIsVisible);
    }
    #endregion

    private void OnDestroy()
    {
        // 버튼 리스너 제거
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
    }
}
