using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Map_Manager : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Active_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        //
        return null;
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, HYJ_Active_ActiveOn);

        HYJ_Cheapter_Start();
        HYJ_Road_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

partial class HYJ_Map_Manager
{
    [Header("CHEAPTER")]
    const int Cheapter_x = 8;
    const int Cheapter_y = 14;

    [SerializeField] int Cheapter_settingRootCount;
    [SerializeField] Transform Cheapter_viewPort;
    [SerializeField] GameObject Cheapter_stage;
    [SerializeField] int Cheapter_level;
    [SerializeField] List<HYJ_Map_Stage> Cheapter_stages;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_Cheapter_Create(params object[] _args)
    {
        Cheapter_level = (int)_args[0];

        Cheapter_viewPort.localPosition = new Vector3(0, 0, 0);

        Cheapter_stages = new List<HYJ_Map_Stage>();
        for (int i = 0; i < Cheapter_x * Cheapter_y; i++)
        {
            Cheapter_stages.Add(null);
        }

        int x = Random.Range(4, 6);
        int y = 0;
        HYJ_Map_Stage element = null;
        // 챕터 생성
        {
            // 베이스캠프 생성
            element = HYJ_Cheapter_Create_SettingStage(x, y);
            element.HYJ_Stage_type = HYJ_Map_Stage_TYPE.BASE_CAMP;

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
                    break;
                }
            }
        }
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
            HYJ_Road_Setting(element.transform.parent.localPosition, _stage.transform.parent.localPosition);
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
        Cheapter_viewPort.localPosition = new Vector3(-(float)_args[0], -(float)_args[1], -(float)_args[2]);

        return null;
    }

    //////////  Default Method  //////////
    void HYJ_Cheapter_Start()
    {
        if (HYJ_ScriptBridge.HYJ_Static_instance == null) Debug.Log("AA");
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__SELECT_RESET,   HYJ_Cheapter_SelectReset    );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__MOVE_CENTER,    HYJ_Cheapter_MoveCenter     );

        HYJ_Cheapter_Create(1);
    }
}

#region ROAD

partial class HYJ_Map_Manager
{
    [SerializeField] Transform Road_parent;
    [SerializeField] List<GameObject> Road_roads;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    void HYJ_Road_Setting(Vector3 _start, Vector3 _destination)
    {
        int num = -1;

        for(int i = 0; i < Road_roads.Count; i++)
        {
            if(!Road_roads[i].activeSelf)
            {
                num = i;
                break;
            }
        }

        if (num == -1)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject element = Instantiate(Road_roads[0]);
                element.SetActive(false);
                element.transform.parent = Road_parent;
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
            element.transform.localPosition = _destination;

            //
            vec2.x = _destination.x - _start.x;
            vec2.y = _destination.y - _start.y;
            float rot = Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg;
            element.transform.eulerAngles = new Vector3(0, 0, rot + 90.0f);

            //
            float length = Vector3.Distance(_start, _destination);
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