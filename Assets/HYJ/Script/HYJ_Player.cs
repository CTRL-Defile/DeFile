using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using JetBrains.Annotations;
using System.IO;
using System.Linq;
using UnityEngine.UIElements;
using System.Data;

public partial class HYJ_Player : MonoBehaviour
{
    public enum UPDATE_PHASE
    {
        FILE,
        BASIC,
        DB,
        ITEM,
        BUFF,
        REPUTATION,
        UNIT,
        MAP,

        UPDATE
    }
    [SerializeField] UPDATE_PHASE Basic_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object CTRL_Basic_GetUpdatePhase(params object[] _args)
    {
        return Basic_phase;
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_UPDATE_PHASE, CTRL_Basic_GetUpdatePhase);

        Basic_phase = UPDATE_PHASE.FILE;
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_phase)
        {
            case UPDATE_PHASE.UPDATE: break;
            //
            case UPDATE_PHASE.FILE:     { if (CTRL_File_Init()  ) { Basic_phase = UPDATE_PHASE.BASIC;   }   }   break;
            case UPDATE_PHASE.BASIC:    { if (HYJ_Basic_Init()  ) { Basic_phase = UPDATE_PHASE.DB;      }   }   break;

            case UPDATE_PHASE.DB:
                {
                    //Debug.Log("Player : " + Basic_phase);

                    int DB_phase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_PHASE);
                    if (DB_phase == -1)
                    {
                        //Player_DB_Init();

                        Player_DB.Instance.Init();

                        Basic_phase = UPDATE_PHASE.ITEM;
                    }
                } 
                break;
            case UPDATE_PHASE.ITEM:         { if (HYJ_Item_Init()       ) { Basic_phase = UPDATE_PHASE.BUFF;        }   }   break;
            case UPDATE_PHASE.BUFF:         { if (HYJ_Buff_Init()       ) { Basic_phase = UPDATE_PHASE.REPUTATION;  }   }   break;
            case UPDATE_PHASE.REPUTATION:   { if (HYJ_Reputation_Init() ) { Basic_phase = UPDATE_PHASE.UNIT;        }   }   break;
            case UPDATE_PHASE.UNIT:         { if (HYJ_Unit_Init()       ) { Basic_phase = UPDATE_PHASE.MAP;         }   }   break;
                //{
                //    BATTLE_PHASE _phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
                //
                //    //Debug.Log("Player : " + Basic_phase);
                //    if (_phase == BATTLE_PHASE.PHASE_PREPARE)
                //    {
                //        if (HYJ_Unit_Init())
                //        {
                //            Basic_phase = UPDATE_PHASE.UPDATE;
                //        }
                //    }
                //}
                //break;
            case UPDATE_PHASE.MAP:          { if (CTRL_Map_Init()       ) { Basic_phase = UPDATE_PHASE.UPDATE;      }   }   break;
        }
    }
}

// 기본 정보를 분류
#region Basic

partial class HYJ_Player
{
//<<<<<<< HEAD
//    [SerializeField] int Basic_level;   // 레벨
//    [SerializeField] int Basic_exp;     // 경험치

//    [SerializeField] int Basic_hp;      // 현재 남은 체력
//    [SerializeField] int Basic_hpMax;   // 최대 체력

//    [SerializeField] int Basic_gold;    // 보유하고 있는 금화
//=======
    [Serializable]
    public class Basic_Data : CTRL_File
    {
        public int Basic_level;   // ����
        public int Basic_exp;     // ����ġ

        public int Basic_hp;      // ���� ���� ü��
        public int Basic_hpMax;   // �ִ� ü��

        public int Basic_gold;    // �����ϰ� �ִ� ��ȭ

        //////////  Getter & Setter //////////

        //////////  Method          //////////

        //////////  Default Method  //////////
    }
//>>>>>>> origin/develop_hyj

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    object LSY_Basic_IncExp(params object[] _args)
    {
        File_saveData.File_basic.Basic_exp = HYJ_Basic_IncExpAddBuff((int)_args[0]);
        return null;
    }

    int HYJ_Basic_IncExpAddBuff(int _exp)
    {
        int res = _exp;
        for(int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            if ((Buff_totalBuffs[i].Basic_index / 10000).Equals(6))
            {
                CTRL_Buff data = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, Buff_totalBuffs[i].Basic_index);
                switch (data.CTRL_Basic_applyType)
                {
                    case "Player_Expbuy":
                    case "Player_Expgain":
                        {
                            res += data.Basic_ratioValue;
                        }
                        break;
                }
            }
        }

        return res;
    }

    // LSY_Basic_DecExp
    object LSY_Basic_DecExp(params object[] _args)
    {
        File_saveData.File_basic.Basic_exp -= (int)_args[0];
        return null;
    }
    object LSY_Basic_getExp(params object[] _args)
    {
        return File_saveData.File_basic.Basic_exp;
    }
    object LSY_Basic_IncLevel(params object[] _args)
    {
        File_saveData.File_basic.Basic_level += (int)_args[0];
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___LEVEL__VIEW_LEVEL, File_saveData.File_basic.Basic_level);
        return null;
    }
    object LSY_Basic_getLevel(params object[] _args)
    {
        return File_saveData.File_basic.Basic_level;
    }


    // Basic_gold   //

    object LSY_Basic_getGold(params object[] _args)
    {
        return File_saveData.File_basic.Basic_gold;
    }

    // 금화가 충분히 있는지 체크
    object HYJ_Basic_GoldIsEnough(params object[] _args)
    {
        bool res = false;

        //
        int pay = (int)_args[0];
        if (File_saveData.File_basic.Basic_gold >= pay)
        {
            res = true;
        }

        return res;
    }

    // 금화를 추가한다.
    object HYJ_Basic_GoldPlus(params object[] _args)
    {
        //
        int value = HYJ_Basic_GoldPlusAddBuff((int)_args[0]);

        File_saveData.File_basic.Basic_gold += value;
        for (int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            if ((Buff_totalBuffs[i].Basic_index / 10000).Equals(4))
            {
                CTRL_Buff data = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, Buff_totalBuffs[i].Basic_index);
                switch (data.CTRL_Basic_applyType)
                {
                    case "Player_moneygain":
                        {
                            File_saveData.File_basic.Basic_gold += data.Basic_ratioValue;
                        }
                        break;
                }
            }
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___GOLD__VIEW_GOLD, File_saveData.File_basic.Basic_gold);

        //
        return null;
    }

//<<<<<<< HEAD
//    // 금화를 지불한다.
//=======
    int HYJ_Basic_GoldPlusAddBuff(int _gold)
    {
        int res = _gold;
        for (int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            CTRL_Buff data = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, Buff_totalBuffs[i].Basic_index);
            switch (data.CTRL_Basic_applyType)
            {
                case "Player_moneygain":
                    {
                        res += data.Basic_ratioValue;
                    }
                    break;
            }
        }

        return res;
    }

