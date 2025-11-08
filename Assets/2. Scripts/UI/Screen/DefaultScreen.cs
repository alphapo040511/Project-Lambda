using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultScreen : ScreenBase
{
    public Canvas canvas;

    public override void Hide()
    {
        canvas.gameObject.SetActive(false);
    }

    public override void Init()
    {
        canvas.gameObject.SetActive(false);
    }

    public override void Show()
    {
        canvas.gameObject.SetActive(true);
    }
}
