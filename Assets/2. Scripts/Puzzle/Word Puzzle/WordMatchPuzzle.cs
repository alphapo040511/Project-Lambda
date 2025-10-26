using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Events;

public class WordMatchPuzzle : MonoBehaviour
{
    [Header("Componenet Reference")]
    public InteractionFocus interaction;
    public TextMeshProUGUI textTMP;
    public TextMeshProUGUI markTMP;
    public TextMeshProUGUI countText;

    [Header("Word Settings")]
    public WordPuzzleDataSO puzzleData;
    [Tooltip("랜덤 세그먼트의 길이")]
    public int segmentLength = 8;
    private int remainingCount;                 // 남은 시도 횟수
    private int wordToLine;

    [Header("Display Settings")]
    public int lineCount = 10;
    public int lineWidth = 2;

    [Header("Event Settings")]
    public UnityEvent onCleared;

    [Header("Characters")]
    [Tooltip("랜덤 문자열 생성에 사용할 문자들")]
    private string symbolChars = "#%&*+=-_/<>[](){}^:.?,!@~|;";

    // 실제 사용할 문자들
    private List<string> randomIndexWords = new List<string>();
    private List<string> wordList = new List<string>();
    private int currentWordIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        segmentLength = 16 - puzzleData.answer.Length;               // 최대 길이인 16에서 단어 길이만큼 제외
        remainingCount = puzzleData.tryChance;
        countText.text = $"TargetWord ... \n Likeness _ \n Remaining Count {remainingCount}";

        RandomIndexing();           // 랜덤 단어 목록 정리
        InitialLine();              // 출력 단어 목록 정리
        ShowText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf || !interaction.isFocused) return;

        if(Input.GetKeyDown(KeyCode.W))
        {
            currentWordIndex = Math.Clamp(currentWordIndex - wordToLine, 0, wordList.Count);
            ShowText();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentWordIndex = Math.Clamp(currentWordIndex - 1, 0, wordList.Count);
            ShowText();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            currentWordIndex = Math.Clamp(currentWordIndex + wordToLine, 0, wordList.Count - 1);
            ShowText();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentWordIndex = Math.Clamp(currentWordIndex + 1, 0, wordList.Count - 1);
            ShowText();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            CheckAnswer();
        }

    }
    
    void CheckAnswer()
    {
        if (remainingCount <= 0 || wordList[currentWordIndex].Length == 1) return;
        int count = SameWord(wordList[currentWordIndex]);
        remainingCount--;
        countText.text = $"TargetWord {wordList[currentWordIndex]}... \n Likeness {count} \n Remaining Count {remainingCount}";

        if(count == puzzleData.answer.Length)
        {
            PuzzleCleared();
        }
    }

    void PuzzleCleared()
    {
        onCleared?.Invoke();
        interaction.DisableInteraction();       // 상호작용 불가 상태로 변경
        interaction.ExitFocus();
        Debug.Log("퍼즐 클리어");
    }

    int SameWord(string word)
    {
        if (word.Length > puzzleData.answer.Length) return 0;

        int count = 0;

        word = word.ToUpper();
        string answer = puzzleData.answer.ToUpper();

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

        for (int i = 0; i < wordList.Count; i++)
        {
            if(i == currentWordIndex)
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

    void RandomIndexing()
    {
        List<string> tempList = new List<string>();
        tempList.Add(puzzleData.answer);                    // 정답 단어 추가
        tempList.AddRange(puzzleData.words);                // 표시 단어 목록 추가

        while(tempList.Count > 0)
        {
            int index = Random.Range(0, tempList.Count);
            randomIndexWords.Add(tempList[index]);        // 임시 리스트에서 랜덤으로 추가
            tempList.RemoveAt(index);
        }
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
            AddRandomSymbol(forward);
        }

        wordList.Add(GetRandomWord());                 // 실제 사용할 단어에 추가

        if (segmentLength - forward > 0)
        {
            AddRandomSymbol(segmentLength - forward); // 뒷부분 생성
        }

        if(wordCount % 2 == 1)
        {
            wordList[wordList.Count - 1] += "\n";
        }

        wordToLine = wordList.Count / lineCount;
    }

    void AddRandomSymbol(int length)
    {

        for(int i = 0; i < length; i++)
        {
            string symbol = "";
            symbol += symbolChars[Random.Range(0, symbolChars.Length)];
            wordList.Add(symbol);
        }
    }

    string GetRandomAddress()
    {
        int major = Random.Range(0xD0, 0xDF);
        int minor = Random.Range(0x000, 0xFFF);
        return $"   0x{major:X2}{minor:X3}   ";
    }

    string GetRandomWord()
    {
        if (randomIndexWords.Count < 0) return "Null";

        int index = Random.Range(0, randomIndexWords.Count);
        string word = randomIndexWords[index];
        randomIndexWords.RemoveAt(index);
        return word;
    }
}
