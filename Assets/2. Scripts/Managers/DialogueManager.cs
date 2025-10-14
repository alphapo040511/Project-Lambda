using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;



public class DialogueManager : SingletonMonoBehaviour<DialogueManager>
{
    public DialogDatabaseSO databaseSO;
    public AudioMixerGroup audioMixer;

    private AudioSource audioSource;

    private Coroutine dialogCoroutine;
    private DialogSO currentDialog;

    protected override void Awake()
    {
        base.Awake();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.outputAudioMixerGroup = audioMixer;
    }

    private void OnEnable()
    {
        GameEvents.OnChangeGameState += HandleStateChange;
    }

    private void OnDisable()
    {
        GameEvents.OnChangeGameState -= HandleStateChange;
    }


    // 다이얼로그 목록을 큐애 넣고 재생, 나중에는 대사 묶음을 DB로 관리
    public void PlayingDialog(string index)
    {
        DialogSO dialogSO = databaseSO.GetDialogById(index);

        if (dialogSO != null)
        {
            currentDialog = dialogSO;

            if (dialogCoroutine != null)
            {
                StopCoroutine(dialogCoroutine);
            }

            dialogCoroutine = StartCoroutine(ShowDialog());
        }
    }

    // 모든 다이얼로그 정지
    public void StopAllDialog()
    {
        if (dialogCoroutine != null)
        {
            StopCoroutine(dialogCoroutine);
        }

        audioSource.Stop();                                 // 오디오 정자

        ToastMessageSystem.Instance.ClearMessage();         // 자막 정지
    }

    private IEnumerator ShowDialog()
    {
        while (true)                     
        {
            ToastMessageSystem.Instance.ShowMessage(currentDialog);                             // 토스트 메세지 표시
            audioSource.PlayOneShot(currentDialog.clip);

            yield return StartCoroutine(Timer(currentDialog.clip.length));

            if (string.IsNullOrEmpty(currentDialog.nextId)) break;                              // 다음 대사가 없을 때 까지 반복

            DialogSO dialogSO = databaseSO.GetDialogById(currentDialog.nextId);
            if (dialogSO == null)
                break;                                                        // 다음 대사가 없는경우
            else
                currentDialog = dialogSO;
        }

        ToastMessageSystem.Instance.ClearMessage();
    }

    // 일시정지가 아닌 경우에만 시간이 가도록 설정
    private IEnumerator Timer(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            if (GameManager.Instance.currentGameState != GameState.Paused)
                timer += Time.deltaTime;
            yield return null;
        }
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Paused)
        {
            audioSource.Pause();
        }
        else if(state == GameState.Menu)            // 메뉴로 돌아가면 종료
        {
            StopAllDialog();
        }
        else
        {
            audioSource.UnPause();
        }
    }
}
