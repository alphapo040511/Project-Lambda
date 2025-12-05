using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Cinemachine.DocumentationSortingAttribute;

public enum MemoryPuzzleState
{
    Idle,       // 기본 상태
    Show,       // 정답을 보여주는 상태
    Select,     // 답 선택
    Error,      // 틀린 답
    Success,    // 정답
    Clear       // 클리어
}

public class MemoryPuzzleController : InteractionFocus
{
    [Header("Puzzle Complete Event")]
    public UnityEvent onPuzzleComplete;

    [Header("Present Settings")]
    public float presentDelay = 1f;
    private float timer = 0;

    [Header("References")]
    public List<MemotyPuzzleMonitor> monitors = new List<MemotyPuzzleMonitor>();
    public List<Sprite> icons = new List<Sprite>();

    [Header("Difficult Settings")]
    public float[] interval = new float[3] { 1f, 0.8f, 0.6f };
    public int[] answerLength = new int[3] { 3, 5, 7 };

    private MemoryPuzzleState currentState = MemoryPuzzleState.Idle;        // 퍼즐 상태

    private int puzzleLevel = 0;                                            // 퍼즐의 레벨 
    private List<int> puzzleAnswer = new List<int>();                       // 현재 레벨의 정답 순서
    private Queue<int> answerQueue = new Queue<int>();                      // 입력 순서 큐



    protected override void Update()
    {
        base.Update();

        if (currentState == MemoryPuzzleState.Idle)            // 포커싱 되지 않았다면
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = presentDelay;
                ShowIdleIcons();
            }
        }
        else if(currentState == MemoryPuzzleState.Error)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(ShowPuzzle(interval[puzzleLevel], puzzleLevel));
            }
        }
        else if(currentState == MemoryPuzzleState.Success)
        {
            if(timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                puzzleLevel++;
                NextPuzzle(puzzleLevel);
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
        puzzleLevel = 0;
        NextPuzzle(puzzleLevel);
    }

    public void SelectDisplay(int index)
    {
        if(answerQueue.Count == 0) return;

        if(index == answerQueue.Dequeue())              // 현재 순서에 맞는 인덱스라면
        {
            monitors[index].SetColor(Color.green);

            if (answerQueue.Count == 0)                 // 맞는 순서고, 남은 답이 없는 경우
            {
                if (puzzleLevel == 2)
                {
                    // 정답처리
                    PuzzleClear();
                }
                else
                {
                    AllIconsSetColor(Color.green);

                    currentState = MemoryPuzzleState.Success;
                    timer = 1f;
                }
            }
        }
        else
        {
            AllIconsSetColor(Color.red);

            timer = 1f;
            currentState = MemoryPuzzleState.Error;
        }
    }

    void PuzzleClear()
    {
        state = ObjectState.Disable;
        currentState = MemoryPuzzleState.Clear;

        AllIconsSetColor(Color.green);

        ExitFocus();
        interactable = false;

        onPuzzleComplete?.Invoke();
    }

    void NextPuzzle(int level = 0)
    {
        puzzleAnswer = GetRandomIndexList();
        icons = GetRandomIconList();
        StartCoroutine(ShowPuzzle(interval[puzzleLevel], level));
    }

    IEnumerator ShowPuzzle(float interval = 1f, int level = 0)
    {
        currentState = MemoryPuzzleState.Show;
        AllIconsSetColor(Color.clear);
        answerQueue.Clear();                                // 정답 비교용 큐에 초기화

        yield return new WaitForSeconds(1f);

        for(int i = 0; i < 9; i++)
        {
            if (i < answerLength[level])        // 문제 횟수 이내라면
            {
                answerQueue.Enqueue(puzzleAnswer[i]);           // 정답 비교용 큐에 삽입
                monitors[puzzleAnswer[i]].SetIcon(icons[i]);
                monitors[puzzleAnswer[i]].SetColor(Color.white);

                yield return new WaitForSeconds(interval);

                monitors[puzzleAnswer[i]].SetColor(Color.clear);

                yield return new WaitForSeconds(interval * 0.5f);
            }
            else
            {
                // 일반 아이콘
                monitors[puzzleAnswer[i]].SetIcon(icons[i]);
                monitors[puzzleAnswer[i]].SetColor(Color.clear);
            }

        }

        AllIconsSetColor(Color.white);

        currentState = MemoryPuzzleState.Select;
    }

    void ShowIdleIcons()
    {
        if (monitors.Count != 9 || icons.Count != 9) return;        // 9개인 경우만 적용

        icons = GetRandomIconList();
        for (int i = 0; i < 9; i++)
        {
            monitors[i].SetIcon(icons[i]);
        }
        AllIconsSetColor(Color.white);
    }

    void AllIconsSetColor(Color color)
    {
        for (int i = 0; i < 9; i++)
        {
            monitors[i].SetColor(color);
        }
    }

    public bool CanInteractMonitor()
    {
        return currentState == MemoryPuzzleState.Select;
    }

    List<Sprite> GetRandomIconList()
    {
        List<Sprite> list = new List<Sprite>(icons.ToArray());
        List<Sprite> newIcons = new List<Sprite>();

        while (list.Count > 0)
        {
            Sprite icon = list[Random.Range(0, list.Count)];
            newIcons.Add(icon);
            list.Remove(icon);
        }

        return newIcons;
    }

    List<int> GetRandomIndexList()
    {
        List<int> reference = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        List<int> rand = new List<int>();
        while(reference.Count > 0)
        {
            int i = reference[Random.Range(0, reference.Count)];
            rand.Add(i);
            reference.Remove(i);
        }

        return rand;
    }

    public override void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;
        this.state = state;

        if (state == ObjectState.Default)
        {
            currentState = MemoryPuzzleState.Idle;
        }

        if (state == ObjectState.Used)
        {
            PuzzleClear();
        }
    }
}
