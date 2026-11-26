using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPlayer : MonoBehaviour
{
    public DialogSO dialog;
    public void Play()
    {
        if(dialog != null)
        {
            DialogueManager.Instance.PlayingDialog(dialog.id);
        }
    }
}
