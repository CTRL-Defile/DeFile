using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JHW_SoundManager : MonoBehaviour
{
    [SerializeField] int Basic_initialize;
    [SerializeField] Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    [SerializeField] private float volume_BGM = 1f;
    [SerializeField] private float volume_SFX = 1f; 

    private static JHW_SoundManager instance;

    private enum SoundType {
        BGM,
        SFX,
    }
    

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_initialize)
        {
            case -1: break;
            //
            case 0:
                {
                    if (instance == null)
                    {
                        instance = this;
                        DontDestroyOnLoad(instance);
                    }
                    else
                    {
                        //Destroy(gameObject);
                    }
                    Basic_initialize = 1;
                }
                break;
            case 1:
                {
                    // 메서드 등록
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___GET__INSTANCE, JHW_GetSoundManager);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__BGM_NAME, JHW_playBGM);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_playSFX);

                    Basic_initialize = -1;
                }
                break;
            }
        }


    // 음악 또는 효과음 불러오기 또는 저장
    AudioClip GetOrAddAudioClip(string name, SoundType st)
    {
        AudioClip audioClip = null;
        

        // 배경음을 찾는다
        if(st == SoundType.BGM)
        {
            // 배경음 클립 없으면 딕셔너리에 붙이기
            if (_audioClips.TryGetValue(name, out audioClip) == false)
            {
                // 배경음 불러오고 딕셔너리 저장
                audioClip = Resources.Load<AudioClip>("Sounds/BGM/" + name);
                _audioClips.Add(name, audioClip);
            }

            // 찾지 못하면 에러
            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {name}");
        }

        // 효과음을 찾는다
        else if (st == SoundType.SFX)
        {
            // 효과음 클립 없으면 딕셔너리에 붙이기
            if (_audioClips.TryGetValue(name, out audioClip) == false)
            {
                // 효과음 불러오고 딕셔너리 저장
                audioClip = Resources.Load<AudioClip>("Sounds/SFX/" + name);
                _audioClips.Add(name, audioClip);
            }

            // 찾지 못하면 에러
            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {name}");
        }

        return audioClip;
    }

    //// 사운드매니저 가져오기
    //public static object JHW_GetSoundManager(params object[] _arg)
    //{
    //    Debug.Log("?");
    //    if (instance == null)
    //    {
    //        instance = new JHW_SoundManager();
    //    }
    //    return instance;
    //}
    
    
    // 사운드 재생 - 배경
    public object JHW_playBGM(params object[] _arg)
    {
        // 사운드 이름
        string playSoundName = (string)_arg[0];

        // 사운드 객체
        GameObject soundObject = null;

        // 만약 실행중인 BGM 이 있다면 중단하고 다른 BGM으로 변경
        if (GameObject.Find("SoundManager/BGM").transform.childCount >= 1)
        {
            soundObject = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject;
            AudioSource audioSource = soundObject.GetComponent<AudioSource>(); // 컴포넌트 불러오기
            audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.BGM); // 음악 불러오기
            audioSource.Play(); // 음악 재생
        }

        // 실행중인 BGM 이 없다면 사운드 오브젝트 생성 후 BGM에 장착
        else
        {
            soundObject = new GameObject("Sound");
            soundObject.transform.parent = GameObject.Find("SoundManager/BGM").transform;
            AudioSource audioSource = soundObject.AddComponent<AudioSource>(); // 컴포넌트 생성
            audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.BGM); // 음악 불러오기
            audioSource.loop = true; // 반복재생
            audioSource.Play(); // 음악 재생
        }

        return true;
    }

    // 사운드 재생 - 효과음 (풀링 적용됨)
    public object JHW_playSFX(params object[] _arg)
    {
        // 사운드 이름
        string playSoundName = (string)_arg[0];

        // 효과음 풀 없으면 생성
        GameObject soundPool = GameObject.Find(playSoundName + "Pool");
        if (soundPool == null)
        {
            soundPool = new GameObject(playSoundName + "Pool");
            soundPool.transform.parent = GameObject.Find("SoundManager/SFX").transform;
        }

        // 사운드 오브젝트 선택
        GameObject soundObject;
        // 사운드 풀에 오브젝트가 없으면 새로 만들고 풀에 넣자
        if (soundPool.transform.childCount==0)
        {
            // 사운드 오브젝트 생성
            soundObject = new GameObject(playSoundName + "Sound");
            soundObject.transform.parent = soundPool.transform; // 사운드풀에 저장
            AudioSource audioSource = soundObject.AddComponent<AudioSource>(); // 컴포넌트 생성
            audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.SFX);
            soundObject.SetActive(false);
        }
        // 사운드 오브젝트 선택
        int idx = 0;
        while(idx<soundPool.transform.childCount)
        {
            // 사용중이지 않은 사운드 오브젝트 고를 때까지 idx 증가
            if (soundPool.transform.GetChild(idx).gameObject.activeSelf == true) idx++;
            else break;
        }
        // 만약 idx가 pool의 최대 인덱스까지 도착하면 오브젝트 새로 만들고 저장
        if (idx==soundPool.transform.childCount)
        {
            // 사운드 오브젝트 생성
            soundObject = new GameObject(playSoundName + "Sound");
            soundObject.transform.parent = soundPool.transform; // 사운드풀에 저장
            AudioSource audioSource = soundObject.AddComponent<AudioSource>(); // 컴포넌트 생성
            audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.SFX);
            soundObject.SetActive(false);
        }
            
        // 사운드오브젝트 오브젝트 선택 후 활성화
        soundObject = soundPool.transform.GetChild(idx).gameObject;
        soundObject.SetActive(true);

        // 사운드 재생 (Play -> PlayOneshot 으로 변경됨)
        soundObject.GetComponent<AudioSource>().PlayOneShot(GetOrAddAudioClip(playSoundName, SoundType.SFX),volume_SFX);

        // 사운드 다 재생되면 비활성화
        StartCoroutine(soundSetActive(soundObject));

        return true;
    }

    IEnumerator soundSetActive(GameObject soundObject)
    {
        // 지연시간 뒤 사운드 오브젝트 비활성화
        yield return new WaitForSeconds(soundObject.GetComponent<AudioSource>().clip.length);

        soundObject.SetActive(false);
        soundObject.transform.SetAsFirstSibling(); // 자식 앞으로 이동
        StopCoroutine(soundSetActive(null));
    } 

    // 배경 볼륨조절
    public void BGM_volumeControl()
    {
        volume_BGM = GameObject.Find("BGM Slider").GetComponent<Slider>().value;

        // 컴포넌트 불러와서 볼륨 조절
        if (GameObject.Find("SoundManager/BGM").transform.childCount == 0) return;
        GameObject soundObject = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject;
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();
        audioSource.volume = volume_BGM;
    }

    // 효과음 볼륨조절
    public void SFX_volumeControl()
    {
        volume_SFX = GameObject.Find("SFX Slider").GetComponent<Slider>().value;

        // SFX 풀에 있는 사운드들 불러와서 볼륨 조절
        GameObject soundPools = GameObject.Find("SoundManager/SFX");

        for(int i=0;i< soundPools.transform.childCount; i++)
        {
            GameObject soundPool = soundPools.transform.GetChild(i).gameObject;
            for(int j = 0; j < soundPool.transform.childCount; j++)
            {
                GameObject soundObject = soundPool.transform.GetChild(j).gameObject;
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                audioSource.volume = volume_SFX;
            }
        }
    }

    // 음악 재생/일시정지 버튼
    public void BGM_playButton()
    {
        // 재생중인 음악 없으면 음악 재생
        if (GameObject.Find("SoundManager/BGM").transform.childCount == 0)
        {
            JHW_playBGM("Leafre");
            return;
        }
        
        // 음악 재생중이면 일시정지, 일시정지면 재생
        AudioSource audioSource = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }
}