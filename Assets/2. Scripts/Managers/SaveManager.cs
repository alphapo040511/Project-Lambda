using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public static class SaveManager
{
    static bool isSaving = false;

    public static async Task<string> Save(SaveMetadata newMeta, SaveData newSave, int slotIndex)
    {
        if (isSaving) return "Save_InProgress";       // 중복 호출 방지 (팝업에 띄울 메세지 키 값 보내기)
        isSaving = true;

        try
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
            return "Save_Success";
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Save failed: {e}");
            return "Save_Failed";
        }
        finally                     //  에러가 생기더라도 무조건 실행
        {
            isSaving = false;       // 저장 상태 취소
        }
    }

    public static async Task<SaveMetadata> MetaLoad(int slotIndex)
    {
        while (isSaving)
            await Task.Yield();         // 저장중이면 대기

        string metaPath = SavePathHelper.GetMetaPath(slotIndex);

        if (!File.Exists(metaPath)) return null;            // 파일이 있는지 확인

        string json = await File.ReadAllTextAsync(metaPath);
        return JsonUtility.FromJson<SaveMetadata>(json);
    }

    public static async Task<SaveData> LoadAsync(int slotIndex)
    {
        while (isSaving)
            await Task.Yield();         // 저장중이면 대기

        string savePath = SavePathHelper.GetSaveDataPath(slotIndex);

        if (!File.Exists(savePath))            // 파일이 있는지 확인
        {
            Debug.LogWarning($"{slotIndex}Slot의 세이브 데이터가 없습니다.");
            return null;            
        }

        string json = await File.ReadAllTextAsync(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static async Task<string> DeleteSaveFile(int slotIndex)
    {
        if (isSaving)
        {
            Debug.LogWarning("현재 저장 중입니다. 삭제를 잠시 후에 시도하세요.");
            return "Save_InProgress";           // 세이브 시도중 메세지 키값 전달
        }

        string metaPath = SavePathHelper.GetMetaPath(slotIndex);
        string savePath = SavePathHelper.GetSaveDataPath(slotIndex);

        if (File.Exists(metaPath) || File.Exists(savePath))
        {
            // 별도의 스레드에서 삭제 처리
            await Task.Run(() => {
                File.Delete(savePath);
                File.Delete(metaPath);
                });

            return "Delete_Success";
        }
        else
        {
            return "Save_Empty";
        }
    }

    public static bool MetaExists(int slotIndex)
    {
        string metaPath = SavePathHelper.GetMetaPath(slotIndex);
        return File.Exists(metaPath);
    }
}
