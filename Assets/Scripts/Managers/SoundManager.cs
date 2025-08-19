using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[System.Serializable]
public class Sound
{
    public string name;                         //사운드의 이름
    public AudioClip clip;                      //사운드 클립

    [Range(0f, 1f)]
    public float volume = 1.0f;
    [Range(0.1f, 3f)]
    public float pitch = 1.0f;                  //사운드 피치
    public bool loop;                           //반복 재생 여부
    public AudioMixerGroup mixerGroup;          //오디오 믹서 그룹
}


public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    public static SoundManager instance;                    //싱글톤 인스턴스 (static은 전역 변수로 올려서 어디서든 접근할 수 있게 해준다.)
    public List<Sound> sounds = new List<Sound>();          //사운드 리스트 관리 (List 자료구조 관리)
    public AudioMixer audioMixer;

    private Dictionary<string, Sound> soundsList = new Dictionary<string, Sound>();

    public Sound FindToGetSoundByNameName(string name)
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
