using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialView : MonoBehaviour
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
        //GameManager.Instance.ChangeGameState(GameState.Menu);
        UIManager.Instance.ShowOverlay(OverlayType.Tutorial);
    }

    public void Hide()
    {
        //GameManager.Instance.ResumeGame();
        UIManager.Instance.HideOverlay(OverlayType.Tutorial);
    }

}
