using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryChekcer
{
    // 사용 안할게욤
    public ItemDataSO NeedItem { get; }
    protected bool HasItem(string id);
}
