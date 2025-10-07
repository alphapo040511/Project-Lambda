using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


public class CustomSelector : MonoBehaviour
{
    [Header("UI Settings")]
    public TextMeshProUGUI optionText;
    public Button leftArrow;
    public Button rightArrow;
    public Button indicatorPrefab;
    public Transform container;
    private List<Button> indicatorButtons = new List<Button>();

    [Header("Option Settings")]
    public List<string> options = new List<string>
    {
        "Option 1",
        "Option 2",
        "Option 3"
    };
    public int value { get; private set; } = 0;

    // 값 변경 이벤트
    public Action<int> onValueChanged;

    private LocalizedString localizedString = new LocalizedString();

    // 테이블 이름 설정 (Inspector 또는 코드)
    [SerializeField] private string tableName = "Settings Table";

    private void Awake()
    {
        leftArrow.onClick.AddListener(() => ChangeOption(-1));
        rightArrow.onClick.AddListener(() => ChangeOption(1));

        SetUI();
    }

    // 옵션 추가
    public void AddOptions(List<string> newOption)
    {
        options.AddRange(newOption);                    // 옵션 배열 붙여 넣기

        SetUI();                                        // 버튼 재생생

        RefreshUI();
    }

    // 옵션 초기화
    public void ClearOptions()
    {
        options.Clear();

        DestroyButton();
    }

    void DestroyButton()
    {
        foreach (Button button in indicatorButtons)
        {
            Destroy(button.gameObject);
        }

        indicatorButtons.Clear();
    }


    // 다음 옵션
    void ChangeOption(int dir)
    {
        if (value + dir >= options.Count || value + dir < 0) return;    // 변경하려는 값이 옵션 밖이라면

        value += dir;
        onValueChanged?.Invoke(value);

        RefreshUI();
    }

    public void SetOption(int index)
    {
        if (index >= 0 && index < options.Count)
        {
            value = index;
            RefreshUI();
        }
    }

    void SetUI()
    {
        DestroyButton();                        // 기존 버튼 삭제

        for (int i = 0; i < options.Count; i++)
        {
            int index = i;
            CreatIndicator(index);
        }
        RefreshUI();
    }

    void CreatIndicator(int index)
    {
        Button button = Instantiate(indicatorPrefab, container);
        button.onClick.AddListener(() => SetOption(index));
        indicatorButtons.Add(button);
    }

    void RefreshUI()
    {
        SetOption(options[value]);                              // 텍스트 적용 String Table

        leftArrow.interactable = value > 0;                     // 첫번째 옵션이라면 비활성화
        rightArrow.interactable = value < options.Count - 1;    // 마지막 옵션이라면 비활성화

        // 인디케이터 버튼 색상 갱신
        for (int i = 0; i < indicatorButtons.Count; i++)
        {
            var colors = indicatorButtons[i].colors;
            if (i == value)
                colors.normalColor = Color.white; // 밝게
            else
                colors.normalColor = Color.gray; // 어둡게
            indicatorButtons[i].colors = colors;
        }
    }

    void SetOption(string keyOrValue)
    {
        // 이전 이벤트 제거 (안전하게)
        localizedString.StringChanged -= UpdateText;

        if (string.IsNullOrEmpty(keyOrValue)) return;

        TableEntry entry = LocalizationSettings.StringDatabase.GetTable(tableName).GetEntry(keyOrValue);

        if(entry != null)
        {
            // 키 존재 → LocalizedString 적용
            localizedString.TableReference = tableName;
            localizedString.TableEntryReference = keyOrValue;
            localizedString.StringChanged += UpdateText;
            localizedString.RefreshString();
        }
        else
        {
            // 키 없으면 그냥 직접 문자열 적용
            optionText.text = keyOrValue;
        }
    }

    private void UpdateText(string value)
    {
        optionText.text = value;
    }

}
