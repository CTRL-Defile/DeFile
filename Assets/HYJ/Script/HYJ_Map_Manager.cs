using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class HYJ_Map_Manager : MonoBehaviour
{
    enum UPDATE_PHASE
    {
        CHEAPTER,
        ROAD,
        CAMERA,
        BRIDGE,

        //
        UPDATE
    }
    [SerializeField] UPDATE_PHASE Basic_initialize;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Active_ActiveOn(params object[] _args)
    {
        bool isActive = (bool)_args[0];

        //
        this.gameObject.SetActive(isActive);

        //
        return null;
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
        switch(Basic_initialize)
        {
            case UPDATE_PHASE.UPDATE:   break;
            //
            case UPDATE_PHASE.CHEAPTER:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__BLACKSCREEN,     GetMapBlackScreen   ); 
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__LOCATIONMARKER,  GetMapLocationMarker);

                    if (HYJ_Cheapter_Start())
                    {
                        Basic_initialize = UPDATE_PHASE.ROAD;
                    }
                }
                break;
            case UPDATE_PHASE.ROAD:
                {
                    HYJ_Road_Start();

                    //
                    Basic_initialize = UPDATE_PHASE.CAMERA;
                }
                break;
            case UPDATE_PHASE.CAMERA:
                {
                    Camera camera
                        = (Camera)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.MASTER___UI__GET_CAMERA,
                            0);

                    if (camera != null)
                    {
                        this.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = camera;

                        Basic_initialize = UPDATE_PHASE.BRIDGE;
                    }
                }
                break;
            case UPDATE_PHASE.BRIDGE:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, HYJ_Active_ActiveOn);

                    this.gameObject.SetActive(false);

                    //
                    Basic_initialize = UPDATE_PHASE.UPDATE;
                }
                break;
        }
    }
}

partial class HYJ_Map_Manager
{
    [Header("==================================================")]
    [Header("CHEAPTER")]
    [SerializeField] int Cheapter_settingRootCount;
    [SerializeField] Transform Cheapter_viewPort;
    [SerializeField] GameObject Cheapter_stage;
    [SerializeField] int Cheapter_level;
    [SerializeField] List<HYJ_Map_Stage> Cheapter_stages;
    [SerializeField] GameObject Map_BlackScreen; // 스테이지 선택 시 다른 스테이지 선택하는거 막기 위해 검은화면 오브젝트 이용
    [SerializeField] GameObject Map_LocationMarker; // 플레이어의 현재 스테이지 위치 표시하는 오브젝트

    public const int Cheapter_x = 8;
    const int Cheapter_y = 14;

    // 현재 플레이어 위치한 스테이지 위치
    public int cur_x;
    public int cur_y;

    //////////  Getter & Setter //////////
    object HYJ_Cheapter_GetLevel(params object[] _args)
    {
        return Cheapter_level;
    }

    object HYJ_Cheapter_GetStages(params object[] _args)
    {
        return Cheapter_stages;
    }

    //////////  Method          //////////
    // 맵 로딩
    bool HYJ_Cheapter_Load()
    {
        bool res = true;

        HYJ_Map_Stage element = null;
        int x = 0;
        int y = 0;

        List<HYJ_Map_Stage.SaveData> datas = (List<HYJ_Map_Stage.SaveData>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_MAP_DATAS);
        if (datas.Count != 0)
        {
            res = false;

            //
            Cheapter_level = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_LEVEL);

            // 스테이지 셋팅
            for (int i = 0; i < datas.Count; i++)
            {
                if ((datas[i] != null) && (datas[i].Stage_type != HYJ_Map_Stage_TYPE.NONE))
                {
                    x = i % Cheapter_x;
                    y = i / Cheapter_x;

                    element = HYJ_Cheapter_Create_SettingStage(x, y);
                    element.HYJ_Stage_saveData = datas[i];
                    element.HYJ_Stage_type = element.HYJ_Stage_saveData.Stage_type;
                    Cheapter_stages[i] = element;
                }
            }

            // 루트 설정
            {
                // 베이스캠프
                y = 0;
                for (x = 0; x < Cheapter_x; x++)
                {
                    element = Cheapter_stages[x + (y * Cheapter_x)];
                    if (element != null)
                    {
                        break;
                    }
                }
            
                y = 1;
                for (x = 0; x < Cheapter_x; x++)
                {
                    HYJ_Cheapter_Create_SetRoot(
                        element,
                        x, y);
                }
            
                //
                for (y = 1; y < Cheapter_y - 2; y++)
                {
                    for (x = 0; x < Cheapter_x; x++)
                    {
                        element = Cheapter_stages[x + (y * Cheapter_x)];
            
                        if (element)
                        {
                            int checkY = y + 1;
            
                            int checkX = x - 1;
                            if (checkX >= 0)
                            {
                                HYJ_Cheapter_Create_SetRoot(
                                    element,
                                    checkX, checkY);
                            }
            
                            checkX = x;
                            HYJ_Cheapter_Create_SetRoot(
                                element,
                                checkX, checkY);
            
                            checkX = x + 1;
                            if (checkX < Cheapter_x)
                            {
                                HYJ_Cheapter_Create_SetRoot(
                                    element,
                                    checkX, checkY);
                            }
                        }
                    }
                }
            }
            
            // 진행사항 갱신하기(표시갱신)
            // 베이스캠프 찾기
            for(int i = 0; i < Cheapter_stages.Count; i++)
            {
                if(
                    (Cheapter_stages[i] != null) &&
                    (Cheapter_stages[i].HYJ_Stage_saveData.Stage_type == HYJ_Map_Stage_TYPE.BASE_CAMP))
                {
                    cur_x = i % Cheapter_x;
                    cur_y = i / Cheapter_x;
                    element = Cheapter_stages[i];
                    break;
                }
            }
            
            HYJ_Cheapter_Load_SettingRoots(element);
            
            // 현재까지 진행한 위치
            int playerPos = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_PLAYER_POS);
            element = Cheapter_stages[playerPos];
            
