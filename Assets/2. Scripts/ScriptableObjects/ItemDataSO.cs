using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item Data", menuName = "Item/New Item")]
public class ItemDataSO : ScriptableObject
{
    [Header("아이템의 고유 UI, 절대 중복 X")]
    public string uniqueID = "fuse_001";

    public string descriptionKey = "Fuse_Discription";

    public bool isCollectable = true;
}
