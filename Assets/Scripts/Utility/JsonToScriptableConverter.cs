#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;
using System.Linq;

public enum ConversionType
{
    Dialog                      // 다른 컨버터 사용시 추가
}

[Serializable]
public class DialogRowData
{
    public string id;
    public string en;
    public string ko;
    public string audioPath;
    public string nextId;
}

public class JsonToScriptableConverter : EditorWindow
{
    private string jsonFilePath = "";                                       //JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/ScriptableObjects";               //출력 SO 파일의 경로 값
    private bool createDatabase = true;                                     //데이터 베이스를 사용 할 것인지에 대한 bool 값
    private ConversionType conversionType = ConversionType.Dialog;

    private TextAsset selectedJson; // 선택된 JSON
    private int selectedIndex = 0;
    private string[] jsonNames;
    private TextAsset[] jsonAssets;

    [MenuItem("Tools/Json Selector")]
    static void Init()
    {
        GetWindow<JsonToScriptableConverter>("Json Selector");
    }

    void OnEnable()
    {
        // 특정 폴더 (예: Assets/GameData/Json) 안의 모든 TextAsset 검색
        string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets/Resources/Json" });
        jsonAssets = guids
            .Select(g => AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(g)))
            .ToArray();
        jsonNames = jsonAssets.Select(a => a.name).ToArray();
    }

    [MenuItem("Tools/JSON to Scriptable Objects")]
    public static void ShowWindow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }

    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable Object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        if (jsonAssets == null || jsonAssets.Length == 0)
        {
            EditorGUILayout.LabelField("⚠️ Assets/GameData/Json 폴더에 JSON 파일이 없습니다.");
            return;
        }

        // 드롭다운 표시
        selectedIndex = EditorGUILayout.Popup("Select JSON", selectedIndex, jsonNames);
        selectedJson = jsonAssets[selectedIndex];

        EditorGUILayout.LabelField("Selected File : ",jsonFilePath);
        EditorGUILayout.Space();

        //변환 타입 선택
        conversionType = (ConversionType)EditorGUILayout.EnumPopup("Conversion Type:" , conversionType);

        //타입에 따라 기본 출력 폴더 설정
        if(conversionType == ConversionType.Dialog)
        {
            outputFolder = "Assets/ScriptableObjects/Dialogs";
        }
        //else if (conversionType == ConversionType.Items)
        //{
        //    outputFolder = "Assets/ScriptableObjects/Items";
        //}

            outputFolder = EditorGUILayout.TextField("Output Folder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if(GUILayout.Button("Convert to Scriptable Objects"))
        {
            if(string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Plaese select a JSON file firest!", "OK");
                return;
            }

            switch(conversionType)
            {
                //case ConversionType.Items:
                //    ConvertJsonToItemScriptableObject();
                //    break;

                case ConversionType.Dialog:
                    ConvertJsonToDialogScriptableObjects();
                    break;
            }
        }
    }
    
    //private void ConvertJsonToItemScriptableObject()                            //JSON 파일을 ScriptableObject 파일로 변환 시켜주는 함수
    //{
    //    //폴더 생성
    //    if(!Directory.Exists(outputFolder))                                 //폴더 위치를 확인하고 없으면 생성 한다.
    //    {
    //        Directory.CreateDirectory(outputFolder);
    //    }

    //    //JSON 파일 읽기
    //    string jsonText = File.ReadAllText(jsonFilePath);                   //JSON 파일을 읽는다.

    //    try
    //    {
    //        //JSON 파싱
    //        List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

    //        List<ItemSO> createdItems = new List<ItemSO>();                 //ItemSO 리스트 생성

    //        //각 아이템 데이터를 스크립터블 오브젝트로 변환
    //        foreach(var itemData in itemDataList)
    //        {
    //            ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();  //ItemSO 파일을 생성

    //            //데이터 복사
    //            itemSO.id = itemData.id;
    //            itemSO.itemName = itemData.itemName;
    //            itemSO.nameEng = itemData.nameEng;
    //            itemSO.description = itemData.description;

    //            //열거형 변환
    //            if(System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
    //            {
    //                itemSO.itemType = parsedType;
    //            }
    //            else
    //            {
    //                Debug.Log($"아이템 '{itemData.itemName}'의 유효하지 않은 타입 : {itemData.itemTypeString}");
    //            }

    //            itemSO.price = itemData.price;
    //            itemSO.power = itemData.power;
    //            itemSO.level = itemData.level;
    //            itemSO.isStackable = itemData.isStackable;

    //            //아이콘 로드 (경로가 있는 경우)
    //            if(!string.IsNullOrEmpty(itemData.iconPath))
    //            {
    //                itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resources/{itemData.iconPath}.png");

    //                if(itemSO.icon == null)
    //                {
    //                    Debug.LogWarning($"아이템 '{itemData.nameEng}'의 아이콘을 찾을 수 없습니다. : {itemData.iconPath}");
    //                }
    //            }

    //            //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
    //            string assetPath = $"{outputFolder}/Item_{itemData.id.ToString("D4")}_{itemData.nameEng}.asset";
    //            AssetDatabase.CreateAsset( itemSO, assetPath );

    //            //에셋 이름 지정
    //            itemSO.name = $"Item_{itemData.id.ToString("D4")}+{itemData.nameEng}";
    //            createdItems.Add( itemSO );

    //            EditorUtility.SetDirty( itemSO );
    //        }

    //        //데이터베이스 생성
    //        if(createDatabase && createdItems.Count > 0 )
    //        {
    //            ItemDatabaseOS database = ScriptableObject.CreateInstance<ItemDatabaseOS>();      //ItemDatabaseSO 생성
    //            database.items = createdItems;

    //            AssetDatabase.CreateAsset(database, $"{outputFolder}/ItemDatabase.asset");
    //            EditorUtility.SetDirty ( database );
    //        }

    //        AssetDatabase.SaveAssets();
    //        AssetDatabase.Refresh();

    //        EditorUtility.DisplayDialog("Sucess", $"Created {createdItems.Count} scriptable objects!", "OK");
    //    }
    //    catch (System.Exception e)
    //    {
    //        EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
    //        Debug.LogError($"JSON 변환 오류 : {e}");
    //    }
    //}

    //대화 JSON을 스크립터블 오브젝트로 변환
    private void ConvertJsonToDialogScriptableObjects()
    {
        //폴더 생성
        if(!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 파싱
            List<DialogRowData> rowDataList = JsonConvert.DeserializeObject<List<DialogRowData>>(jsonText);

            //대화 데이터 재구성
            Dictionary<string, DialogSO> dialogMap = new Dictionary<string, DialogSO>();
            List<DialogSO> createDialogs = new List<DialogSO>();

            //1단계 : 대화 항목 생성
            foreach(var rowData in rowDataList)
            {
                //id 있는지 확인
                if(!string.IsNullOrEmpty(rowData.id))
                {
                    DialogSO dialogSO = ScriptableObject.CreateInstance<DialogSO>();

                    //데이터 복사
                    dialogSO.id = rowData.id;
                    dialogSO.en = rowData.en;
                    dialogSO.ko = rowData.ko;
                    dialogSO.nextId = rowData.nextId;

                    //사운드 로드 (경로가 있는 경우)
                    if(!string.IsNullOrEmpty(rowData.audioPath))
                    {
                        dialogSO.clip = Resources.Load<AudioClip>(rowData.audioPath);

                        if(dialogSO.clip == null)
                        {
                            Debug.LogWarning($"대화 {rowData.id}의 사운드를 찾을 수 없습니다.");
                        }
                    }

                    //dialogMap에 추가
                    dialogMap[dialogSO.id] = dialogSO;
                    createDialogs.Add(dialogSO);
                }
            }

            //2단계 : 대화 스크립터블 오브젝트 저장
            foreach(var dialog in createDialogs)
            {
                //스크립터블 오브젝트 저장 - ID를 4자리 숫자로 포맷팅
                string assetPath = $"{outputFolder}/Dialog_{dialog.id}.asset";
                AssetDatabase.CreateAsset(dialog, assetPath);

                //에셋 이름 지정
                dialog.name = $"Dialog_{dialog.id}";

                EditorUtility.SetDirty(dialog);
            }

            //데이터 베이스 생성
            if(createDatabase && createDialogs.Count > 0)
            {
                DialogDatabaseSO database = ScriptableObject.CreateInstance<DialogDatabaseSO>();
                database.dialogs = createDialogs;

                AssetDatabase.CreateAsset(database, $"{outputFolder}/DialogDatabase.asset");
                EditorUtility.SetDirty(database);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Success", $"Created {createDialogs.Count} dialog scriptable objects!", "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to convert JSON: {e.Message}", "OK");
            Debug.LogError($"JSOJN 변환 오류 : {e}");
        }
    }
}

#endif
