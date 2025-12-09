using DG.Tweening;
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
    Look,
    wordPuzzle
}

public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
{
    [Header("UI References")]
    public GameObject movePanel;
    public GameObject interactionPanel;
    public GameObject lookPanel;
    public GameObject wordPuzzlePanel;

    private void Start()
    {
        movePanel.SetActive(false);
        interactionPanel.SetActive(false);
        lookPanel.SetActive(false);
        wordPuzzlePanel.SetActive(false);
    }

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

    private void SlideIn(GameObject panel)
    {
        RectTransform rectT = panel.GetComponent<RectTransform>();

        panel.SetActive(true);

        float startX = Screen.width + rectT.rect.width;

        Vector2 endPos = rectT.anchoredPosition;

        rectT.anchoredPosition = new Vector2(startX, endPos.y);

        rectT.DOAnchorPos(endPos, 0.35f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    private void SlideOut(params GameObject[] panels)
    {
        foreach (var panel in panels)
        {
            RectTransform rectT = panel.GetComponent<RectTransform>();

            Vector2 startPos = rectT.anchoredPosition;
            float endX = Screen.width + rectT.rect.width;
            Vector2 endPos = new Vector2(endX, startPos.y);

            rectT.DOAnchorPos(endPos, 0.35f)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    panel.SetActive(false);
                    rectT.anchoredPosition = startPos;
                });
        }
    }

    public void Show(TutorialType type)
    {
        switch (type)
        {
            case TutorialType.Move:
                movePanel.SetActive(true);
                SlideIn(movePanel);
                break;
            case TutorialType.Interaction:
                interactionPanel.SetActive(true);
                SlideIn(interactionPanel);
                break;
            case TutorialType.Look:
                lookPanel.SetActive(true);
                SlideIn(lookPanel);
                break;
            case TutorialType.wordPuzzle:
                wordPuzzlePanel.SetActive(true);
                SlideIn(wordPuzzlePanel);
                break;
        }

        UIManager.Instance.ShowOverlay(OverlayType.Tutorial);
    }

    public void Hide()
    {
        SlideOut();
        UIManager.Instance.HideOverlay(OverlayType.Tutorial);
    }

}
