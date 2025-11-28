using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FuseBox : Interactable
{
    public ItemDataSO[] targetFuses = new ItemDataSO[2];
    public GameObject[] fuseObject = new GameObject[2];

    public UnityEvent firstFuseMounted;
    public UnityEvent secondFuseMounted;

    private int mountedFuseCount = 0;

    protected override void Complete()
    {
        base.Complete();

        if (mountedFuseCount >= targetFuses.Length) return;

        ItemDataSO item = targetFuses[mountedFuseCount];

        if (item != null && InventoryManager.ContainItem(item.uniqueID))        // 아이템이 선택 되어있고, 해당 아이템을 보유 중이라면
        {
            MountFuse();        // 퓨즈 장착
        }
        else
        {
            Debug.Log("장착할  퓨즈가 없습니다.");
        }
    }

    void MountFuse()
    {
        fuseObject[mountedFuseCount].SetActive(true);
        mountedFuseCount++;

        if(mountedFuseCount >= targetFuses.Length)
        {
            Debug.Log("모든 퓨즈 장착 완료");
            secondFuseMounted?.Invoke();                // 1개 아니면 2개니까 이렇게 구분 해놓겠습니다.
        }
        else
        {
            firstFuseMounted?.Invoke();
        }
    }
}
