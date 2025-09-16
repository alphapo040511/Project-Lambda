using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerWindow : MonoBehaviour
{
    [Header("Tab Settings")]
    public Canvas tabCavas;
    public TextMeshProUGUI tabName;
    public Button closeButton;
    public float fadeSpeed = 3f;

    private ComputerSystem targetComputer;
    private Coroutine fadeCoroutine;
    private bool isActive = false;
    private Vector2 originSize;

    private void Awake()
    {
        originSize = transform.localScale;
    }

    public void Initialize(ComputerSystem pc, string name)
    {
        targetComputer = pc;
        if(tabName != null )
            tabName.text = name;
    }

    #region Window
    // 창 열기
    public virtual void OpenWindow()
    {
        if (tabCavas == null || isActive) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ScaleChange(true));
    }


    // 창 닫기
    public virtual void CloseWindow()
    {
        if (tabCavas == null || !isActive) return;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(ScaleChange(false));
    }

    #endregion

    #region Tab
    public void ShowTab(string tabName)
    {
        //if (programs.ContainsKey(tabName))
        //{
        //    tabStack.Push(currentTab);                      // 기존 탭을 스텍에 푸쉬
        //    ComputerWindow tab = programs[tabName];
        //    tab.OpenWindow();
        //    currentTab = tab;                               // 현재 탭을 변경
        //}
    }

    public void CloseTab()
    {
        //if (tabStack.Count > 0)
        //{
        //    currentTab.CloseWindow();                          // 기존 탭 닫기
        //    ComputerWindow tab = tabStack.Pop();               // 마지막 탭 팝
        //    tab.OpenWindow();
        //    currentTab = tab;                               // 마지막 탭 활성화 및 현재 탭으로 변경
        //}
    }
    #endregion

    // 캔버스 크기 변화 연출
    private IEnumerator ScaleChange(bool scaleIn)
    {
        Vector2 start = scaleIn ? Vector2.zero : originSize;
        Vector2 end = scaleIn ? originSize : Vector2.zero;

        tabCavas.transform.localScale = start;                          // 스케일 초기화

        if (scaleIn)
        {
            tabCavas.enabled = true;                                    // 스케일 인이면 활성화
        }

        float t = 0;

        while(t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            tabCavas.transform.localScale = Vector2.Lerp(start, end, t);

            yield return null;
        }

        tabCavas.transform.localScale = end;

        if (!scaleIn)
        {
            tabCavas.enabled = false;                                   // 스케일 아웃이면 비활성화
        }
    }
}
