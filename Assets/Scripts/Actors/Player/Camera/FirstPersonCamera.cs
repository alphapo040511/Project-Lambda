using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : Actor
{
    public Transform playerTransform;                // 플레이어 본체 (Y축 회전용)

    [Header("Camera Settings")]
    public float mouseSensitivity = 200f;       // 마우스 감도 (나중에 설정값 받아오도록 수정)
    public float clampAngle = 80f;              // 상하 회전 제한 각도

    private float xRotation = 0f;               // 상하 회전 값

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 마우스 커서 고정
        Cursor.visible = false;
    }
    
    protected override void ActorUpdate()
    {
        // 마우스 입력
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 좌우 회전
        if(playerTransform != null)
        playerTransform.Rotate(Vector3.up * mouseX);
    }
}
