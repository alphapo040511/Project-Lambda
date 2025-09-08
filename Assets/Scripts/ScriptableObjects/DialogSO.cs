using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/Dialog")]

public class DialogSO : ScriptableObject
{
    public string id;
    public string en;
    public string ko;
    public AudioClip clip;
    public string nextId;

    public string GetLocalizedText(Language language)
    {
        string text = language switch
        {
            Language.en => en,
            Language.ko => ko,
            _ => en,
        };

        return en;
    }
}

