using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;  // LocalizeStringEvent


public class PopupView : MonoBehaviour
{
    public TextMeshProUGUI contentText;
    public LocalizeStringEvent confirmText;
    public LocalizeStringEvent cancelText;
    public Button confirmButton;
    public Button cancelButton;

    // 콜백 저장용 이벤트
    private Action confirmCallback;
    private Action cancleCallback;

    private void Start()
    {
        // 버튼 리스너 등록
        if (confirmCallback != null)
        {
            confirmButton.onClick.AddListener(() =>
            {
                confirmCallback.Invoke();
                gameObject.SetActive(false);
            });
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(() =>
            {
                cancleCallback?.Invoke();
                gameObject.SetActive(false);
            });
        }
    }

    public void Show(string message, string confirmMessage, Action onConfirm, string cancleMessage = "Close",  Action onCancle = null)
    {
        // 텍스트 업데이트
        contentText.text = message;

        if (confirmText != null)
        {
            confirmText.StringReference.TableEntryReference = confirmMessage;
            confirmText.RefreshString();
        }

        if (cancelText != null)
        {
            cancelText.StringReference.TableEntryReference = cancleMessage;
            cancelText.RefreshString();
        }

        // 콜백 저장
        confirmCallback = onConfirm;
        cancleCallback = onCancle;

        gameObject.SetActive(true);
    }
}
