using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Shop_Manager : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        if(aa)
        {
            HYJ_Relic_SettingBtns();
            HYJ_Unit_SettingBtns();
            HYJ_Potion_SettingBtns();
        }

        //
        return null;
    }

    public void HYJ_On(bool _isActive)
    {
        //HYJ_Relic_SettingBtns();
        //HYJ_Item_SettingBtns();
        //HYJ_Potion_SettingBtns();
        this.gameObject.SetActive(_isActive);
    }

    void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    void HYJ_SettingBtns(
        List<HYJ_Shop_Button> _btns,
        //
        int _count, HYJ_Shop_Button _btn, Transform _parent)
    {
        while (_btns.Count > 1)
        {
            Destroy(_btns[0]);
            _btns.RemoveAt(0);
        }

        //
        for (int i = 0; i < _count; i++)
        {
            HYJ_Shop_Button element = Instantiate(_btn, _parent);
            element.HYJ_Default_Transform(_btn.transform, i);

            //
            _btns.Add(element);
        }
    }
    protected bool HYJ_Default_GoldCheck(int _gold)
    {
        bool res = false;

        //
        object element = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            //
            HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_IS_ENOUGH,
            //
            _gold);
        if (element != null)
        {
            bool isPossible = (bool)element;

            if (isPossible)
            {
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    //
                    HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS,
                    //
                    _gold);

                res = true;
            }
            else
            {
            }
        }
        else
        {

        }

        return res;
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = 0;

        HYJ_Relic_Start();
        HYJ_Unit_Start();
        HYJ_Potion_Start();
    }

    // Update is called once per frame
    void Update()
    {
        switch(Basic_phase)
        {
            case 0:
                {
                    object element = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BASIC__GET_IS_INITIALIZE);

                    if ((element != null) && (bool)element )
                    {
                        //HYJ_Relic_SettingBtns();
                        //HYJ_Item_SettingBtns();
                        //HYJ_Potion_SettingBtns();

                        Basic_phase += 1;
                    }
                }
                break;
            case 1:
                {
                    Basic_phase = -1;

                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON, HYJ_ActiveOn);

                    this.transform.Find("Canvas").GetComponent<Canvas>().worldCamera
                        = (Camera)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.MASTER___UI__GET_CAMERA,
                            0);

                    this.gameObject.SetActive(false);
                }
                break;
        }
    }
}

#region RELIC

partial class HYJ_Shop_Manager
{
    [Header("RELIC")]
    [Header("INPUT")]
    [SerializeField] Transform Relic_parent;
    [SerializeField] HYJ_Shop_Relic Relic_btn;

    [Header("SET")]
    [SerializeField] List<HYJ_Shop_Button> Relic_btns;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Relic_Buy(params object[] _args)
    {
        bool res = false;

        //
        string name = (string)_args[0];

        //
        bool isPossible = HYJ_Default_GoldCheck(10);
        if (isPossible)
        {
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                //
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,
                //
                "RELIC", name, 0);

            res = true;
        }

        //
        return res;
    }

    void HYJ_Relic_SettingBtns()
    {
        HYJ_SettingBtns(
            Relic_btns,
            4, Relic_btn, Relic_parent);

        //
        int dataCount = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_COUNT);

        for (int i = 0; i < Relic_btns.Count; i++)
        {
            string name
                = (string)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_NAME,
                    Random.Range(0, dataCount));

            Relic_btns[i].HYJ_Info_DataSetting(name);
            Relic_btns[i].gameObject.SetActive(true);
        }
    }

    //////////  Default Method  //////////
    void HYJ_Relic_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SHOP___RELIC__BUY, HYJ_Relic_Buy);
    }
}

#endregion

#region UNIT

partial class HYJ_Shop_Manager
{
    [Header("UNIT")]
    [Header("INPUT")]
    [SerializeField] Transform Unit_parent;
    [SerializeField] HYJ_Shop_Item Unit_btn;

    [Header("SET")]
    [SerializeField] List<HYJ_Shop_Button> Unit_btns;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Unit_Buy(params object[] _args)
    {
        bool res = false;

        //
        string name = (string)_args[0];

        //
        bool isPossible = HYJ_Default_GoldCheck(10);
        if (isPossible)
        {
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                //
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,
                //
                "UNIT", name, 0);

            res = true;
        }

