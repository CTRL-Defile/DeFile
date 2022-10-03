using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

public partial class HYJ_Player : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        HYJ_Basic_Init();
        HYJ_Unit_Init();
        HYJ_Item_Init();
        HYJ_Buff_Init();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    // Basic_gold   //

    // 금화가 충분히 있는지 체크
    object HYJ_Basic_GoldIsEnought(params object[] _args)
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

    //////////  Default Method  //////////
    void HYJ_Basic_Init()
    {
        Basic_gold = 10000;

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_IS_ENOUGHT, HYJ_Basic_GoldIsEnought );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_PLUS,       HYJ_Basic_GoldPlus      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS,      HYJ_Basic_GoldMinus     );
    }
}

#endregion

// 유닛(기물)에 대한 정보
#region Unit

partial class HYJ_Player
{
    [SerializeField] List<string> Unit_waitUnits;
    [SerializeField] List<string> Unit_fieldUnits;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    // 유닛을 추가한다.
    void HYJ_Unit_Insert()
    {
        Debug.Log("HYJ_Unit_Insert");
    }

    // 유닛을 추가한다.(외부에서 접근할 때 사용)
    object HYJ_Unit_Insert_Bridge(params object[] _args)
    {
        HYJ_Unit_Insert();

        //
        return null;
    }

    //////////  Default Method  //////////
    void HYJ_Unit_Init()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__INSERT, HYJ_Unit_Insert_Bridge);
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
            case "ITEM":
                {
                    HYJ_Unit_Insert();
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
    void HYJ_Item_Init()
    {
        Item_relics = new List<HYJ_Player_Item>();

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,  HYJ_Item_Insert );
    }
}

#endregion

// 버프 정보
#region Buff

// 버프의 정보를 담은 클래스. 버프와 디버프가 공유함.
[Serializable]
public class HYJ_Player_Buff : IDisposable
{
    public string Data_name;
    public int Data_value;
    public int Data_count;

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
    public HYJ_Player_Buff(HYJ_Item _data)
    {
        Data_name = _data.HYJ_Data_name;
        Data_value = UnityEngine.Random.Range(_data.HYJ_Data_valueMin, _data.HYJ_Data_valueMax + 1);
        Data_count = _data.HYJ_Data_limit;
    }
}

partial class HYJ_Player
{
    [SerializeField] List<HYJ_Player_Buff> Buff_buffs;
    [SerializeField] List<HYJ_Player_Buff> Buff_debuffs;

    //////////  Getter & Setter //////////

    //
    object HYJ_Buff_GetBuffFromCount(params object[] _args)
    {
        HYJ_Player_Buff res = null;

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
        HYJ_Player_Buff res = null;

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
                    Buff_buffs.Add(new HYJ_Player_Buff(element));
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
    void HYJ_Buff_Init()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__SETTING,         HYJ_Buff_Insert_Bridge  );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_FROM_COUNT, HYJ_Buff_GetBuffFromCount   );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_COUNT,      HYJ_Buff_GetBuffCount       );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT,   HYJ_Buff_GetDeBuffFromCount );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT,        HYJ_Buff_GetDeBuffCount     );
    }
}

#endregion