using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryManager
{
    //public static Dictionary<string, int> inventory = new Dictionary<string, int>();
    static HashSet<string> inventory = new HashSet<string>();

    public static void LoadInventory(List<ItemSaveData> items)
    {
        inventory = new HashSet<string>();          // 해쉬셋 초기화

        int count = 0;
        foreach (var item in items)
        {
            if (inventory.Add(item.itemId))
            {
                count++;
            }
            else
            {
                Debug.LogWarning($"아이템 [{item.itemId}]가 중복 저장 되었습니다.");
            }
        }
    }

    public static bool GetItems(string itemId)
    {
        if (!inventory.Add(itemId))
        {
            Debug.LogWarning($"아이템 [{itemId}]를 이미 보유중 입니다.");
            return false;
        }

        return true;
    }

    public static bool ContainItem(string itemId)
    {
        return inventory.Contains(itemId);
    }

    public static bool UseItem(string itemId)
    {
        return inventory.Remove(itemId);
    }

    public static List<ItemSaveData> GetInventoryData()
    {
        List<ItemSaveData> inventoryData = new List<ItemSaveData>();
        foreach(var item in inventory)
        {
            ItemSaveData data = new ItemSaveData();
            data.itemId = item;
            data.quatity = 1;       // 일단 1로 저장
            inventoryData.Add(data);
        }

        return inventoryData;
    }
}
