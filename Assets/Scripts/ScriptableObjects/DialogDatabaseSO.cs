using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogDatabase", menuName = "Dialog System/Database")]

public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<string, DialogSO> dialogsById;                                   //캐싱을 위한 딕셔너리 사용

    public void Initialize()
    {
        dialogsById = new Dictionary<string, DialogSO>();

        foreach (var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogsById[dialog.id] = dialog;
            }
        }
    }

    public DialogSO GetDialogById(string id)
    {
        if (dialogsById == null)
        {
            Initialize();
        }

        if (dialogsById.ContainsKey(id))
        {
            return dialogsById[id];
        }
        else
        {
            Debug.LogWarning($"ID:{id} DialogSO 파일을 찾을 수 없습니다.");
            return null;
        }
    }
}
