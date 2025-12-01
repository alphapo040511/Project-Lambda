using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CornerPipe : PipeBase
{
    [Header("Corner Pipe Settings")]
    public Button rotateButton;
    public bool isRotateble = true;
    private int rotate = 0;                 // 0=오른쪽, 1=아래, 2=왼쪽, 3=위

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

    void Start()
    {
        InitializePowerDirs();

        if (rotateButton != null && isRotateble)
            rotateButton.onClick.AddListener(RotatePipe);
    }

    private void OnDestroy()
    {
        if (rotateButton != null && isRotateble)
            rotateButton.onClick.RemoveListener(RotatePipe);
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
        if(powerType == PowerType.None) return;                 // 파워가 들어오지 않은 경우 무시;
        
        Vector2Int rotatedDir = GetRotatedDirection(-dir);                                      // 입력값 보정(입력 방향 이므로 부호 반전)
        if (!powerDirs.ContainsKey(rotatedDir) || powerDirs[rotatedDir] != powerType) return;   // 보정된 방향에 출력값이 없거나, 파워 타입이 다른 경우 무시

        StartCoroutine(SendPowerToNeighbors(-dir, powerType));      // 입력 방향 반전 후 이웃에게 파워 전달
    }

    public override IEnumerator SendPowerToNeighbors(Vector2Int input, PowerType powerType)
    {
        yield return new WaitForSeconds(0.1f);

        foreach (var neighbor in neighbors)
        {
            if (neighbor.Key == input) continue;        // 입력 방향과 동일한 이웃이면 무시

            Vector2Int rotatedDir = GetRotatedDirection(neighbor.Key);                      // 출력 방향 보정

            if(powerDirs.ContainsKey(rotatedDir))                                           // 보정된 방향에 출력값이 있다면
            {
                neighbor.Value.ReceivePower(neighbor.Key, powerDirs[rotatedDir]);           // 해당 이웃 방향으로 출력 타입을 전달
            }
            yield return new WaitForSeconds(0.1f);
        }
    }


    // 보정된 방향값을 반환
    Vector2Int GetRotatedDirection(Vector2Int dir)
    {
        int origin = 0;                                     // 들어온 방향을 정수값으로 변환
        if (dir == Vector2Int.down) origin = 1;
        else if (dir == Vector2Int.left) origin = 2;
        else if (dir == Vector2Int.up) origin = 3;

        int target = (origin - rotate + 4) % 4;             // 최종 회전값으로 변환
        if (target == 0) return Vector2Int.right;
        else if (target == 1) return Vector2Int.down;
        else if (target == 2) return Vector2Int.left;
        else return Vector2Int.up;
    }


    public void RotatePipe()
    {
        if (!isRotateble) return;       // 회전이 불가능한 경우 무시
        if (puzzleController != null && puzzleController.isCleard) return;      // 퍼즐이 클리어 된 경우 조작 멈춤

        rotate = (rotate + 1) % 4;      // 회전
        transform.localEulerAngles = new Vector3(0, 0, -rotate * 90);       // 오른쪽 회전을 위해 음수 처리 (일단 보간 없이 회전)

        puzzleController.OnPipeUpdate();    // 파이프 회전 이벤트 발생
    }

    protected override void PipeUpdatedHandle()
    {
        // 중간 지점은 따로 변경 사항 없음
    }

    protected override void PipeResetHandle()
    {
        // 회전값 초기화
        rotate = 0;
        transform.localEulerAngles = new Vector3(0, 0, -rotate * 90);
    }
}
