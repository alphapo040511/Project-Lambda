using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetPipe : PipeBase
{
    [Header("Direction Images")]
    public Image rightImage;
    public Image leftImage;
    public Image upImage;
    public Image downImage;

    [Header("Power Type Settings")]
    public PowerType rightType;
    public PowerType leftType;
    public PowerType upType;
    public PowerType downType;

    private Dictionary<Vector2Int, PowerType> powerDirs = new Dictionary<Vector2Int, PowerType>();
    private int completeCount = 0;

    private void Start()
    {
        InitializePowerDirs();
    }
    void InitializePowerDirs()
    {
        if (rightType != PowerType.None)
            powerDirs.Add(Vector2Int.right, rightType);

        if (leftType != PowerType.None)
            powerDirs.Add(Vector2Int.left, leftType);

        if (upType != PowerType.None)
            powerDirs.Add(Vector2Int.up, upType);

        if (downType != PowerType.None)
            powerDirs.Add(Vector2Int.down, downType);

        SetColor(); // 색상도 적용
    }

    [ContextMenu("색상 적용")]
    void SetColor()
    {
        if (rightImage == null || leftImage == null || downImage == null || upImage == null)
        {
            Debug.LogWarning("코너 파이프의 이미지가 할당 되지 않았습니다.");
            return;        // 이미지가 없는 경우 에러
        }

        if (rightType == PowerType.None)
            rightImage.enabled = false;
        else
        {
            rightImage.enabled = true;
            rightImage.color = GetPipeColor(rightType);
        }

        if (leftType == PowerType.None)
            leftImage.enabled = false;
        else
        {
            leftImage.enabled = true;
            leftImage.color = GetPipeColor(leftType);
        }

        if (upType == PowerType.None)
            upImage.enabled = false;
        else
        {
            upImage.enabled = true;
            upImage.color = GetPipeColor(upType);
        }

        if (downType == PowerType.None)
            downImage.enabled = false;
        else
        {
            downImage.enabled = true;
            downImage.color = GetPipeColor(downType);
        }
    }

    public override void ReceivePower(Vector2Int dir, PowerType powerType)
    {
        if (!powerDirs.ContainsKey(-dir) || powerDirs[-dir] != powerType) return;           // 입력 방향의 파워 값이 할당되지 않았거나, 목표 값이랑 다르다면 무시

        completeCount++;
        Debug.Log($"퍼즐 진행도 ({completeCount}/{powerDirs.Count})");

        // 원하는 입력 개수 만큼 입력이 들어왔다면
        if(puzzleController != null && completeCount == powerDirs.Count)
        {
            puzzleController.OnPuzzleComplete();
        }
    }

    public override IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType)
    {
        // 출력 없음   
        yield break;
    }

    protected override void PipeResetHandle()
    {
        completeCount = 0;
    }

    protected override void PipeUpdatedHandle()
    {
        completeCount = 0;
    }
}
