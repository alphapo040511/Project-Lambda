using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class InteractionObservation : Interactable
{
    public Camera mainCamera;
    public Volume volume;
    private DepthOfField dof;

    private Vector3 originPosition;
    private Quaternion originRatation;

    private Vector3 targetPosition;
    private bool isObserved = false;

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

        if (isObserved == false) return;                      // 상호작용중이 아니면 return

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);

        if (Input.GetMouseButton(0))
        {
            float rotX = -Input.GetAxis("Mouse X") * 3;
            float rotY = -Input.GetAxis("Mouse Y") * 3;

            // 카메라 기준 좌우 회전
            transform.Rotate(mainCamera.transform.up, rotX, Space.World);
            // 카메라 기준 상하 회전
            transform.Rotate(mainCamera.transform.right, -rotY, Space.World);
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            ExitObseravtion();
        }
    }


    protected override void Complete()
    {
        base.Complete();

        GameManager.Instance.ChangeGameState(GameState.Menu);

        isObserved = true;

        SetObservePosition();

        if (dof != null)
        {
            dof.focusDistance.Override(0.5f); // 포커스 거리 변경
        }
    }


    void SetObservePosition()
    {
        originPosition = transform.localPosition;
        originRatation = transform.localRotation;
        targetPosition = mainCamera.transform.position + mainCamera.transform.forward * 0.5f;
    }

    public void ExitObseravtion()
    {
        base.Reset();

        GameManager.Instance.ChangeGameState(GameState.Playing);

        isObserved = false;

        transform.localPosition = originPosition;
        transform.localRotation = originRatation;


        if (dof != null)
        {
            dof.focusDistance.Override(2f); // 포커스 거리 변경
        }
    }

}
