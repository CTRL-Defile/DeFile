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
        HYJ_Gold_Start();
        HYJ_Power_Start();
        HYJ_Buff_Start();
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
            HYJ_Player_Buff element = (HYJ_Player_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_BUFF_FROM_COUNT, i);

            Buff_buffs[Buff_buffs.Count - 1 - i].gameObject.SetActive(true);
        }

        //
        count = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_COUNT);

        for (int i = 0; i < count; i++)
        {
            HYJ_Player_Buff element = (HYJ_Player_Buff)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__GET_DEBUFF_FROM_COUNT, i);

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