        //
        return res;
    }

    void HYJ_Unit_SettingBtns()
    {
        HYJ_SettingBtns(
            Unit_btns,
            6, Unit_btn, Unit_parent);

        //
        int dataCount = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_COUNT);

        for (int i = 0; i < Unit_btns.Count; i++)
        {
            string name
                = (string)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_NAME,
                    Random.Range(0, dataCount));

            Unit_btns[i].HYJ_Info_DataSetting(name);
            Unit_btns[i].gameObject.SetActive(true);
        }
    }

    //////////  Default Method  //////////
    void HYJ_Unit_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SHOP___UNIT__BUY, HYJ_Unit_Buy);
    }
}

#endregion

#region POTION

partial class HYJ_Shop_Manager
{
    [Header("POTION")]
    [Header("INPUT")]
    [SerializeField] Transform Potion_parent;
    [SerializeField] HYJ_Shop_Potion    Potion_btn;

    [Header("SET")]
    [SerializeField] List<HYJ_Shop_Button> Potion_btns;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_Potion_Buy(params object[] _args)
    {
        bool res = false;

        //
        string name = (string)_args[0];

        //
        bool isPossible = HYJ_Default_GoldCheck(10);
        if (isPossible)
        {
            string[] names = name.Split('/');
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                //
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___ITEM__INSERT,
                //
                names[0], names[1], 0);

            res = true;
        }

        //
        return res;
    }

    void HYJ_Potion_SettingBtns()
    {
        HYJ_SettingBtns(
            Potion_btns,
            6, Potion_btn, Potion_parent);

        List<Dictionary<string, object>> data = CSVReader.Read("HYJ/Potion_csv");

        //
        List<int> data_heal     = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            switch((string)data[i]["TYPE"])
            {
                case "HEAL":        { data_heal.Add(i);     }   break;
                    //
                //case "BUFF":        { data_buff.Add(i);     }   break;
                //case "FRIENDLY":    { data_buff.Add(i);     }   break;
                    //
                //case "UNDEBUFF":    { data_undebuff.Add(i); }   break;
            }
        }

        // 버프 리스트 적용
        List<int> data_buff = (List<int>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_KEYS);
        int whileNum = 0;
        while(whileNum < data_buff.Count)
        {
            CTRL_Buff whileData = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA, data_buff[whileNum]);
            if(whileData.Basic_isShop)
            {
                whileNum++;
            }
            else
            {
                data_buff.RemoveAt(whileNum);
            }
        }

        List<int> data_debuff = (List<int>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_KEYS);
        whileNum = 0;
        while (whileNum < data_debuff.Count)
        {
            CTRL_Buff whileData = (CTRL_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_DATA, data_debuff[whileNum]);
            if (whileData.Basic_isShop)
            {
                whileNum++;
            }
            else
            {
                data_debuff.RemoveAt(whileNum);
            }
        }

        // 2개씩이라 한 번에 처리
        for (int i = 0; i < 2; i++)
        {
            int select = Random.Range(0, data_heal.Count);
            Potion_btns[i * 3].HYJ_Info_DataSetting("POTION/" + (string)data[data_heal[select]]["NAME"]);
            data_heal.RemoveAt(select);

            select = Random.Range(0, data_buff.Count);
            Potion_btns[1 + (i * 3)].HYJ_Info_DataSetting("BUFF/" + data_buff[select]);
            data_buff.RemoveAt(select);

            // 수정해야할 코드
            select = Random.Range(0, data_debuff.Count);
            Potion_btns[2 + (i * 3)].HYJ_Info_DataSetting("DEBUFF/" + data_debuff[select]);
            data_debuff.RemoveAt(select);
        }

        for (int i = 0; i < Potion_btns.Count; i++)
        {
            Potion_btns[i].gameObject.SetActive(true);
        }
    }

    //////////  Default Method  //////////
    void HYJ_Potion_Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.SHOP___POTION__BUY, HYJ_Potion_Buy);
    }
}

#endregion