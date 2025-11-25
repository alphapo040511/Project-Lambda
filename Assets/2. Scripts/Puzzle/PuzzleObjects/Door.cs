using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : InteractionReceiver
{
    [Header("문 오브젝트")]
    public List<DoorPanel> doorPanels = new List<DoorPanel>();

    [Header("문 설정")]
    public Vector3 openPosition;
    public Vector3 closePosition;

    [Header("임시 애니메이션 설정")]
    public float moveSpeed = 2f;

    [Header("문 열림 확인")]
    public bool isOpened = false;

    private bool isMoving = false;
    private bool isPermissionDoor = false;
    private Vector3 targetPosition;

    private void Start()
    {
        if (gameObject.name.Contains("PermissionDoor"))
            isPermissionDoor = true;
    }

    protected override void ActorUpdate()
    {
        foreach (var panel in doorPanels)
        {
            panel.UpdateMovement(moveSpeed);
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (isPermissionDoor)
        {
            //DialogueManager.Instance.StopAllDialog();
            //DialogueManager.Instance.PlayingDialog("AI_Door_Open");
        }

        if (collider.CompareTag("Player"))
        {
            OpenDoor();
            isOpened = true;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (isPermissionDoor) return;

        if (isOpened == true && collider.CompareTag("Player"))
        {
            CloseDoor();
            isOpened = false;
        }
    }

    public void OpenDoor()
    {
        //DialogueManager.Instance.StopAllDialog();
        //DialogueManager.Instance.PlayingDialog("AI_Door_Open");

        foreach (var panel in doorPanels)
        {
            panel.StartMove(true);
        }
    }

    public void CloseDoor()
    {
        foreach (var panel in doorPanels)
        {
            panel.StartMove(false);
        }
    }

    public override void OnInteractionComplete(bool shouldActivate)
    {
        if(shouldActivate)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    [System.Serializable]
    public class DoorPanel
    {
        public Transform doorTransform;
        public Vector3 openPosition;
        public Vector3 closePosition;

        private Vector3 targetPosition;
        private bool isMoving = false;

        public void StartMove(bool open)
        {
            targetPosition = open ? openPosition : closePosition;
            isMoving = true;
        }

        public void UpdateMovement(float speed)
        {
            if (!isMoving) return;

            if (Vector3.Distance(doorTransform.localPosition, targetPosition) > 0.01f)
            {
                doorTransform.localPosition =
                    Vector3.Lerp(doorTransform.localPosition, targetPosition, Time.deltaTime * speed);
            }
            else
            {
                doorTransform.localPosition = targetPosition;
                isMoving = false;
            }
        }
    }

    #region 인스펙터 기능
    [ContextMenu("열림 위치 저장")]
    public void SaveOpenPosition()
    {
        openPosition = transform.localPosition;
    }

    [ContextMenu("닫힘 위치 저장")]
    public void SaveClosePosition()
    {
        closePosition = transform.localPosition;
    }
    #endregion
}
