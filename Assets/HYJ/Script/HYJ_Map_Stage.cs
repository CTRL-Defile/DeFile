using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.UI;

public enum HYJ_Map_Stage_TYPE
{
    NONE,
    BASE_CAMP,
    BATTLE_NORMAL,  // 30%
    BATTLE_ELITE,   // 30%
    BATTLE_BOSS,
    SHOP,
    EVENT           // 40%
}

public partial class HYJ_Map_Stage : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //
    public void HYJ_Init(
        Transform _parent,
        int _x, int _y)
    {
        Transform parent = this.transform.parent;
        parent.parent = _parent;
        parent.localScale = new Vector3(1, 1, 1);
        parent.localPosition = new Vector3(_x * 150, _y * 150, 0);

        HYJ_Stage_Init();
        HYJ_UI_Init();
    }
}

partial class HYJ_Map_Stage
{
    [SerializeField] HYJ_Map_Stage_TYPE Stage_type;
    [SerializeField] string Stage_reward;
    [SerializeField] int Stage_power;
    [SerializeField] List<HYJ_Map_Stage> Stage_roots;
    [SerializeField] List<GameObject> Stage_icons;

    //////////  Getter & Setter //////////
    public HYJ_Map_Stage_TYPE HYJ_Stage_type
    {
        get { return Stage_type; }
        set
        {
            Stage_type = value;

            //
            for(int i = 0; i < Stage_icons.Count; i++)
            {
                Stage_icons[i].SetActive(false);
            }

            Stage_icons[(int)Stage_type].SetActive(true);
        }
    }

    //////////  Method          //////////
    public void HYJ_Stage_SettingType(int _level)
    {
        if(_level <= 4)
        {
            int rand = Random.Range(0, 100);

            int randCount = 40;
            if(rand < randCount)
            {
                HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_NORMAL;
            }
            else
            {
                randCount += 20;
                if (rand < randCount)
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_ELITE;
                }
                else
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.EVENT;
                }
            }
        }
    }

    //
    public void HYJ_Stage_AddRoot(HYJ_Map_Stage _root)
    {
        Stage_roots.Add(_root);
    }

    public void HYJ_Stage_Select()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__SELECT_RESET);

        Transform parent = this.transform.parent;
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__MOVE_CENTER,
            parent.localPosition.x, parent.localPosition.y, parent.localPosition.z);

        //
        for (int i = 0; i < Stage_roots.Count; i++)
        {
            Stage_roots[i].HYJ_UI_Active(true);
        }

        switch(Stage_type)
        {
            case HYJ_Map_Stage_TYPE.BASE_CAMP:
                {
                    //
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON,   true);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,         false);
                }
                break;
            case HYJ_Map_Stage_TYPE.SHOP:
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                    //
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON,    true);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
                }
                break;
            case HYJ_Map_Stage_TYPE.EVENT:
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                    //
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON,   true);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
                }
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
            case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
            case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                    //
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  true);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
                }
                break;
        }
    }

    //////////  Default Method  //////////
    void HYJ_Stage_Init()
    {
        Stage_roots = new List<HYJ_Map_Stage>();
    }
}

partial class HYJ_Map_Stage
{
    Button UI_btn;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_UI_Active(bool _isActive)
    {
        UI_btn.interactable = _isActive;
    }

    //////////  Default Method  //////////
    void HYJ_UI_Init()
    {
        UI_btn = gameObject.GetComponent<Button>();
        UI_btn.interactable = false;
    }
}