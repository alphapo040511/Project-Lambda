using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 상태
public enum GameState
{
    Menu,
    Playing,
    Paused,
    GameOver,
    Cutscene,
    CutscenePause,
    Loading
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Game Setting")]
    public GameState currentGameState = GameState.Menu;
    private CursorLockMode lastCursorMode = CursorLockMode.None;
    public bool isGamePaused = false;

    [Header("Game Stats")]
    public float gameTime = 0f;

    private GameState previousGameState;

    protected override void Awake()
    {
        base.Awake();                   // SingletonMonoBehaviour의 싱글톤 선언
        InitializeGame();
    }

    private void Start()
    {
        // 다른 매니저들이 초기화된 후 실행
        StartCoroutine(InitializeManagers());
    }

    private void Update()
    {
        if (currentGameState == GameState.Playing && !isGamePaused)
        {
            gameTime += Time.deltaTime;
        }

        HandleInput();
    }

    private void InitializeGame()
    {
        // 게임 시작 시 기본 설정
        Application.targetFrameRate = 60;                       // 게임 프레임 제한
        Screen.sleepTimeout = SleepTimeout.NeverSleep;          // 자동으로 화면이 꺼지지 않도록 설정

        Debug.Log("GameManager 초기화 완료");
    }

    private IEnumerator InitializeManagers()
    {
        yield return new WaitForEndOfFrame();                   // 현재 프레임 랜더링이 끝난 후 다른 매니저 확인

        // 매니저들 초기화 순서 중요
        if (SceneManager.Instance != null)
            Debug.Log("SceneManager 연결 확인");
        //if (SoundManager.Instance != null)
        //    Debug.Log("SoundManager 연결 확인");
        //if (UIManager.Instance != null)
        //    Debug.Log("UIManager 연결 확인");
        //if (SaveManager.Instance != null)
        //    Debug.Log("SaveManager 연결 확인");
    }


    private void HandleInput()
    {
        // ESC 키로 게임 일시정지/재개
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentGameState == GameState.Playing || currentGameState == GameState.Cutscene)
            {
                PauseGame();
            }
            else if (currentGameState == GameState.Paused || currentGameState == GameState.CutscenePause)
            {
                ResumeGame();
            }
        }
    }

    //게임 상태 관리 영역
    #region Game State Management

    public void ChangeGameState(GameState newState)
    {
        if (currentGameState == newState) return;

        if(currentGameState == GameState.Playing)
        {
            lastCursorMode = Cursor.lockState;
        }

        previousGameState = currentGameState;
        currentGameState = newState;

        OnGameStateChanged(newState);

        Debug.Log($"게임 상태 변경: {previousGameState} -> {currentGameState}");
    }

    private void OnGameStateChanged(GameState newState)
    {
        // 일단 마우스만 관리 (혹시 몰라서 case 분리)
        switch (newState)
        {
            case GameState.Menu:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Playing:
                if(currentGameState == GameState.CutscenePause)             // 컷씬 중 Pause 상태였다면
                {
                    newState = GameState.Paused;                            // 일반 Pause 상태로 변경
                }
                else
                {
                    Cursor.lockState = lastCursorMode;
                }
                break;
            case GameState.Paused:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Loading:
                Cursor.lockState = CursorLockMode.None;
                break;
            case GameState.Cutscene:
                if (currentGameState == GameState.Paused)                   // Pause 상태에서 컷씬으로 변경 될 때
                {
                    newState = GameState.CutscenePause;                     // CutscenePause 상태로 변경
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                break;
        }

        GameEvents.ChangeGameState(newState);
    }

    public void StartGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
        GameEvents.GameResumed();
    }

    public void PauseGame()
    {
        if (currentGameState != GameState.Playing && currentGameState != GameState.Cutscene) return;

        isGamePaused = true;
        ChangeGameState(GameState.Paused);
        GameEvents.GamePaused();

        UIManager.Instance.Pause();
    }

    public void ResumeGame()
    {
        if (currentGameState != GameState.Paused && currentGameState != GameState.CutscenePause) return;

        isGamePaused = false;
        ChangeGameState(currentGameState == GameState.Paused ? GameState.Playing : GameState.Cutscene);                 // 컷씬중 정지 였다면 컷씬으로
        GameEvents.GameResumed();

        UIManager.Instance.Resume();
    }

    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);

        // 게임 오버 처리
        //SaveManager.Instance?. 점수 등 저장

        //UIManager.Instance?. 게임오버 UI 표시
    }

    public void RestartGame()
    {
        ResetGameStats();
        ChangeGameState(GameState.Playing);
    }

    public void GoToMainMenu()
    {
        ChangeGameState(GameState.Menu);
        //SceneManager.Instance? 메인 씬 로드
    }

    #endregion

    private void ResetGameStats()
    {
        gameTime = 0f;                         
        // 게임 시간 또는 점수 등 초기화
        // GameEvents. 점수 변경 이벤트 등 호출
    }

    //유틸리티 영역
    #region Utility Methods

    //플레이 중인지 확인
    public bool IsGamePlaying()
    {
        return currentGameState == GameState.Playing && !isGamePaused;      //현재 상태가 Playing이고, 게임이 정지 중이 아닐때
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;                // 에디터에서는 플레이 종료
#else
        Application.Quit();                                             // 이 외의 경우는 게임 종료
#endif
    }

    #endregion

    // 게임이 일시정지 될 때 자동으로 호출
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }

    // 포커스를 받거나 잃을 때 자동으로 호출
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && currentGameState == GameState.Playing)
        {
            PauseGame();
        }
    }
}
