using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


// 임시 아이템 데이터
[System.Serializable]
public class TempItemData
{ 
    public string itemName = "itemName";             // 키 값으로 변경
    [TextArea]public string desctipntion = "Description";

    public bool isCollectible = false;          // 획득 가능한가
}

public class InteractionObservation : Interactable
{
    [Header("Camera & Volume Settings")]
    public Camera mainCamera;
    public Volume volume;
    private DepthOfField dof;


    [Header("Observation Settings")]
    public float focusDistance = 0.5f;
    public float minSize = 0.5f;
    public float maxSize = 2f;

    [Header("Item Setting")]
    public TempItemData itemData;

    // 위치 저장
    private Vector3 originPosition;
    private Quaternion originRatation;
    private Vector3 targetPosition;

    // 크기 저장
    private Vector3 originSize;
    private float sizeMultiplier;

    private bool isObserving = false;
    private bool isInitializing = false;

    private void Start()
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if(volume == null)
        {
            volume = FindObjectOfType<Volume>();
        }

        if(volume != null)
        {
            if (volume.profile.TryGet<DepthOfField>(out dof))
            {
                Debug.Log("Depth of Field 찾음!");
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        if (isObserving == false) return;                                                   // 관찰중이 아니면 return

        if (isInitializing)
        {
            SetInitial();                       // 초기 위치에 도달할 때까지 이동
        }
        else    // 초기 위치까지 이동한 후 키 입력 가능
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !itemData.isCollectible)                // ESC 키로 관찰 종료 (획득 불가능한 경우)
            {
                ExitObseravtion();
            }

            if (Input.GetKeyDown(KeyCode.E) && itemData.isCollectible)                      // E 키로 관찰 획득 (획득 가능한 경우)
            {
                Destroy(gameObject);            // 임시로 파괴
                ExitObseravtion();
            }
        }


        Rotate();
        Zoom();
    }

    public override void OnInteractStart()
    {
        base.OnInteractStart();
        Complete();
    }

    public override void OnInteractHold(float deltaTime)
    {
        // 홀드 중 기능 없음
    }



    // 초기 위치 설정
    void SetInitial()
    {
        // 포커스 거리 변경
        float focus = Mathf.Lerp(dof.focusDistance.value, focusDistance, Time.deltaTime * 5f);

        if (dof != null)                 
        {
            dof.focusDistance.Override(focus);
        }

        // 초기 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);  


        // 초기 위치 및 포커스에 도달 할 경우 종료
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f
            && focusDistance - dof.focusDistance.value < 0.01f)
        {
            transform.position = targetPosition;
            dof.focusDistance.Override(focusDistance);

            isInitializing = false;
        }
    }

    // 오브젝트 회전
    void Rotate()
    {
        if (Input.GetMouseButton(0))
        {
            float rotX = -Input.GetAxis("Mouse X") * 3;
            float rotY = -Input.GetAxis("Mouse Y") * 3;

            // 카메라 기준 좌우 회전
            transform.Rotate(mainCamera.transform.up, rotX, Space.World);
            // 카메라 기준 상하 회전
            transform.Rotate(mainCamera.transform.right, -rotY, Space.World);
        }
    }

    // 확대 및 축소
    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            sizeMultiplier += scroll * Time.deltaTime * 20f;
            sizeMultiplier = Mathf.Clamp(sizeMultiplier, minSize, maxSize);
            transform.localScale = Vector3.Lerp(transform.localScale, originSize * sizeMultiplier, Time.deltaTime * 10f);
        }
    }

    protected override void Complete()
    {
        base.Complete();

        GameManager.Instance.ChangeGameState(GameState.Observation);

        ShowOverlay();

        isObserving = true;
        isInitializing = true;

        // 위치 및 각도, 크기 저장
        originPosition = transform.localPosition;
        originRatation = transform.localRotation;
        originSize = transform.localScale;

        // 목표 크기 초기화
        sizeMultiplier = 1f;

        targetPosition = mainCamera.transform.position + mainCamera.transform.forward * focusDistance;  // 카메라 앞쪽에 위치
    }

    void ShowOverlay()
    {
        if(itemData.isCollectible)
        {
            ObservationUI.Instance.ShowButton("획득하기", itemData.desctipntion, () =>
            {
                Debug.Log($"{itemData.itemName} 획득!");
                Destroy(gameObject);
                ExitObseravtion();
            });
        }
        else
        {
            ObservationUI.Instance.ShowButton("내려놓기", itemData.desctipntion, () =>
            {
                ExitObseravtion();
            });
        }

    }

    public void ExitObseravtion()
    {
        base.Reset();

        GameManager.Instance.ChangeGameState(GameState.Playing);

        isObserving = false;
        isInitializing = false;

        // 원래 위치로 복귀    
        transform.localPosition = originPosition;
        transform.localRotation = originRatation;
        transform.localScale = originSize;

        UIManager.Instance.HideOverlay(OverlayType.Observation);

        if (dof != null)
        {
            dof.focusDistance.Override(2f); // 포커스 거리 변경
        }
    }

}
