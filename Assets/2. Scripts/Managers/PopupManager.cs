using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : SingletonMonoBehaviour<PopupManager>
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;                  // this(이 객체)를 T 형식으로 변환
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    
}
