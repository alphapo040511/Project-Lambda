using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound
{
    //public string name;                         //사운드의 이름( 클립 이름으로 대체 )
    public AudioClip clip;                      //사운드 클립

    [Range(0f, 1f)]
    public float volume = 1.0f;
    [Range(0.1f, 3f)]
    public float pitch = 1.0f;                  //사운드 피치
    public bool loop = false;                   //반복 재생 여부
    public AudioMixerGroup mixerGroup;          //오디오 믹서 그룹
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public List<Sound> sounds = new List<Sound>();          //사운드 리스트 관리 (List 자료구조 관리)
    public AudioMixer audioMixer;


    private Dictionary<string, AudioSource> soundsList = new Dictionary<string, AudioSource>();

    private void Start()
    {
        InitializedSound();
    }

    // AudioSource 목록 초기화
    private void InitializedSound()
    {
        foreach (var sound in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = sound.mixerGroup;

            soundsList[sound.clip.name] = source;           // 클립 이름과 동일하게 설정
        }
    }

    public bool PlaySound(string name)
    {
        AudioSource source = FindToGetSoundByNameName(name);

        if(source == null)
        {
            return false;
        }
        else
        {
            source.Play();
            return true;
        }
    }

    public bool StopSound(string name)
    {
        AudioSource source = FindToGetSoundByNameName(name);

        if (source == null)
        {
            return false;
        }
        else
        {
            source.Stop();
            return true;
        }
    }

    public AudioSource FindToGetSoundByNameName(string name)
    {
        if (soundsList.ContainsKey(name))
        {
            return soundsList[name];
        }
        else
        {
            Debug.LogWarning($"{name} 사운드를 찾을 수 없습니다.");
            return null;
        }

    }
}
