using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastMessageView : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void SetText(string msg)
    {
        if(tmp != null)
            tmp.text = msg;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
