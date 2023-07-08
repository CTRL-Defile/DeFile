using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.UI;
using DG.Tweening;

public enum HYJ_Map_Stage_TYPE
{
    NONE,
    BASE_CAMP,
    BATTLE_NORMAL,  // 30%
    BATTLE_ELITE,   // 30%
    BATTLE_BOSS,
    SHOP,
    EVENT           // 40%
}

public partial class HYJ_Map_Stage : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //
    public void HYJ_Init(
        Transform _parent,
        int _x, int _y)
    {
        Transform parent = this.transform.parent;
        //parent.parent = _parent;
        parent.SetParent(_parent);
        parent.localScale = new Vector3(1, 1, 1);

        // >> 현우 - 위치 랜덤조정
        //parent.localPosition = new Vector3(_x * 150, _y * 150, 0); // < 원래 있던 스크립트
        parent.localPosition = new Vector3(_x * 150 + Random.Range(-40f,40f), _y * 170 + Random.Range(-30f,30f), 0);
        // >> 현우 - end

        HYJ_Stage_Init();
        HYJ_UI_Init();
    }
}

partial class HYJ_Map_Stage
{
    [System.Serializable]
    public class SaveData
    {
        public HYJ_Map_Stage_TYPE Stage_type;
        public string Stage_reward;
        public bool isSelected = false; // 유저가 선택한 스테이지인지 저장하는 변수

        //////////  Getter & Setter //////////

        //////////  Method          //////////

        //////////  Default Method  //////////
    }
    [SerializeField] SaveData Stage_data;
    [SerializeField] int Stage_power;
    [SerializeField] List<HYJ_Map_Stage> Stage_roots;
    [SerializeField] List<GameObject> Stage_icons;

    // 맵에 보여지는 스테이지 위치
    public int stage_x;
    public int stage_y;

    //////////  Getter & Setter //////////
    public SaveData HYJ_Stage_saveData { get { return Stage_data;   }   set { Stage_data = value;   }   }

    public HYJ_Map_Stage_TYPE HYJ_Stage_type
    {
        get { return Stage_data.Stage_type; }
        set
        {
            Stage_data.Stage_type = value;

            //
            for(int i = 0; i < Stage_icons.Count; i++)
            {
                Stage_icons[i].SetActive(false);
            }

            Stage_icons[(int)Stage_data.Stage_type].SetActive(true);
        }
    }

    public List<HYJ_Map_Stage> HYJ_Stage_roots { get { return Stage_roots;  }   }

    //////////  Method          //////////
    public void HYJ_Stage_SettingType(int _level)
    {
        if(_level <= 4)
        {
            int rand = Random.Range(0, 100);

            int randCount = 40;
            if(rand < randCount)
            {
                HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_NORMAL;
            }
            else
            {
                randCount += 20;
                if (rand < randCount)
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_ELITE;
                }
                else
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.EVENT;
                }
            }
        }
    }

    //
    public void HYJ_Stage_AddRoot(HYJ_Map_Stage _root)
    {
        Stage_roots.Add(_root);
    }

    public void HYJ_Stage_Select()
    {
        bool isSelected = Stage_data.isSelected;

        // 유저가 선택한 스테이지인지를 나타내는 변수 false -> true
        Stage_data.isSelected = true;

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__SET_PLAYER_POS,
            //
            stage_x, stage_y);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__SELECT_RESET);

        Transform parent = this.transform.parent;
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__MOVE_CENTER,
            parent.localPosition.x, parent.localPosition.y, parent.localPosition.z);

        this.transform.DOScale(1.1f, RoadUX_Duration).SetEase(Ease.OutBack);

        // 플레이어가 선택한 버튼을 경유하는 길 얻어오기, ( 만약 시작점에 있을경우 찾지말자  (stage_y!=0) )
        if (stage_y != 0) {
            selectedRoad = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__SELECTED_ROAD, stage_x, stage_y);
           // StartCoroutine(Road_ON_UX(selectedRoad.transform.childCount));
        }
        // 플레이어 위치 갱신 ( 만약 시작점이면 설정X, 매니저에서 시작점 설정 )
        if (stage_y != 0) HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHANGE__PLAYER_POSITION, stage_x, stage_y);
        // 선택한 아이콘 노란색으로
        for (int i = 0; i < Stage_icons.Count; i++) // 선택한 아이콘이 active 되있는 걸 탐색합니다. 만약 상점 아이콘이 active 되있으면 그 active 된 타겟을 UX 적용
            if (Stage_icons[i].activeSelf) {
                GameObject targetObject = Stage_icons[i].transform.GetChild(0).gameObject;
                //targetObject.SetActive(false);

                Sequence IconSequence;
                IconSequence = DOTween.Sequence();
                IconSequence.OnStart(() => targetObject.GetComponent<Image>().color = new Color(1f,1f,1f,1f)); // 처음에 기본 아이콘에서 지연시간뒤 노란색으로 적용
                IconSequence.Insert(RoadUX_Duration-0.5f, targetObject.GetComponent<Image>().DOFade(0f,0.5f))
                    .Join(targetObject.transform.DOScale(1.5f,0.5f))
                    .OnComplete(()=> targetObject.SetActive(false));
                 
                
                break;
            }

        //
        for (int i = 0; i < Stage_roots.Count; i++)
        {
            Stage_roots[i].HYJ_UI_Active(true);
        }

        // 현우 - UX를 위해 기존 아래 구문은 아래 UX에 옮겨 두었습니다.

        //switch(Stage_type)
        //{
        //    case HYJ_Map_Stage_TYPE.BASE_CAMP:
        //        {
        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON,   true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,         false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.SHOP:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON,    true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.EVENT:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON,   true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
        //    case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
        //    case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //}
        StartCoroutine(Delay_ChangeStage(!isSelected));

    }

    public void HYJ_Stage_Select_0(bool _isStage)
    {

    }

    //////////  Default Method  //////////
    void HYJ_Stage_Init()
    {
        Stage_roots = new List<HYJ_Map_Stage>();
    }
}