//    // ��ȭ�� �����Ѵ�.
//>>>>>>> origin/develop_hyj
    object HYJ_Basic_GoldMinus(params object[] _args)
    {
        bool res = false;

        //
        int value = (int)_args[0];

        if (File_saveData.File_basic.Basic_gold >= value)
        {
            File_saveData.File_basic.Basic_gold -= value;

            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___GOLD__VIEW_GOLD, File_saveData.File_basic.Basic_gold);

            res = true;
        }

        //
        return res;
    }

    // 금화 이자
    object Basic_GoldInterest(params object[] _args)
    {
        if (File_saveData.File_basic.Basic_gold >= 50)
            File_saveData.File_basic.Basic_gold += 5;
        else if (File_saveData.File_basic.Basic_gold >= 40)
            File_saveData.File_basic.Basic_gold += 4;
        else if (File_saveData.File_basic.Basic_gold >= 30)
            File_saveData.File_basic.Basic_gold += 3;
        else if (File_saveData.File_basic.Basic_gold >= 20)
            File_saveData.File_basic.Basic_gold += 2;
        else if (File_saveData.File_basic.Basic_gold >= 10)
            File_saveData.File_basic.Basic_gold += 1;
        else
            ;

        return true;
    }
    object Basic_getCurHP(params object[] _args)
    {
        return File_saveData.File_basic.Basic_hp;
    }

    object Basic_getMaxHP(params object[] _args)
    {
        return File_saveData.File_basic.Basic_hpMax;
    }

    // 플레이어 체력 회복
    object JHW_Basic_hp_Increase(params object[] _args)
    {
        int value = (int)_args[0];
        if (File_saveData.File_basic.Basic_hpMax < File_saveData.File_basic.Basic_hp + value) File_saveData.File_basic.Basic_hp = File_saveData.File_basic.Basic_hpMax;
        else File_saveData.File_basic.Basic_hp = File_saveData.File_basic.Basic_hp + value;

        for (int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            if ((Buff_totalBuffs[i].Basic_index / 10000).Equals(5))
            {
                CTRL_Buff data = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, Buff_totalBuffs[i].Basic_index);
                switch (data.CTRL_Basic_applyType)
                {
                    case "health_currentHealth":
                        {
                            File_saveData.File_basic.Basic_hp += data.Basic_ratioValue;
                            if(File_saveData.File_basic.Basic_hp > File_saveData.File_basic.Basic_hpMax)
                                File_saveData.File_basic.Basic_hp = File_saveData.File_basic.Basic_hpMax;
                        }
                        break;
                }
            }
        }

        // 플레이어 HP 상단바 조정
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___HP__VIEW_HP);

        return true;
    }

    //////////  Default Method  //////////
    bool HYJ_Basic_Init()
    {
        if(File_saveData != null)
        {
            if (File_saveData.File_basic.Basic_level == 0)
            {
                File_saveData.File_basic.Basic_gold = 10;
                File_saveData.File_basic.Basic_level = 1;
                File_saveData.File_basic.Basic_exp = 0;

                //
                File_saveData.File_basic.Basic_hpMax = 99;
                File_saveData.File_basic.Basic_hp = 80;
            }
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_GOLD, LSY_Basic_getGold);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_IS_ENOUGH, HYJ_Basic_GoldIsEnough);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_PLUS,       HYJ_Basic_GoldPlus      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS,      HYJ_Basic_GoldMinus     );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_INTEREST, Basic_GoldInterest);

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_INCREASE, LSY_Basic_IncExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_DECREASE, LSY_Basic_DecExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP, LSY_Basic_getExp);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__LEVEL_INCREASE, LSY_Basic_IncLevel);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL, LSY_Basic_getLevel);

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__CURRENT_HP, Basic_getCurHP);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__MAX_HP, Basic_getMaxHP);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__HP_INCREASE, JHW_Basic_hp_Increase);

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_PLAYER_UNIT_DATABASE, Player_Unit_GetUnitDataBase);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE, Player_DB_Update);

        // 플레이어 HP 상단바 조정
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___HP__VIEW_HP);

        return true;
    }
}

#endregion

// Database
#region Database
public class Player_DB
{
    private Player_DB() { }

    static List<List<Dictionary<string, object>>> _original;
    static List<List<SerialDictionary<string, object>>> _serial;
    private static Player_DB instance;
    public static Player_DB Instance
    {
        get
        {
            Debug.Log("PlayerDB.Instance...");
            if (instance == null)
            {
                instance = new Player_DB();
                _original = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);
            }
            return instance;
        }
    }

    public List<List<Dictionary<string, object>>> get_original { get { return _original; } }
    public List<List<SerialDictionary<string, object>>> get_serial { get { return _serial; } }
    public int unit_cnt { get { return _original[0].Count; } }

    public void idx_delete(int idx)
    {
        int star_cnt = _original.Count;
        int unit_cnt = _original[0].Count;
        for (int star=0; star <star_cnt; star++)
        {
            for (int i=0; i<unit_cnt; i++)
            {
                int unit_idx = (int)_original[star][i]["Index"];

                if (unit_idx == idx)
                {
                    _original[star].RemoveAt(i);
                    _serial[star].RemoveAt(i);
                    player_update();
                    return;
                }
            }
        }

        Debug.Log("[WARNING] There is none of id " + idx + " is exist in DB");

    }

    public void Init()
    {
        //_original = new List<List<Dictionary<string, object>>>();
        _serial = new List<List<SerialDictionary<string, object>>>();

        Debug.Log("PlayerDB_Init....");

        for (int i = 0; i < 3; i++)
        {
            //_original.Add(new List<Dictionary<string, object>>());
            _serial.Add(new List<SerialDictionary<string, object>>());
        }


        int star_cnt = _original.Count;
        int unit_cnt = _original[0].Count;

        for (int star = 0; star < star_cnt; star++)
        {
            for (int n = 0; n < unit_cnt; n++)
            {
                SerialDictionary<string, object> tmp = new SerialDictionary<string, object>();
                tmp.Dic_Copy(_original[star], n);
                _serial[star].Add(tmp);
            }
        }

        player_update();
    }

    public void player_update()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE);
    }


}

#endregion

// 유닛(기물)에 대한 정보
#region Unit

[Serializable]
public class HYJ_Player_Unit_Datas
{
    public List<CTRL_Character_Data> unitDatas;

    //////////  Getter & Setter //////////
    public CTRL_Character_Data HYJ_Data_GetUnitData(int _count) { return unitDatas[_count]; }
    public void HYJ_Data_SetUnitData(CTRL_Character_Data _data, int _count) { unitDatas[_count] = _data; }
    public void HYJ_Data_SetUnitData(CTRL_Character_Data _data) { unitDatas.Add(_data); }

    public int HYJ_Data_GetUnitDataCount() { return unitDatas.Count; }

    //////////  Method          //////////
    // �����δ� ��������� ä���� �ִ� ��츦 ���� ������ ó���ϴ� �޼���.
    public void HYJ_Data_SaveSetting()
    {
        for(int i = 0; i < unitDatas.Count; i++)
        {
            if(unitDatas[i] != null)
            {
                if((unitDatas[i].Data_ID == null) || (unitDatas[i].Data_ID.Equals("")))
                {
                    unitDatas[i] = null;
                }
            }
        }
    }

    //////////  Default Method  //////////
    public HYJ_Player_Unit_Datas(int _count)
    {
        unitDatas = new List<CTRL_Character_Data>();

        for (int i = 0; i < _count; i++)
        {
            unitDatas.Add(null);
        }
    }

    //
}

partial class HYJ_Player
{
    [Serializable]
    class Unit_Data : CTRL_File
    {
        public HYJ_Player_Unit_Datas Unit_buyUnits;
        public HYJ_Player_Unit_Datas Unit_waitUnits;
        public List<HYJ_Player_Unit_Datas> Unit_fieldUnits;

        //////////  Getter & Setter //////////

        //////////  Method          //////////
        // �����δ� ��������� ä���� �ִ� ��츦 ���� ������ ó���ϴ� �޼���.
        public void HYJ_Data_SaveSetting()
        {
            Unit_buyUnits.HYJ_Data_SaveSetting();
            Unit_waitUnits.HYJ_Data_SaveSetting();
            for(int i = 0; i < Unit_fieldUnits.Count; i++)
            {
                Unit_fieldUnits[i].HYJ_Data_SaveSetting();
            }
        }

        //////////  Default Method  //////////
    }

    //[SerializeField] HYJ_Player_Unit_Datas Unit_buyUnits;
    //[SerializeField] HYJ_Player_Unit_Datas Unit_waitUnits;
    //[SerializeField] List<HYJ_Player_Unit_Datas> Unit_fieldUnits;
    [SerializeField] List<int> synergy_list;
    [SerializeField] SerialList<int>[] id_list = new SerialList<int>[3];
    //[SerializeField] int[] id_array, synergy_array;
    Dictionary<int, int> synergy_dic;
    List<List<Dictionary<string, object>>> Player_Unit_csv;

