using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class WordView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image textBox;
    public TextMeshProUGUI wordText;

    // 버튼 클릭 이벤트
    public event Action<string> onClick;

    // 텍스트 박스 색상
    private Color defaultBoxColor = Color.clear;
    private Color selectedBoxColor = Color.white;

    // 텍스트 색상
    private Color defaultTextColor = Color.white;
    private Color selectedTextColor = Color.black;

    private void Start()
    {
        UnSelecting();
    }

    public void Show(string text)
    {
        UnSelecting();
        wordText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Selecting()
    {
        textBox.color = selectedBoxColor;
        wordText.color = selectedTextColor;
    }

    private void UnSelecting()
    {
        textBox.color = defaultBoxColor;
        wordText.color = defaultTextColor;
    }

    // 클릭시 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(wordText.text);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Selecting();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnSelecting();
    }
}
