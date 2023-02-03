using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using JetBrains.Annotations;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;

public partial class HYJ_Player : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_phase)
        {
            case -1: break;
            //
            case 0: { if (HYJ_Basic_Init()) { Basic_phase = 1; } } break;

            case 1:
                {
                    int DB_phase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_PHASE);
                    if (DB_phase == -1)
                    {
                        if (HYJ_Unit_Init())
                        {
                            Basic_phase = 2;
                        }

                    }
                } 
                break;

            case 2: { if (HYJ_Item_Init()) { Basic_phase = 3; } } break;
            case 3: { if (HYJ_Buff_Init()) { Basic_phase = -1; } } break;
        }
    }
}

// 기본 정보를 분류
#region Basic

partial class HYJ_Player
{
    [SerializeField] int Basic_level;   // 레벨
    [SerializeField] int Basic_exp;     // 경험치

    [SerializeField] int Basic_hp;      // 현재 남은 체력
    [SerializeField] int Basic_hpMax;   // 최대 체력

    [SerializeField] int Basic_gold;    // 보유하고 있는 금화

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    object LSY_Basic_IncExp(params object[] _args)
    {
        Basic_exp += (int)_args[0];
        return null;
    }
    object LSY_Basic_DecExp(params object[] _args)
    {
        Basic_exp -= (int)_args[0];
        return null;
    }
    object LSY_Basic_getExp(params object[] _args)
    {
        return Basic_exp;
    }
    object LSY_Basic_IncLevel(params object[] _args)
    {
        Basic_level += (int)_args[0];
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___LEVEL__VIEW_LEVEL, Basic_level);
        return null;
    }
    object LSY_Basic_getLevel(params object[] _args)
    {
        return Basic_level;
    }


    // Basic_gold   //

    object LSY_Basic_getGold(params object[] _args)
    {
        return Basic_gold;
    }

    // 금화가 충분히 있는지 체크
    object HYJ_Basic_GoldIsEnough(params object[] _args)
    {
        bool res = false;

        //
        int pay = (int)_args[0];
        if(Basic_gold >= pay)
        {
            res = true;
        }

        return res;
    }

    // 금화를 추가한다.
    object HYJ_Basic_GoldPlus(params object[] _args)
    {
        //
        int value = (int)_args[0];

        Basic_gold += value;

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___GOLD__VIEW_GOLD, Basic_gold);

        //
        return null;
    }

    // 금화를 지불한다.
    object HYJ_Basic_GoldMinus(params object[] _args)
    {
        bool res = false;

        //
        int value = (int)_args[0];

        if (Basic_gold >= value)
        {
            Basic_gold -= value;

            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___GOLD__VIEW_GOLD, Basic_gold);

            res = true;
        }

        //
        return res;
    }

    // 플레이어 체력 회복
    object JHW_Basic_hp_Increase(params object[] _args)
    {
        int value = (int)_args[0];
        Basic_hp = (Basic_hp + value) % Basic_hpMax;
        return true;
    }

    //////////  Default Method  //////////
    bool HYJ_Basic_Init()
    {
        Basic_gold = 10000;
        Basic_level = 1;
        Basic_exp = 0;

        //
        Basic_hpMax = 99;
        Basic_hp = Basic_hpMax;

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_GOLD, LSY_Basic_getGold);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_IS_ENOUGH, HYJ_Basic_GoldIsEnough);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_PLUS,       HYJ_Basic_GoldPlus      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS,      HYJ_Basic_GoldMinus     );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_INCREASE, LSY_Basic_IncExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_DECREASE, LSY_Basic_DecExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP, LSY_Basic_getExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__LEVEL_INCREASE, LSY_Basic_IncLevel);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL, LSY_Basic_getLevel);

        return true;
    }
}

#endregion

// 유닛(기물)에 대한 정보
#region Unit

