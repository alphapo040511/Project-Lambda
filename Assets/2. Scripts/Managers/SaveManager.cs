using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public static class SaveManager
{
    public static async Task Save(SaveMetadata newMeta, SaveData newSave, int slotIndex)
    {
        string forderPath = SavePathHelper.GetSlotFolder(slotIndex);
        string savaePath = SavePathHelper.GetSaveDataPath(slotIndex);
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);

        // ✅ 경로가 존재하지 않으면 자동 생성
        if (!Directory.Exists(forderPath))
            Directory.CreateDirectory(forderPath);

        // SaveData 저장
        await File.WriteAllTextAsync(savaePath, JsonUtility.ToJson(newSave, true));

        // Metadata 저장
        newMeta.saveTime = TimeFormatter.GetUnixTimestamp(DateTime.UtcNow);
        await File.WriteAllTextAsync(metaPath, JsonUtility.ToJson(newMeta, true));
    }

    public static async Task<SaveMetadata> MetaLoad(int slotIndex)
    {
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);

        if (!File.Exists(metaPath)) return null;            // 파일이 있는지 확인

        string json = await File.ReadAllTextAsync(metaPath);
        return JsonUtility.FromJson<SaveMetadata>(json);
    }

    public static async Task<SaveData> LoadAsync(int slotIndex)
    {
        string savePath = SavePathHelper.GetSaveDataPath(slotIndex);

        if (!File.Exists(savePath))            // 파일이 있는지 확인
        {
            Debug.LogWarning($"{slotIndex}Slot의 세이브 데이터가 없습니다.");
            return null;            
        }

        string json = await File.ReadAllTextAsync(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static async Task DeleteSaveFile(int slotIndex)
    {
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);
        string savePath = SavePathHelper.GetSaveDataPath(slotIndex);

        if (File.Exists(metaPath) || File.Exists(savePath))
        {
            // 별도의 스레드에서 삭제 처리
            await Task.Run(() => {
                File.Delete(savePath);
                File.Delete(metaPath);
                });

            Debug.Log("세이브 파일이 삭제되었습니다.");
        }
        else
        {
            Debug.LogWarning("삭제할 세이브 파일이 없습니다.");
        }
    }

    public static bool MetaExists(int slotIndex)
    {
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);
        return File.Exists(metaPath);
    }
}
