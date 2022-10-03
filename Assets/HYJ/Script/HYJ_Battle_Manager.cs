using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////
    object HYJ_Basic_GetPhase(params object[] _args)
    {
        return Basic_phase;
    }

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        //
        return null;
    }

    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        //
        Basic_phase = 0;

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE, HYJ_Basic_GetPhase);
        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON, HYJ_ActiveOn);

        //this.HYJ_SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(Basic_phase)
        {
            case -1:
                {

                }
                break;
            //
            case 0:
                {
                    HYJ_Field_Init();

                    //
                    Basic_phase = -1;
                }
                break;
        }
    }
}

// 전장에 대한 정보(주로 타일)
#region FIELD

[Serializable]
public class HYJ_Battle_Manager_Line
{
    [SerializeField] List<HYJ_Battle_Tile> tiles;

    //////////  Getter & Setter //////////
    public HYJ_Battle_Tile HYJ_Data_Tile(int _count) { return tiles[_count]; }

    public int HYJ_Data_GetCount() { return tiles.Count; }

    public HYJ_Character HYJ_Data_GetUnitOnTile(int _count) { return tiles[_count].HYJ_Basic_onUnit; }

    //////////  Method          //////////
    public void HYJ_Tile_Add(Transform _trans)
    {
        tiles.Add(_trans.GetComponent<HYJ_Battle_Tile>());
    }

    //////////  Default Method  //////////
    public HYJ_Battle_Manager_Line()
    {
        tiles = new List<HYJ_Battle_Tile>();
    }
}

partial class HYJ_Battle_Manager
{
    [Header ("==================================================")]
    [Header ("FIELD")]

    [SerializeField] Transform Field_parent;
    [SerializeField] int Field_x;
    [SerializeField] int Field_y;

    [SerializeField] List<HYJ_Battle_Manager_Line> Field_tiles;

    //////////  Getter & Setter //////////
    object HYJ_Field_GetFieldX(params object[] _args)
    {
        return Field_x;
    }
    object HYJ_Field_GetFieldY(params object[] _args)
    {
        return Field_y;
    }

    // Field_tiles
    object HYJ_Field_GetTile(params object[] _args)
    {
        int x = (int)_args[0];
        int y = (int)_args[1];

        return Field_tiles[y].HYJ_Data_Tile(x);
    }

    // 캐릭터가 위치한 타일을 찾아낸다.
    object HYJ_Field_GetTileFromCharacter(params object[] _args)
    {
        Vector2 pos = (Vector2)HYJ_Field_GetXYFromCharacter(_args);

        return Field_tiles[(int)pos.y].HYJ_Data_Tile((int)pos.x);
    }

    // 캐릭터를 찾아낸다. xy값으로
    object HYJ_Field_GetCharacter(params object[] _args)
    {
        HYJ_Character res = null;

        //
        HYJ_Battle_Tile element = (HYJ_Battle_Tile)HYJ_Field_GetTile(_args);
        res = element.HYJ_Basic_onUnit;

        return res;
    }

    // y의 값을 보내준다.
    object HYJ_Field_GetTilesCount(params object[] _args)
    {
        return Field_tiles.Count;
    }

    // 해당 y의 x값을 보내준다.
    object HYJ_Field_GetTilesGetCount(params object[] _args)
    {
        int count = (int)_args[0];

        return Field_tiles[count].HYJ_Data_GetCount();
    }

    // 해당 캐릭터의 위치를 찾는다.
    object HYJ_Field_GetXYFromCharacter(params object[] _args)
    {
        HYJ_Character target = (HYJ_Character)_args[0];

        //
        Vector2 res = new Vector2(-1, -1);

        for(int forY = 0; forY < Field_tiles.Count; forY++)
        {
            for(int forX = 0; forX < Field_tiles[forY].HYJ_Data_GetCount(); forX++)
            {
                HYJ_Character element = Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX);

                if ((element != null) && Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX).Equals(target))
                {
                    res.x = forX;
                    res.y = forY;

                    break;
                }
            }
        }

        return res;
    }

    //////////  Method          //////////


    //////////  Default Method  //////////
    void HYJ_Field_Init()
    {
        GameObject element = Field_parent.GetChild(0).gameObject;

        Vector3 pos0 = element.transform.localPosition;
        Vector3 pos1 = Field_parent.GetChild(1).localPosition;

        //
        Field_tiles = new List<HYJ_Battle_Manager_Line>();
        for (int forY = 0; forY < Field_y; forY++)
        {
            HYJ_Battle_Manager_Line line = new HYJ_Battle_Manager_Line();

            //
            int countX = Field_x;
            if((forY % 2) == 1)
            {
                countX += 1;
            }

            //
            for (int forX = 0; forX < countX; forX++)
            {
                GameObject obj = Instantiate(element, Field_parent);
                obj.SetActive(true);
                obj.name = forX + "_" + forY;
                if (countX == Field_x)
                {
                    obj.transform.localPosition = new Vector3(pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                else
                {
                    obj.transform.localPosition = new Vector3(-pos1.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                obj.transform.localScale = element.transform.localScale;

                //
                line.HYJ_Tile_Add(obj.transform);
            }

            //
            Field_tiles.Add(line);
        }

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X,                HYJ_Field_GetFieldX             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y,                HYJ_Field_GetFieldY             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE,                   HYJ_Field_GetTile               );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER,    HYJ_Field_GetTileFromCharacter  );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_CHARACTER,              HYJ_Field_GetCharacter          );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT,            HYJ_Field_GetTilesCount         );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT,        HYJ_Field_GetTilesGetCount      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER,      HYJ_Field_GetXYFromCharacter    );
    }
}

#endregion