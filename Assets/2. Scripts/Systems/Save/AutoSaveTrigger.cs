using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class AutoSaveTrigger : SaveObject
{
    [Header("재사용 여부")]
    public bool resueable = false;
    private bool useable = true;

    public override void SetObjectState(string id, ObjectState state)
    {
        this.state = state;
        if (state == ObjectState.Used)
            useable = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && useable)
        {
            AutoSave();
            if(resueable == false)
            {
                useable = false;
                state = ObjectState.Used;
            }
        }
    }

    // 외부에서 저장 기능 호출 시 사용
    public async void AutoSave()
    {
        UIManager.Instance.ShowOverlay(OverlayType.AutoSave);

        Debug.Log("AutoSave...");

        string message = await SaveManager.Save(0);     // 0번 슬롯에 저장

        if (message == "Save_Success")
        {
            Debug.Log("저장 성공");
        }
        else
        {
            PopupManager.Instance.ShowConfirmPopup(message,
                "Popup_Confirm",
                onConfirm: () => SceneManager.Instance.LoadMainMenu()
            );
        }

        UIManager.Instance.HideOverlay(OverlayType.AutoSave);
    }
}