    List<HYJ_Battle_Manager_Line> field_tiles;
    HYJ_Battle_Manager_Line wait_tiles;
    int DB_cnt;

    HYJ_Battle_Tile _Tile;
    [SerializeField]
    List<GameObject> field_units, stand_units, _candidate_units;
    int field_cnt, stand_cnt;
    [SerializeField]
    List<SerialDictionary<string, object>> Serial_dic;

    // Ȳ���� �߰�
    Dictionary<string, List<GameObject>> Unit_starCheck;

    //////////  Getter & Setter //////////
    //
    object HYJ_Unit_GetBuyUnitData(params object[] _args)
    {
        int _count = (int)_args[0];

        return File_saveData.File_unit.Unit_buyUnits.HYJ_Data_GetUnitData(_count);
    }

    object HYJ_Unit_GetBuyUnitCount(params object[] _args)
    {
        return File_saveData.File_unit.Unit_buyUnits.HYJ_Data_GetUnitDataCount();
    }

    //
    object HYJ_Unit_GetWaitUnitData(params object[] _args)
    {
        int _count = (int)_args[0];

        return File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitData(_count);
    }

    //
    object HYJ_Unit_GetFieldUnitData(params object[] _args)
    {
        int _x = (int)_args[0];
        int _y = (int)_args[1];

        return File_saveData.File_unit.Unit_fieldUnits[_y].HYJ_Data_GetUnitData(_x);
    }

    //
    object Player_Unit_GetUnitDataBase(params object[] _args)
    {
        Debug.Log("PlayerDB.Count : " + Player_Unit_csv[0].Count);
        return Player_Unit_csv;
    }

    //////////  Method          //////////
    // 유닛을 추가한다.
    // -1이면 빈 칸에 추가한다.

    bool HYJ_Unit_Insert(string _name, int _count)
    {
        bool res = false;

        CTRL_Character_Data element = new CTRL_Character_Data(_name);
        File_saveData.File_unit.Unit_buyUnits.HYJ_Data_SetUnitData(element);

        return res;
    }

    // ������ �߰��Ѵ�.(����)
    object HYJ_Unit_BuyFromBattle_bridge(params object[] _args)
    {
        string name = (string)_args[0];
        int count = (int)_args[1];

        //
        HYJ_Unit_BuyFromBattle(name, count);

        //
        return true;
    }

    bool HYJ_Unit_BuyFromBattle(string _name, int _count)
    {
        bool res = false;

        CTRL_Character_Data element = new CTRL_Character_Data(_name);
        if (_count == -1)
        {
            for (int i = 0; i < File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitDataCount(); i++)
            {
                if ((File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitData(i) == null) || (File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitData(i).Data_ID == null))
                {
                    File_saveData.File_unit.Unit_waitUnits.HYJ_Data_SetUnitData(element, i);
                    res = true;
                    break;
                }
            }
        }
        else
        {
            File_saveData.File_unit.Unit_waitUnits.HYJ_Data_SetUnitData(element, _count);
            res = true;
        }

        return res;
    }


    //
    object HYJ_Unit_Data_Update_Bridge(params object[] _args)
    {
        BATTLE_PHASE _PHASE = (BATTLE_PHASE)_args[0];

        Debug.Log("[Player.UnitDataUpdate] " +  _PHASE);

        field_tiles = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
        wait_tiles  = (HYJ_Battle_Manager_Line)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_TILES);

        field_units = (List<GameObject>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__GET_FIELD_UNIT);
        stand_units = (List<GameObject>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__GET_STAND_UNIT);
        field_cnt = field_units.Count;
        stand_cnt = stand_units.Count;

        //DB_cnt = Player_Unit_csv[0].Count;
        DB_cnt = Player_DB.Instance.unit_cnt;

        for (int i=0; i<3; i++) id_list[i] = new SerialList<int>(DB_cnt);
        synergy_list = new List<int>(DB_cnt);
        synergy_dic = new Dictionary<int, int>(DB_cnt);

        for (int i=0; i<DB_cnt; i++)
        {
            id_list[0].m_List.Add(0);
            id_list[1].m_List.Add(0);
            id_list[2].m_List.Add(0);
            synergy_list.Add(0);
            synergy_dic.Add(i, 0);
        }

        _candidate_units = new List<GameObject>();

        switch (_PHASE)
        {
            case BATTLE_PHASE.PHASE_INIT:
            case BATTLE_PHASE.PHASE_PREPARE:
            case BATTLE_PHASE.PHASE_COMBAT_OVER:
                {
                    Update_StarCheck();
                    Update_Stand_Tiles(_PHASE);
                    Update_Field_Tiles(_PHASE);
                }
                break;
            case BATTLE_PHASE.PHASE_COMBAT:
                {
                    Update_Stand_Tiles(_PHASE);
                }
                break;
        }