[Serializable]
public class HYJ_Player_Unit_Datas
{
    [SerializeField] List<CTRL_Character_Data> unitDatas;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public CTRL_Character_Data HYJ_Data_GetUnitData(int _count) { return unitDatas[_count]; }
    public void HYJ_Data_SetUnitData(CTRL_Character_Data _data, int _count) { unitDatas[_count] = _data; }
    public void HYJ_Data_SetUnitData(CTRL_Character_Data _data) { unitDatas.Add(_data); }

    public int HYJ_Data_GetUnitDataCount() { return unitDatas.Count; }

    //////////  Default Method  //////////
    public HYJ_Player_Unit_Datas(int _count)
    {
        unitDatas = new List<CTRL_Character_Data>();

        for (int i = 0; i < _count; i++)
        {
            unitDatas.Add(null);
        }
    }
}

partial class HYJ_Player
{
    [SerializeField] HYJ_Player_Unit_Datas Unit_buyUnits;
    [SerializeField] HYJ_Player_Unit_Datas Unit_waitUnits;
    [SerializeField] List<HYJ_Player_Unit_Datas> Unit_fieldUnits;
    [SerializeField] List<int> synergy_list;
    [SerializeField] SerialList<int>[] id_list = new SerialList<int>[3];
    //[SerializeField] int[] id_array, synergy_array;
    Dictionary<int, int> synergy_dic;
    List<Dictionary<string, object>> Player_Unit_csv;

    //////////  Getter & Setter //////////
    //
    object HYJ_Unit_GetBuyUnitData(params object[] _args)
    {
        int _count = (int)_args[0];

        return Unit_buyUnits.HYJ_Data_GetUnitData(_count);
    }

    object HYJ_Unit_GetBuyUnitCount(params object[] _args)
    {
        return Unit_buyUnits.HYJ_Data_GetUnitDataCount();
    }

    //
    object HYJ_Unit_GetWaitUnitData(params object[] _args)
    {
        int _count = (int)_args[0];

        return Unit_waitUnits.HYJ_Data_GetUnitData(_count);
    }

    //////////  Method          //////////
    // 유닛을 추가한다.
    // -1이면 빈 칸에 추가한다.
    bool HYJ_Unit_Insert(string _name, int _count)
    {
        bool res = false;

        CTRL_Character_Data element = new CTRL_Character_Data(_name);
        Unit_buyUnits.HYJ_Data_SetUnitData(element);
        //if (_count == -1)
        //{
        //    for (int i = 0; i < Unit_waitUnits.HYJ_Data_GetUnitDataCount(); i++)
        //    {
        //        if ((Unit_waitUnits.HYJ_Data_GetUnitData(i) == null) || (Unit_waitUnits.HYJ_Data_GetUnitData(i).Data_ID == null))
        //        {
        //            Unit_waitUnits.HYJ_Data_SetUnitData(element, i);
        //            res = true;
        //            break;
        //        }
        //    }
        //}
        //else
        //{
        //    Unit_waitUnits.HYJ_Data_SetUnitData(element, _count);
        //    res = true;
        //}

        return res;
    }

    object HYJ_Unit_Insert_Bridge(params object[] _args)
    {
        string name = (string)_args[0];
        int count = (int)_args[1];

        //
        HYJ_Unit_Insert(name, count);

        //
        return true;
    }

    //
    object HYJ_Unit_Data_Update_Bridge(params object[] _args)
    {
        List<HYJ_Battle_Manager_Line>   field_tiles = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
        HYJ_Battle_Manager_Line         wait_tiles  = (HYJ_Battle_Manager_Line)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_TILES);

        int db_cnt = Player_Unit_csv.Count;

        for (int i=0; i<3; i++) id_list[i] = new SerialList<int>(db_cnt);
        synergy_list = new List<int>(db_cnt);
        synergy_dic = new Dictionary<int, int>(db_cnt);

        for (int i=0; i<db_cnt; i++)
        {
            id_list[0].m_List.Add(0);
            id_list[1].m_List.Add(0);
            id_list[2].m_List.Add(0);
            synergy_list.Add(0);
            synergy_dic.Add(i, 0);
        }

