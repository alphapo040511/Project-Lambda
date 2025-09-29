using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;


public enum ScreenType                      // 겹치는게 불가능한 UI
{
    None,
    Menu,
    GamePlay,
    Pause,
    GameOver
}

public enum OverlayType                     // 겹치는게 가능한 UI
{
    None,
    Letterbox,
    Dialog,
    Popup,
    Observation,
    Quest,
    Tooltip
}

[System.Serializable]
public class UIScreen
{
    public ScreenType screenType;
    public ScreenBase screen;
}

[System.Serializable]
public class UIOverlay
{
    public OverlayType overlayType;
    public ScreenBase overlay;
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
    
    private Dictionary<ScreenType, ScreenBase> screenDictionary = new Dictionary<ScreenType, ScreenBase>();

    [SerializeField] private List<UIOverlay> overlays = new List<UIOverlay>();

    private Dictionary<OverlayType, ScreenBase> overlayDictionary = new Dictionary<OverlayType, ScreenBase>();

    // 현재 활성화된 화면
    public ScreenType CurrentScreen { get; private set; } = ScreenType.None;


    private void InitializeScreens()
    {
        screenDictionary.Clear();

        foreach (UIScreen screen in screens)
        {
            screenDictionary[screen.screenType] = screen.screen;
            screenDictionary[screen.screenType].Init();
        }

        overlayDictionary.Clear();

        foreach (UIOverlay overlay in overlays)
        {
            overlayDictionary[overlay.overlayType] = overlay.overlay;
            overlayDictionary[overlay.overlayType].Init();
        }
    }

    #region ScreenUI

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
            screenDictionary[CurrentScreen].Hide();
        }

        if (screenDictionary.ContainsKey(screenType))
        {
            screenDictionary[screenType].Show();
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
            screenDictionary[CurrentScreen].Hide();
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
            screenDictionary[CurrentScreen].Hide();
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

    #endregion

    #region Overlay

    public void ShowOverlay(OverlayType overlayType)
    {
        if(overlayDictionary.ContainsKey(overlayType))
        {
            overlayDictionary[overlayType].Show();
        }
    }

    public void HideOverlay(OverlayType overlayType)
    {
        if (overlayDictionary.ContainsKey(overlayType))
        {
            overlayDictionary[overlayType].Hide();
        }
    }

    public void HideAllOverlay()
    {
        foreach (var overlay in overlayDictionary.Keys)
        {
            HideOverlay(overlay);
        }
    }
    #endregion

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