        //
        return true;
    }

    bool Find_Unit_On_Tile(GameObject _obj, BATTLE_PHASE _phase)
    {
        int cnt = 0, sac_num = 3;
        Character _char = _obj.GetComponent<Character>();
        int _star = _char.StarInt();
        int _id = _char.Character_Status_ID;

        _candidate_units.Clear();
        object[] param = new object[3];

        if (_phase == BATTLE_PHASE.PHASE_PREPARE)
        {
            for (int i = 0; i < field_cnt; i++)
            {
                GameObject obj = field_units[i];
                Character obj_char = field_units[i].GetComponent<Character>();

                if (obj_char.StarInt() == _star && obj_char.Character_Status_ID == _id && obj_char.UnitType == Character.Unit_Type.Ally)
                {
                    Debug.Log(obj.name + " Find it, star : " + obj_char.StarInt());
                    _Tile = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>();
                    _candidate_units.Add(obj);
                    param[cnt++] = obj;

                    if (cnt == sac_num)
                        return true;
                }
            }

        }

        for (int i=0; i<stand_cnt; i++)
        {
            GameObject obj = stand_units[i];
            Character obj_char = stand_units[i].GetComponent<Character>();

            if (obj_char.StarInt() == _star && obj_char.Character_Status_ID == _id && obj_char.UnitType == Character.Unit_Type.Ally)
            {
                Debug.Log(obj.name + " Find it, star : " + obj_char.StarInt());
                if (_Tile == null)
                    _Tile = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>();
                _candidate_units.Add(obj);
                param[cnt++] = obj;

                if (cnt == sac_num)
                    return true;
            }
        }

        /*
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
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
                        Debug.Log(obj.name + " Find it");
                        if ((HYJ_Battle_Tile.Tile_Type)param[0] == HYJ_Battle_Tile.Tile_Type.Field)
                            _Tile = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>();
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
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
                    Debug.Log(obj.name + " Find it");
                    if ((HYJ_Battle_Tile.Tile_Type)param[0] == HYJ_Battle_Tile.Tile_Type.Field)
                        _Tile = obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>();
                    cnt++;
                    if (cnt == 2) return true;
                }

            }
        }
        */

        Debug.Log("여기까지옴? " + _candidate_units.Count);
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);

        return false;
    }

    /// <summary>
    /// ���ֵ��� üũ�ϰ� ���� ���� 3���� ã�� ����.
    /// </summary>
    void Update_StarCheck()
    {
        if (Unit_starCheck == null)
        {
            Unit_starCheck = new Dictionary<string, List<GameObject>>();
        }

        while(Unit_starCheck.Count > 0)
        {
//<<<<<<< HEAD
//            obj = field_units[i];
//            obj_char = obj.GetComponent<Character>();
//            _data = obj_char.HYJ_Status_saveData;
//            int _star = obj_char.StarInt(), _id = obj_char.Character_Status_Index;

//            synergy_list[obj_char.Stat_Cost - 1]++;
//            id_list[_star].m_List[_id]++;
//            //Debug.Log(obj.name + " _star : " + _star + ", _cnt : " + id_list[_star].m_List[_id]);

//            // 2,3성 감지
//            if (_star < 3 && id_list[_star].m_List[_id] == 3)
//=======
            foreach(List<GameObject> element in Unit_starCheck.Values)
//>>>>>>> origin/develop_hyj
            {
                element.Clear();
            }
            Unit_starCheck.Clear();
        }

        for (int y = 0; y < field_tiles.Count; y++)
        {
            for (int x = 0; x < field_tiles[y].HYJ_Data_GetCount(); x++)
            {
                GameObject obj = field_tiles[y].HYJ_Data_GetUnitOnTile(x);
                if (obj != null)
                {
                    Character obj_char = obj.GetComponent<Character>();

//<<<<<<< HEAD
//                    obj_char.StarUp(_Tile);
//                    obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);

//                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);

//                    _Tile = null;

//                    // 시너지 리스트 초기화
//                    synergy_list[obj_char.Stat_Cost - 1] = 0;
//                    synergy_list[obj_char.Stat_Cost]++;

//                    // ID 리스트 초기화
//                    id_list[_star].m_List[_id] = 0;
//                    id_list[_star + 1].m_List[_id]++;
//                    Debug.Log(obj.name + " _star : " + obj_char.StarInt() + ", _cnt : " + id_list[obj_char.StarInt()].m_List[_id]);
//                }

//                // 3성 감지
//                if (_star < 2 && id_list[_star + 1].m_List[_id] == 3)
//                {
//                    //Debug.Log("STAR: " + (_star + 1) + " is 3! candi num " + _candidate_units.Count);
//                    //_candidate_units.Clear();
//                    //Debug.Log("STAR: " + (_star + 1) + " is 3! candi num " + _candidate_units.Count);
//                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE, _PHASE);
//=======
                    if (obj_char.m_UnitType == Character.Unit_Type.Ally)
                    {
                        if (!Unit_starCheck.ContainsKey(obj_char.HYJ_Status_saveData.Data_ID))
                        {
                            Unit_starCheck.Add(obj_char.HYJ_Status_saveData.Data_ID, new List<GameObject>());
                        }
                        Unit_starCheck[obj_char.HYJ_Status_saveData.Data_ID].Add(obj);
                    }
//>>>>>>> origin/develop_hyj
                }
            }
        }

        for (int x = 0; x < wait_tiles.HYJ_Data_GetCount(); x++)
        {
            GameObject obj = wait_tiles.HYJ_Data_GetUnitOnTile(x);
            if (obj != null)
            {
                Character obj_char = obj.GetComponent<Character>();

                if (!Unit_starCheck.ContainsKey(obj_char.HYJ_Status_saveData.Data_ID))
                {
                    Unit_starCheck.Add(obj_char.HYJ_Status_saveData.Data_ID, new List<GameObject>());
                }
                Unit_starCheck[obj_char.HYJ_Status_saveData.Data_ID].Add(obj);
            }
        }

        foreach (string key in Unit_starCheck.Keys)
        {
            List<GameObject> units = Unit_starCheck[key];
            if (units.Count >= 3)
            {
                Character obj_char = units[0].GetComponent<Character>();
                obj_char.StarUp(obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>());
                obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);

                units.RemoveAt(0);

                object[] param = new object[2];
                param[0] = units[0];
                param[1] = units[1];

                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
            }
        }
    }

    void Update_Field_Tiles(BATTLE_PHASE _PHASE)
    {
        for (int y = 0; y < field_tiles.Count; y++)
        {
            for (int x = 0; x < field_tiles[y].HYJ_Data_GetCount(); x++)
            {
                //
                CTRL_Character_Data data = null;

                GameObject obj = field_tiles[y].HYJ_Data_GetUnitOnTile(x);
                if (obj != null)
                {
                    Character obj_char = obj.GetComponent<Character>();
                    if (obj_char.UnitType == Character.Unit_Type.Ally)
                    {
                        data = obj_char.HYJ_Status_saveData;

        //                // Synergy Update
        //                //Debug.Log("[Synergy] COST : " + obj_char.Stat_Cost);
        //                synergy_list[obj_char.Stat_Cost - 1]++;

        //                int id = Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID);
        //                int starint = obj_char.StarInt();
        //                id_list[starint].m_List[id]++;
        //                Debug.Log(obj.name + " _star : " + starint + ", _cnt : " + id_list[starint].m_List[id]);

        //                if (starint < 3 && id_list[starint].m_List[id] == 3)
        //                {
        //                    if (Find_Unit_On_Tile(obj))
        //                    //if ((int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__FIND_FIELD, starint, id) == 2)
        //                    {
        //                        obj_char.StarUp(_Tile);
        //                        obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
        //                        _Tile = null;
        //                    }

        //                    id_list[starint].m_List[id] = 0;
        //                    id_list[starint + 1].m_List[id]++;
        //                    Debug.Log(obj.name + " _star : " + starint + ", _cnt : " + id_list[starint].m_List[id]);

        //                    if (id_list[starint + 1].m_List[id] == 3 && starint < 2)
        //                    {
        //                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE, _PHASE);
        //                    }
        //                }
                    }
                }

                //
                File_saveData.File_unit.Unit_fieldUnits[y].HYJ_Data_SetUnitData(data, x);
            }
        }

        {
            //GameObject obj = null;
            //Character obj_char = null;
            //CTRL_Character_Data _data = null;
            //
            //for (int i = 0; i < field_cnt; i++)
            //{
            //    obj = field_units[i];
            //    obj_char = obj.GetComponent<Character>();
            //    _data = obj_char.HYJ_Status_saveData;
            //    int _star = obj_char.StarInt(), _id = obj_char.Character_Status_Index;
            //
            //    synergy_list[obj_char.Stat_Cost - 1]++;
            //    id_list[_star].m_List[_id]++;
            //    //Debug.Log(obj.name + " _star : " + _star + ", _cnt : " + id_list[_star].m_List[_id]);
            //
            //    if (_star < 3 && id_list[_star].m_List[_id] == 3)
            //    {
            //        if (Find_Unit_On_Tile(obj, _PHASE))
            //        {
            //            Debug.Log("Candidate count " + _candidate_units.Count);
            //            _candidate_units.Remove(obj);
            //            object[] param = new object[2];
            //            param[0] = _candidate_units[0];
            //            param[1] = _candidate_units[1];
            //
            //            obj_char.StarUp(_Tile);
            //            obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
            //
            //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
            //
            //            _Tile = null;
            //
            //            id_list[_star].m_List[_id] = 0;
            //            id_list[_star + 1].m_List[_id]++;
            //            Debug.Log(obj.name + " _star : " + obj_char.StarInt() + ", _cnt : " + id_list[obj_char.StarInt()].m_List[_id]);
            //        }
            //        if (id_list[_star + 1].m_List[_id] == 3 && _star < 2)
            //        {
            //            //Debug.Log("STAR: " + (_star + 1) + " is 3! candi num " + _candidate_units.Count);
            //            //_candidate_units.Clear();
            //            //Debug.Log("STAR: " + (_star + 1) + " is 3! candi num " + _candidate_units.Count);
            //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE, _PHASE);
            //        }
            //    }
            //
            //}
            //
            //for (int i = 0; i < DB_cnt; i++)
            //    synergy_dic[i + 1] = synergy_list[i];
        }

        // Tile.Ally_Enter에서 Player.UnitDataUpdate 호출, 이후 SynergyUpdate 호출
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE__SYNERGY_UPDATE, synergy_dic);
    }

    void Update_Stand_Tiles(BATTLE_PHASE _PHASE)
    {
        for (int x = 0; x < wait_tiles.HYJ_Data_GetCount(); x++)
        {
            //
            CTRL_Character_Data data = null;

            GameObject obj = wait_tiles.HYJ_Data_GetUnitOnTile(x);
            if (obj != null)
            {
                data = wait_tiles.HYJ_Data_GetUnitOnTile(x).GetComponent<Character>().HYJ_Status_saveData;
        //        Character obj_char = obj.GetComponent<Character>();

        //        int id = Int32.Parse(obj_char.HYJ_Status_saveData.Data_ID);
        //        int starint = obj_char.StarInt();
        //        id_list[starint].m_List[id]++;
        //        Debug.Log(obj.name + " _star : " + starint + ", _cnt : " + id_list[starint].m_List[id]);

        //        if (starint < 3 && id_list[starint].m_List[id] == 3)
        //        {
        //            Debug.Log("STARUP " + obj.name + " _star : " + starint + ", _cnt : " + id_list[starint].m_List[id]);
        //            if (Find_Unit_On_Tile(obj))
        //            //if ((int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__FIND_STAND, starint, id) == 2)
        //            {
        //                Debug.Log("ASDF");
        //                obj_char.StarUp(_Tile);
        //                obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
        //                _Tile = null;
        //            }

        //            id_list[starint].m_List[id] = 0;
        //            id_list[starint + 1].m_List[id]++;
        //            Debug.Log(obj.name + " _star : " + starint + ", _cnt : " + id_list[starint].m_List[id]);


        //            if (id_list[starint + 1].m_List[id] == 3 && starint < 2)
        //            {
        //                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE, _PHASE);
        //            }


        //        }
            }

            //
            File_saveData.File_unit.Unit_waitUnits.HYJ_Data_SetUnitData(data, x);
        }

        {
            //
            //GameObject obj = null;
            //Character obj_char = null;
            //CTRL_Character_Data _data = null;
            //
            //for (int i = 0; i < stand_cnt; i++)
            //{
            //    obj = stand_units[i];
            //    obj_char = obj.GetComponent<Character>();
            //    _data = obj_char.HYJ_Status_saveData;
            //    int _star = obj_char.StarInt(), _id = obj_char.Character_Status_Index;
            //
            //    id_list[_star].m_List[_id]++;
            //    //Debug.Log(obj.name + " _star : " + _star + ", _cnt : " + id_list[_star].m_List[_id]);
            //
            //    if (_star < 3 && id_list[_star].m_List[_id] == 3)
            //    {
            //        if (Find_Unit_On_Tile(obj, _PHASE))
            //        {
            //            Debug.Log("Candidate count " + _candidate_units.Count);
            //            _candidate_units.Remove(obj);
            //            object[] param = new object[2];
            //            param[0] = _candidate_units[0];
            //            param[1] = _candidate_units[1];
            //
            //            obj_char.StarUp(_Tile);
            //            obj_char.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
            //
            //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED, param);
            //
            //            _Tile = null;
            //
            //            id_list[_star].m_List[_id] = 0;
            //            id_list[_star + 1].m_List[_id]++;
            //            Debug.Log(obj.name + " _star : " + _star + ", _cnt : " + id_list[obj_char.StarInt()].m_List[_id]);
            //        }
            //        if (id_list[_star + 1].m_List[_id] == 3 && _star < 2)
            //        {
            //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE, _PHASE);
            //        }
            //
            //    }
            //
            //}
        }

        //
    }

    //////////  Default Method  //////////

    // Battle_Manager.Unit_init(), BaseCamp_Manager_DeleteUnit() 에서 호출
    object Player_DB_Update(params object [] _args)
    {
        // 1,2,3성 csv를 List로 처리. 
        //Player_Unit_csv = new List<List<Dictionary<string, object>>>();
        //string csv_path = "DataBase/Player_Unit_DataBase";

        //for (int i = 0; i < 3; i++)
        //{
        //    List<Dictionary<string, object>> tmp = CSVReader.Read(csv_path + "_" + (i + 1).ToString());
        //    Player_Unit_csv.Add(new List<Dictionary<string, object>>());

        //    int len = tmp.Count;
        //    for (int k = 0; k < len; k++)
        //    {
        //        Player_Unit_csv[i].Add(tmp[k]);
        //    }

        //}

        //int unit_cnt = Player_Unit_csv[0].Count;
        //for (int i = 0; i < unit_cnt; i++)
        //{
        //    Player_Unit_csv[0][i]["Index"] = i;
        //    Player_Unit_csv[1][i]["Index"] = i;
        //    Player_Unit_csv[2][i]["Index"] = i;
        //}

        var _ori = Player_DB.Instance.get_original[0];
        int unit_cnt = _ori.Count;

        Serial_dic = new List<SerialDictionary<string, object>>();
        for (int i = 0; i < unit_cnt; i++)
        {
            SerialDictionary<string, object> tmp = new SerialDictionary<string, object>();
            tmp.Dic_Copy(_ori, i);
            Serial_dic.Add(tmp);
        }

        return null;
    }

    void Player_DB_Init()
    {
        {
            //Player_Unit_csv = new List<List<Dictionary<string, object>>>();
            //Player_Unit_csv = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);

            //for (int _star = 0; _star < 3; _star++)
            //{
            //    StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase_"
            //        + (_star + 1).ToString() + ".csv");
            //    // 헤더 추가
            //    int row_cnt = Player_Unit_csv[_star][0].Keys.ToList().Count;
            //    for (int i = 0; i < row_cnt - 1; i++)
            //    {
            //        outStream.Write(Player_Unit_csv[_star][0].Keys.ToList()[i]);
            //        outStream.Write(",");
            //    }
            //    outStream.Write(Player_Unit_csv[_star][0].Keys.ToList()[row_cnt - 1]);
            //    outStream.Write("\n");

            //    // 내용 추가
            //    int col_cnt = Player_Unit_csv[0].Count;
            //    for (int k = 0; k < col_cnt; k++)
            //    {
            //        for (int i = 0; i < row_cnt - 1; i++)
            //        {
            //            outStream.Write(Player_Unit_csv[_star][k].Values.ToList()[i]);
            //            outStream.Write(",");
            //        }
            //        outStream.Write(Player_Unit_csv[_star][k].Values.ToList()[row_cnt - 1]);
            //        outStream.Write("\n");
            //    }
            //    outStream.Close();

            //} 이 코드는 딕셔너리를 읽어서 파일을 생성하는 형식임. 아래의 전체 파일 복사와는 다름.
        }

        string[] lines;
        for (int i = 1; i <= 3; i++)
        {
            // UsingDB 읽어오기
            lines = File.ReadAllLines("Assets/Resources/DataBase/DB_Using_Character_" + i.ToString() + ".csv");
            // PlayerDB 삭제 후 생성
            System.IO.File.Delete("Assets/Resources/DataBase/Player_Unit_DataBase_" + i.ToString() + ".csv");
            StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase_" + i.ToString() + ".csv");
            for (int j = 0; j < lines.Length; j++)
            {
                outStream.WriteLine(lines[j].ToString());
            }
            outStream.Close();
        }

        Debug.Log("[PlayerDB] DB_Using_Character_#.csv 생성 완료");


        object var = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);
        


        Player_DB_Update();

    }

    bool HYJ_Unit_Init()
    {
        if (File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitDataCount() != 0)
        {
            Debug.Log("HYJ_Unit_Init 0");
        }
        else
        {
            if (File_saveData.File_unit == null)
            {
                File_saveData.File_unit = new Unit_Data();
            }

            object count0 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_X);
            if (count0 == null)
            {
                Debug.Log("HYJ_Unit_Init 1");
                return false;
            }
            else
            {
                if (File_saveData.File_unit.Unit_waitUnits.HYJ_Data_GetUnitDataCount() == 0)
                    File_saveData.File_unit.Unit_waitUnits = new HYJ_Player_Unit_Datas((int)count0);

            }

            //
            if (true)
            {
                //
                count0 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
                object count1 = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y);
                if (File_saveData.File_unit.Unit_fieldUnits == null)
                    File_saveData.File_unit.Unit_fieldUnits = new List<HYJ_Player_Unit_Datas>();

                if ((count0 != null) && (count1 != null))
                {
                    for (int forY = 0; forY < (int)count1; forY++)
                    {
                        File_saveData.File_unit.Unit_fieldUnits.Add(null);
                    }

                    for (int forY = 0; forY < (int)count1; forY++)
                    {
                        int countX = (int)count0;
                        if ((forY % 2) == 1)
                        {
                            countX += 1;
                        }

                        File_saveData.File_unit.Unit_fieldUnits[forY] = new HYJ_Player_Unit_Datas(countX);
                    }
                }
                else
                {
                    Debug.Log("HYJ_Unit_Init 2");
                    //res = false;
                    return false;
                }
            }
        }

        //

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_BUY_UNIT_DATA,   HYJ_Unit_GetBuyUnitData         );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_BUY_UNIT_COUNT,  HYJ_Unit_GetBuyUnitCount        );
        
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_WAIT_UNIT_DATA,  HYJ_Unit_GetWaitUnitData        );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_FIELD_UNIT_DATA, HYJ_Unit_GetFieldUnitData       );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__BUY_FROM_BATTLE,     HYJ_Unit_BuyFromBattle_bridge   );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE,         HYJ_Unit_Data_Update_Bridge     );

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
    [Serializable]
    class Item_Data : IDisposable
    {
        public List<HYJ_Player_Item> Item_relics;
        public List<HYJ_Player_Item> Item_relicsEquip;

        //////////  Getter & Setter //////////

        //////////  Method          //////////

        //////////  Default Method  //////////
        public Item_Data()
        {
            Item_relics = new List<HYJ_Player_Item>();
            Item_relicsEquip = new List<HYJ_Player_Item>();
        }

        public void Dispose()
        {

        }
    }

    //////////  Getter & Setter //////////
    object HYJ_Item_GetRelics(params object[] _args) { return File_saveData.File_item.Item_relics; }

    object HYJ_Item_GetRelicsEquip(params object[] _args) { return File_saveData.File_item.Item_relicsEquip; }

    //////////  Method          //////////
    // HYJ_Item_Insert
    object HYJ_Item_Insert(params object[] _args)
    {
        string type = (string)_args[0];
        string name = (string)_args[1];
        int count = (int)_args[2];

        //
        switch (type)
        {
            case "RELIC":   { HYJ_Item_Insert__Relic(name, count);  }   break;
            case "UNIT":    { HYJ_Unit_Insert(name, -1);            }   break;
            case "POTION":  { HYJ_Buff_PotionInsert(name);          }   break;
            case "BUFF":    { HYJ_Buff_BuffInsert(name);            }   break;
            case "DEBUFF":  { HYJ_Buff_DeBuffInsert(name);          }   break;
        }

        //
        return true;
    }

    void HYJ_Item_Insert__Relic(string _name, int _count)
    {
        bool isContinue = true;

        // ������ ����ִ� ���� �ִٸ� �ű⿡ �־�����.
        for (int i = 0; i < File_saveData.File_item.Item_relicsEquip.Count; i++)
        {
            if(File_saveData.File_item.Item_relicsEquip[i] == null)
            {
                File_saveData.File_item.Item_relicsEquip[i] = new HYJ_Player_Item(_name, _count);
                isContinue = false;
                break;
            }
        }

        // �� ĭ�� �ִٸ� �ű⿡ �־�����. ���ٸ� �׳� ����.
        if(isContinue)
        {
            for (int i = 0; i < File_saveData.File_item.Item_relics.Count; i++)
            {
                if (File_saveData.File_item.Item_relics[i] == null)
                {
                    File_saveData.File_item.Item_relics[i] = new HYJ_Player_Item(_name, _count);
                    isContinue = false;
                    break;
                }
            }
        }
    }

    // HYJ_Item_Equip
    object HYJ_Item_Equip(params object[] _args)
    {
        int equipCount      = (int)_args[0];
        int inventoryCount  = (int)_args[1];

        HYJ_Player_Item item = File_saveData.File_item.Item_relics[inventoryCount];
        File_saveData.File_item.Item_relics[inventoryCount] = File_saveData.File_item.Item_relicsEquip[equipCount];
        File_saveData.File_item.Item_relicsEquip[equipCount] = item;

        //
        return true;
    }

    void HYJ_Item_SettingSkills()
    {
        bool isBuff = false;

        for(int i = 0; i < File_saveData.File_item.Item_relicsEquip.Count; i++)
        {
            HYJ_Item element
                = (HYJ_Item)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_FROM_NAME,
                    //
                    File_saveData.File_item.Item_relicsEquip[i].Data_name);
            string[] strs = element.HYJ_Data_valueEffect.Split('/');
            switch(strs[0])
            {
                case "BUFF":
                    {
                        if(!isBuff)
                        {
                            isBuff = true;
                            Buff_itemBuffs.Clear();
                        }

                        bool isContinue = true;
                        for(int i1 = 0; i1 < Buff_itemBuffs.Count; i1++)
                        {
                            if(Buff_itemBuffs[i1].Basic_index.Equals(int.Parse(strs[1])))
                            {
                                Buff_itemBuffs[i1].Basic_durationValue += element.HYJ_Data_valueMax;

                                isContinue = false;
                                break;
                            }
                        }

                        if(isContinue)
                        {
                            CTRL_Buff buffData = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, int.Parse(strs[1]));
                            CTRL_Buff_Save data = new CTRL_Buff_Save(buffData.Basic_data);

                            data.Basic_durationValue = element.HYJ_Data_valueMax;

                            Buff_itemBuffs.Add(data);
                        }
                    }
                    break;
            }
        }
    }

    //////////  Default Method  //////////
    bool HYJ_Item_Init()
    {
        if(File_saveData != null)
        {
            HYJ_Item_SettingSkills();
        }
        else
        {
            if (File_saveData.File_item == null)
            {
                File_saveData.File_item = new Item_Data();
            }

            File_saveData.File_item.Item_relics = new List<HYJ_Player_Item>();
            for (int i = 0; i < 20; i++)
            {
                File_saveData.File_item.Item_relics.Add(null);
            }

            File_saveData.File_item.Item_relicsEquip = new List<HYJ_Player_Item>();
            for (int i = 0; i < 4; i++)
            {
                File_saveData.File_item.Item_relicsEquip.Add(null);
            }
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__GET_RELICS,          HYJ_Item_GetRelics      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__GET_RELICS_EQUIP,    HYJ_Item_GetRelicsEquip );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,  HYJ_Item_Insert );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__EQUIP,   HYJ_Item_Equip  );

        return true;
    }
}

