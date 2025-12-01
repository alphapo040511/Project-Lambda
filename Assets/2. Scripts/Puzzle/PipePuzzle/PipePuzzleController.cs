using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PipePuzzleController : InteractionFocus
{
    [Header("Puzzle Settings")]
    public List<PipeBase> pipes = new List<PipeBase>();
    public UnityEvent onPuzzleComplete;
    public bool isCleard { get; private set; } = false;

    private Dictionary<Vector2Int, PipeBase> grid = new Dictionary<Vector2Int, PipeBase>();

    public event Action onPipeUpdated;
    public event Action onPipeReset;

    private List<Vector2Int> dirs = new List<Vector2Int> { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

    private void Start()
    {
        GridInit();
    }

    void GridInit()
    {
        // 파이프 각
        foreach (PipeBase pipe in pipes)
        {
            pipe.InitializePipe(this);
            if (grid.ContainsKey(pipe.gridPos))
            {
                Debug.LogWarning($"중복된 위치에 파이프가 있습니다. (Position : {pipe.gridPos})");
                return;
            }
            grid[pipe.gridPos] = pipe;
        }

        // 이웃 파이프 설정
        foreach(var pipe in grid)
        {
            foreach(Vector2Int dir in dirs)
            {
                Vector2Int targetPos = pipe.Key + dir;
                if (grid.ContainsKey(targetPos))
                {
                    pipe.Value.neighbors.Add(dir, grid[targetPos]);     // 이웃 딕셔너리에 추가
                }
            }
        }
    }

    protected override void Complete()
    {
        base.Complete();
        OnPipeReset();
    }

    public void OnPipeUpdate()
    {
        onPipeUpdated?.Invoke();
    }

    public void OnPipeReset()
    {
        onPipeReset?.Invoke();
    }

    public void OnPuzzleComplete()
    {
        Debug.Log("퍼즐 완료");
        ExitFocus();
        interactable = false;
        isCleard = true;
        onPuzzleComplete?.Invoke();
    }
}
