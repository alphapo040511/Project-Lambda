using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObservationUI : SingletonMonoBehaviour<ObservationUI>
{
    public Button button;
    public TextMeshProUGUI descriptionText;

    private Action callback;                    // 콜백 저장용 Action

    private void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        button.onClick.RemoveListener(ButtonClick);
    }

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowButton(string content, string description, Action callback)
    {
        button.GetComponentInChildren<TextMeshProUGUI>().text = content;
        descriptionText.text = description;

        this.callback = callback;

        UIManager.Instance.ShowOverlay(OverlayType.Observation);
    }

    void ButtonClick()
    {
        callback?.Invoke();          // 콜백이 null이 아니면 실행

        UIManager.Instance.HideOverlay(OverlayType.Observation);
    }
}
