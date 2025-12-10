using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObservationUI : SingletonMonoBehaviour<ObservationUI>
{
    public Button button;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI descriptionText;

    private Action callback;                    // 콜백 저장용 Action

    private TextLocalizer textLocalizer;
    private TextLocalizer descriptionLocalizer;

    private void Start()
    {
        button.onClick.AddListener(ButtonClick);
        if (buttonText != null)
            textLocalizer = new TextLocalizer(buttonText, "Quest Table");

        if(descriptionText != null)
            descriptionLocalizer = new TextLocalizer(descriptionText, "Item Table");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        button.onClick.RemoveListener(ButtonClick);
    }

    public void ShowButton(string content, string description, Action callback)
    {
        if(textLocalizer != null)
            textLocalizer.SetKey(content);

        if(descriptionLocalizer != null)
            descriptionLocalizer.SetKey(description);

        this.callback = callback;

        UIManager.Instance.ShowOverlay(OverlayType.Observation);
    }

    void ButtonClick()
    {
        callback?.Invoke();          // 콜백이 null이 아니면 실행

        UIManager.Instance.HideOverlay(OverlayType.Observation);
    }
}
