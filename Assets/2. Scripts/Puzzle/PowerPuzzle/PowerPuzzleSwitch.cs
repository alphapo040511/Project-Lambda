using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerPuzzleSwitch : MonoBehaviour, IPointerClickHandler
{
    public PowerPuzzleController controller;
    public int switchIndex;
    public float rotateSpeed = 5f;
    private bool isPowered = false;

    private float targetAngle
    {
        get
        {
            return isPowered ? 0 : 90;
        }
    }

    private float currentAngle;

    private bool isMoving = true;

    private void Start()
    {
        SetupSwitch(isPowered);
    }

    private void Update()
    {
        if (isMoving == false || currentAngle == targetAngle) return;

        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * rotateSpeed);

        transform.localEulerAngles = new Vector3(currentAngle, 0, 0);
        if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
        {
            currentAngle = targetAngle;
            isMoving = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || !controller.isFocused || isMoving) return;        // 포커싱 되어 있지 않거나, 움직이는 중 일때

        SetupSwitch(!isPowered);
    }

    public void SetupSwitch(bool isPower)
    {
        isPowered = isPower;
        isMoving = true;
    }
}
