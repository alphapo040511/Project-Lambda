using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PopupPresenter
{
    private PopupView view;
    private PopupModel model;

    // 콜백 저장용 필드
    private Action onConfirmCallback;
    private Action onCancelCallback;

    public PopupPresenter(PopupView view)
    {
        this.view = view;
        this.model = new PopupModel();

        // View 이벤트 리스너 등록
        this.view.OnConfirmClicked += HandleConfirm;
        this.view.OnCancelClicked += HandleCancel;
    }

    // 팝업 표시 메서드
    public void ShowPopup(string message, string confirmMessage = null, string cancelMessage = null, Action onConfirm = null, Action onCancel = null)
    {
        // Model 업데이트
        model.Message = message;
        model.ConfirmMessage = confirmMessage;
        model.CancelMessage = cancelMessage;

        // 콜백 저장
        onConfirmCallback = onConfirm;
        onCancelCallback = onCancel;

        // View 업데이트
        UpdateView();

        // 팝업 표시
        view.Show();
    }

    // 팝업 숨김 메서드
    public void HidePopup()
    {
        view.Hide();
    }

    // View 업데이트
    private void UpdateView()
    {
        view.SetMessage(model.Message);
        
        if(model.HasConfirmButton)
            view.SetConfirm(model.ConfirmMessage);

        if(model.HasCancelButton)
            view.SetConfirm(model.CancelMessage);

        view.SetButtonsVisible(model.HasConfirmButton, model.HasCancelButton);
    }

    // 이벤트 핸들러
    private void HandleConfirm()
    {
        Debug.Log("Presenter 에서 적용 확인");
        onConfirmCallback?.Invoke();
        HidePopup();
    }

    private void HandleCancel()
    {
        onCancelCallback?.Invoke();
        HidePopup();
    }
    private void HandleClose()
    {
        HidePopup();
    }

    // 리소스 정리
    public void Cleanup()
    {
        view.OnConfirmClicked -= HandleConfirm;
        view.OnCancelClicked -= HandleCancel;
    }
}
