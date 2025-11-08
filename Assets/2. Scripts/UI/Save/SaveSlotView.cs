using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotView : MonoBehaviour
{
    public Button button;
    public Image thumbnailImage;
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI locationName;
    public TextMeshProUGUI saveTime;
    public TextMeshProUGUI playTime;

    public Action<int> onSlotClicked;

    private int slotIndex;
    private TextLocalizer titleLocalizer;
    private TextLocalizer locationLocalizer;

    public void Init(int slotIndex)
    {
        this.slotIndex = slotIndex;

        button.onClick.AddListener(() => onSlotClicked?.Invoke(slotIndex));     // 버튼 이벤트 등록

        titleLocalizer = new TextLocalizer(questTitle, "Quest Table");
        locationLocalizer = new TextLocalizer(locationName, "Quest Table");
    }

    public void UpdateUI(Sprite thumbnail, string quest, string location, string saveT, string playT)
    {
        if(thumbnail != null)
            thumbnailImage.sprite = thumbnail;      // 썸네일 적용

        if(titleLocalizer != null)                  // 로컬라저가 없다면 재할당
            titleLocalizer = new TextLocalizer(questTitle, "Quest Table");

        titleLocalizer.SetKey(quest);

        if (locationLocalizer != null)              // 로컬라저가 없다면 재할당
            locationLocalizer = new TextLocalizer(locationName, "Quest Table");

        locationLocalizer.SetKey(location);

        saveTime.text = saveT;
        playTime.text = playT;
    }
}
