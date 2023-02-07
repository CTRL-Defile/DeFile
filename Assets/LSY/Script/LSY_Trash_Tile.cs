using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSY_Trash_Tile : MonoBehaviour
{
    HYJ_Battle_Tile.Tile_Type tile_Type;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trash Tile Enter.. " + other.name);

        Character other_character = other.GetComponent<Character>();
        if (other_character != null)
        {
            Character.Unit_Star _star = other_character.UnitStar;
            Character.Unit_Type _type = other_character.UnitType;
            if (_type != Character.Unit_Type.Ally) return;

            tile_Type = other_character.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().TileType;
            //int _id = other_character.Character_Status_ID;
            int _idx = other_character.Character_Status_Index;
            Debug.Log(other.name + " star : " + _star + " _id : " + _idx);
            if (_star != Character.Unit_Star.ONE)
            {
                other_character.UnitStar = Character.Unit_Star.ONE;
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__SACRIFICED_TO_POOL, _idx, _star);
            }

            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, tile_Type, other.gameObject);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_PLUS, other.gameObject.GetComponent<Character>().Stat_Cost);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, true);
            this.gameObject.SetActive(false);


        }
        //
    }


}
