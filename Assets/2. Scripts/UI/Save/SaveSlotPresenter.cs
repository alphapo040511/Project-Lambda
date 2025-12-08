using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SaveState
{ 
    Save,
    Load,
    Delete
}

public class SaveSlotPresenter : ScreenBase
{
    public Canvas canvas;
    public List<SaveSlotView> views = new List<SaveSlotView>();

    public SaveState isSaveState = SaveState.Save;

    public Button saveButton;
    public Button loadButton;
    public Button deleteButton;
    public Button closeButton;


    public override void Show()
    {
        ChangeSaveState(isSaveState);
        UpdateView();
        canvas.gameObject.SetActive(true);
        GameManager.Instance.ChangeGameState(GameState.Paused);         // 임시로 일시정지 상태로 변경 (상태 부분 정리 필요)
    }

    public override void Hide()
    {
        canvas.gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
    }

    public override void Init()
    {
        ViewInitialize();
        SetupEventListeners();
        ChangeSaveState(isSaveState);

        // 닫기 버튼 색상 변경
        var closeColor = closeButton.colors;
        closeColor.normalColor = Color.gray;
        closeButton.colors = closeColor;

        canvas.gameObject.SetActive(false);
    }

    void SetupEventListeners()
    {
        if(saveButton != null)
            saveButton.onClick.AddListener(() =>
            {
                ChangeSaveState(SaveState.Save);
            });

        if (loadButton != null)
            loadButton.onClick.AddListener(() =>
            {
                ChangeSaveState(SaveState.Load);
            });

        if (deleteButton != null)
            deleteButton.onClick.AddListener(() =>
            {
                ChangeSaveState(SaveState.Delete);
            });

        if (closeButton != null)
            closeButton.onClick.AddListener(() =>
            {
                Hide();
            });
    }

    void ViewInitialize()
    {
        for(int i = 0; i < views.Count; i++)
        {
            views[i].Init(i);
            views[i].onSlotClicked += OnClickSaveSlot;
        }
    }

    async void UpdateView()
    {
        for (int i = 0; i < views.Count; i++)
        {
            SaveMetadata meta = await SaveManager.MetaLoad(i);

            views[i].UIActiveSetting(meta != null);                     // 데이터 여부에 따른 UI 활성화 상태 변경

            if (meta != null)
            {
                views[i].UpdateUI(
                    GetThumbnail(meta.thumbnailPath),
                    meta.currentQuest,
                    meta.saveLocationName,
                    TimeFormatter.GetDateTimeToString(meta.saveTime),
                    TimeFormatter.FormatPlayTime(meta.playTime)
                    );
            }
            else
            {
                views[i].UpdateUI(
                   null,                            // 빈 이미지(기본 이미지 적용)
                   "Empty Slot",
                   "N/A",
                   "----/--/-- --:--:--",
                   "-:--\'--\""
                   );
            }
        }
    }

    // [Test] 클릭시 저장 상태에 기능 분리 필요
    void OnClickSaveSlot(int slotIndex)
    {
        Debug.Log($"[{SaveState.Save}]{slotIndex} 슬롯을 클릭하였습니다.");

        if(isSaveState == SaveState.Save && slotIndex != 0)                 // 저장하기 일 때 (0번은 자동저장)
        {
            Save(slotIndex);
        }
        else if(isSaveState == SaveState.Delete)
        {
            Delete(slotIndex);
        }
        else if (isSaveState == SaveState.Load)         // 불러오기 일 때
        {
            Load(slotIndex);
        }
    }

    async void Save(int slotIndex)
    {
        if (SaveManager.MetaExists(slotIndex))                          // 이미 파일이 존재하는 경우
        {
            PopupManager.Instance.ShowConfirmPopup(
                "Save_Overwrite_Warning",
                "Save_Save",
                "Popup_Cancel",
                async () => {
                    // 팝업 알림 띄우기
                    PopupManager.Instance.ShowConfirmPopup(await SaveManager.Save(slotIndex), "Popup_Confirm");
                    UpdateView();
                });
        }
        else
        {
            await SaveManager.Save(slotIndex);
            UpdateView();
        }
    }

    async void Load(int slotIndex)
    {
        if (SaveManager.MetaExists(slotIndex))      // 데이터가 존재하는 경우
        {
            SaveData save = await SaveManager.LoadAsync(slotIndex);
            Hide();
            SaveObjectLoader.Instance.SetSaveData(save);
            SceneManager.Instance.LoadSceneWithLoadingScreen(save.saveSceneName);
        }
        else                                        // 데이터가 없는 경우
        {
            Hide();
            SaveObjectLoader.Instance.SetSaveData(null);                        // 빈 데이터로 시작
            SceneManager.Instance.LoadSceneWithLoadingScreen("Level");          // Level 씬으로 이동
        }
    }

    void Delete(int slotIndex)
    {
        if (SaveManager.MetaExists(slotIndex))              // 이미 파일이 존재하는 경우
        {
            PopupManager.Instance.ShowConfirmPopup(
                "Save_Delete_Warning",
                "Save_Delete",
                "Popup_Cancel",
                async () => {
                    // 팝업 메세지 띄우기 (성공, 실패)
                    PopupManager.Instance.ShowConfirmPopup(await SaveManager.DeleteSaveFile(slotIndex), "Popup_Confirm");
                    UpdateView();
                });
        }
        else
        {
            Debug.LogWarning("삭제할 데이터가 없습니다.");
        }
    }

    void ChangeSaveState(SaveState newState)
    {
        isSaveState = newState;

        var saveColor = saveButton.colors;
        var loadColor = loadButton.colors;
        var deleteColor = deleteButton.colors;

        saveColor.normalColor = (isSaveState == SaveState.Save ? Color.white : Color.gray);
        loadColor.normalColor = (isSaveState == SaveState.Load ? Color.white : Color.gray);
        deleteColor.normalColor = (isSaveState == SaveState.Delete ? Color.white : Color.gray);

        saveButton.colors = saveColor;
        loadButton.colors = loadColor;
        deleteButton.colors = deleteColor;
    }

    Sprite GetThumbnail(string path)
    {
        Sprite thumbnail = Resources.Load<Sprite>(path);
        if(thumbnail == null)
        {
            Debug.Log($"썸네일을 찾을 수 없습니다. (경로 : {path})");
            return null;
        }

        return thumbnail;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < views.Count; i++)
        {
            views[i].onSlotClicked -= OnClickSaveSlot;
        }
    }
}
