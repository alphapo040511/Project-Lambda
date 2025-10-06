using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ToastMessageView : MonoBehaviour
{
    public LocalizeStringEvent tmp;

    public void SetText(string msg)
    {
        if (tmp != null)
        {
            tmp.StringReference.TableEntryReference = msg;
            tmp.RefreshString();
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
