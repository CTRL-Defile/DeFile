using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//
using System;
using TMPro;
using System.Data.SqlTypes;
using AutoBattles;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine.Diagnostics;
using TMPro.Examples;

public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////
    object HYJ_Basic_GetPhase(params object[] _args)
    {
        return Basic_phase;
    }

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        //
        return null;
    }

    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }
    object LSY_Set_ShopUI(params object[] _args)
    {
        bool _isActive = (bool)_args[0];

        //for(int i=0; i<Shop_Pannel_cnt; i++)
        //    Shop_UnitList[i].gameObject.SetActive(_isActive);
        //Shop_Coin.SetActive(_isActive);
        //EXP_Bar.SetActive(_isActive);

        Shop_UI.SetActive(_isActive);

        return null;
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        //
        Basic_phase = 0;

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE,   HYJ_Basic_GetPhase);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  HYJ_ActiveOn);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, LSY_Set_ShopUI);
    }

    // Update is called once per frame
    void Update()
    {
        Shop_Coin.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_GOLD).ToString();

        switch (Basic_phase)
        {
            case -1:
                {
                    if (this.gameObject.activeSelf == true)
                    {
                        // battle active
                        Basic_phase = -1;
                    }
                }
                break;
            //
            case 0:
                {
                    //Debug.Log("is Basic_phase == 0?");
                    HYJ_Field_Init();

                    //
                    Basic_phase = 1;
                }
                break;
            case 1:
                {
                    //Debug.Log("this.gameObject : " + this.gameObject);    // Battle
                    this.gameObject.SetActive(false);

                    //
                    Basic_phase = 2;
                }
                break;

                // 전투 준비
            case 2:
                {
                    //Debug.Log("전투 준비..");
                    if(!UL_isInitialized)
                        LSY_UnitList_Init();

                }
                break;

                // 전투 상태
            case 3:
                {

                }
                break;
        }
    }
}

// 전장에 대한 정보(주로 타일)
#region FIELD

[Serializable]
public class HYJ_Battle_Manager_Line
{
    [SerializeField] List<HYJ_Battle_Tile> tiles;

    //////////  Getter & Setter //////////
    public HYJ_Battle_Tile HYJ_Data_Tile(int _count) { return tiles[_count]; }

    public int HYJ_Data_GetCount() { return tiles.Count; }

    // HYJ_Character형에서 -> GameObject 형으로 수정. Battle_Tiles의 주석 참조.
    public GameObject HYJ_Data_GetUnitOnTile(int _count) { return tiles[_count].HYJ_Basic_onUnit; }

    public int LSY_Count_GetUnitOnTile()
    {
        int cnt = 0;
        for (int idx=0; idx<this.HYJ_Data_GetCount(); idx++)
        {
            if (tiles[idx].HYJ_Basic_onUnit != null)
                cnt++;
        }
        return cnt;
    }

    //////////  Method          //////////
    public void HYJ_Tile_Add(Transform _trans)
    {
        tiles.Add(_trans.GetComponent<HYJ_Battle_Tile>());
    }

    //////////  Default Method  //////////
    public HYJ_Battle_Manager_Line()
    {
        tiles = new List<HYJ_Battle_Tile>();
    }
}

partial class HYJ_Battle_Manager
{
    [Header("==================================================")]
    [Header("FIELD")]

    [SerializeField] Transform Battle_Map;
    [SerializeField] Transform Field_parent, Stand_parent, Trash_parent;
    [SerializeField] Transform Unit_parent;
    [SerializeField] int Field_x;
    [SerializeField] int Field_y;
    [SerializeField] int Stand_x;

    [SerializeField] List<HYJ_Battle_Manager_Line> Field_tiles;
    [SerializeField] HYJ_Battle_Manager_Line Stand_tiles;

    //////////  Getter & Setter //////////
    object HYJ_Field_GetFieldX(params object[] _args)
    {
        return Field_x;
    }
    object HYJ_Field_GetFieldY(params object[] _args)
    {
        return Field_y;
    }

    object LSY_Field_GetStandX(params object[] _args)
    {
        // Bridge에 추가,,
        return Stand_x;
    }

    // Field_tiles
    object HYJ_Field_GetTile(params object[] _args)
    {
        int x = (int)_args[0];
        int y = (int)_args[1];

        return Field_tiles[y].HYJ_Data_Tile(x);
    }

    // 캐릭터가 위치한 타일을 찾아낸다.
    object HYJ_Field_GetTileFromCharacter(params object[] _args)
    {
        Vector2 pos = (Vector2)HYJ_Field_GetXYFromCharacter(_args);

        return Field_tiles[(int)pos.y].HYJ_Data_Tile((int)pos.x);
    }

