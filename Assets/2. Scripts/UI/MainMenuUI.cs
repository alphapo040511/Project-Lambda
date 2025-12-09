using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button levelSceneButton;
    public Button startGameButton;
    public Button settingButton;
    public Button gameQuitButton;
    public Button startGameWithLoadingButton;
    public Button saveButton;

    // Start is called before the first frame update
    void Start()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(() => {
                SceneManager.Instance.LoadGameScene();
            });
        }

        if (startGameWithLoadingButton != null)
        {
            startGameWithLoadingButton.onClick.AddListener(() => {
                SceneManager.Instance.LoadSceneWithLoadingScreen("GameScene");
            });
        }

        if(levelSceneButton != null)
        {
            levelSceneButton.onClick.AddListener(() => {
                SceneManager.Instance.LoadSceneWithLoadingScreen("Level");
            });
        }

        if(saveButton  != null)
        {
            saveButton.onClick.AddListener(() =>{
                UIManager.Instance.ShowScreen(ScreenType.Save);
                GameManager.Instance.ChangeGameState(GameState.Load);   // 로드 상태로 변경
            });
        }

        if (settingButton != null)
        {
            settingButton.onClick.AddListener(() => {
                GameManager.Instance.PauseGame();
            });
        }

        if (gameQuitButton != null)
        {
            gameQuitButton.onClick.AddListener(() => {
                PopupManager.Instance.ShowConfirmPopup(
                    "Popup_QuitGame_Title",
                    "Popup_Confirm",
                    "Popup_Cancel",
                    () => Application.Quit()
                    );
            });
        }
    }

    
}
