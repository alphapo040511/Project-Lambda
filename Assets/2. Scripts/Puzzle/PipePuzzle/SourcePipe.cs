using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SourcePipe : PipeBase
{
    [Header("SourcePipe Settings")]
    public PowerType sourcePower;
    public Image sourceImage;

    private void Start()
    {
        SetColor();
        StartCoroutine(SendPowerToNeighbors(Vector2Int.zero, PowerType.None));
    }

    [ContextMenu("색상 적용")]
    void SetColor()
    {
        if (sourcePower != PowerType.None || sourceImage != null)
            sourceImage.color = GetPipeColor(sourcePower);
    }

    public override void ReceivePower(Vector2Int dir, PowerType powerType)
    {
        // 파워를 받지 않는다.
    }

    public override IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType)
    {
        if (sourcePower == PowerType.None) yield break;        // 파워가 없는 경우 무시

        yield return new WaitForSeconds(0.1f);

        // 입력이 없음으로 모든 방향으로 전달
        foreach (var neighbor in neighbors)
        {
            neighbor.Value.ReceivePower(neighbor.Key, sourcePower);
            yield return new WaitForSeconds(0.1f);
        }
    }

    protected override void PipeResetHandle()
    {
        StartCoroutine(SendPowerToNeighbors(Vector2Int.zero, PowerType.None));
    }

    protected override void PipeUpdatedHandle()
    {
        StartCoroutine(SendPowerToNeighbors(Vector2Int.zero, PowerType.None));
    }
}
