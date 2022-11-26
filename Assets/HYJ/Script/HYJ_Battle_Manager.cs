using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
using System;
using TMPro;
using System.Data.SqlTypes;
using AutoBattles;
using JetBrains.Annotations;

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

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        //
        Basic_phase = 0;

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE,   HYJ_Basic_GetPhase);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  HYJ_ActiveOn);
    }

    // Update is called once per frame
    void Update()
    {
        switch(Basic_phase)
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
                    Debug.Log("is Basic_phase == 0?");
                    HYJ_Field_Init();

                    //
                    Basic_phase = 1;
                }
                break;
            case 1:
                {
                    Debug.Log("this.gameObject : " + this.gameObject);    // Battle
                    this.gameObject.SetActive(false);

                    //
                    Basic_phase = -1;
                }
                break;

                // 전투 준비
            case 2:
                {
                    //Debug.Log("전투 준비..");
                    if(!isUpdated)
                        LSY_update_UnitList();

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
    [SerializeField] Transform Field_parent, Stand_parent;
    [SerializeField] Transform Unit_parent;
    [SerializeField] int Field_x;
    [SerializeField] int Field_y;
    [SerializeField] int Stand_x;

    [SerializeField] List<HYJ_Battle_Manager_Line> Field_tiles;
    [SerializeField] HYJ_Battle_Manager_Line Stand_tiles;

    //////////  Getter & Setter //////////
    object Get_Field_tiles(params object[] _args)
    {
        return Field_tiles;
    }
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

        Vector3 pos0 = element.transform.localPosition; // 0,0,0
        Vector3 pos1 = Battle_Map.GetChild(1).localPosition;    // 1,0,-2

        //
        Field_tiles = new List<HYJ_Battle_Manager_Line>();
        for (int forY = 0; forY < Field_y; forY++)
        {
            HYJ_Battle_Manager_Line line = new HYJ_Battle_Manager_Line();

            //
            int countX = Field_x;
            if((forY % 2) == 1)
            {
                countX += 1;
            }

            //
            for (int forX = 0; forX < countX; forX++)
            {
                GameObject obj = Instantiate(element, Field_parent);
                obj.SetActive(true);
                obj.name = forX + "_" + forY;
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

            std_obj.transform.localPosition = new Vector3(left.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));

            //if (Field_y % 2 == 1)
            //    std_obj.transform.localPosition = new Vector3(pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));
            //else
            //    std_obj.transform.localPosition = new Vector3(-pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));

            std_obj.transform.localScale = element.transform.localScale;
            Stand_tiles.HYJ_Tile_Add(std_obj.transform);
        }

		//Stand_tiles.Add(std_line);


		//
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES,                    Get_Field_tiles         );
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
    private GameObject[] Shop_UnitList = new GameObject[5];
    [SerializeField]
    List<LSY_Shop_UnitList> Shop_unit = new List<LSY_Shop_UnitList>();

    bool isUpdated = false;

    public void LSY_update_UnitList()
    {
        // Text, Image, Cost
        LSY_Shop_UnitList sh;
        //Shop_unit.Add(sh);



        var mon_list = new List<string>() { "Orc", "Bear" };

        for (int i = 0; i < Shop_UnitList.Length; i++)
        {
            System.Random r = new System.Random();
            int idx = r.Next(2);

            Shop_UnitList[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = mon_list[idx].ToString();
        }

        isUpdated = true;
    }

    public void LSY_buy_Unit()
    {
        Debug.Log("Buy Unit..");

        int cnt = Stand_tiles.LSY_Count_GetUnitOnTile(), pos_num = -1;
        if(cnt < Stand_x)
        {
            for(int idx=0;idx<cnt; idx++)
            {
                if (Stand_tiles.HYJ_Data_GetUnitOnTile(idx) == null)
                {
                    pos_num = idx;
                    break;
                }
            }
            if (pos_num == -1)
                pos_num = cnt;
            Debug.Log("pos_num : " + pos_num);

            Vector3 pos = Stand_tiles.HYJ_Data_Tile(pos_num).transform.position;

            GameObject unitData
                = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                    //
                    0);
            Instantiate(unitData,
                //this.gameObject.transform.GetChild(2).gameObject.transform.GetChild(0),
                //this.gameObject.transform.GetChild(1).gameObject.transform.Find("stand_0").localPosition,
                pos, Quaternion.identity, Unit_parent);
        }


    }
}

public class LSY_Shop_UnitList
{
    // Monster Name, Image, Cost
    List<string> mon_list = new List<string>() { "Orc", "Bear" };
    public string mon_name;
    public int mon_cost;

    public LSY_Shop_UnitList()
    {
        mon_name = "asdf";
        mon_cost = 1;
    }




    //public void Shop_Refresh()
    //{
    //    for (int i = 0; i < 5; i++)
    //    {
    //        System.Random r = new System.Random();
    //        int idx = r.Next(2);

    //        Shop_UnitList[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = mon_list[idx].ToString();
    //    }

    //}


}