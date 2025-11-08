using System;
using System.IO;
using UnityEngine;


public static class SaveManager
{
    public static void Save(SaveMetadata newMeta, SaveData newSave, int slotIndex)
    {
        string savaePath = SavePathHelper.GetSaveDataPath(slotIndex);
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);

        // SaveData 저장
        File.WriteAllText(savaePath, JsonUtility.ToJson(newSave, true));

        // Metadata 저장
        newMeta.saveTime = GetUnixTimestamp(DateTime.UtcNow);
        File.WriteAllText(metaPath, JsonUtility.ToJson(newMeta, true));
    }

    public static SaveMetadata MetaLoad(int slotIndex)
    {
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);

        if (!File.Exists(metaPath)) return null;            // 파일이 있는지 확인

        string json = File.ReadAllText(metaPath);
        return JsonUtility.FromJson<SaveMetadata>(json);
    }

    public static SaveData Load(int slotIndex)
    {
        string savaePath = SavePathHelper.GetSaveDataPath(slotIndex);

        if (!File.Exists(savaePath))            // 파일이 있는지 확인
        {
            Debug.LogWarning($"{slotIndex}Slot의 세이브 데이터가 없습니다.");
            return null;            
        }

        string json = File.ReadAllText(savaePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    #region Utility

    public static long GetUnixTimestamp(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }

    public static string GetDateTimeToString(long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp)
        .ToLocalTime()                                                      // 로컬 시간 적용
        .ToString("yy.MM.dd HH:mm:ss");
    }

    #endregion
}