        //
        for(int y = 0; y < field_tiles.Count; y++)
        {
            for(int x = 0; x < field_tiles[y].HYJ_Data_GetCount(); x++)
            {
                //
                CTRL_Character_Data data = null;

                GameObject obj = field_tiles[y].HYJ_Data_GetUnitOnTile(x);
                if(obj != null)
                {
                    Character obj_char = obj.GetComponent<Character>();
                    if (obj_char.UnitType == Character.Unit_Type.Ally)
                    {
                        data = obj_char.HYJ_Status_saveData;
                        // Synergy Update
                        Debug.Log("[Synergy] COST : " + obj_char.Stat_Cost);
                        synergy_list[obj_char.Stat_Cost - 1]++;

                        int id = Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID);
                        int starint = obj_char.StarInt();
                        id_list[starint].m_List[id]++;
                        
                        if (starint < 3 && id_list[starint].m_List[id] == 3)
                        {
                            if (Find_Unit_On_Tile(starint, id))
                            {
                                obj_char.StarUp(pos);
                                obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
                                pos = Vector3.zero;
                            }

                            id_list[starint].m_List[id] = 0;
                            id_list[starint + 1].m_List[id]++;
                        }

                    }
                }

                //
                Unit_fieldUnits[y].HYJ_Data_SetUnitData(data, x);
            }
        }

        for(int x = 0; x < wait_tiles.HYJ_Data_GetCount(); x++)
        {
            //
            CTRL_Character_Data data = null;

            GameObject obj = wait_tiles.HYJ_Data_GetUnitOnTile(x);
            if (obj != null)
            {
                Character obj_char = obj.GetComponent<Character>();
                data = wait_tiles.HYJ_Data_GetUnitOnTile(x).GetComponent<Character>().HYJ_Status_saveData;

                int id = Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID);
                int starint = obj_char.StarInt();
                id_list[starint].m_List[id]++;

                if (starint < 3 && id_list[starint].m_List[id] == 3)
                {
                    if (Find_Unit_On_Tile(starint, id))
                    {
                        obj_char.StarUp(pos);
						obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
						pos = Vector3.zero;
                    }


                    id_list[starint].m_List[id] = 0;
                    id_list[starint + 1].m_List[id]++;
                }
            }
            //
            Unit_waitUnits.HYJ_Data_SetUnitData(data, x);
        }

        for (int i=0; i<db_cnt; i++)
            synergy_dic[i+1] = synergy_list[i];

        // Tile.Ally_Enter에서 Player.UnitDataUpdate 호출, 이후 SynergyUpdate 호출
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE__SYNERGY_UPDATE, synergy_dic);

        //
        return true;
    }
    Vector3 pos = Vector3.zero;

    bool Find_Unit_On_Tile(int _star, int _id)
    {
        int cnt = 0;

        List<HYJ_Battle_Manager_Line> field_tiles = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
        HYJ_Battle_Manager_Line wait_tiles = (HYJ_Battle_Manager_Line)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_TILES);

        for (int y = 0; y < field_tiles.Count; y++)
        {
            for (int x = 0; x < field_tiles[y].HYJ_Data_GetCount(); x++)
            {
                GameObject obj = field_tiles[y].HYJ_Data_GetUnitOnTile(x);

                if (obj != null)
                {
                    Character obj_char = obj.GetComponent<Character>();

                    if (obj_char.StarInt() == _star && Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID) == _id && obj_char.UnitType == Character.Unit_Type.Ally)
                    {
                        object[] param = new object[2];
                        param[0] = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().tile_type;
                        param[1] = obj;
                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, param);
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
                        if ((HYJ_Battle_Tile.Tile_Type)param[0] == HYJ_Battle_Tile.Tile_Type.Field)
                            pos = obj.transform.position;
                        cnt++;
                        if (cnt == 2) return true;
                    }

                }

            }

        }


        for (int x = 0; x < wait_tiles.HYJ_Data_GetCount(); x++)
        {

            GameObject obj = wait_tiles.HYJ_Data_GetUnitOnTile(x);
            if (obj != null)
            {
                Character obj_char = obj.GetComponent<Character>();

                if (obj_char.StarInt() == _star && Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID) == _id && obj_char.UnitType == Character.Unit_Type.Ally)
                {
                    object[] param = new object[2];
                    param[0] = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().tile_type;
                    param[1] = obj;
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, param);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
                    if ((HYJ_Battle_Tile.Tile_Type)param[0] == HYJ_Battle_Tile.Tile_Type.Field)
                        pos = obj.transform.position;
                    cnt++;
                    if (cnt == 2) return true;
                }

            }
        }

        return true;
    }


    //////////  Default Method  //////////
    bool HYJ_Unit_Init()
    {
        object count0 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_X);
        if (count0 == null)
        {
            return false;
        }
        else
        {
            if (Unit_waitUnits.HYJ_Data_GetUnitDataCount() == 0)
                Unit_waitUnits = new HYJ_Player_Unit_Datas((int)count0);

        }

        Player_Unit_csv = (List<Dictionary<string, object>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);
        StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase.csv");

        // 헤더 추가
        int row_cnt = Player_Unit_csv[0].Keys.ToList().Count;
        for (int i=0; i< row_cnt - 1; i++)
        {
            outStream.Write(Player_Unit_csv[0].Keys.ToList()[i]);
            outStream.Write(",");
        }
        outStream.Write(Player_Unit_csv[0].Keys.ToList()[row_cnt - 1]);
        outStream.Write("\n");

        // 내용 추가
        int col_cnt = Player_Unit_csv.Count;
        for (int k=0; k<col_cnt; k++)
        {
            for (int i = 0; i < row_cnt - 1; i++)
            {
                outStream.Write(Player_Unit_csv[k].Values.ToList()[i]);
                outStream.Write(",");
            }
            outStream.Write(Player_Unit_csv[k].Values.ToList()[row_cnt - 1]);
            outStream.Write("\n");
        }
        outStream.Close();

        //
        if (true)
        {
                    count0 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
            object  count1 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y);
            if (Unit_fieldUnits == null)
                Unit_fieldUnits = new List<HYJ_Player_Unit_Datas>();

            if((count0 != null) && (count1 != null))
            {
                for (int forY = 0; forY < (int)count1; forY++)
                {
                    Unit_fieldUnits.Add(null);
                }

                for (int forY = 0; forY < (int)count1; forY++)
                {
                    int countX = (int)count0;
                    if ((forY % 2) == 1)
                    {
                        countX += 1;
                    }

                    Unit_fieldUnits[forY] = new HYJ_Player_Unit_Datas(countX);
                }
            }
            else
            {
                //res = false;
                return false;
            }
        }

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_BUY_UNIT_DATA,   HYJ_Unit_GetBuyUnitData     );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_BUY_UNIT_COUNT,  HYJ_Unit_GetBuyUnitCount    );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_WAIT_UNIT_DATA,  HYJ_Unit_GetWaitUnitData    );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__INSERT,              HYJ_Unit_Insert_Bridge      );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE,         HYJ_Unit_Data_Update_Bridge );

        return true;
    }
}

