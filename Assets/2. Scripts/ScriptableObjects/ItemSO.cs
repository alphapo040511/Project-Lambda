using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item Data", menuName = "ItemSO/Item Data")]
public class ItemSO : ScriptableObject
{
    public int itemId;
    public bool isUnique;           // 유니크 아이템은 감소 안됨
}
