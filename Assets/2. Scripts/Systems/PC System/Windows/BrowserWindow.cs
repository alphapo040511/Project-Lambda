using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;

public class BrowserWindow : WindowBase
{
    [Header("Browser Settings")]
    public Button undoButton;
    public Button redoButton;
    public TextMeshProUGUI pathText;

    [Header("Page Settings")]
    public GameObject startPage;
    public List<GameObject> pageList = new List<GameObject>();                                  // 페이지는 일단 오브젝트로 관리
    private Dictionary<string, GameObject> pages = new Dictionary<string, GameObject>();


    // 경로 관리를 위한 스택
    private Stack<GameObject> undoStack = new Stack<GameObject>();
    private Stack<GameObject> redoStack = new Stack<GameObject>();
    private GameObject currentPage;

    protected override void Awake()
    {
        base.Awake();
        if (undoButton != null)
            undoButton.onClick.AddListener(() => Undo());

        if (redoButton != null)
            redoButton.onClick.AddListener(() => Redo());
    }


    protected override void OnDestroy()
    {
        if (undoButton != null)
            undoButton.onClick.RemoveListener(() => Undo());

        if (redoButton != null)
            redoButton.onClick.RemoveListener(() => Redo());
    }

    private void Start()
    {
        InitPages();
        
        if (startPage != null)
        {
            startPage.SetActive(true);
            currentPage = startPage;
        }

        RefreshUI();
    }

    void InitPages()
    {
        foreach(var page in pageList)
        {
            page.SetActive(false);
            pages.Add(page.name, page);
        }
    }

    // 새로운 페이지 열기
    public void OpenPage(string pageName)
    {
        if(pages.ContainsKey(pageName))
        {
            currentPage.SetActive(false);               // 기존 페이지 종료
            PushPage(currentPage);                      // 스택에 푸쉬

            GameObject newPage = pages[pageName];
            newPage.SetActive(true);                    // 새 페이지 활성화
            currentPage = newPage;                      // 현재 페이지를 새 페이지로 변경


            RefreshUI();                                // 새로고침
        }
    }

    public void ClosePage()
    {
        Undo();
    }

    private void PushPage(GameObject currentPage)
    {
        undoStack.Push(currentPage);
        redoStack.Clear();
    }

    void Undo()
    {
        if (undoStack.Count <= 0 || currentPage == startPage) return;               // 맨 처음 페이지에서는 뒤로 못 가도록

        currentPage.SetActive(false);
        redoStack.Push(currentPage);

        GameObject newPage = undoStack.Pop();
        newPage.SetActive(true);
        currentPage = newPage;

        RefreshUI();
    }

    void Redo()
    {
        if (redoStack.Count <= 0) return;

        currentPage.SetActive(false);
        undoStack.Push(currentPage);

        GameObject newPage = redoStack.Pop();
        newPage.SetActive(true);
        currentPage = newPage;

        RefreshUI();
    }

    // 경로 표시 및 버튼 업데이트
    void RefreshUI()
    {
        if(pathText != null)
        {
            pathText.text = GetPathString();
        }

        // Undo / Redo 가능한 경우만 버튼 활성화
        if(undoStack.Count <= 0 || currentPage == startPage)
            undoButton.interactable = false;
        else
            undoButton.interactable = true;


        if (redoStack.Count <= 0)
            redoButton.interactable = false;
        else
            redoButton.interactable = true;
    }

    string GetPathString()
    {
        string path = windowName;
        foreach (var page in undoStack.Reverse())
        {
            path += $" > {page.name}";                  // 일단 오브젝트 이름으로 경로 표시
        }

        path += $" > {currentPage.name}";
        return path;
    }
}
