using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using TMPro;


// 텍스트 언어 변경 기능
public class TextLocalizer
{
    // 각 TMP 마다 new 키워드로 객체를 만들어 사용
    public TextLocalizer(TextMeshProUGUI tmp, string tableName)
    {
        targetText = tmp;
        this.tableName = tableName;
    }

    private TextMeshProUGUI targetText;
    private LocalizedString localizedString = new LocalizedString();

    // 테이블 이름 설정 (Inspector 또는 코드)
    [SerializeField] private string tableName = "Quest Table";

    
    // 인자가 없이 키값만 있는 경우
    public void SetKey(string keyOrValue)
    {
        SetKey(keyOrValue, null);
    }

    public void SetKey(string keyOrValue, params object[] args)
    {
        // 이전 이벤트 제거 (안전하게)
        localizedString.StringChanged -= UpdateText;

        if (string.IsNullOrEmpty(keyOrValue)) return;

        TableEntry entry = LocalizationSettings.StringDatabase.GetTable(tableName).GetEntry(keyOrValue);

        if (entry != null)
        {
            // 키 존재 → LocalizedString 적용
            localizedString.TableReference = tableName;
            localizedString.TableEntryReference = keyOrValue;
            
            // 인자가 있는 경우 설정 "횟수 {0}/{1}" 과 같은 방식으로 사용
            if (args != null)
                localizedString.Arguments = args;

            localizedString.StringChanged += UpdateText;
            localizedString.RefreshString();
        }
        else
        {
            // 키 없으면 그냥 직접 문자열 적용
            targetText.text = keyOrValue;
        }
    }

    private void UpdateText(string value)
    {
        targetText.text = value;
    }
}
