using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastMessageView : MonoBehaviour
{
    public TextMeshProUGUI tmp;

    public void ShowMessage(string message)
    {
        if(tmp != null)
        {
            tmp.text = message;
        }
    }

    public void HideMessage()
    {
        // 일단 파괴
        Destroy(gameObject);
    }

    public void HideMessage(float lifeTime)
    {
        Destroy(gameObject, lifeTime);
    }
}