#endregion

// 버프 정보
#region Buff

partial class HYJ_Player
{
    [Serializable]
    class Buff_Data : CTRL_File
    {
        public List<CTRL_Buff_Save> Buff_buffs;
        public List<CTRL_Buff_Save> Buff_debuffs;

        //////////  Getter & Setter //////////

        //////////  Method          //////////

        //////////  Default Method  //////////
        public Buff_Data()
        {
            if (Buff_buffs == null)
            {
                Buff_buffs = new List<CTRL_Buff_Save>();
            }

            if (Buff_debuffs == null)
            {
                Buff_debuffs = new List<CTRL_Buff_Save>();
            }
        }
    }

    [SerializeField] List<CTRL_Buff_Save> Buff_itemBuffs;

    [SerializeField] List<CTRL_Buff_Save> Buff_totalBuffs;

    //////////  Getter & Setter //////////

    object HYJ_Buff_GetTotalBuffs(params object[] _args)
    {
        //
        return Buff_totalBuffs;
    }

    //
    object HYJ_Buff_GetDeBuffFromCount(params object[] _args)
    {
        CTRL_Buff_Save res = null;

        //
        int count = (int)_args[0];

        res = File_saveData.File_buff.Buff_debuffs[count];

        //
        return null;
    }

    object HYJ_Buff_GetDeBuffCount(params object[] _args)
    {
        int res = -1;

