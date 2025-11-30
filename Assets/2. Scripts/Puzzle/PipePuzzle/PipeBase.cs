using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerType
{
    None,
    Red,
    Blue
}

public abstract class PipeBase : MonoBehaviour
{
    [Header("Puzzle Controller")]
    public PipePuzzleController puzzleController;

    [Header("Neighbor Pipes")]
    public PipeBase rightPipe;
    public PipeBase leftPipe;
    public PipeBase upPipe;
    public PipeBase downPipe;

    protected Dictionary<Vector2Int, PipeBase> neighbors = new Dictionary<Vector2Int, PipeBase>();

    void Awake()
    {
        InitializeNeighbors();
        if (puzzleController != null)
        {
            puzzleController.onPipeUpdated += PipeUpdatedHandle;
            puzzleController.onPipeReset += PipeResetHandle;
        }
    }

    private void OnDestroy()
    {
        if (puzzleController != null)
        {
            puzzleController.onPipeUpdated -= PipeUpdatedHandle;
            puzzleController.onPipeReset -= PipeResetHandle;
        }
    }

    // 네 방향의 파이프 또는 이웃 찾기
    void InitializeNeighbors()
    {
        if (rightPipe != null)
            neighbors.Add(Vector2Int.right, rightPipe);

        if (leftPipe != null)
            neighbors.Add(Vector2Int.left, leftPipe);

        if (upPipe != null)
            neighbors.Add(Vector2Int.up, upPipe);

        if (downPipe != null)
            neighbors.Add(Vector2Int.down, downPipe);
    }

    public abstract void ReceivePower(Vector2Int dir, PowerType powerType);

    public abstract IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType);

    protected abstract void PipeUpdatedHandle();

    protected abstract void PipeResetHandle();

    public Color GetPipeColor(PowerType powerType)
    {
        switch(powerType)
        {
            case PowerType.Red:
                return Color.red;
            case PowerType.Blue:
                return Color.blue;
            default:
                return Color.white;
        }
    }
}
