using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotPresenter : ScreenBase
{
    public Canvas canvas;
    public List<SaveSlotView> views = new List<SaveSlotView>();

    public override void Show()
    {
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
        canvas.gameObject.SetActive(false);
    }

    void ViewInitialize()
    {
        for(int i = 0; i < views.Count; i++)
        {
            views[i].Init(i);
            views[i].onSlotClicked += OnClickSaveSlot;
        }
    }

    void UpdateView()
    {
        for (int i = 0; i < views.Count; i++)
        {
            SaveMetadata meta = SaveManager.MetaLoad(i);
            if(meta != null)
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
                   "Save Empty",
                   "N/A",
                   "----/--/-- --:--:--",
                   "--:--:--"
                   );
            }
        }
    }

    void OnClickSaveSlot(int slotIndex)
    {
        Debug.Log($"{slotIndex} 슬롯을 클릭하였습니다.");
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
