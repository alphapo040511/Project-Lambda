using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePuzzleController : MonoBehaviour
{
    public event Action onPipeUpdated;
    public event Action onPipeReset;

    public void OnPipeUpdate()
    {
        onPipeUpdated?.Invoke();
    }

    public void OnPipeReset()
    {
        onPipeReset?.Invoke();
    }

    public void OnPuzzleComplete()
    {
        Debug.Log("퍼즐 완료");
    }
}