partial class HYJ_Map_Stage
{
    Button UI_btn;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_UI_Active(bool _isActive)
    {
        // 비활성화된 버튼은 반투명하게, 활성화된 버튼은 선명하게
        int stageIdx = 0;
        switch (this.Stage_data.Stage_type)
        {
            case HYJ_Map_Stage_TYPE.BASE_CAMP:
                stageIdx = 1;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
                stageIdx = 2;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
                stageIdx = 3;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
                stageIdx = 4;
                break;
            case HYJ_Map_Stage_TYPE.SHOP:
                stageIdx = 5;
                break;
            case HYJ_Map_Stage_TYPE.EVENT:
                stageIdx = 6;
                break;
        }

        if (!_isActive) // 버튼 비활성화시
        {
            Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            // 유저가 이미 선택한 버튼일경우 투명도 유지 및 크기 조정
            if (Stage_data.isSelected)
            {
                Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                // 버튼 활성화/비활성화 여부 저장
                UI_btn.interactable = _isActive;
                return;
            }

            Stage_icons[stageIdx].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            this.transform.DOScale(0.7f, 0.5f);
        }
        else // 버튼 활성화시
        {
            Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            Stage_icons[stageIdx].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            this.transform.DOScale(1.5f, 0.5f);
        }

        // 버튼 활성화/비활성화 여부 저장
        UI_btn.interactable = _isActive;

    }

    //////////  Default Method  //////////
    void HYJ_UI_Init()
    {
        UI_btn = gameObject.GetComponent<Button>();
        UI_btn.interactable = false;
    }
}

// UX
partial class HYJ_Map_Stage
{
    GameObject selectedRoad = null; // UX 적용할 발바닥 자국 오브젝트
    GameObject pathLightMarker = null; // UX 적용할 길 위 마커 오브젝트
    int footPrintCnt = 0; // UX 적용할 발바닥 자국 개수
    int curFootPrintCnt = 0;
    float RoadUX_Duration = 1.5f; // 길 UX 지속시간

    //IEnumerator Road_ON_UX(int _footPrintCnt)
    //{
    //    for (int i = 0; i < _footPrintCnt * 3; i++)
    //    {
    //        selectedRoad.transform.GetChild(i / 3).GetChild(i % 3).GetChild(0).gameObject.SetActive(true);
    //        //yield return new WaitForSeconds(0.5f);
    //    }
    //    yield return null;
    //}

