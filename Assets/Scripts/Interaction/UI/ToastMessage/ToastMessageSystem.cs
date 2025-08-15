using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMessageSystem : SingletonMonoBehaviour<ToastMessageSystem>
{
    public ToastMessageView viewPrefab;

    protected override void Awake()
    {
        base.Awake();

        if(GetComponent<Canvas>() != null)
        {
            
        }
    }

    public void ShowMessage(string message)
    {
        ToastMessageView view = Instantiate(viewPrefab,transform);

        view.ShowMessage(message);
    }
}