        //
        res = File_saveData.File_buff.Buff_debuffs.Count;

        //
        return res;
    }

    //////////  Method          //////////
    // Potion
    void HYJ_Buff_PotionInsert(string _name)
    {
        HYJ_Item element
            = (HYJ_Item)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.DATABASE___POTION__GET_DATA_FROM_NAME,
                _name);
        Debug.Log("HYJ_Buff_Insert " + element.HYJ_Data_type);

        switch (element.HYJ_Data_type)
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
        HYJ_Buff_PotionInsert("");
        //
        return true;
    }

    // Buff
    void HYJ_Buff_BuffInsert(string _name)
    {
        // 여기에 버프의 종류가 늘어남에 따라 추가합니다.
        CTRL_Buff element
            = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA,
                int.Parse(_name));

        //Debug.Log("HYJ_Buff_BuffInsert " + element.Basic_data.Basic_name);

        int num = -1;
        for (int i = 0; i < File_saveData.File_buff.Buff_buffs.Count; i++)
        {
            if (File_saveData.File_buff.Buff_buffs[i].CTRL_Basic_GetIsSame(element.Basic_data))
            {
                num = i;
                break;
            }
        }

        if (num == -1)
        {
            File_saveData.File_buff.Buff_buffs.Add(new CTRL_Buff_Save(element.Basic_data));
        }
        else
        {
            File_saveData.File_buff.Buff_buffs[num] = new CTRL_Buff_Save(element.Basic_data);
        }

        //
        HYJ_Buff_SettingTotalBuffs();

        // 친밀도 관련 버프 갱신이 있다면 발동하라.
        if (element.CTRL_Basic_applyType.ToString().Split('_')[1].Equals("change"))
        {
            HYJ_Reputation_Setting();
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___BUFF__VIEW);
    }

    // DeBuff
    void HYJ_Buff_DeBuffInsert(string _name)
    {
        // 여기에 버프의 종류가 늘어남에 따라 추가합니다.
        CTRL_Buff element
            = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_DATA,
                int.Parse(_name));

        //Debug.Log("HYJ_Buff_BuffInsert " + element.Basic_data.Basic_name);

        int num = -1;
        for (int i = 0; i < File_saveData.File_buff.Buff_debuffs.Count; i++)
        {
            if (File_saveData.File_buff.Buff_debuffs[i].CTRL_Basic_GetIsSame(element.Basic_data))
            {
                num = i;
                break;
            }
        }

        if (num != -1)
        {
            File_saveData.File_buff.Buff_debuffs.RemoveAt(num);
        }

        // 친밀도 관련 버프 갱신이 있다면 발동하라.
        if (element.CTRL_Basic_applyType.ToString().Split('_')[1].Equals("change"))
        {
            HYJ_Reputation_Setting();
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___BUFF__VIEW);
    }

    object BuffInsertByEvent(params object[] _args) // 이벤트에서 넘어온 버프 플레이어한테 추가
    {
        CTRL_Buff_Save data = (CTRL_Buff_Save)_args[0];
        File_saveData.File_buff.Buff_buffs.Add(data);
        HYJ_Buff_SettingTotalBuffs();
        return true;
    }

    //
    void HYJ_Buff_SettingTotalBuffs()
    {
        Buff_totalBuffs.Clear();

        for (int i = 0; i < File_saveData.File_buff.Buff_buffs.Count; i++)
        {
            Buff_totalBuffs.Add(File_saveData.File_buff.Buff_buffs[i]);
        }

        for (int i = 0; i < Buff_itemBuffs.Count; i++)
        {
            Buff_totalBuffs.Add(Buff_itemBuffs[i]);
        }
    }

    //
    object HYJ_Buff_EndStage(params object[] _args)
    {
        int whileNum = 0;
        while (whileNum < File_saveData.File_buff.Buff_buffs.Count)
        {
            if (File_saveData.File_buff.Buff_buffs[whileNum].CTRL_Basic_EndStage())
            {
                whileNum++;
            }
            else
            {
                File_saveData.File_buff.Buff_buffs.RemoveAt(whileNum);
            }
        }

        whileNum = 0;
        while (whileNum < File_saveData.File_buff.Buff_debuffs.Count)
        {
            if (File_saveData.File_buff.Buff_debuffs[whileNum].CTRL_Basic_EndStage())
            {
                whileNum++;
            }
            else
            {
                File_saveData.File_buff.Buff_debuffs.RemoveAt(whileNum);
            }
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___BUFF__VIEW);

        return true;
    }

    //
    object HYJ_Buff_UnitBuffs(params object[] _args)
    {
        List<CTRL_Buff> res = new List<CTRL_Buff>();

        //
        HYJ_Buff_SettingTotalBuffs();

        for (int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            if ((Buff_totalBuffs[i].Basic_index / 10000).Equals(1) |
                (Buff_totalBuffs[i].Basic_index / 10000).Equals(2))
            {
                CTRL_Buff element
                    = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                        HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA,
                        Buff_totalBuffs[i].Basic_index);
                res.Add(element);
            }
        }

        for (int i = 0; i < File_saveData.File_buff.Buff_debuffs.Count; i++)
        {
            if ((File_saveData.File_buff.Buff_debuffs[i].Basic_index / 10000).Equals(1))
            {
                CTRL_Buff element
                    = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                        HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_DATA,
                        File_saveData.File_buff.Buff_debuffs[i].Basic_index);
                res.Add(element);
            }
        }

        //
        return res;
    }

    //
    List<CTRL_Buff> HYJ_Buff_RaceReputation()
    {
        List<CTRL_Buff> res = new List<CTRL_Buff>();

        //
        HYJ_Buff_SettingTotalBuffs();

        for (int i = 0; i < Buff_totalBuffs.Count; i++)
        {
            if ((Buff_totalBuffs[i].Basic_index / 10000).Equals(0))
            {
                CTRL_Buff element
                    = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                        HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA,
                        Buff_totalBuffs[i].Basic_index);
                res.Add(element);
            }
        }

        return res;
    }

    //////////  Default Method  //////////
    bool HYJ_Buff_Init()
    {
        if (File_saveData != null)
        {
        }
        else
        {
            if (File_saveData.File_buff == null)
            {
                File_saveData.File_buff = new Buff_Data();
            }
        }

        Buff_totalBuffs = new List<CTRL_Buff_Save>();

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__SETTING,         HYJ_Buff_Insert_Bridge  );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_TOTAL_BUFFS,  HYJ_Buff_GetTotalBuffs );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT,   HYJ_Buff_GetDeBuffFromCount );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT,        HYJ_Buff_GetDeBuffCount     );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__END_STAGE,   HYJ_Buff_EndStage   );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__UNIT_BUFFS,  HYJ_Buff_UnitBuffs  );

        // JHW - Buff insert to player by event
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__INSERT_BY_EVENT,  BuffInsertByEvent);

        return true;
    }
}