    private void Road_UX()
    {
        if (curFootPrintCnt < footPrintCnt)
        {
            GameObject targetRoad = selectedRoad.transform.GetChild(curFootPrintCnt / 3).GetChild(curFootPrintCnt % 3).GetChild(0).gameObject;
            targetRoad.SetActive(true);

            Sequence RoadSequence;
            RoadSequence = DOTween.Sequence();
            RoadSequence.OnStart( () => targetRoad.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f) ); // 처음에 갈색 발자국
            RoadSequence.Append(targetRoad.GetComponent<Image>().DOFade(1f, 0.2f)) // 0.2초 뒤 노란색 발자국으로 변경
                .Join( targetRoad.transform.DOScale(1.3f,0.2f)); // 발자국 크기 증가
            curFootPrintCnt++;

            Invoke("Road_UX", RoadUX_Duration/footPrintCnt);
        }
    }

    IEnumerator Delay_ChangeStage(bool _isStage)
    {
        if (selectedRoad != null && curFootPrintCnt==0)
        {
            // 다른 스테이지를 선택하는거 막기 위해 화면에 검은 화면 active
            GameObject blackScreen = null;
            if (_isStage)
            {
                blackScreen = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__BLACKSCREEN);
                blackScreen.SetActive(true);
            }

            // active되어있는 발자국 수만큼 발자국 노란색으로 되는 UX 적용
            while (selectedRoad.transform.GetChild(footPrintCnt/3).gameObject.activeSelf == true)
            {
                footPrintCnt+=3;
            }

            curFootPrintCnt = 0;
            Road_UX();

            if (_isStage)
            {
                // 플레이어 위치 표시하는 마커 UX
                GameObject StageLightMarker = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__LOCATIONMARKER);
                StageLightMarker.transform.parent.DOScale(0.8f, RoadUX_Duration).SetEase(Ease.OutQuart);
                if (pathLightMarker == null)
                {
                    pathLightMarker = Instantiate(StageLightMarker);
                    pathLightMarker.transform.SetParent(StageLightMarker.transform.parent);
                    pathLightMarker.transform.localPosition = Vector3.zero;
                    pathLightMarker.transform.localScale = new Vector3(0.3f, 0.3f);
                    pathLightMarker.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
                    pathLightMarker.transform.DOMove(new Vector3(this.transform.position.x, this.transform.position.y), RoadUX_Duration+0.5f).SetEase(Ease.OutCubic);
                    pathLightMarker.GetComponent<Image>().DOFade(0f, RoadUX_Duration).SetEase(Ease.InExpo);
                }
                Sequence IconSequence;
                IconSequence = DOTween.Sequence();
                IconSequence.OnStart(() => { StageLightMarker.transform.localPosition = Vector3.zero; StageLightMarker.transform.localScale = Vector3.one; })
                    .Join(StageLightMarker.transform.DORotate(new Vector3(0, 0, 360), RoadUX_Duration, RotateMode.FastBeyond360).SetEase(Ease.InCubic))
                    .Join(StageLightMarker.transform.DOScale(0f,RoadUX_Duration-0.5f).SetEase(Ease.InBack));

                //pathLightMarker.transform.DOLocalMove(new Vector3(this.transform.localPosition.x, this.transform.localPosition.y), RoadUX_Duration);//.OnComplete(()=>Destroy(newLightMarker));


                // UX 딜레이
                yield return new WaitForSeconds(RoadUX_Duration+0.5f); // 길 UX 지속시간+0.5 만큼 기다린 후에 스테이지 변경

                // 다음에 누를 스테이지와 연결된 발자국 UX 적용 위해 ux변수 초기화
                footPrintCnt = 0;
                curFootPrintCnt = 0;
                curFootPrintCnt = 0;

                // 다른 스테이지를 선택하는거 막기 위해 화면에 검은 화면 해제
                if (blackScreen != null)
                {
                    blackScreen.SetActive(false);
                }

                // 현재 플레이어의 스테이지 위치 체크하는 아이콘(마커) 이동 
                StageLightMarker.transform.parent = this.transform.parent;
                StageLightMarker.transform.SetAsFirstSibling();
                StageLightMarker.transform.localPosition = Vector3.zero;
                StageLightMarker.transform.localScale = Vector3.one;

                switch (Stage_data.Stage_type)
                {
                    case HYJ_Map_Stage_TYPE.BASE_CAMP:
                        {
                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.SHOP:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.EVENT:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
                    case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
                    case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                }
            }
        }
    }
}