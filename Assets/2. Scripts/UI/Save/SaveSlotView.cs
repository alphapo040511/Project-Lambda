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
    public TextMeshProUGUI slotNumber;
    public TextMeshProUGUI playTime;

    public GameObject UIContent;
    public GameObject emptyText;

    public Action<int> onSlotClicked;

    private int slotIndex;
    private TextLocalizer titleLocalizer;
    private TextLocalizer locationLocalizer;
    private TextLocalizer slotLocalizer;

    public void Init(int slotIndex)
    {
        // 버튼 이벤트 등록
        button.onClick.AddListener(() => onSlotClicked?.Invoke(slotIndex));   
        
        // Localizer 생성
        titleLocalizer = new TextLocalizer(questTitle, "Quest Table");
        locationLocalizer = new TextLocalizer(locationName, "Quest Table");
        slotLocalizer = new TextLocalizer(slotNumber, "Settings Table");

        // 슬롯 표시
        this.slotIndex = slotIndex;
        if (slotIndex == 0)
            slotLocalizer.SetKey("Auto Save");
        else
            slotLocalizer.SetKey("Slot", slotIndex);
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

    public void UIActiveSetting(bool hasData)
    {
        UIContent.SetActive(hasData);
        emptyText.SetActive(!hasData);

        thumbnailImage.color = hasData ? Color.white : Color.gray;
    }
}