#endregion

// 평판 정보
#region Reputation

partial class HYJ_Player
{
    const float Reputation_defaultValue = 1000f;

    [Header ("REPUTATION ==================================================")]
    [SerializeField] List<float> Reputation_races;

    //////////  Getter & Setter //////////
    object HYJ_Reputation_GetValue(params object[] _args)
    {
        int res = -1;

        //
        HYJ_Character_REPUTATION_RACE type = (HYJ_Character_REPUTATION_RACE)_args[0];

        //
        res = (int)Reputation_races[(int)type];

        //
        return res;
    }

    //////////  Method          //////////
    void HYJ_Reputation_Setting()
    {
        // 초기화
        for (int i = 0; i < 10; i++)
        {
            Reputation_races[i] = Reputation_defaultValue;
        }

        //
        List<CTRL_Buff> buffs = HYJ_Buff_RaceReputation();
        for (int i = 0; i < buffs.Count; i++)
        {
            string[] applyTypes = buffs[i].CTRL_Basic_applyType.ToString().Split('_');
            if(applyTypes[1].Equals("change"))
            {
                float ratio = 1.0f;
                if(buffs[i].Basic_ratioType == CTRL_Buff.RATIO_TYPE.percent)
                {
                    ratio = buffs[i].Basic_ratioValue * 0.01f;
                }
                float insertValue = Reputation_defaultValue * ratio;
                Reputation_races[(int)Enum.Parse(typeof(HYJ_Character_REPUTATION_RACE), applyTypes[0])] += insertValue;
            }
        }
    }
    //// HYJ_Reputation_PlusValue
    //void HYJ_Reputation_PlusValue(HYJ_Player_REPUTATION_RACE _type, float _value)
    //{
    //    Reputation_races[(int)_type] += _value;
    //}
    //
    //object HYJ_Reputation_PlusValue_Bridge(params object[] _args)
    //{
    //    HYJ_Player_REPUTATION_RACE type = (HYJ_Player_REPUTATION_RACE)_args[0];
    //    float value = (float)_args[1];
    //
    //    HYJ_Reputation_PlusValue(type, value);
    //
    //    //
    //    return true;
    //}
    //
    //// HYJ_Reputation_PlusValue
    //void HYJ_Reputation_MinusValue(HYJ_Player_REPUTATION_RACE _type, float _value)
    //{
    //    Reputation_races[(int)_type] -= _value;
    //}
    //
    //object HYJ_Reputation_MinusValue_Bridge(params object[] _args)
    //{
    //    HYJ_Player_REPUTATION_RACE type = (HYJ_Player_REPUTATION_RACE)_args[0];
    //    float value = (float)_args[1];
    //
    //    HYJ_Reputation_MinusValue(type, value);
    //
    //    //
    //    return true;
    //}

