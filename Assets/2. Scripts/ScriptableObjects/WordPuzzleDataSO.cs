using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newWordData", menuName = "Puzzle/Word Puzzle")]
public class WordPuzzleDataSO : ScriptableObject
{
    [Header("정답 단어")] public string answer = "Wood";
    [Header("표시 단어 목록(정답 단어 포함 X)")] public string[] words = new string[19];
    [Header("시도횟수")] public int tryChance = 5;
}
