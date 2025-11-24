using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerSwitchData
{
    public bool switchState = false;       // 나중에 초기값 선언시에도 사용
    public int powerValue;
}

public class PowerPuzzleController : InteractionFocus
{
    [Header("Puzzle Settings")]
    public int answerValue = 85;
    public List<PowerSwitchData> switchDatas = new List<PowerSwitchData>();
    public float controllButtonCooldown = 3f;
    [SerializeField]private float currentAngle = 0;
    [SerializeField]private float targetAngle = 0;

    [Header("PuzzleObject References")]
    public GameObject controlNeedle;
    public GameObject answerPoint;
    public float needleRotateSpeed = 10;
    private float needleStartAngle = -50;

    public bool isMoving = false;
    public float timer = 0;

    void Start()
    {
        currentAngle = controlNeedle.transform.localEulerAngles.z;
        targetAngle = needleStartAngle;
        answerPoint.transform.localEulerAngles = new Vector3(0, 90, needleStartAngle + answerValue);
    }

    protected override void Update()
    {
        base.Update();

        if(isMoving) MoveNeedle();


        if (timer > 0)
            timer -= Time.deltaTime;
    }

    public void OnChangeSwitchState(bool isPowered, int index)
    {
        if (index < 0 || index >= switchDatas.Count) return;

        switchDatas[index].switchState = isPowered;
    }

    public void AnswerButtonPressed()
    {
        if (isMoving) return;

        // 목표 위치 값 저장
        targetAngle = needleStartAngle + GetTotalValue();       
        targetAngle = Mathf.Clamp(targetAngle, needleStartAngle, -needleStartAngle);

        isMoving = true;
        timer = controllButtonCooldown;
    }

    void AnswerCheck()
    {
        if (GetTotalValue() == answerValue)
        {
            ExitFocus();
            interactable = false;
        }
    }

    private void MoveNeedle()
    {
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * needleRotateSpeed);
        controlNeedle.transform.localEulerAngles = new Vector3(0, 0, currentAngle);

        if(Mathf.Abs(currentAngle - targetAngle) < 0.1f)
        {
            currentAngle = targetAngle;
            isMoving = false;
            AnswerCheck();
        }    
    }

    private int GetTotalValue()
    {
        int total = 0;
        foreach(var data in switchDatas)
        {
            if (data.switchState)
                total += data.powerValue;
        }
        return total;
    }

    public bool CanPressAnswerButton()
    {
        return (isMoving == false && timer <= 0);
    }
}
