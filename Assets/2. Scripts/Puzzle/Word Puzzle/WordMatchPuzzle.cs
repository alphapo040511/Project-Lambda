using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

public class WordMatchPuzzle : MonoBehaviour
{
    [Header("TMP Reference")]
    public TextMeshProUGUI textTMP;
    public TextMeshProUGUI markTMP;
    public TextMeshProUGUI countText;

    [Header("Word Settings")]
    public string answer = "WOOD";
    public List<string> words = new List<string>();
    [Tooltip("랜덤 세그먼트의 길이 (기본은 12)")]
    public int segmentLength = 12;

    [Header("Display Settings")]
    public int lineCount = 10;
    public int lineWidth = 2;

    [Header("Characters")]
    [Tooltip("랜덤 문자열 생성에 사용할 문자들")]
    [TextArea]
    public string symbolChars = "#%&*+=-_/<>[](){}^:.?,!@~|;";

    // 실제 사용할 문자들
    private List<int> wordIndex = new List<int>();
    private List<string> wordList = new List<string>();
    private int currentWordIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        InitialLine();
        ShowText();
        CheckAnswer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf) return;

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentWordIndex = Math.Clamp(currentWordIndex - lineWidth, 0, wordIndex.Count);
            ShowText();
            CheckAnswer();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentWordIndex = Math.Clamp(currentWordIndex - 1, 0, wordIndex.Count);
            ShowText();
            CheckAnswer();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentWordIndex = Math.Clamp(currentWordIndex + lineWidth, 0, wordIndex.Count - 1);
            ShowText();
            CheckAnswer();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentWordIndex = Math.Clamp(currentWordIndex + 1, 0, wordIndex.Count - 1);
            ShowText();
            CheckAnswer();
        }
    }
    
    void CheckAnswer()
    {
        int count = SameWord(words[currentWordIndex]);

        countText.text = $"Likeness = {count}";
    }

    int SameWord(string word)
    {
        int count = 0;
        for(int i = 0; i < word.Length; i++)
        {
            if (word[i] == answer[i])
            {
                count++;
            }
        }

        return count;
    }

    void ShowText()
    {
        string content = "";
        string mark = "";
        int index = wordIndex[currentWordIndex];

        for (int i = 0; i < wordList.Count; i++)
        {
            if(i == index)
            {
                content += $"<color=black>{wordList[i]}</color>";
                mark += $"<mark=#FFFFFFFF>{wordList[i]}</mark>";
            }
            else
            {
                content += wordList[i];
                mark += wordList[i];
            }
        }

        textTMP.text = content;
        markTMP.text = mark;
    }

    void InitialLine()
    {
        for(int i = 0; i < lineCount; i++)
        {
            for(int w = 0; w < lineWidth; w++)
            {
                wordList.Add(GetRandomAddress());
                CreateWord(i * lineWidth + w);                  // 총 개수
            }
        }
    }

    void CreateWord(int wordCount)
    {
        int forward = Random.Range(0, segmentLength) + 1;           // 앞부분 문자열 길이
        if(forward > 0)
        {
            wordList.Add(GetRandomSymbol(forward));
        }

        wordList.Add(words[wordCount]);                 // 실제 사용할 단어에 추가
        wordIndex.Add(wordList.Count - 1);              // 각 단어가 어느 위치에 있는 인덱스 저장

        if (segmentLength - forward > 0)
        {
            wordList.Add(GetRandomSymbol(segmentLength - forward)); // 뒷부분 생성
        }

        if(wordCount % 2 == 1)
        {
            wordList.Add("\n");
        }
    }

    string GetRandomSymbol(int length)
    {
        string symbol = "";
        for(int i = 0; i < length; i++)
        {
            symbol += symbolChars[Random.Range(0, symbolChars.Length)];
        }

        return symbol;
    }

    string GetRandomAddress()
    {
        int major = Random.Range(0xD0, 0xDF);
        int minor = Random.Range(0x000, 0xFFF);
        return $"   0x{major:X2}{minor:X3}   ";
    }
}