#endregion

// 아이템에 대한 정보
#region Item

// 아이템 정보를 모은 클래스
[Serializable]
public class HYJ_Player_Item : IDisposable
{
    public string   Data_name;  // 아이템의 DB이름
    public int      Data_count; // 현재 보유하고 있는 갯수

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void Dispose()
    {

    }

    public void HYJ_Data_AddCount(int _count)
    {
        Data_count = _count;
    }

    //////////  Default Method  //////////
    public HYJ_Player_Item(string _name, int _count)
    {
        Data_name = _name;
        Data_count = _count;
    }
}

partial class HYJ_Player
{
    [SerializeField] List<HYJ_Player_Item> Item_relics;
    [SerializeField] List<HYJ_Player_Item> Item_relicsEquip;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Item_Insert(params object[] _args)
    {
        string type = (string)_args[0];
        string name = (string)_args[1];
        int count = (int)_args[2];

        //
        switch(type)
        {
            case "RELIC":
                {
                    Item_relics.Add(new HYJ_Player_Item(name, count));
                }
                break;
            case "UNIT":
                {
                    HYJ_Unit_Insert(name, -1);
                }
                break;
            case "POTION":
                {
                    HYJ_Buff_Insert(name);
                }
                break;
        }

        //
        return null;
    }

    //////////  Default Method  //////////
    bool HYJ_Item_Init()
    {
        Item_relics = new List<HYJ_Player_Item>();

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,  HYJ_Item_Insert );

