
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class WaveGraph : MaskableGraphic
{
    [Header("Wave Settings")]
    [Range(0.1f, 10f)]
    public float frequency = 3f;

    [Range(0f, 360f)]
    public float phaseDegrees = 0f;

    [Range(0f, 0.5f)]
    public float amplitude = 0.3f;

    [Range(1f, 10f)]
    public float lineThickness = 2f;

    [Range(20, 500)]
    public int resolution = 160;

    [Range(0f, 360f)]
    public float animationPhaseDegrees;

    public void SetRandom()
    {
        frequency = Random.Range(3f, 10f);
        phaseDegrees = Random.Range(0f, 360f);
        amplitude = Random.Range(0.1f, 0.5f);

        SetWave(frequency, phaseDegrees, amplitude);
    }

    public void SetWave(float newFrequency, float newPhase, float newAmplitude)
    {
        frequency = newFrequency;
        phaseDegrees = newPhase;
        amplitude = newAmplitude;

        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        Rect rect = GetPixelAdjustedRect();

        float halfHeight = rect.height * amplitude;
        float centerY = rect.center.y;

        // 실제 위상 + 애니메이션 위상
        float phase = (phaseDegrees + animationPhaseDegrees) * Mathf.Deg2Rad;

        Vector2 previousPoint = Vector2.zero;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / resolution;

            float x = Mathf.Lerp(rect.xMin, rect.xMax, t);

            float y = centerY +
                Mathf.Sin(2f * Mathf.PI * frequency * t + phase)
                * halfHeight;

            Vector2 currentPoint = new Vector2(x, y);

            if (i > 0)
            {
                DrawSegment(vh, previousPoint, currentPoint);
            }

            previousPoint = currentPoint;
        }
    }

    private void DrawSegment(
        VertexHelper vh,
        Vector2 start,
        Vector2 end)
    {
        Vector2 direction = (end - start).normalized;
        Vector2 normal = new Vector2(
            -direction.y,
            direction.x
        ) * (lineThickness * 0.5f);

        int index = vh.currentVertCount;

        AddVertex(vh, start + normal);
        AddVertex(vh, start - normal);
        AddVertex(vh, end - normal);
        AddVertex(vh, end + normal);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }

    private void AddVertex(VertexHelper vh, Vector2 position)
    {
        UIVertex vertex = UIVertex.simpleVert;

        vertex.position = position;
        vertex.color = color;

        vh.AddVert(vertex);
    }
}
