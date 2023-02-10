using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class JHW_SoundManager : MonoBehaviour
{
    [SerializeField] int Basic_initialize;
    [SerializeField] Dictionary<BGM_list, AudioClip> BGM_audioclips = new Dictionary<BGM_list, AudioClip>();
    [SerializeField] Dictionary<SFX_list, AudioClip> SFX_audioclips = new Dictionary<SFX_list, AudioClip>();

    [SerializeField] private float volume_BGM = 1f;
    [SerializeField] private float volume_SFX = 1f;

    [SerializeField] public List<BGM_Datas> BGM_datas = new List<BGM_Datas>();
    [SerializeField] public List<SFX_Datas> SFX_datas = new List<SFX_Datas>();

    private static JHW_SoundManager instance;

    // 멈출 효과음
    private SFX_list toStopSfx;

    private enum SoundType {
        BGM,
        SFX,
    }

    [System.Serializable]
    [SerializeField]
    public struct SFX_Datas
    {
        public SFX_list sfx_name;
        public AudioClip audio;
    }

    [System.Serializable]
    [SerializeField]
    public struct BGM_Datas
    {
        public BGM_list bgm_name;
        public AudioClip audio;
    }


    // 효과음 목록
    public enum SFX_list
    {
        EQUIP_ON_UNIT,
        UI_CLICK1,
        UI_CLICK2,
        BOW_ATTACK1,
        BOW_ATTACK2,
        BOW_ATTACK3,
        ITEM_UNEQUIP,
        SHOP_SELL,
        HUMAN_005_ATTACK,
        HUMAN_005_CRIT,
        DWARP_001_ATTACK,
        UNLOCK1,
        UNLOCK2,
        UNLOCK3,
        UNLOCK4,
        UPGRADE,
        DARKELF_005_SKILL,
        HIGHELF_002_SKILL,
        HIGHELF_004_SKILL,
        HIGHELF_001_SKILL,
        SPIRIT_007_SKILL1,
        SPIRIT_007_SKILL3,
        SPIRIT_007_SKILL_ICE1,
        SPIRIT_007_SKILL_ICE2,
        SPIRIT_007_SKILL_ICE3,
        DWARP_003_SKILLL,
        CELESTIAL_003_SKILL,
        DEVIL_004_SKILL,
        DEVIL_005_SKILL,
        GOBLIN_006_SKILL,
        DEVIL_002_SKILL,
        UNDEAD_001_SKILL,
        UNIT_DEATH,
        EVENT_OPEN,
        EVENT_SELECT_CHOICE,
        GOLD_GET,
        PAPER_WHIP,
        BUTTON_MOUSEOVER,
        UNIT_PURCHASE,
        ITEM_PURCHASE,
        UNIT_2STAR,
        UNIT_3STAR,
        ITEM_EQUIP,
        BUTTON_CLICK,
        DUNGEON_ENTER,
        SHOP_ENTER,
        BASECAMP_OPEN_UNIT_DELETE_TITLE,
        BASECAMP_DELETE_UNIT,
        RECOVER,
        BUFF_ACTIVE,
        BUFF_DEACTIVE,
        DRINK_POTION,
        UNIT_ARRANGE,
        UNIT_HOLD,
        SYNERGY_FLATINUM,
        SYNERGY_GOLD,
        SYNERGY_SILVER,
        SYNERGY_BRONZE,
        REROLL,
        LEVELUP_PURCHASE_CLICK,
        LEVELUP,
        UNIT_PANEL_OPEN,
        UNIT_DROP,
        UNIT_PANEL_CLOSE,
        BONFIRE,
        BATTLEFIELD_ANY_SOUND,
        OPTION_OPEN,
        OPTION_CLOSE,
        BOOKSHELF_WHIP,
        GAME_VICTORY,
        GAME_DEFEAT,
        ROUND_VICTORY,
        ROUND_DEFEAT
    }

    // 배경음 목록
    public enum BGM_list
    {
        temp_BGM,
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
                    // 리스트에 넣은 SFX audioClip 을 모두 dictionary에 저장
                    for(int i = 0; i < SFX_datas.Count; i++)
                    {
                        if (SFX_datas[i].audio == null) continue; // 효과음 없으면 저장X
                        SFX_audioclips.Add(SFX_datas[i].sfx_name, SFX_datas[i].audio);
                    }
                    // 리스트에 넣은 BGM audioClip 을 모두 dictionary에 저장
                    for (int i = 0; i < BGM_datas.Count; i++)
                    {
                        if (BGM_datas[i].audio == null) continue; // 배경음 없으면 저장X
                        BGM_audioclips.Add(BGM_datas[i].bgm_name, BGM_datas[i].audio);
                    }
                    // 메서드 등록
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___GET__INSTANCE, JHW_GetSoundManager);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__BGM_NAME, PlayBGM);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, PlaySFX);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_STOP, SFX_stop);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_ISPLAYING, SFX_isPlaying);

                    Basic_initialize = -1;
                }
                break;
            }
        }


    //// 음악 또는 효과음 불러오기 또는 저장
    AudioClip GetOrAddAudioClip(SFX_list name, SoundType st)
    {
        AudioClip audioClip = null;


        // 배경음을 찾는다
        if (st == SoundType.BGM)
        {
            // 배경음 클립 없으면 딕셔너리에 붙이기
            if (SFX_audioclips.TryGetValue(name, out audioClip) == false)
            {
                // 배경음 불러오고 딕셔너리 저장
                audioClip = Resources.Load<AudioClip>("Sounds/BGM/" + name);
                SFX_audioclips.Add(name, audioClip);
            }

            // 찾지 못하면 에러
            if (audioClip == null)
                Debug.Log($"AudioClip Missing ! {name}");
        }

        // 효과음을 찾는다
        else if (st == SoundType.SFX)
        {
            // 효과음 클립 없으면 딕셔너리에 붙이기
            if (SFX_audioclips.TryGetValue(name, out audioClip) == false)
            {
                // 효과음 불러오고 딕셔너리 저장
                audioClip = Resources.Load<AudioClip>("Sounds/SFX/" + name);
                SFX_audioclips.Add(name, audioClip);
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
    public object PlayBGM(params object[] _arg)
    {
        // 사운드 이름
        BGM_list playSoundName = (BGM_list)_arg[0];

        // 사운드 객체
        GameObject soundObject = null;

        // 만약 실행중인 BGM 이 있다면 중단하고 다른 BGM으로 변경
        if (GameObject.Find("SoundManager/BGM").transform.childCount >= 1)
        {
            soundObject = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject;
            AudioSource audioSource = soundObject.GetComponent<AudioSource>(); // 컴포넌트 불러오기
            audioSource.clip = BGM_audioclips[playSoundName]; // 음악 불러오기
            audioSource.Play(); // 음악 재생
        }

        // 실행중인 BGM 이 없다면 사운드 오브젝트 생성 후 BGM에 장착
        else
        {
            soundObject = new GameObject("Sound");
            soundObject.transform.parent = GameObject.Find("SoundManager/BGM").transform;
            AudioSource audioSource = soundObject.AddComponent<AudioSource>(); // 컴포넌트 생성
            audioSource.clip = BGM_audioclips[playSoundName]; // 음악 불러오기
            audioSource.loop = true; // 반복재생
            audioSource.Play(); // 음악 재생
        }

        return true;
    }

    // 사운드 재생 - 효과음 (풀링 적용됨)
    public object PlaySFX(params object[] _arg)
    {
        // 사운드 이름
        SFX_list playSoundName = (SFX_list)_arg[0];

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
            //audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.SFX);
            audioSource.clip = SFX_audioclips[playSoundName];
            audioSource.volume = volume_SFX;
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
            //audioSource.clip = GetOrAddAudioClip(playSoundName, SoundType.SFX);
            audioSource.clip = SFX_audioclips[playSoundName];
            audioSource.volume = volume_SFX;
            soundObject.SetActive(false);
        }
            
        // 사운드오브젝트 오브젝트 선택 후 활성화
        soundObject = soundPool.transform.GetChild(idx).gameObject;
        soundObject.SetActive(true);

        // 사운드 재생 (Play -> PlayOneshot 으로 변경됨)
        //soundObject.GetComponent<AudioSource>().PlayOneShot(GetOrAddAudioClip(playSoundName, SoundType.SFX),volume_SFX);
        soundObject.GetComponent<AudioSource>().PlayOneShot(SFX_audioclips[playSoundName], volume_SFX * 0.1f);

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
    
    // 음악 일시정지 또는 플레이
    public void BGM_pauseOrPlay()
    {
        AudioSource audioSource = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        if (audioSource == null) return;
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }


    // 음악 재생/일시정지 버튼
    public void BGM_playButton()
    {
        // 음악 재생중이면 일시정지, 일시정지면 재생
        AudioSource audioSource = GameObject.Find("SoundManager/BGM").transform.GetChild(0).gameObject.GetComponent<AudioSource>();
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }

    // 지연시간 뒤 효과음 정지
    public object SFX_stop(params object[] _arg)
    {
        toStopSfx = (SFX_list)_arg[0];
        float playtime = (float)_arg[1];
        Invoke("sfxStop", playtime);
        return true;
    }
    private void sfxStop()
    {
        GameObject.Find(toStopSfx.ToString() + "Pool").transform.GetChild(0).gameObject.SetActive(false);
    }

    // 효과음 재생중인지 체크
    public object SFX_isPlaying(params object[] _arg)
    {
        SFX_list toCheckSfx = (SFX_list)_arg[0];
        if (GameObject.Find(toCheckSfx.ToString() + "Pool").transform.GetChild(0).gameObject.activeSelf == true) return true;
        else return false;
    }
}