        return true;
    }
}

#endregion

// 버프 정보
#region Buff

partial class HYJ_Player
{
    [SerializeField] List<CTRL_Buff_Save> Buff_buffs;
    [SerializeField] List<CTRL_Buff_Save> Buff_debuffs;

    //////////  Getter & Setter //////////

    //
    object HYJ_Buff_GetBuffFromCount(params object[] _args)
    {
        CTRL_Buff_Save res = null;

        //
        int count = (int)_args[0];

        res = Buff_buffs[count];

        //
        return null;
    }

    object HYJ_Buff_GetBuffCount(params object[] _args)
    {
        int res = -1;

        //
        res = Buff_buffs.Count;

        //
        return res;
    }

    //
    object HYJ_Buff_GetDeBuffFromCount(params object[] _args)
    {
        CTRL_Buff_Save res = null;

        //
        int count = (int)_args[0];

        res = Buff_debuffs[count];

        //
        return null;
    }

    object HYJ_Buff_GetDeBuffCount(params object[] _args)
    {
        int res = -1;

        //
        res = Buff_debuffs.Count;

        //
        return res;
    }

    //////////  Method          //////////
    void HYJ_Buff_Insert(string _name)
    {

        HYJ_Item element
            = (HYJ_Item)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.DATABASE___POTION__GET_DATA_FROM_NAME,
                _name);
        Debug.Log("HYJ_Buff_Insert " + element.HYJ_Data_type);

        switch(element.HYJ_Data_type)
        {
            case "BUFF":
            case "FRIENDLY":
                {
                    //Buff_buffs.Add(new HYJ_Player_Buff(element));
                }
                break;
        }

    }

    object HYJ_Buff_Insert_Bridge(params object[] _args)
    {
        HYJ_Buff_Insert("");
        //
        return true;
    }

    //////////  Default Method  //////////
    bool HYJ_Buff_Init()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__SETTING,         HYJ_Buff_Insert_Bridge  );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_FROM_COUNT, HYJ_Buff_GetBuffFromCount   );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_COUNT,      HYJ_Buff_GetBuffCount       );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT,   HYJ_Buff_GetDeBuffFromCount );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT,        HYJ_Buff_GetDeBuffCount     );

        return true;
    }
}

#endregion

// 평판 정보
#region Reputation

public enum HYJ_Player_REPUTATION_RACE
{
    HIGH_ELF = 0
}

partial class HYJ_Player
{
    [SerializeField] List<int> Reputation_races;
    const int Reputation_defaultValue = 1000;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    void HYJ_Reputation_ChangeValue(float _value)
    {
    }

    object HYJ_Reputation_ChangeValue_Bridge(params object[] _args)
    {
        float value = (float)_args[0];

        HYJ_Reputation_ChangeValue(value);

        //
        return true;
    }

    //////////  Default Method  //////////
    bool HYJ_Reputation_Init()
    {
        Reputation_races = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            Reputation_races.Add(Reputation_defaultValue);
        }

        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__SETTING, HYJ_Buff_Insert_Bridge);
        //
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_FROM_COUNT, HYJ_Buff_GetBuffFromCount);
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_COUNT, HYJ_Buff_GetBuffCount);
        //
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT, HYJ_Buff_GetDeBuffFromCount);
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT, HYJ_Buff_GetDeBuffCount);

        return true;
    }
}

#endregion