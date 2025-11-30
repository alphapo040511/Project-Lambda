using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinePipe : PipeBase
{
    [Header("Line PipeSetting")]
    public Image lineImage;
    private bool isUsed = false;

    public override void ReceivePower(Vector2Int dir, PowerType powerType)
    {
        if (isUsed) return;     // 이미 사용한 통로 파이프는 사용 불가

        if (powerType != PowerType.None)
            isUsed = true;                      // 빈 파워가 아닌 경우에만 사용됨으로 변경

        if (lineImage != null)
            lineImage.color = GetPipeColor(powerType);

        StartCoroutine(SendPowerToNeighbors(-dir, powerType));      // 이웃에게 파워 전달
    }

    public override IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.Key == input) continue;        // 입력 방향과 동일한 이웃이면 무시

            neighbor.Value.ReceivePower(neighbor.Key, powerType);       // 나머지 이웃에게 파워 전달
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void PipeResetHandle()
    {
        isUsed = false;

        if (lineImage != null)
            lineImage.color = GetPipeColor(PowerType.None);
    }

    protected override void PipeUpdatedHandle()
    {
        isUsed = false;

        if (lineImage != null)
            lineImage.color = GetPipeColor(PowerType.None);
    }
}