            // >> 혀누J 추가 - viewPort 처음위치
            Cheapter_viewPort.localPosition = new Vector3(200f, -Cheapter_viewPort.parent.GetComponent<RectTransform>().rect.height + 100f, 0);
            // 유저 위치 나타내는 별 아이콘 처음위치
            Map_LocationMarker.transform.parent = element.transform.parent;
            Map_LocationMarker.transform.SetAsFirstSibling();
            Map_LocationMarker.transform.localPosition = Vector3.zero;
        }

        return res;
    }

    void HYJ_Cheapter_Load_SettingRoots( HYJ_Map_Stage _element )
    {
        HYJ_Map_Stage next = null;

        _element.HYJ_Stage_Select();

        for (int i = 0; i < _element.HYJ_Stage_roots.Count; i++)
        {
            if(_element.HYJ_Stage_roots[i].HYJ_Stage_saveData.isSelected)
            {
                next = _element.HYJ_Stage_roots[i];
                break;
            }
        }

        if( next != null )
        {
            HYJ_Cheapter_Load_SettingRoots( next );
        }
    }

    // 맵 생성
    public void HYJ_Cheapter_Create(params object[] _args)
    {
        Cheapter_level = (int)_args[0];

        int x = Random.Range(4, 6);
        int y = 0;
        HYJ_Map_Stage element = null;
        // 챕터 생성
        {
            // 베이스캠프 생성
            element = HYJ_Cheapter_Create_SettingStage(x, y);
            element.HYJ_Stage_type = HYJ_Map_Stage_TYPE.BASE_CAMP;

            // 플레이어 위치
            cur_x = x;
            cur_y = y;

            // 무작위 생성
            for (int i = 0; i < Cheapter_settingRootCount; i++)
            {
                for (y = 1; y < Cheapter_y - 1; y++)
                {
                    x += Random.Range(0, 3) - 1;
                    if (x.Equals(-1)) { x = 0; }
                    else if (x.Equals(Cheapter_x)) { x = Cheapter_x - 1; }

                    element = HYJ_Cheapter_Create_SettingStage(x, y);
                    //if (y == 4)
                    //{
                    //    element.HYJ_Stage_type = HYJ_Map_Stage_TYPE.SHOP;
                    //}
                    //else
                    {
                        element.HYJ_Stage_SettingType(Cheapter_level);
                    }
                }


            }

            // 상점으로 전환
            int shop = 2;
            y = 4;
            while(shop > 0)
            {
                x = Random.Range(0, Cheapter_x);
                element = Cheapter_stages[x + (y * Cheapter_x)];

                if(element != null)
                {
                    if (element.HYJ_Stage_type != HYJ_Map_Stage_TYPE.SHOP)
                    {
                        shop--;
                        element.HYJ_Stage_type = HYJ_Map_Stage_TYPE.SHOP;
                    }
                }
            }

            // 보스전 생성
            x = Random.Range(4, 6);
            y = Cheapter_y - 1;
            element = HYJ_Cheapter_Create_SettingStage(x, y);
            element.HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_BOSS;
        }

        // 루트 설정
        {
            // 베이스캠프
            y = 0;
            for (x = 0; x < Cheapter_x; x++)
            {
                element = Cheapter_stages[x + (y * Cheapter_x)];
                if (element != null)
                {
                    break;
                }
            }

            y = 1;
            for (x = 0; x < Cheapter_x; x++)
            {
                HYJ_Cheapter_Create_SetRoot(
                    element,
                    x, y);
            }



            //
            for (y = 1; y < Cheapter_y - 2; y++)
            {
                for (x = 0; x < Cheapter_x; x++)
                {
                    element = Cheapter_stages[x + (y * Cheapter_x)];

                    if (element)
                    {
                        int checkY = y + 1;

                        int checkX = x - 1;
                        if (checkX >= 0)
                        {
                            HYJ_Cheapter_Create_SetRoot(
                                element,
                                checkX, checkY);
                        }

                        checkX = x;
                        HYJ_Cheapter_Create_SetRoot(
                            element,
                            checkX, checkY);

                        checkX = x + 1;
                        if (checkX < Cheapter_x)
                        {
                            HYJ_Cheapter_Create_SetRoot(
                                element,
                                checkX, checkY);
                        }
                    }
                }
            }

            // 보스
            int bossX = 0;
            int bossY = Cheapter_y - 1;
            for (bossX = 0; bossX < Cheapter_x; bossX++)
            {
                element = Cheapter_stages[bossX + (bossY * Cheapter_x)];
                if(element != null)
                {
                    break;
                }
            }

            y = Cheapter_y - 2;
            for (x = 0; x < Cheapter_x; x++)
            {
                element = Cheapter_stages[x + (y * Cheapter_x)];
                if (element != null)
                {
                    HYJ_Cheapter_Create_SetRoot(
                        element,
                        bossX, bossY);
                }
            }
        }

        // 베이스캠프 활성화
        for (int i = 0; i < Cheapter_stages.Count; i++)
        {
            element = Cheapter_stages[i];

            if (element != null)
            {
                if(element.HYJ_Stage_type.Equals( HYJ_Map_Stage_TYPE.BASE_CAMP ))
                {
                    element.HYJ_Stage_Select();

                    // >> 혀누J 추가 - viewPort 처음위치
                    Cheapter_viewPort.localPosition = new Vector3(200f, -Cheapter_viewPort.parent.GetComponent<RectTransform>().rect.height + 100f, 0);
                    // 유저 위치 나타내는 별 아이콘 처음위치
                    Map_LocationMarker.transform.parent = element.transform.parent;
                    Map_LocationMarker.transform.SetAsFirstSibling();
                    Map_LocationMarker.transform.localPosition = Vector3.zero;// new Vector3(Cheapter_viewPort.parent.GetComponent<RectTransform>().rect.width, Cheapter_viewPort.parent.GetComponent<RectTransform>().rect.height);
                    
                    break;
                }
            }
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__MAP_SETTING,
            //
            false);
    }

    HYJ_Map_Stage HYJ_Cheapter_Create_SettingStage(int _x, int _y)
    {
        //
        HYJ_Map_Stage res = Cheapter_stages[_x + (_y * Cheapter_x)];

        //
        if (res == null)
        {
            res = GameObject.Instantiate(Cheapter_stage).transform.Find("Button").GetComponent<HYJ_Map_Stage>();
            res.HYJ_Init(
                Cheapter_viewPort,
                _x, _y);
            Cheapter_stages[_x + (_y * Cheapter_x)] = res;
        }

        // 스테이지 아이콘 좌표
        res.stage_x = _x;
        res.stage_y = _y;

        //
        return res;
    }

    void HYJ_Cheapter_Create_SetRoot(
        HYJ_Map_Stage _stage,
        int _x, int _y)
    {
        HYJ_Map_Stage element = Cheapter_stages[_x + (_y * Cheapter_x)];

        if (element != null)
        {
            //element.HYJ_Stage_AddRoot(_stage);
            _stage.HYJ_Stage_AddRoot(element);

            //
            // HYJ_Road_Setting(element.transform.parent.localPosition, _stage.transform.parent.localPosition);
            HYJ_Road_Setting(element, _stage);
        }
    }

    object HYJ_Cheapter_SelectReset(params object[] _args)
    {
        for (int i = 0; i < Cheapter_stages.Count; i++)
        {
            HYJ_Map_Stage element = Cheapter_stages[i];

            if(element != null)
            {
                element.HYJ_UI_Active(false);
            }
        }

        return null;
    }

    object HYJ_Cheapter_MoveCenter(params object[] _args)
    {
        // 영재님께서 예전에 작성한 함수 > // Cheapter_viewPort.localPosition = new Vector3(-(float)_args[0], -(float)_args[1], -(float)_args[2]);

        // 스크롤 뷰 화면 x중앙, 스크롤바 화면 y로 이동, 
        this.transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Scrollbar>().value = ((float)_args[1]-100f) / Cheapter_viewPort.parent.GetComponent<RectTransform>().rect.height;

        return null;
    }

    object ChangePlayerPos(params object[] _args) // 플레이어가 위치한 x,y좌표 변경
    {
        // _args[0] 은 클릭한 버튼 x좌표, _args[1]은 클릭한 버튼 y좌표
        cur_x = (int)_args[0];
        cur_y = (int)_args[1];
        return null;
    }
    object GetSelectedRoad(params object[] _args) // 플레이어가 선택한 버튼을 경유하는 도로 찾아서 리턴
    {
        // _args[0] 은 클릭한 버튼 x좌표, _args[1]은 클릭한 버튼 y좌표
        return (Road_parent.Find("Road_" + cur_x + "," + cur_y + "to" + (int)_args[0] + "," + (int)_args[1] )).gameObject;
    }

    object GetMapBlackScreen(params object[] _args)
    {
        // 지도 검은화면
        return Map_BlackScreen;
    }

    object GetMapLocationMarker(params object[] _args)
    {
        // 지도 유저위치 표시하는 마커
        return Map_LocationMarker;
    }

    //////////  Default Method  //////////
    bool HYJ_Cheapter_Start()
    {
        bool res
            = (HYJ_Player.UPDATE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_UPDATE_PHASE)
                == HYJ_Player.UPDATE_PHASE.UPDATE;

        if (res)
        {
            Cheapter_stages = new List<HYJ_Map_Stage>();
            for (int i = 0; i < Cheapter_x * Cheapter_y; i++)
            {
                Cheapter_stages.Add(null);
            }
            Cheapter_viewPort.localPosition = new Vector3(0, 0, 0);

            if (HYJ_ScriptBridge.HYJ_Static_instance == null) Debug.Log("AA");

            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__GET_LEVEL,       HYJ_Cheapter_GetLevel   );
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__GET_STAGES,      HYJ_Cheapter_GetStages  );

            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__SELECT_RESET,    HYJ_Cheapter_SelectReset);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__MOVE_CENTER,     HYJ_Cheapter_MoveCenter );
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHANGE__PLAYER_POSITION,   ChangePlayerPos         );
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__SELECTED_ROAD,        GetSelectedRoad         );

            if (HYJ_Cheapter_Load())
            {
                HYJ_Cheapter_Create(1);
            }
        }

        return res;
    }

}

