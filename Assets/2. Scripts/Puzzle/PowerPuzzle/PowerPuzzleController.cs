using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowerSwitchData
{
    public bool state = false;       // 나중에 초기값 선언 시 사용
    public float powerValue;
}

public class PowerPuzzleController : InteractionFocus
{
    [Header("Puzzle Settings")]
    public float answerValue = 8.5f;
    public List<PowerSwitchData> switchDatas = new List<PowerSwitchData>();
    public float controllButtonCooldown = 3f;

    [Header("PuzzleObject References")]
    public GameObject controlNeedle;
    public GameObject answerPoint;

    private float needleStartAngle = -55;
    private float needleEndAngle = 55;
    private float currentValue;

    void Start()
    {
        
    }

    protected override void ActorUpdate()
    {
        base.ActorUpdate();
    }

    public void OnChangeSwitchState(bool isPowered, int index)
    {
        if (index < 0 || index >= switchDatas.Count) return;

        switchDatas[index].state = isPowered;
    }

    public void CheckAllSwitch()
    {

    }
}
