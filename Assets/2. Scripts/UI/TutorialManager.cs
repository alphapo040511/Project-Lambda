using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialType
{
    Move,
    Interaction,
    Look
}

public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
{
    [Header("UI References")]
    public GameObject movePanel;
    public GameObject interactionPanel;
    public GameObject lookPanel;

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
  
    public void Show(TutorialType type)
    {
        movePanel.SetActive(false);
        interactionPanel.SetActive(false);
        lookPanel.SetActive(false);

        switch (type)
        {
            case TutorialType.Move:
                movePanel.SetActive(true);
                break;
            case TutorialType.Interaction:
                interactionPanel.SetActive(true);
                break;
            case TutorialType.Look:
                lookPanel.SetActive(true);
                break;
        }

        UIManager.Instance.ShowOverlay(OverlayType.Tutorial);
    }

    public void Hide()
    {
        UIManager.Instance.HideOverlay(OverlayType.Tutorial);
    }

}
