using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : SingletonMonoBehaviour<TutorialView>
{
    [Header("UI References")]
    public TextMeshProUGUI moveDescriptionText;
    public TextMeshProUGUI lookAroundDescriptionText;
    public TextMeshProUGUI interactionDescriptionText;

    public Image moveImage;
    public Image interactionImage;
    public Image mouseImage;

    private TextLocalizer moveDescription;
    private TextLocalizer lookAroundDescription;
    private TextLocalizer interactionDescription;

    private const string tableName = "Tutorial Table";

    private void OnEnable()
    {
        GameEvents.OnChangeGameState += ChangeGameState;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= ChangeGameState;
    }

    void ChangeGameState(GameState state)
    {
        // 게임 상태 변경시 자동으로 꺼지도록 설정
        if (state == GameState.Menu || state == GameState.Loading)
            Hide();
    }

    public void Init()
    {
        LocalizerSetting();
    }

    void LocalizerSetting()
    {
        moveDescription = new TextLocalizer(moveDescriptionText, tableName);
        lookAroundDescription = new TextLocalizer(lookAroundDescriptionText, tableName);
        interactionDescription = new TextLocalizer(interactionDescriptionText, tableName);
    }
    public void Show()
    {
        UIManager.Instance.ShowOverlay(OverlayType.Tutorial);
    }

    public void Hide()
    {
        UIManager.Instance.HideOverlay(OverlayType.Tutorial);
    }

}