#region ROAD

partial class HYJ_Map_Manager
{
    [Header("==================================================")]
    [Header("ROAD")]
    [SerializeField] Transform Road_parent;
    [SerializeField] List<GameObject> Random_roads;
    [SerializeField] List<GameObject> Road_roads;

    private int road_cnt = 1; // Road_roads 에 담긴 도로 수

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    void HYJ_Road_Setting(HYJ_Map_Stage _start, HYJ_Map_Stage _destination)
    {
        int num = -1;

        for (int i = 0; i < Road_roads.Count; i++)
        {
            if (!Road_roads[i].activeSelf)
            {
                num = i;
                break;
            }
        }

        if (num == -1)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject element = Instantiate(Random_roads[Random.Range(0, Random_roads.Count)]);
                //element = Random_roads[Random.Range(0, Random_roads.Count)];
                element.SetActive(false);
                //element.transform.parent = Road_parent;
                element.transform.SetParent(Road_parent);
                element.transform.localScale = new Vector3(1, 1, 1);
                Road_roads.Add(element);
            }

            HYJ_Road_Setting(_start, _destination);
        }
        else
        {
            Vector2 vec2 = new Vector2();

            //
            GameObject element = Road_roads[num];
            // start to dest 해보니께 스테이지 시작점과 도착점 매개변수가 잘못된 것 같네요.. dest to start 이렇게 해야 오류안나네
            element.name = "Road_" + _destination.stage_x + "," + _destination.stage_y + "to" + _start.stage_x + "," + _start.stage_y;
            element.transform.localPosition = _destination.transform.parent.localPosition;

            //
            vec2.x = _destination.transform.parent.localPosition.x - _start.transform.parent.localPosition.x;
            vec2.y = _destination.transform.parent.localPosition.y - _start.transform.parent.localPosition.y;
            float rot = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
            element.transform.eulerAngles = new Vector3(0, 0, rot + 90.0f);

            //
            float length = Vector3.Distance(_start.transform.parent.localPosition, _destination.transform.parent.localPosition);

            // >> 현우 - 정점 사이 거리에 따라 발자국 길이 수정 (발자국 길이 = 발자국 오브젝트 묶음)
            int footCntPerLength = (int)(length-10) / 50; // 이때 50는 발자국 오브젝트 1개 길이
            for(int i = 0; i < footCntPerLength-1; i++)
            {
                element.transform.GetChild(i).gameObject.SetActive(true);
            }
            for(int i = footCntPerLength-1; i < element.transform.childCount; i++)
            {
                element.transform.GetChild(i).gameObject.SetActive(false);
            }

            RectTransform rt = element.GetComponent<RectTransform>();
            vec2.x = 5.0f;
            vec2.y = length;
            rt.sizeDelta = vec2;

            //
            element.SetActive(true);
        }
    }

    //////////  Default Method  //////////
    void HYJ_Road_Start()
    {
    }
}

#endregion