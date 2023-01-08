using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_Trash_Tile : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Debug.Log("Trash Tile Enter.." + other);
            Character.Unit_Type _Type = other.GetComponent<Character>().UnitType;
            switch (_Type)
            {
                case Character.Unit_Type.Ally:
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_PLUS, other.gameObject.GetComponent<Character>().Stat_Cost);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, true);
                    //Destroy(other.gameObject);
                    this.gameObject.SetActive(false);

                    break;
            }
        }
    }


}
