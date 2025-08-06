using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class InteractionUIManager : SingletonMonoBehaviour<InteractionUIManager>
{
    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this;                       // 게임 씬에서만 사용하므로 DontDestroyOnLoad 제외
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    public InteractionUIView UIPrefab;

    public InteractionUIView CreatingInteractionUI(Interactable targetObject)
    {
        InteractionUIView view = Instantiate(UIPrefab, transform);
        view.Initialize(targetObject);
        return view;
    }
}
