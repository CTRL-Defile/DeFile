using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.UI;

// 상단바를 관리하는 클래스
public partial class HYJ_TopBar : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        HP_Start();
        HYJ_Level_Start();
        HYJ_Gold_Start();
        HYJ_Power_Start();
        HYJ_Buff_Start();
        HYJ_Relic_Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

partial class HYJ_TopBar
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
}
# region HP
partial class HYJ_TopBar
{
    [SerializeField] public Slider HP_bar;
    [SerializeField] public Text HP_text;

    object HP_ViewHP(params object[] _args)
    {
        int curHP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__CURRENT_HP);
        int maxHP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__MAX_HP);

        //
        HP_text.text = curHP +" / "+ maxHP;
        HP_bar.value = (float)curHP / maxHP;

        //
        return true;
    }

    void HP_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___HP__VIEW_HP, HP_ViewHP);
    }
}
#endregion HP

#region LEVEL
partial class HYJ_TopBar
{
    [SerializeField] Text Level_text;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Level_ViewLevel(params object[] _args)
    {
        int level = (int)_args[0];

        //
        Level_text.text = "Lv. " + level;

        //
        return true;
    }

    //////////  Default Method  //////////
    void HYJ_Level_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___LEVEL__VIEW_LEVEL, HYJ_Level_ViewLevel);
    }
}
# endregion

#region GOLD

partial class HYJ_TopBar
{
    [SerializeField] Text Gold_text;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Gold_ViewGold(params object[] _args)
    {
        int gold = (int)_args[0];

        //
        Gold_text.text = gold + "G";

        //
        return true;
    }

    //////////  Default Method  //////////
    void HYJ_Gold_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___GOLD__VIEW_GOLD, HYJ_Gold_ViewGold);
    }
}

#endregion

#region BATTLE

partial class HYJ_TopBar
{
    [SerializeField] Text Battle_powerText;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Battle_ViewPower(params object[] _args)
    {
        int power = (int)_args[0];

        //
        Battle_powerText.text = power + "";

        //
        return true;
    }

    //////////  Default Method  //////////
    void HYJ_Power_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___BATTLE__VIEW_POWER,    HYJ_Battle_ViewPower    );
    }
}

#endregion

#region BUFF

partial class HYJ_TopBar
{
    [Header("==================================================")]
    [Header("BUFF")]
    [SerializeField] Transform      Buff_parent;
    [SerializeField] List<Image>    Buff_buffs;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Buff_View(params object[] _args)
    {
        //
        //
        int count = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_COUNT);

        for(int i = 0; i < count; i++)
        {
            CTRL_Buff_Save element = (CTRL_Buff_Save)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_FROM_COUNT, i);

            Buff_buffs[Buff_buffs.Count - 1 - i].gameObject.SetActive(true);
        }

        //
        count = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT);

        for (int i = 0; i < count; i++)
        {
            CTRL_Buff_Save element = (CTRL_Buff_Save)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT, i);

            Buff_buffs[i].gameObject.SetActive(true);
        }

        //
        return true;
    }

    //////////  Default Method  //////////
    void HYJ_Buff_Start()
    {
        for(int i = 0; i < Buff_parent.childCount; i++)
        {
            Image image = Buff_parent.GetChild(i).GetComponent<Image>();
            image.gameObject.SetActive(false);

            Buff_buffs.Add(image);
        }

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.TOPBAR___BUFF__VIEW,    HYJ_Buff_View   );
    }
}

#endregion

#region RELIC
partial class HYJ_TopBar
{
    [Header("RELIC ==================================================")]
    [SerializeField] GameObject Relic_UIParent;

    [Header("RUNNING")]
    [SerializeField] List<Transform> Relic_UIEquipBtns;
    [SerializeField] List<Transform> Relic_UIInventoryBtns;

    [SerializeField] string Relic_UIEquipSelect;
    [SerializeField] string Relic_UIInventorySelect;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_Relic_UIActive()
    {
        Relic_UIParent.SetActive(!Relic_UIParent.activeSelf);

        if(Relic_UIParent.activeSelf)
        {
            HYJ_Relic_UISetting();
        }
    }

    // 플레이어의 인벤토리 정보에 따라 UI를 갱신해준다.
    void HYJ_Relic_UISetting()
    {
        Relic_UIEquipSelect = null;
        Relic_UIInventorySelect = null;

        //
        List<HYJ_Player_Item> equipList = (List<HYJ_Player_Item>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__GET_RELICS_EQUIP);
        for (int i = 0; i < Relic_UIEquipBtns.Count; i++)
        {
            if (equipList[i] != null)
            {
                if (!((equipList[i].Data_name == null) || (equipList[i].Data_name.Equals(""))))
                {
                    HYJ_Item element
                        = (HYJ_Item)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_FROM_NAME,
                            //
                            equipList[i].Data_name);
                    Relic_UIEquipBtns[i].Find("icon").GetComponent<Image>().sprite = element.HYJ_Data_sprite;
                }
                else
                {
                    Relic_UIEquipBtns[i].Find("icon").GetComponent<Image>().sprite = null;
                }
            }
            else
            {
                Relic_UIEquipBtns[i].Find("icon").GetComponent<Image>().sprite = null;
            }
        }

        //
        List<HYJ_Player_Item> inventoryList = (List<HYJ_Player_Item>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__GET_RELICS);
        for (int i = 0; i < Relic_UIInventoryBtns.Count; i++)
        {
            if (inventoryList[i] != null)
            {
                if (!((inventoryList[i].Data_name == null) || (inventoryList[i].Data_name.Equals(""))))
                {
                    HYJ_Item element
                        = (HYJ_Item)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_FROM_NAME,
                            //
                            inventoryList[i].Data_name);
                    Relic_UIInventoryBtns[i].Find("icon").GetComponent<Image>().sprite = element.HYJ_Data_sprite;
                }
                else
                {
                    Relic_UIInventoryBtns[i].Find("icon").GetComponent<Image>().sprite = null;
                }
            }
            else
            {
                Relic_UIInventoryBtns[i].Find("icon").GetComponent<Image>().sprite = null;
            }
        }
    }

    void HYJ_Relic_UIBtnClick(Transform _obj)
    {
        string[] names = _obj.name.Split('_');

        switch(names[0])
        {
            // 장비칸일 때
            case "0":
                {
                    Relic_UIEquipSelect = names[1];
                }
                break;
            // 인벤토리일 때
            case "1":
                {
                    Relic_UIInventorySelect = names[1];
                }
                break;
        }

        if(
            !((Relic_UIEquipSelect == null)     || (Relic_UIEquipSelect.Equals(""))) &&
            !((Relic_UIInventorySelect == null) || (Relic_UIInventorySelect.Equals(""))))
        {
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__EQUIP,
                //
                int.Parse(Relic_UIEquipSelect), int.Parse(Relic_UIInventorySelect) );

            HYJ_Relic_UISetting();
        }
    }

    //////////  Default Method  //////////
    void HYJ_Relic_Start()
    {
        for(int i = 0; i < 4; i++)
        {
            Transform element = Relic_UIParent.transform.Find("0_" + i);
            element.GetComponent<Button>().onClick.AddListener(() => HYJ_Relic_UIBtnClick(element));
            Relic_UIEquipBtns.Add(element);
        }

        for (int i = 0; i < 20; i++)
        {
            Transform element = Relic_UIParent.transform.Find("1_" + i);
            element.GetComponent<Button>().onClick.AddListener(() => HYJ_Relic_UIBtnClick(element));
            Relic_UIInventoryBtns.Add(element);
        }
    }
}

#endregion