using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScreenBase : MonoBehaviour, IScreen
{
    public abstract void Hide();
    public abstract void Init();
    public abstract void Show();
}