    //////////  Default Method  //////////
    bool HYJ_Reputation_Init()
    {
        Reputation_races = new List<float>();

        for (int i = 0; i < 10; i++)
        {
            Reputation_races.Add(Reputation_defaultValue);
        }
        
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___REPUTATION__GET_VALUE, HYJ_Reputation_GetValue );
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___REPUTATION__PLUS_VALUE,    HYJ_Reputation_PlusValue_Bridge     );
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___REPUTATION__MINUS_VALUE,   HYJ_Reputation_MinusValue_Bridge    );

        return true;
    }
}

#endregion

#region MAP

partial class HYJ_Player
{
    [Serializable]
    class Map_Data
    {
        public int Data_level;
        public int Data_playerPos;
        public List<HYJ_Map_Stage.SaveData> Data_mapDatas;

        public HYJ_Map_Stage_TYPE Data_stage;
        public List<string> Data_stageDatas;

        //////////  Getter & Setter //////////

        //////////  Method          //////////
        public void Data_MapSetting(bool _isDelete)
        {
            if (_isDelete)
            {
                Data_level = -1;
                Data_mapDatas.Clear();
            }
            else
            {
                Data_level = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__GET_LEVEL);

                List<HYJ_Map_Stage> mapData = (List<HYJ_Map_Stage>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__GET_STAGES);
                while(Data_mapDatas.Count < mapData.Count)
                {
                    Data_mapDatas.Add(null);
                }

                for (int i = 0; i < mapData.Count; i++)
                {
                    HYJ_Map_Stage.SaveData saveData = null;
                    if (mapData[i] != null)
                    {
                        saveData = mapData[i].HYJ_Stage_saveData;
                    }

                    Data_mapDatas[i] = saveData;
                }
            }
        }

        //////////  Default Method  //////////
        public Map_Data()
        {
            Data_stage = HYJ_Map_Stage_TYPE.BASE_CAMP;
        }
    }

    //////////  Getter & Setter //////////
    object CTRL_Map_GetLevel(params object[] _args)
    {
        return File_saveData.File_map.Data_level;
    }

    //
    object CTRL_Map_GetPlayerPos(params object[] _args)
    {
        return File_saveData.File_map.Data_playerPos;
    }

    object CTRL_Map_SetPlayerPos(params object[] _args)
    {
        int x = (int)_args[0];
        int y = (int)_args[1];

        File_saveData.File_map.Data_playerPos = x + (HYJ_Map_Manager.Cheapter_x * y);

        return true;
    }

    //
    object CTRL_Map_GetMapDatas(params object[] _args)
    {
        return File_saveData.File_map.Data_mapDatas;
    }

    //
    object CTRL_Map_GetStage(params object[] _args)
    {
        return File_saveData.File_map.Data_stage;
    }
    object CTRL_Map_SetStage(params object[] _args)
    {
        HYJ_Map_Stage_TYPE stageType = (HYJ_Map_Stage_TYPE)_args[0];

        File_saveData.File_map.Data_stage = stageType;

        return true;
    }

    //
    object CTRL_Map_GetStageDatas(params object[] _args)
    {
        return File_saveData.File_map.Data_stageDatas;
    }

    //////////  Method          //////////
    object CTRL_Map_MapSetting(params object[] _args)
    {
        bool isDelete = (bool)_args[0];

        File_saveData.File_map.Data_MapSetting(isDelete);

        return true;
    }

    //////////  Default Method  //////////
    bool CTRL_Map_Init()
    {
        bool res = true;
        
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_LEVEL,        CTRL_Map_GetLevel       );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_PLAYER_POS,   CTRL_Map_GetPlayerPos   );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__SET_PLAYER_POS,   CTRL_Map_SetPlayerPos   );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_MAP_DATAS,    CTRL_Map_GetMapDatas    );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE,        CTRL_Map_GetStage       );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__SET_STAGE,        CTRL_Map_SetStage       );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE_DATAS,  CTRL_Map_GetStageDatas  );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__MAP_SETTING,      CTRL_Map_MapSetting     );

        return res;
    }
}

#endregion

#region File

partial class HYJ_Player
{
    [Serializable]
    class File_Data : IDisposable
    {
        public Basic_Data   File_basic;
        public Unit_Data    File_unit;
        public Item_Data    File_item;
        public Buff_Data    File_buff;
        public Map_Data     File_map;

        //////////  Getter & Setter //////////

        //////////  Method          //////////
        public void HYJ_Data_SaveSetting()
        {
            File_unit.HYJ_Data_SaveSetting();
        }

        //////////  Default Method  //////////
        public File_Data()
        {
            File_basic  = new Basic_Data();
            File_unit   = new Unit_Data();
            File_buff   = new Buff_Data();
            File_map    = new Map_Data();
        }

        public void Dispose()
        {

        }
    }
    [SerializeField] File_Data File_saveData;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    // CTRL_File_Save
    object CTRL_File_Save_bridge(params object[] _args)
    {
        CTRL_File_Save();

        return true;
    }

    void CTRL_File_Save()
    {
        if(File_saveData == null)
        {
            File_saveData = new File_Data();
        }

        if(File_saveData.File_basic.Basic_level == 0)
        {
            Debug.Log("CTRL_File_Save " + Application.dataPath);
        }

        File_saveData.HYJ_Data_SaveSetting();
        File.WriteAllText(Application.dataPath + "/Player.save", JsonUtility.ToJson(File_saveData));
    }

    //////////  Default Method  //////////
    bool CTRL_File_Init()
    {
        bool res = true;

        try
        {
            string data = File.ReadAllText(Application.dataPath + "/Player.save");
            File_saveData = JsonUtility.FromJson<File_Data>(data);
        }
        catch(FileNotFoundException _e)
        {
            Debug.Log("CTRL_File_Init ANG!!");
        }

        if(File_saveData == null)
        {
            File_saveData = new File_Data();
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___FILE__SAVE, CTRL_File_Save_bridge);

        return res;
    }
}

#endregion