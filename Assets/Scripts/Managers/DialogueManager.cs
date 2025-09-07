using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class DialogData
{
    public string index;
    public AudioClip clip;
    public string text;
    // 시트로 정리해서 다음 인덱스 저장 및 자동 불러오기 기능 추가
}


public class DialogueManager : SingletonMonoBehaviour<DialogueManager>
{
    public List<DialogData> dialogues = new List<DialogData>();
    public AudioMixerGroup audioMixer;

    private Dictionary<string, DialogData> dialogList = new Dictionary<string, DialogData>();

    private Queue<DialogData> dialogQueue = new Queue<DialogData>();
    private bool isShowing = false;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioMixer;

        InitializedDialog();
    }

    private void InitializedDialog()
    {
        foreach(DialogData data in dialogues)
        {
            dialogList[data.index] = data;
        }
    }

    // 다이얼로그 목록을 큐애 넣고 재생, 나중에는 대사 묶음을 DB로 관리
    public void EnqueueDialog(string index)
    {
        DialogData data = GetDialogData(index);

        if(data != null)
        {
            dialogQueue.Enqueue(data);

            if (!isShowing)
            {
                StartCoroutine(ProcessQueue());
            }
        }
    }

    // 모든 다이얼로그 정지
    public void StopAllDialog()
    {
        StopAllCoroutines();                    // 코루틴은 저장해서 사용하는걸로 변경
        dialogQueue.Clear();
        isShowing = false;
        audioSource.Stop();
    }

    public DialogData GetDialogData(string index)
    {
        if (dialogList.ContainsKey(index))
        {
            return dialogList[index];
        }
        else
        {
            Debug.LogWarning($"{index} Dialog 파일을 찾을 수 없습니다.");
            return null;
        }
    }

    private IEnumerator ProcessQueue()
    {
        isShowing = true;

        while (dialogQueue.Count > 0)
        {
            var message = dialogQueue.Dequeue();

            ToastMessageSystem.Instance.ShowMessage(message.text);                                  // 토스트 메세지 표시
            audioSource.PlayOneShot(message.clip);

            yield return new WaitForSeconds(message.clip.length);
        }

        ToastMessageSystem.Instance.ClearMessage();

        isShowing = false;
    }
}
