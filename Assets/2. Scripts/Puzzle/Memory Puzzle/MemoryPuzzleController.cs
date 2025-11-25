using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MemoryPuzzleState
{
    Idle,       // 기본 상태
    Show,       // 정답을 보여주는 상태
    Select,     // 답 선택
    Error,      // 틀린 답
    Success     // 정답
}

public class MemoryPuzzleController : InteractionFocus
{
    [Header("Present Settings")]
    public float presentDelay = 1f;
    private float timer = 0;

    [Header("References")]
    public List<MemotyPuzzleMonitor> monitors = new List<MemotyPuzzleMonitor>();
    public List<Sprite> icons = new List<Sprite>();

    private MemoryPuzzleState currentState = MemoryPuzzleState.Idle;        // 퍼즐 상태

    private int puzzleLevel = 0;                                            // 퍼즐의 레벨 
    private List<int> puzzleAnswer = new List<int>();                       // 현재 레벨의 정답 순서
    private Queue<int> answerQueue = new Queue<int>();                      // 입력 순서 큐

    private float[] interval = new float[3] { 1f, 0.8f, 0.6f };

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
    }

    protected override void Complete()
    {
        base.Complete();
        NextPuzzle();
        
    }

    public bool SelectDisplay(int index)
    {
        if(answerQueue.Count == 0) return false;

        if(index == answerQueue.Dequeue())              // 현재 순서에 맞는 인덱스라면
        {
            if (answerQueue.Count == 0)                 // 맞는 순서고, 남은 답이 없는 경우
            {
                // 정답처리
                Success();
            }

            return true;
        }

        return false;
    }

    void Success()
    {

    }

    void NextPuzzle()
    {
        currentState = MemoryPuzzleState.Show;
        ClearIcons();
        puzzleAnswer = GetRandomIndexList();
        StartCoroutine(ShowPuzzle(interval[puzzleLevel]));
    }

    IEnumerator ShowPuzzle(float interval = 1f)
    {
        answerQueue.Clear();                                // 정답 비교용 큐에 초기화

        yield return new WaitForSeconds(interval);

        for(int i = 0; i < 9; i++)
        {
            answerQueue.Enqueue(puzzleAnswer[i]);           // 정답 비교용 큐에 삽입
            monitors[i].SetIcon(icons[i]);
            monitors[i].SetColor(Color.blue);
        }
    }

    void ShowIdleIcons()
    {
        if (monitors.Count != 9 || icons.Count != 9) return;        // 9개인 경우만 적용

        List<int> rand = GetRandomIndexList();
        for (int i = 0; i < 9; i++)
        {
            monitors[i].SetIcon(icons[rand[i]]);
            monitors[i].SetColor(Color.white);
        }
    }

    void ClearIcons()
    {
        for (int i = 0; i < 9; i++)
        {
            monitors[i].SetColor(Color.clear);
        }
    }

    public bool CanInteractMonitor()
    {
        return true;        // 나중에 인터렉션 조건 추가
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
}
