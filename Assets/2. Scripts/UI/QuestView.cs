using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestView : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    // 테이블 이름 설정 (Inspector 또는 코드)
    [SerializeField] private string tableName = "Quest Table";

    // Localizer 저장
    private TextLocalizer title;
    private TextLocalizer description;

    private string descriptionKey;

    private void Start()
    {
        title = new TextLocalizer(titleText, tableName);
        description = new TextLocalizer(descriptionText, tableName);
    }

    public void Show(string titleKey, string descriptionKey)
    {
        this.descriptionKey = descriptionKey;

        if (title == null) title = new TextLocalizer(titleText, tableName);
        title.SetKey(titleKey);

        if(description == null) description = new TextLocalizer(descriptionText, tableName);
        description.SetKey(descriptionKey);

        gameObject.SetActive(true);         // 일단 연출 없이 활성화
    }

    public void UpdateProgress(int target, int current)
    {
        if(target != 1)
        {
            description.SetKey(descriptionKey, target, current);
        }
    }

    public void Hide()
    {
        UIManager.Instance.HideOverlay(OverlayType.Quest);
    }
}
