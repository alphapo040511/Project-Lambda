using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ComputerSystem : InteractionFocus
{
    [Header("Display Settings")]
    public CanvasGroup displayCanvas;           // 디스플레이 화면
    public Transform bootingMessage;            // 부팅 메세지 화면
    public Transform DesktopBackground;         // 바탕 화면

    [Header("Program Settings")]
    public List<WindowBase> programList = new List<WindowBase>();
    private Dictionary<string, WindowBase> programs = new Dictionary<string, WindowBase>();

    [Header("Password Settings")]
    public Transform passwordDisplay;           // 패스워드 화면
    public TMP_InputField passwordInput;        // 패스워드 인풋 필드
    public PasswordData passwordData;           // 비밀번호가 있는 경우 비밀번호 데이터
    private bool isUnlocked = false;            // 잠금이 해제 되었는지 여부
    private bool isPoweredOn = false;           // 전원이 켜졌는지 여부

    private Stack<WindowBase> tabStack = new Stack<WindowBase>();
    private WindowBase currentTab;

    private void Start()
    {
        InitializeTab();
        InitPassword();

        if(displayCanvas != null)
        {
            displayCanvas.alpha = 0f;
            displayCanvas.gameObject.SetActive(false);
        }
    }

    private void InitializeTab()
    {
        foreach(var program in programList)
        {
            program.Initialize(this);
            programs.Add(program.windowName, program);
        }
    }

    private void InitPassword()
    {
        if (passwordData.password == "")
            isUnlocked = true;                      // 비밀번호가 비어있는 경우 잠금 해제
    }

    protected override void EnterFocus()
    {
        base.EnterFocus();

        if (!isPoweredOn)
            StartCoroutine(SetupDisplay());                         // 처음 1회만 전원 키기
    }

    // 비밀번호 체크 (임시 -> Password 탭 분리 예정)
    public void CheckPassword()
    {
        if(passwordInput != null && passwordData.password != "")
        {
            if(passwordData.IsUnlocked(passwordInput.text))             // 패스워드가 맞는 경우
            {
                passwordDisplay.gameObject.SetActive(false);
                DesktopBackground.gameObject.SetActive(true);                        // 메인 화면 활성화
            }
            else
            {
                passwordInput.text = "Error";
            }
        }
    }

    public void OpenWindow(string windowName)
    {
        if(programs.ContainsKey(windowName) && !programs[windowName].isActive)
        {
            programs[windowName].OpenWindow();
        }
    }

    public void CloseWindow(string windowName)
    {
        if (programs.ContainsKey(windowName) && programs[windowName].isActive)
        {
            programs[windowName].CloseWindow();
        }
    }

    IEnumerator SetupDisplay()
    {
        yield return StartCoroutine(PowerOn());                     // 부팅 화면 연출

        if (isUnlocked)                                             // 패스워드가 존재/해제된 경우
        {
            DesktopBackground.gameObject.SetActive(true);           // 홈 활성화
        }
        else                                                        // 패스워드가 존재/해제안된 경우
        {
            passwordDisplay.gameObject.SetActive(true);             // 패드워드 화면 활성화
        }
    }

    IEnumerator PowerOn()
    {
        if(displayCanvas != null)
        {
            yield return DisplayFade(true);                         // 배경 캔버스 페이드
        }

        bootingMessage.gameObject.SetActive(true);                  // 로딩 메세지는 따로 활성화

        yield return new WaitForSeconds(1f);

        bootingMessage.gameObject.SetActive(false);                 // 대기 후 로딩 메세지 제거
    }

    // 디스플레이 페이드 인/아웃
    private IEnumerator DisplayFade(bool fadeIn)
    {
        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;

        displayCanvas.alpha = start;

        if (fadeIn)
        {
            displayCanvas.gameObject.SetActive(true);                   // 페이드 인이면 활성화
        }

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime;
            displayCanvas.alpha = Mathf.Lerp(start, end, t);

            yield return null;
        }

        displayCanvas.alpha = end;

        if (!fadeIn)
        {
            displayCanvas.gameObject.SetActive(false);                   // 페이드 아웃이면 비활성화
        }
    }
}
