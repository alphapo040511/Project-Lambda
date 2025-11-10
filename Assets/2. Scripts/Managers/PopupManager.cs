using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    public PopupView view;
    private PopupPresenter presenter;

    private void Start()
    {
        if (view != null)
        {
            view.Init();
            presenter = new PopupPresenter(view);
        }
    }

    // 팝업 표시를 위한 공개 메서드
    public void ShowPopup(string message)
    {
        presenter.ShowPopup(message);
    }

    // 확인/취소 버튼이 있는 팝업
    public void ShowConfirmPopup(string message, string confirmMessage = "", string cancelMessage = "",
                                Action onConfirm = null, Action onCancel = null)
    {
        presenter.ShowPopup( message, confirmMessage, cancelMessage, onConfirm, onCancel);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        presenter.Cleanup();
    }
}
