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

    private float smoothX;
    private float smoothY;
    private float smoothXVelocity;
    private float smoothYVelocity;

    public float mouseSmoothTime = 0.03f;

    private void Start()
    {
        OnSensitivityChanged(SettingsManager.Instance.currentSettings.mouseSentivity);
    }

    private void OnEnable()
    {
        GameEvents.OnSensitivityChanged += OnSensitivityChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnSensitivityChanged -= OnSensitivityChanged;
    }

    void OnSensitivityChanged(float value)
    {
        mouseSensitivity = value * 0.1f;        // 너무 강해서 임의로 보정
    }

    protected override void ActorUpdate()
    {
        float rawX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float rawY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // 보간
        smoothX = Mathf.SmoothDamp(smoothX, rawX, ref smoothXVelocity, mouseSmoothTime);
        smoothY = Mathf.SmoothDamp(smoothY, rawY, ref smoothYVelocity, mouseSmoothTime);

        // 상하
        xRotation -= smoothY;
        xRotation = Mathf.Clamp(xRotation, -clampAngle, clampAngle);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 좌우
        playerTransform.Rotate(Vector3.up * smoothX);
    }
}
