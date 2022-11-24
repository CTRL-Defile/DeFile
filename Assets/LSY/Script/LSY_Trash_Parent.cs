using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_Trash_Parent : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trash_Parent Enter.." + other);

        switch (other.tag)
        {
            case "Ally":
                this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, false);
                break;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trash_Parent Exit.." + other);

        switch (other.tag)
        {
            case "Ally":
                this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, true);
                break;
        }

    }




}
