using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupModel : MonoBehaviour
{
    public string Message { get; set; }
    public string ConfirmMessage { get; set; }
    public string CancelMessage { get; set; }
    public bool HasConfirmButton
    {
        get
        {
            return !string.IsNullOrEmpty(ConfirmMessage);
        }
    }

    public bool HasCancelButton
    {
        get
        {
            return !string.IsNullOrEmpty(ConfirmMessage);
        }
    }
}
