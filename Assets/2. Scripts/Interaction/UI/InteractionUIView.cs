using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUIView : MonoBehaviour
{
    private Interactable targetObject;

    [Header("Main UI Elements")]
    public TMP_Text keycodeText;
    public Image groundImage;
    public Image outlineImage;

    [Header("Color Settings")]
    public Color selectedColor = new Color(0, 0, 0, 0.8f);
    public Color deselectedColor= new Color(0, 0, 0, 0.3f);

    private void OnEnable()
    {
        GameEvents.OnChangeGameState += ChangeGameStats;
    }
    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= ChangeGameStats;
    }

    public void Initialize(Interactable targetObject)
    {
        this.targetObject = targetObject;
        OnDeselected();
    }

    public void ActivateInteractionUI()
    {
        if (targetObject == null)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 pos = targetObject.FloatingUIPosition;

        Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);
        transform.position = screenPos;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (targetObject == null)               // 오브젝트가 제거된 경우 비활성화
        {
            gameObject.SetActive(false);
            return;
        }

        Vector3 pos = targetObject.FloatingUIPosition;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(pos);

        bool isVisible = viewportPos.z > 0 &&
                         viewportPos.x >= 0 && viewportPos.x <= 1 &&
                         viewportPos.y >= 0 && viewportPos.y <= 1;

        if (!isVisible)                                                 // 화면 밖에 있는 경우
        {
            targetObject.OnDeactivate();                                // 범위 이탈로 비활성화
        }
        else                                                            // 화면 안에 있는 경우
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(pos);    // 타겟 위치에 지속적으로 플로팅
            transform.position = screenPos;
        }
    }

    public void ChangeGameStats(GameState gameState)
    {
        // 게임이 멈추면 UI 제거
        if(gameState != GameState.Playing)
        {
            targetObject.OnDeactivate();
        }
    }

    #region Interaction System

    // 선택되었을 때 색상 변경
    public void OnSelected()
    {
        //keycodeText.enabled = true;
        groundImage.color = selectedColor;
    }

    // 선택 해제되었을 때 색상 원상복귀
    public void OnDeselected()
    {
        //keycodeText.enabled = false;
        groundImage.color = deselectedColor;
        outlineImage.fillAmount = 0;
    }

    public void OnInteractHold(float amount)
    {
        outlineImage.fillAmount = amount;
    }

    #endregion
}
