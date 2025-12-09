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

    public override void SetObjectState(string id, ObjectState state)
    {
        if (UniqueId != id) return;
        this.state = state;

        switch (state)
        {
            case ObjectState.On:                    // 활성화 된 경우
                interactable = true;
                break;
            case ObjectState.Used:                // 1개만 장착된 경우
                fuseObject[0].SetActive(true);
                mountedFuseCount = 1;
                interactable = true;
                break;
            case ObjectState.Disable:           // 2개 모두 장착된 경우
                fuseObject[0].SetActive(true);
                fuseObject[1].SetActive(true);
                interactable = false;
                mountedFuseCount = 2;
                break;
        }
    }

    protected override void Complete()
    {
        interactable = true;
        interacting = false;
        onCompleted?.Invoke();

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
            interactable = false;
            state = ObjectState.Disable;
        }
        else
        {
            state = ObjectState.Used;
            firstFuseMounted?.Invoke();
        }
    }
}
