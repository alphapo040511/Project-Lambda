using UnityEngine;
using static UnityEngine.Rendering.HableCurve;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractionFinder : MonoBehaviour
{
    public LayerMask interactionLayer;

    [Header("Detection Settings")]
    public float detectionRange = 2f;                   // 탐지 범위
    public float detectionAngle = 60f;                  // 탐지 시야각

    [Header("Interaction Settings")]
    public float interactionRange = 1.5f;               // 사용 가능 범위
    public float interactionAngle = 45f;                // 사용 가능 시야각

    [Header("Debug")]
    public Color detectionColor = Color.yellow;         // 디버그 색상
    public Color interactionColor = Color.green;
    public int segments = 16;                           // 원뿔 꼭짓점 개수

    public Transform currentTarget;

    private List<Transform> previousTargets = new List<Transform>();      // UI 비활성화용 오브젝트 저장

    void Update()
    {
        FindTarget();
    }

    #region Detection
    // 타겟 검색
    private void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, detectionRange, interactionLayer);
        List<Transform> currentTargets = new List<Transform>();     // 이전 목록과 비교용 리스트

        Transform bestTarget = null;
        float bestScore = Mathf.Infinity;

        foreach (Collider col in targets)
        {
            Transform t = col.transform;                            // 탐지된 오브젝트 저장
            currentTargets.Add(t);

            Vector3 dirToTarget = (col.transform.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, col.transform.position);

            // 감지 각도 확인
            float dot = Vector3.Dot(transform.forward, dirToTarget);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= detectionAngle * 0.5f) // 시야각 안에 있음
            {
                col.GetComponent<Interactable>()?.OnActivate();             // UI 활성화

                float score = angle * 0.7f + distance * 0.3f;               // 시야각과 거리에 가중치를 주어 점수로 비교
                if (score < bestScore)
                {
                    bestScore = score;
                    bestTarget = col.transform;
                }
            }
        }

        if (currentTarget != null)
        {
            currentTarget.GetComponent<Interactable>()?.OnUntargeted();           // 기존 타겟 지정 해제
        }

        
        if (bestTarget != null)
        {
            bool canInteract = IsInInteractionRange(bestTarget);                    // 상호작용 범위 이내에 있는지 확인
            if (canInteract)                                                        // 상호작용 가능 하다면
            {
                bestTarget.GetComponent<Interactable>()?.OnTargeted();              // UI 활성화
                currentTarget = bestTarget;                                         // 타겟 등록
            }
        }


        // 기존 리스트와 비교해 거리가 멀어져 탐지가 안되는 오브젝트 UI 비활성화
        foreach (var prev in previousTargets)
        {
            if (!currentTargets.Contains(prev) && prev.GetComponent<Interactable>() != null)
            {
                prev.GetComponent<Interactable>().OnDeactivate();       // UI 끄기
            }
        }

        previousTargets = currentTargets; // 다음 프레임 대비
    }

    #endregion

    #region Validation
    // 인터렉션 가능 범위 안에 있는지 확인
    private bool IsInInteractionRange(Transform target)
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > interactionRange) return false;

        Vector3 dirToTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, dirToTarget);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        return angle <= interactionAngle * 0.5f;
    }

    // 물체가 무언가에 가려져 있는지 확인
    private bool IsOccluded(Transform target)
    {
        Vector3 dir = target.position - transform.position;
        if(Physics.Raycast(transform.position, dir, out RaycastHit hit, detectionRange))        // Ray로 막혀있는지 판단
        {
            return hit.transform == target;
        }

        return true;            // 아무것도 안 막아도 문제 없는것을 처리
    }
    #endregion

    #region Getter
    public bool CanInteract()
    {
        return currentTarget != null && currentTarget.GetComponent<Interactable>() != null;
        // 타겟이 있고, interactable 컴포넌트가 있는 경우
    }

    public Interactable GetTarget()
    {
        return currentTarget.GetComponent<Interactable>();
    }
    #endregion

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        DrawCone(transform.position, transform.forward, detectionRange, detectionAngle, segments, detectionColor);
        DrawCone(transform.position, transform.forward, interactionRange, interactionAngle, segments, interactionColor);
    }

    void DrawCone(Vector3 origin, Vector3 direction, float length, float angle, int segments, Color color)
    {
        Handles.color = color;

        // 시야각 절반 (좌우)
        float halfAngle = angle * 0.5f;

        // 중심에서 시작
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(up, direction).normalized;

        // 바닥 원의 각도 단위
        float step = 360f / segments;

        // 원뿔 바닥 원의 반지름 계산
        float radius = Mathf.Tan(Mathf.Deg2Rad * halfAngle) * length;

        Vector3[] points = new Vector3[segments + 1];

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = step * i;
            Quaternion rot = Quaternion.AngleAxis(currentAngle, direction);
            Vector3 point = origin + direction * length + rot * (right * radius);
            points[i] = point;
        }

        // 원 테두리
        Handles.DrawAAPolyLine(2f, points);

        // 중심에서 각 점으로 라인
        foreach (var p in points)
        {
            Handles.DrawLine(origin, p);
        }
    }
#endif

}

