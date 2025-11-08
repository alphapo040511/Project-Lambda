using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SavePathHelper
{
    public static string SaveRoot => Path.Combine(Application.persistentDataPath, "Saves");

    public static string GetSlotFolder(int slotIndex)
    {
        return Path.Combine(SaveRoot, $"Slot{slotIndex}");
    }

    public static string GetSaveDataPath(int slotIndex)
    {
        return Path.Combine(GetSlotFolder(slotIndex), "SaveData.json");
    }

    public static string GetMetaPath(int slotIndex)
    {
        return Path.Combine(GetSlotFolder(slotIndex), "Metadata.json");
    }
}
