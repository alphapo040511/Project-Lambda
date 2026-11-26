
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public int maxTryCount = 5;
    private int remainingCount = 5;
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
    private string symbolChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    // 실제 사용할 문자들
    private string answer;
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

        if (state == ObjectState.On)
            CreateWordPuzzle();
    }

    public void CreateWordPuzzle()
    {
        if (isCreated) return;
        if (state == ObjectState.Used) return;

        isCreated = true;

        remainingCount = maxTryCount;
        currentWordIndex = 0;
        timer = 0;

        state = ObjectState.On;

        RandomIndexing();
        InitialLine();

        countText.text = "Selected -----\nLikeness □□□□□";
        remainingText.text = $"Remaining Count\n{GetRemaningText()}";

        ShowText();
    }

    private void Update()
    {
        if (state != ObjectState.On || interaction == null || !interaction.isFocused)
            return;
        bool isMoved = false;

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentWordIndex -= wordToLine;
            isMoved = true;
        }

        if (Input.GetKeyDown(KeyCode.S))
        { 
            currentWordIndex += wordToLine;
            isMoved = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            currentWordIndex--;
            isMoved = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        { 
            currentWordIndex++;
            isMoved = true;
        }

        if (isMoved && audioSource != null && selectSound != null)
            audioSource.PlayOneShot(selectSound);

        currentWordIndex = Mathf.Clamp(currentWordIndex, 0, wordList.Count - 1);

        if (Input.GetKeyDown(KeyCode.E) ||
            Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            CheckAnswer();
        }

        ShowText();
    }

    void CheckAnswer()
    {
        if (remainingCount <= 0)
        {
            interaction.ExitFocus();
            ResetPuzzle();
            return;
        }

        // 주소 문자열은 선택할 수 없음
        // wordList 구조: 주소(짝수 인덱스), 암호(홀수 인덱스)
        if (currentWordIndex % 2 == 0)
        {
            countText.text = "Invalid selection";
            return;
        }

        TutorialManager.Instance.Hide();

        string selectedWord = wordList[currentWordIndex];

        int count = SameWord(selectedWord);

        remainingCount--;

        countText.text =
            $"Selected {selectedWord}\nLikeness {CheckGuess(selectedWord)}";

        remainingText.text = $"Remaining Count\n{GetRemaningText()}";

        if (count == answer.Length)
        {
            PuzzleCleared();
        }
        else
        {
            if (audioSource != null && incorrectSound != null)
                audioSource.PlayOneShot(incorrectSound);

            if (remainingCount <= 0)
            {
                interaction.ExitFocus();
                ResetPuzzle();
                return;
            }
        }
    }

    void PuzzleCleared()
    {
        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);

        state = ObjectState.Used;

        interaction.DisableInteraction();
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
        if (word.Length != answer.Length)
            return 0;

        int count = 0;

        for (int i = 0; i < word.Length; i++)
        {
            if (word[i] == answer[i])
                count++;
        }

        return count;
    }

    void ShowText()
    {
        string content = "";
        string mark = "";

        for (int i = 0; i < wordList.Count; i++)
        {
            if (i == currentWordIndex)
            {
                content += $"<color=black>{wordList[i]}</color>";
                mark += $"<mark=#FFFFFFFF>{wordList[i]}</mark>";
            }
            else
            {
                content += wordList[i];
                mark += wordList[i];
            }

            // 두 번째 암호마다 줄바꿈
            // 실제 wordList 데이터에는 개행 문자를 추가하지 않음
            if (i % (lineWidth * 2) == lineWidth * 2 - 1)
            {
                content += "\n";
                mark += "\n";
            }
        }

        textTMP.text = content;
        markTMP.text = mark;
    }

    void RandomIndexing()
    {
        randomIndexWords.Clear();

        // 정답 생성
        answer = GeneratePassword();

        randomIndexWords.Add(answer);

        // 정답을 제외한 후보 생성
        int targetCount = lineCount * lineWidth;

        while (randomIndexWords.Count < targetCount)
        {
            // 정답과 같은 위치에 있는 문자 개수: 0~4
            int matchCount = Random.Range(0, 5);

            string hint = GenerateHint(answer, matchCount);

            if (!randomIndexWords.Contains(hint))
                randomIndexWords.Add(hint);
        }

        // 후보 순서 섞기
        for (int i = 0; i < randomIndexWords.Count; i++)
        {
            int index = Random.Range(i, randomIndexWords.Count);

            (randomIndexWords[i], randomIndexWords[index]) =
                (randomIndexWords[index], randomIndexWords[i]);
        }
    }

    string GeneratePassword()
    {
        string password = "";

        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, symbolChars.Length);
            password += symbolChars[index];
        }

        return password;
    }

    string GenerateHint(string target, int matchCount)
    {
        char[] hint = target.ToCharArray();

        List<int> indices = new List<int>();

        for (int i = 0; i < target.Length; i++)
            indices.Add(i);

        // 위치를 무작위로 섞기
        for (int i = 0; i < indices.Count; i++)
        {
            int randomIndex = Random.Range(i, indices.Count);

            (indices[i], indices[randomIndex]) =
                (indices[randomIndex], indices[i]);
        }

        // 지정한 일치 개수만큼 유지하고 나머지 변경
        for (int i = matchCount; i < indices.Count; i++)
        {
            int index = indices[i];

            char newChar;

            do
            {
                newChar = symbolChars[
                    Random.Range(0, symbolChars.Length)
                ];
            }
            while (newChar == target[index]);

            hint[index] = newChar;
        }

        return new string(hint);
    }

    void InitialLine()
    {
        wordList.Clear();

        for (int i = 0; i < lineCount; i++)
        {
            for (int w = 0; w < lineWidth; w++)
            {
                wordList.Add(GetRandomAddress());

                CreateWord(i * lineWidth + w);
            }
        }
    }

    void CreateWord(int wordCount)
    {
        wordList.Add(GetRandomWord());

        // 한 줄에 들어가는 전체 항목 수
        // 주소 + 암호가 한 묶음
        wordToLine = lineWidth * 2;
    }

    string GetRandomAddress()
    {
        int major = Random.Range(0xD0, 0xDF);
        int minor = Random.Range(0x00, 0xFF);

        return $"  0x{major:X2}{minor:X2}  ";
    }

    string GetRandomWord()
    {
        if (randomIndexWords.Count <= 0)
            return "Null";

        int index = Random.Range(0, randomIndexWords.Count);

        string word = randomIndexWords[index];

        randomIndexWords.RemoveAt(index);

        return word;
    }

    string CheckGuess(string guess)
    {
        if (guess.Length != answer.Length)
            return "Invalid";

        string[] result = new string[guess.Length];
        bool[] used = new bool[answer.Length];

        // 1단계: 완전 일치
        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == answer[i])
            {
                result[i] = "■";
                used[i] = true;
            }
        }

        // 2단계: 다른 위치에 존재
        for (int i = 0; i < guess.Length; i++)
        {
            if (result[i] != null)
                continue;

            bool found = false;

            for (int j = 0; j < answer.Length; j++)
            {
                if (!used[j] && guess[i] == answer[j])
                {
                    found = true;
                    used[j] = true;
                    break;
                }
            }

            result[i] = found
                ? "<color=#888888>■</color>"
                : "□";
        }

        return string.Join("", result);
    }

    string GetRemaningText()
    {
        string remaingText = "";

        for (int i = 0; i < maxTryCount; i++)
        {
            remaingText += i < remainingCount ? '■' : '□';
        }

        remaingText = $"<size=180%>{remaingText}</size>";

        return remaingText;
    }

    public void ShowTutorial()
    {
        TutorialManager.Instance.Show(TutorialType.wordPuzzle);
    }
}