    // 캐릭터를 찾아낸다. xy값으로
    object HYJ_Field_GetCharacter(params object[] _args)
    {
        //HYJ_Character res = null;
        GameObject res = null;

        //
        HYJ_Battle_Tile element = (HYJ_Battle_Tile)HYJ_Field_GetTile(_args);
        res = element.HYJ_Basic_onUnit;

        return res;
    }

    // y의 값을 보내준다.
    object HYJ_Field_GetTilesCount(params object[] _args)
    {
        return Field_tiles.Count;
    }

    // 해당 y의 x값을 보내준다.
    object HYJ_Field_GetTilesGetCount(params object[] _args)
    {
        int count = (int)_args[0];

        return Field_tiles[count].HYJ_Data_GetCount();
    }

    // 해당 캐릭터의 위치를 찾는다.
    object HYJ_Field_GetXYFromCharacter(params object[] _args)
    {
        HYJ_Character target = (HYJ_Character)_args[0];

        //
        Vector2 res = new Vector2(-1, -1);

        for(int forY = 0; forY < Field_tiles.Count; forY++)
        {
            for(int forX = 0; forX < Field_tiles[forY].HYJ_Data_GetCount(); forX++)
            {
                //HYJ_Character element = Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX);
                GameObject element = Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX);

                if ((element != null) && Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX).Equals(target))
                {
                    res.x = forX;
                    res.y = forY;

                    break;
                }
            }
        }

        return res;
    }

    //////////  Method          //////////


    //////////  Default Method  //////////
    void HYJ_Field_Init()
    {
        // Battle_Map, Field_parent, Stand_parent 변수명임
        GameObject element = Battle_Map.GetChild(0).gameObject;
        GameObject std_element = Battle_Map.GetChild(2).gameObject;
        GameObject trash_element = Battle_Map.GetChild(3).gameObject;

        Vector3 pos0 = element.transform.localPosition; // 0,0,0
        Vector3 pos1 = Battle_Map.GetChild(1).localPosition;    // 1,0,-2

        //
        Field_tiles = new List<HYJ_Battle_Manager_Line>();
        for (int forY = 0; forY < Field_y; forY++)  // Y 가 "행"
        {
            HYJ_Battle_Manager_Line line = new HYJ_Battle_Manager_Line();

            //
            int countX = Field_x;
            if((forY % 2) == 1)
            {
                countX += 1;
            }

            for (int forX = 0; forX < countX; forX++)  // X 가 "열"
            {
                GameObject obj = Instantiate(element, Field_parent);
                obj.SetActive(true);
                obj.name = forY + "_" + forX;
                obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forY);
                obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forX);

                if ((Field_y + 1) / 2 <= forY) obj.GetComponent<HYJ_Battle_Tile>().tile_Available = HYJ_Battle_Tile.Tile_Available.Available;

                if (countX == Field_x)
                {
                    obj.transform.localPosition = new Vector3(pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                else
                {
                    obj.transform.localPosition = new Vector3(-pos1.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                obj.transform.localScale = element.transform.localScale;
                

                //
                line.HYJ_Tile_Add(obj.transform);
            }

            //
            Field_tiles.Add(line);
        }

        // 대기열 9좌석 +@, 레벨업이나 유물로 늘어날 수 있음.
        Stand_tiles = new HYJ_Battle_Manager_Line();

        // left는 Stand_tiles의 좌측 시작점 위치
        int num = Field_tiles[Field_y - 1].HYJ_Data_GetCount();
        Vector3 left;
        if (num % 2 == 0)
        {
            // 마지막 행의 tile 수가 짝수일 때,
            left = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2 - 1).gameObject.transform.localPosition;
            Vector3 right = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2).gameObject.transform.localPosition;
            Vector3 mid = new Vector3((left.x + right.x) / 2.0f, (left.y + right.y) / 2.0f, (left.z + right.z) / 2.0f);

            left = new Vector3(mid.x - 4 * 2.0f, 0, mid.z);
        }
        else
        {
            // 마지막 행의 tile 수가 홀수일 때,
            left = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2).gameObject.transform.localPosition;
            left = new Vector3(left.x - 4 * 2.0f, 0, left.z);
        }

        // Stand_x 개 만큼 Stand_tile 생성
        for (int forX = 0; forX < Stand_x; forX++)
        {
            GameObject std_obj = Instantiate(std_element, Stand_parent);
            std_obj.SetActive(true);
            std_obj.name = "stand_" + forX;
            std_obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(0);    // Stand는 행이 한 개 뿐이다.
            std_obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forX);
            std_obj.GetComponent<HYJ_Battle_Tile>().tile_Available = HYJ_Battle_Tile.Tile_Available.Available;
            std_obj.transform.localPosition = new Vector3(left.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));

            //if (Field_y % 2 == 1)
            //    std_obj.transform.localPosition = new Vector3(pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));
            //else
            //    std_obj.transform.localPosition = new Vector3(-pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));

            std_obj.transform.localScale = element.transform.localScale;
            Stand_tiles.HYJ_Tile_Add(std_obj.transform);
        }

        GameObject trash_obj = Instantiate(trash_element, Trash_parent);
        trash_obj.SetActive(false);
        trash_obj.name = "trash_0";

        



        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X,                HYJ_Field_GetFieldX             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y,                HYJ_Field_GetFieldY             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE,                   HYJ_Field_GetTile               );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER,    HYJ_Field_GetTileFromCharacter  );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_CHARACTER,              HYJ_Field_GetCharacter          );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT,            HYJ_Field_GetTilesCount         );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT,        HYJ_Field_GetTilesGetCount      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER,      HYJ_Field_GetXYFromCharacter    );
    }
}

