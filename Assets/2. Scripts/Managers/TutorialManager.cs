using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SingletonMonoBehaviour<TutorialManager>
{
    public TutorialView view;

    // Start is called before the first frame update
    private void Start()
    {
        if (view != null)
        {
            view.Init();
        }
    }

    public void ShowMoveDestciption(string moveDescription)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
