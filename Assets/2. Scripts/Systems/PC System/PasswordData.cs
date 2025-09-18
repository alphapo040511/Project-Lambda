using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 비밀번호 정보 -> 필요에 따라 SO로 변경
[System.Serializable]
public class PasswordData
{
    public string password;
    public string enHint;                       // 영어 힌트
    public string koHint;                       // 한국어 힌트


    // 언어 설정에 맞는 힌트 텍스트 반환
    public string GetHint(Language language)
    {
        string hint = language switch
        {
            Language.en => enHint,
            Language.ko => koHint,
                _ => enHint
        };

        return hint;
    }

    public bool IsUnlocked(string input)
    {
        return input == password;
    }
}