#endregion


public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [Header("==================================================")]
    [Header("SHOP")]

    [SerializeField]
    GameObject Shop_UI;
    [SerializeField]
    GameObject[] Shop_UnitList = new GameObject[5];
    [SerializeField]
    GameObject Shop_Coin, EXP_Bar;
    [SerializeField]
    List<int> Prob_list = new List<int>();
    Image EXP_Img;
    //[SerializeField]
    //List<LSY_Shop_UnitList> Shop_Unit = new List<LSY_Shop_UnitList>();

    List<Dictionary<string, object>> Unit_DB;

    [SerializeField]
    List<string> UnitName_list = new List<string>();
    [SerializeField]
    List<int> UnitID_list = new List<int>();
    [SerializeField]
    List<int> UnitIdx_list = new List<int>();
    int Shop_Pannel_cnt = 5;

    List<int> Max_EXP_List = new List<int>() { 0, 2, 6, 10, 20, 36, 56, 80, 108, 140, 170, 190, 210 };
    float Max_EXP;
    int Player_Lv = 1, cur_EXP = 0;

    bool UL_isInitialized = false;

    public void LSY_UnitList_Init()
    {
        // Level
        EXP_Img = EXP_Bar.GetComponent<Image>();
        cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);
        Max_EXP = Max_EXP_List[Player_Lv];
        EXP_Img.fillAmount = cur_EXP / Max_EXP;
        Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);

        // Text, Image, Cost
        Unit_DB = (List<Dictionary<string, object>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);

        for (int i = 0; i < Unit_DB.Count; i++)
        {
            UnitName_list.Add(Unit_DB[i]["NAME"].ToString());
            UnitID_list.Add((int)Unit_DB[i]["ID"]);
        }

        LSY_Shop_Reload(1);
        Debug.Log("Init end..");

        UL_isInitialized = true;
    }

    public void LSY_Shop_Reload(int n)
    {
        Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);

        if (n != 1) HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, 2);

        // Before Reload, Clear Idx list
        UnitIdx_list.Clear();
        LSY_Calc_Proba();

        for (int i = 0; i < Shop_UnitList.Length; i++)  // 0~5
        {
            int idx = UnitIdx_list[i];
            Shop_UnitList[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = UnitName_list[idx].ToString();
        }
    }

    public void LSY_Calc_Proba()
    {
        // List<int> Prob_list = new List<int>();   // SerializeField로 인스펙터에서 보이게끔 위에서 선언
        List<int> Cost_list = new List<int>();

        // 확률 적용 방법에 따라, 전체 파이가 100이 아닐 수 있음. 그때 수정하면 됨.
        int tot_num = 100;

        // 레벨에 따른 유닛 등장 확률
        switch (Player_Lv)
        {
            case 1:
                List<int> Prob_1 = new List<int>() { 100, 0, 0, 0, 0, 0 }; Prob_list = Prob_1; break;
            case 2:
                List<int> Prob_2 = new List<int>() { 90, 10, 0, 0, 0, 0 }; Prob_list = Prob_2; break;
            case 3:
                List<int> Prob_3 = new List<int>() { 75, 25, 0, 0, 0, 0 }; Prob_list = Prob_3; break;
            case 4:
                List<int> Prob_4 = new List<int>() { 60, 30, 10, 0, 0, 0 }; Prob_list = Prob_4; break;
            case 5:
                List<int> Prob_5 = new List<int>() { 45, 38, 15, 2, 0, 0 }; Prob_list = Prob_5; break;
            case 6:
                List<int> Prob_6 = new List<int>() { 35, 40, 20, 5, 0, 0 }; Prob_list = Prob_6; break;
            case 7:
                List<int> Prob_7 = new List<int>() { 34, 30, 25, 10, 1, 0 }; Prob_list = Prob_7; break;
            case 8:
                List<int> Prob_8 = new List<int>() { 18, 20, 40, 20, 2, 0 }; Prob_list = Prob_8; break;
            case 9:
                List<int> Prob_9 = new List<int>() { 16, 15, 30, 30, 8, 1 }; Prob_list = Prob_9; break;
            case 10:
                List<int> Prob_10 = new List<int>() { 11, 10, 20, 40, 15, 4 }; Prob_list = Prob_10; break;
            case 11:
                List<int> Prob_11 = new List<int>() { 0, 7, 15, 45, 25, 8 }; Prob_list = Prob_11; break;
            case 12:
                List<int> Prob_12 = new List<int>() { 0, 0, 10, 50, 30, 10 }; Prob_list = Prob_12; break;
        }


        // 등장 확률 누적 리스트
        for(int i = 0; i < Prob_list.Count - 1; i++)
        {
            if (Prob_list[i] != tot_num)
                Prob_list[i + 1] += Prob_list[i];
        }

        // 유닛 코스트 배열 초기화
        for(int i = 0; i < Shop_Pannel_cnt; i++)  // UnitIdx_list.Count => 아직 size가 0임
        {
            System.Random r = new System.Random();
            int n = r.Next(1, tot_num + 1); // min 이상, max 미만
            for(int j = 0; j < Prob_list.Count; j++)
            {
                if (n <= Prob_list[j])
                {
                    Cost_list.Add(j + 1);
                    break;
                }
            }
        }
        //Debug.Log(Cost_list.Count + "<-costlist cnt");

        // 코스트 별 유닛 랜덤 설정
        for(int i = 0; i < Shop_Pannel_cnt; i++)
        {
            List<int> Unit_Candi = new List<int>();
            for(int j = 0; j < Unit_DB.Count; j++)  // header 3줄 제외.. 할 필요가 없네 어차피 3줄떼고 읽어왔구나.
            {
                //Debug.Log((int)Unit_DB[j]["COST"] + "<-db.cost || costlist->" + Cost_list[i]);
                if ((int)Unit_DB[j]["COST"] == Cost_list[i])
                {
                    Unit_Candi.Add((int)Unit_DB[j]["ID"]);
                }
            }

            if (Unit_Candi.Count > 0)
            {
                System.Random r = new System.Random();
                int n = r.Next(Unit_Candi.Count);
                //Debug.Log(Unit_Candi.Count + "<-unitcandi.count || n->" + n);
                UnitIdx_list.Add(Unit_Candi[n]);
            }
            else
            {
                Debug.Log("None of Unit COST " + Cost_list[i] + " is Available...");
                UnitIdx_list.Add(0);
            }
        }
        //Debug.Log("db.count : " + Unit_DB.Count);

    }


    public void LSY_Buy_Unit()
    {
        // Detect clicked btn -> getName -> Calc pos -> Instant
        var Btn_idx = EventSystem.current.currentSelectedGameObject;
        string Btn_name = Btn_idx.name.ToString();
        Btn_name = Btn_name.Substring(Btn_name.Length - 1);
        Debug.Log("Btn " + Btn_name + " is clicked");

        int unit_idx = UnitIdx_list[int.Parse(Btn_name)];


        int cnt = Stand_tiles.LSY_Count_GetUnitOnTile(), pos_num = -1;
        if (cnt < Stand_x)
        {
            for (int idx = 0; idx < cnt; idx++)
            {
                if (Stand_tiles.HYJ_Data_GetUnitOnTile(idx) == null)
                {
                    pos_num = idx;
                    break;
                }
            }
            if (pos_num == -1)
                pos_num = cnt;

            Debug.Log(pos_num + " " + cnt);

            Vector3 pos = Stand_tiles.HYJ_Data_Tile(pos_num).transform.position;

            GameObject unitData
                = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                    unit_idx);
            if (unitData)
            {
                Instantiate(unitData, pos, Quaternion.identity, Unit_parent);
                // 돈 빠지는거 고쳐야함
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, 1);
                Debug.Log("Unit " + unit_idx + " is spawned");
            }
            else
                Debug.Log("Unit " + unit_idx + " is NULL");
        }


    }

    public void LSY_Buy_EXP()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, 4);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_INCREASE, 4);
        cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);

        if (cur_EXP >= Max_EXP)
        {
            Debug.Log("cur : " + cur_EXP + ", Max_Exp : " + Max_EXP + ", LV : " + Player_Lv);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__LEVEL_INCREASE, 1);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_DECREASE, Max_EXP_List[Player_Lv]);
            Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);
            cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);
            Max_EXP = Max_EXP_List[Player_Lv];
        }

        EXP_Img.fillAmount = cur_EXP / Max_EXP;

    }




}

