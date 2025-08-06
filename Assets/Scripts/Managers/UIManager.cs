using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ScreenType
{
    None,
    Menu,
    GamePlay,
    Pause,
    GameOver
}

[System.Serializable]
public class UIScreen
{
    public ScreenType screenType;
    public GameObject screenObject;
}

public class UIManager : SingletonMonoBehaviour<UIManager>
{ 
    private bool isWaiting = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeScreens();
    }

    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();
    
    private Dictionary<ScreenType, GameObject> screenDictionary = new Dictionary<ScreenType, GameObject>();


    // 현재 활성화된 화면
    public ScreenType CurrentScreen { get; private set; } = ScreenType.None;


    private void InitializeScreens()
    {
        screenDictionary.Clear();

        foreach (UIScreen screen in screens)
        {
            screenDictionary[screen.screenType] = screen.screenObject;
            screen.screenObject.SetActive(false);
        }
    }

    public void ShowScreen(ScreenType screenType)
    {
        if(screenType == ScreenType.None)
        {
            HideScreen();
            return;
        }


        //기존 화변 비활성화
        if (CurrentScreen != ScreenType.None && screenDictionary.ContainsKey(CurrentScreen))
        {
            screenDictionary[CurrentScreen].SetActive(false);
        }

        if (screenDictionary.ContainsKey(screenType))
        {
            screenDictionary[screenType].SetActive(true);
            CurrentScreen = screenType;
        }
        else
        {
            Debug.LogWarning("Screen " + screenType + " not found in UIManager!");
        }
    }

    public void HideScreen()
    {
        if (isWaiting) return;
        //기존 화변 비활성화
        if (CurrentScreen != ScreenType.None && screenDictionary.ContainsKey(CurrentScreen))
        {
            screenDictionary[CurrentScreen].SetActive(false);
            CurrentScreen = ScreenType.None;
        }
    }

    public void HideScreen(float timer)
    {
        if (isWaiting) return;
        StartCoroutine(HideScreenWait(timer));
    }

    private IEnumerator HideScreenWait(float timer)
    {
        isWaiting = true;

        yield return new WaitForSecondsRealtime(timer);

        //기존 화변 비활성화
        if (CurrentScreen != ScreenType.None && screenDictionary.ContainsKey(CurrentScreen))
        {
            screenDictionary[CurrentScreen].SetActive(false);
            CurrentScreen = ScreenType.None;
        }

        isWaiting = false;
    }

    public void AddOnScreen(UIScreen newScreen)
    {
        screens.Add(newScreen);
        InitializeScreens();
    }

    public void RemoveAtScreen(UIScreen screen)
    {
        screens.Remove(screen);
        InitializeScreens();
    }

    #region Quick Method
    public void Pause()
    {
        ShowScreen(ScreenType.Pause);
    }

    public void Resume()
    {
        if (CurrentScreen == ScreenType.Pause)
        {
            HideScreen();
        }
    }

    #endregion
}
