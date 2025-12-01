using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Door : InteractionReceiver
{
    [Header("문 오브젝트")]
    public List<DoorPanel> doorPanels = new List<DoorPanel>();

    [Header("문 오디오 설정")]
    public AudioSource doorAudioSource;
    public AudioClip doorOpenAudioClip;
    public AudioClip doorCloseAudioClip;

    //[Header("문 설정")]
    [HideInInspector] public Vector3 openPosition;
    [HideInInspector] public Vector3 closePosition;

    [Header("임시 애니메이션 설정")]
    public float moveSpeed = 2f;

    [Header("문 열림 확인")]
    public bool isOpened = false;

    private bool isMoving = false;
    private bool isPermissionDoor = false;
    private Vector3 targetPosition;

    [Header("문을 열기 위한 아이템")]
    public ItemDataSO needItem;

    private bool canPlayDialog = true; //대사 재생 가능 여부
    public float dialogCooldown;  //쿨타임

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
    private void TryPlayNoPermissionDialog()
    {
        if (!canPlayDialog)
        {
            return;
        }

        canPlayDialog = false;
        DialogueManager.Instance.StopAllDialog();
        DialogueManager.Instance.PlayingDialog("AI_Door_NoPermission");

        StartCoroutine(DialogCooldown());
    }

    private IEnumerator DialogCooldown()
    {
        yield return new WaitForSeconds(dialogCooldown);
        canPlayDialog = true;
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
            if(needItem != null && InventoryManager.ContainItem(needItem.uniqueID))
            {
                OpenDoor();
                isOpened = true;
            }
            else
            {
                DialogueManager.Instance.StopAllDialog();
                TryPlayNoPermissionDialog();
            }
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (isPermissionDoor)
        {
            return;
        }

        if (isOpened == true && collider.CompareTag("Player"))
        {
            CloseDoor();
            isOpened = false;
        }
    }

    private bool hasPlayedDoorOpen = false;

    public void OpenDoor()
    {
        foreach (var panel in doorPanels)
        {
            panel.StartMove(true);
        }

        doorAudioSource.PlayOneShot(doorOpenAudioClip);

        if (hasPlayedDoorOpen)
        {
            return;
        }

        hasPlayedDoorOpen = true;

        DialogueManager.Instance.StopAllDialog();
        DialogueManager.Instance.PlayingDialog("AI_Door_Open");
    }

    public void CloseDoor()
    {
        foreach (var panel in doorPanels)
        {
            panel.StartMove(false);
            doorAudioSource.PlayOneShot(doorCloseAudioClip);
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
