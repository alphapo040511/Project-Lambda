using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class WordMatchPuzzle : SaveObject
{
    [Header("Componenet Reference")]
    public InteractionFocus interaction;
    public TextMeshProUGUI textTMP;
    public TextMeshProUGUI markTMP;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI remainingText;

    [Header("Word Settings")]
    public WordPuzzleDataSO puzzleData;
    public int maxLength = 8;
    [Tooltip("랜덤 세그먼트의 길이")]
    public int segmentLength = 8;
    public int maxTryCount = 5;
    private int remainingCount = 5;                 // 남은 시도 횟수
    private int wordToLine;

    [Header("Display Settings")]
    public int lineCount = 10;
    public int lineWidth = 2;

    [Header("Event Settings")]
    public UnityEvent onCleared;

    [Header("Sound Settings")]
    public AudioSource audioSource;
    public AudioClip selectSound;
    public AudioClip incorrectSound;
    public AudioClip successSound;

    [Header("Characters")]
    [Tooltip("랜덤 문자열 생성에 사용할 문자들")]
    private string symbolChars = "#%&*+=-_/<>[](){}^:.?,!@~|;";

    // 실제 사용할 문자들
    private List<string> randomIndexWords = new List<string>();
    private List<string> wordList = new List<string>();
    private int currentWordIndex = 0;

    float holdInterval = 0.1f;
    float timer = 0;

    bool isCreated = false;

    public override void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;
        this.state = state;

        if (state == ObjectState.On)                    // On 상태인 경우 퍼즐 생성
            CreateWordPuzzle();
    }

    public void CreateWordPuzzle()
    {
        if (isCreated) return;                          // 이미 만들어진 경우 일단 무시
        isCreated = true;

        if (state == ObjectState.Used) return;          // 사용(클리어)된 경우 새로 생성 X

        remainingCount = maxTryCount;

        state = ObjectState.On;
        segmentLength = maxLength - puzzleData.answer.Length;               // 최대 길이에서 단어 길이만큼 제외ingCount = puzzleData.tryChance;

        string testAnswer = puzzleData.words[Random.Range(0, puzzleData.words.Length)];

        countText.text = $"TargetWord {testAnswer}\nLikeness {CheckGuess(testAnswer)}";
        remainingText.text = $"Remaining Count\n{GetRemaningText()}";

        RandomIndexing();           // 랜덤 단어 목록 정리
        InitialLine();              // 출력 단어 목록 정리
        ShowText();
    }

    void Update()
    {
        if (!gameObject.activeSelf || !interaction.isFocused) return;

        Vector2Int dir = Vector2Int.zero;

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                dir += Vector2Int.down * wordToLine;
            }

            if (Input.GetKey(KeyCode.A))
            {
                dir += Vector2Int.left;
            }

            if (Input.GetKey(KeyCode.S))
            {
                dir += Vector2Int.up * wordToLine;
            }

            if (Input.GetKey(KeyCode.D))
            {
                dir += Vector2Int.right;
            }

            if(dir != Vector2Int.zero)
            {
                timer = holdInterval;
                currentWordIndex = Mathf.Clamp(currentWordIndex + dir.x + dir.y, 0, wordList.Count - 1);

                if (audioSource != null && selectSound != null)
                    audioSource.PlayOneShot(selectSound);

                ShowText();
            }
        }
        

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            CheckAnswer();
        }

    }
    
    void CheckAnswer()
    {
        if (remainingCount <= 0)
        {
            interaction.ExitFocus();
            ResetPuzzle();
            return;
        }

        if (wordList[currentWordIndex].Length == 1)
        {
            countText.text = "Invalid selection";
            return;
        }

        TutorialManager.Instance.Hide();

        int count = SameWord(wordList[currentWordIndex]);
        remainingCount--;
        countText.text = $"TargetWord {wordList[currentWordIndex]}\nLikeness {CheckGuess(wordList[currentWordIndex])}";
        remainingText.text = $"Remaining Count\n{GetRemaningText()}";

        if (count == puzzleData.answer.Length)
        {
            PuzzleCleared();
        }
        else
        {
            if (audioSource != null && incorrectSound != null)
                audioSource.PlayOneShot(incorrectSound);

            if (remainingCount <= 0)        // 틀리고 횟수도 다 사용한 경우
            {
                interaction.ExitFocus();
                ResetPuzzle();
                return;
            }
        }
    }

    void PuzzleCleared()
    {
        if(audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        state = ObjectState.Used;
        interaction.DisableInteraction();       // 상호작용 불가 상태로 변경
        interaction.ExitFocus();
        onCleared?.Invoke();
        Debug.Log("퍼즐 클리어");
    }

    void ResetPuzzle()
    {
        randomIndexWords.Clear();
        wordList.Clear();
        currentWordIndex = 0;
        isCreated = false;
        CreateWordPuzzle();
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
        int minor = Random.Range(0x00, 0xFF);
        return $"  0x{major:X2}{minor:X2}  ";
    }

    string GetRandomWord()
    {
        if (randomIndexWords.Count < 0) return "Null";

        int index = Random.Range(0, randomIndexWords.Count);
        string word = randomIndexWords[index];
        randomIndexWords.RemoveAt(index);
        return word;
    }

    string CheckGuess(string guess)
    {
        if (puzzleData.answer.Length != guess.Length)
            Debug.Log("문자열 길이가 같아야 합니다.");

        char[] result = new char[guess.Length];
        bool[] used = new bool[puzzleData.answer.Length];

        // 1단계: 완전 일치 (■)
        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == puzzleData.answer[i])
            {
                result[i] = '■';
                used[i] = true;
            }
        }

        // 2단계: 다른 위치에 존재 (▣)
        for (int i = 0; i < guess.Length; i++)
        {
            if (result[i] == '■')
                continue;

            bool found = false;

            for (int j = 0; j < puzzleData.answer.Length; j++)
            {
                if (!used[j] && guess[i] == puzzleData.answer[j])
                {
                    found = true;
                    used[j] = true;
                    break;
                }
            }

            result[i] = found ? '▲' : '□';
        }

        return new string(result);
    }

    string GetRemaningText()
    {
        string remaingText = "";
        for(int i = 0; i < maxTryCount; i++)
        {
            remaingText += i < remainingCount ? '■' : '□';
        }

        return remaingText;
    }

    public void ShowTutorial()
    {
        TutorialManager.Instance.Show(TutorialType.wordPuzzle);
    }
}
