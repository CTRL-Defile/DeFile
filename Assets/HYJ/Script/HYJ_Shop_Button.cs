using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Shop_Button : MonoBehaviour
{
    [SerializeField] protected HYJ_Shop_Manager Default_manager;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public virtual void HYJ_Default_Transform(Transform _trans, int _num)
    {

    }

    // HYJ_Default_Buy
    public virtual void HYJ_Default_Buy()
    {
    }

    protected bool HYJ_Default_Buy_GoldCheck(int _gold)
    {
        bool res = false;

        //
        object element = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            //
            HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_IS_ENOUGHT,
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

                this.gameObject.SetActive(false);

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

partial class HYJ_Shop_Button
{
    [SerializeField] protected string Info_name;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_Info_DataSetting(string _name)
    {
        Info_name = _name;
    }

    //////////  Default Method  //////////
}