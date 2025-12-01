using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinePipe : PipeBase
{
    [Header("Line PipeSetting")]
    public Image horizontalLine;
    public Image verticalLine;
    public bool horizontal = true;
    public bool vertical = true;

    private bool horizontalUsed = false;
    private bool verticalUsed = false;

    void Start()
    {
        // 사용하지 않는 방향 비활성화
        if (!horizontal && horizontalLine != null)
            horizontalLine.enabled = false;

        if (!vertical && verticalLine != null)
            verticalLine.enabled = false;
    }

    public override void ReceivePower(Vector2Int dir, PowerType powerType)
    {
        if(dir == Vector2Int.left || dir == Vector2Int.right)       // 좌우로 파워가 이동할 경우
        {
            if (horizontalUsed) return;
                horizontalUsed = true;

            if (horizontal && horizontalLine != null)
                horizontalLine.color = GetPipeColor(powerType);

            // 해당 방향을 사용할 경우
            if(horizontal)
                StartCoroutine(SendPowerToNeighbors(-dir, powerType));      // 이웃에게 파워 전달
        }
        else
        {
            if (verticalUsed) return;
                verticalUsed = true;

            if (vertical && verticalLine != null)
                verticalLine.color = GetPipeColor(powerType);

            if (vertical)
                StartCoroutine(SendPowerToNeighbors(-dir, powerType));      // 이웃에게 파워 전달
        }
    }

    public override IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.Key == -input)
            {
                neighbor.Value.ReceivePower(neighbor.Key, powerType);       // 입력 방향 (상하 / 좌우)의 이웃에게만 전달
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    protected override void PipeResetHandle()
    {
        horizontalUsed = false;
        verticalUsed = false;

        if (horizontal && horizontalLine != null)
            horizontalLine.color = GetPipeColor(PowerType.None);

        if (vertical && verticalLine != null)
            verticalLine.color = GetPipeColor(PowerType.None);
    }

    protected override void PipeUpdatedHandle()
    {
        PipeResetHandle();
    }
}
