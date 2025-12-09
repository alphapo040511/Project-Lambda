using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button titleButton;
    public Button lastSaveButton;
    public Button loadButton;

    private void OnEnable()
    {
        if(titleButton != null)
            titleButton.onClick.AddListener(OnExitClick);

        if (lastSaveButton != null)
            lastSaveButton.onClick.AddListener(LastSaveStartClick);

        if (loadButton != null)
            loadButton.onClick.AddListener(LoadDataButtonClick);
    }

    private void OnDisable()
    {
        if (titleButton != null)
            titleButton.onClick.RemoveListener(OnExitClick);

        if (lastSaveButton != null)
            lastSaveButton.onClick.RemoveListener(LastSaveStartClick);

        if (loadButton != null)
            loadButton.onClick.RemoveListener(LoadDataButtonClick);
    }

    public void OnExitClick()
    {
        PopupManager.Instance.ShowConfirmPopup(
            "Popup_QuitGame_Title",
            "Popup_Confirm",
            "Popup_Cancel",
            () => {
                GameManager.Instance.ChangeGameState(GameState.Menu);
                SceneManager.Instance.LoadMainMenu();
                UIManager.Instance.HideScreen();
            });
    }

    public async void LastSaveStartClick()
    {
        UIManager.Instance.HideScreen();

        if (SaveManager.MetaExists(0))      // 데이터가 존재하는 경우
        {
            SaveData save = await SaveManager.LoadAsync(0);
            SaveObjectLoader.Instance.SetSaveData(save);
            SceneManager.Instance.LoadScene(save.saveSceneName);
        }
        else                                        // 데이터가 없는 경우
        {
            SaveObjectLoader.Instance.SetSaveData(null);                        // 빈 데이터로 시작
            SceneManager.Instance.LoadSceneWithLoadingScreen("Level");          // Level 씬으로 이동
        }
    }

    public void LoadDataButtonClick()
    {
        GameManager.Instance.ChangeGameState(GameState.Load);
        UIManager.Instance.ShowScreen(ScreenType.Save);
    }
}
