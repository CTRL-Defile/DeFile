using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using UnityEngine.EventSystems;

public partial class HYJ_Character : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = 0;

        HYJ_Action_Init();
    }

    // Update is called once per frame
    void Update()
    {
        switch(Basic_phase)
        {
            case -1:
                {
                    HYJ_Action_Update();
                }
                break;
            //
            case 0:
                {
                    // Battle_Manager.Basic_phase
                    int battlePhase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);

                    if(battlePhase == -1)
                    {
                        HYJ_Astar_Init();

                        //
                        Basic_phase = -1;
                    }
                }
                break;
        }
    }
}

// ĳ������ ����
#region INFO

public enum HYJ_Character_FACTION
{
    PLAYER,
    ENEMY
}

partial class HYJ_Character
{
    [Header ("==================================================")]
    [Header ("INFO")]
    [SerializeField] HYJ_Character_FACTION Info_faction;    // ĳ���Ͱ� ���� ����
    [SerializeField] int Info_DBNumber;                     // ĳ������ DB ��ȣ

    //////////  Getter & Setter //////////
    public HYJ_Character_FACTION HYJ_Info_faction
    {
        get { return Info_faction; }
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
}

#endregion

// ĳ������ �ɷ�ġ
// ������ ������, ���� ���� �ܺο�ҷ� ���� ���ϴ� ��ġ���� �������ֱ� ���� ���Ǵ� ������
#region STATUS

partial class HYJ_Character
{
    [Header("==================================================")]
    [Header("STATUS")]
    [SerializeField] int Status_HP;     // ü��
    [SerializeField] int Status_MaxHP;  // �ִ�ü��
    [SerializeField] int Status_MP;     // ����
    [SerializeField] int Status_MaxMP;  // �ִ븶��

    [SerializeField] int Status_atk;    // ���ݷ�
    [SerializeField] int Status_magic;  // ����

    [SerializeField] int Status_atkSpeed;   // ����

    [SerializeField] int Status_critValue;  // ġ��Ÿ ��ġ
    [SerializeField] int Status_critPer;    // ġ��Ÿ Ȯ��

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    
    //////////  Default Method  //////////
}

#endregion

// AStar����
#region ASTAR

public enum HYJ_Character_AStar_TYPE
{
    DISTANCE,   // �Ÿ��� Ư���� ��
    FIELD       // ���� ���� ������ ��
}

// AStar�� ������ ���� Ŭ����
[Serializable]
public class HYJ_Character_AStar
{
    [SerializeField] List<int> tiles;   // �Ÿ���

    //////////  Getter & Setter //////////
    public int HYJ_Data_GetTile(int _count) { return tiles[_count]; }   // �Ÿ����� �޾ƿ´�.
    public void HYJ_Data_SetTile(int _count, int _value) { tiles[_count] = _value; }    // �Ÿ����� �Է��Ѵ�.

    //
    public int HYJ_Data_GetTileCount() { return tiles.Count; }  // �ش� Y������ X������ ���Ѵ�.

    //////////  Method          //////////
    // ��� ���� �� �ʱ�ȭ
    public void HYJ_Data_Reset()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i] = 10000;
        }
    }

    //////////  Default Method  //////////
    public HYJ_Character_AStar(int _count)
    {
        tiles = new List<int>();

        for (int i = 0; i < _count; i++)
        {
            tiles.Add(0);
        }
    }
}

partial class HYJ_Character
{
    [Header("==================================================")]
    [Header("ASTAR")]
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_distance;    // �ܼ��� �Ÿ��� ����ϴ� AStar
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_field;       // �ʵ� ���� ��ֹ����� ��꿡 ���� AStar
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_enemy;       // ���� �������� ����� AStar(�ʵ� ����)
    List<string> AStar_wait;    // ������ ����ؾ��ϴ� Ÿ���� ��ȣ����

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    void HYJ_AStar_Calc(
        List<HYJ_Character_AStar> _tiles)
    {
        // ����� ���� AStar�� ������ ���� ����� ����� ���Ѵ�.    //
        HYJ_Character_AStar_TYPE type = HYJ_Character_AStar_TYPE.DISTANCE;

        if((_tiles == AStar_tiles_distance))
        {
            type = HYJ_Character_AStar_TYPE.DISTANCE;
        }
        else
        {
            type = HYJ_Character_AStar_TYPE.FIELD;
        }

        // ��ġ�� �ʱ�ȭ  //
        int tilesCount = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT);
        for (int i = 0; i < tilesCount; i++)
        {
            _tiles[i].HYJ_Data_Reset();
        }

        // �������� 0���� //
        Vector2 coordinate = new Vector2();
        // ĳ������ AStar�� ����� ������ �ڱ� �ڽ��� ��ġ�� 0���� �Ѵ�.
        if ((_tiles == AStar_tiles_distance) || (_tiles == AStar_tiles_field))
        { coordinate = (Vector2)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER, this); }
        // ���� ����� ���� ��ǥ�� �Ǵ� ���� ��ġ�� 0���� �Ѵ�.
        else if (_tiles == AStar_tiles_enemy) { coordinate = (Vector2)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER, (HYJ_Character)Action_target); }

        _tiles[(int)coordinate.y].HYJ_Data_SetTile((int)coordinate.x, 0);
        AStar_wait.Add((int)coordinate.x + "_" + (int)coordinate.y);

        // AStar_wait�� 0�� �� ������ �ݺ��Ѵ�.
        while (AStar_wait.Count > 0)
        {
            string[] strs = AStar_wait[0].Split('_');
            int whileX = int.Parse(strs[0]);
            int whileY = int.Parse(strs[1]);

            int range = _tiles[whileY].HYJ_Data_GetTile(whileX) + 1;
            int fieldX = 0;

            // ������
            {
                // Up
                int calcY = whileY - 1;
                if (calcY >= 0)
                {
                    HYJ_AStar_Calc_UpDown(
                        _tiles,
                        //
                        type, range, whileX, calcY);
                }

                // Middle
                calcY = whileY;

                int clacX = whileX - 1;
                if (clacX >= 0)
                {
                    if (range < _tiles[calcY].HYJ_Data_GetTile(clacX))
                    {
                        HYJ_AStar_Calc_Setting(
                            _tiles,
                            //
                            type, range, clacX, calcY);
                    }
                }

                clacX = whileX + 1;
                fieldX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
                if ((calcY % 2) == 1)
                {
                    fieldX += 1;
                }
                if (clacX < fieldX)
                {
                    if (range < _tiles[calcY].HYJ_Data_GetTile(clacX))
                    {
                        HYJ_AStar_Calc_Setting(
                            _tiles,
                            //
                            type, range, clacX, calcY);
                    }
                }

                // Down
                calcY = whileY + 1;
                if (calcY < (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y))
                {
                    HYJ_AStar_Calc_UpDown(
                        _tiles,
                        //
                        type, range, whileX, calcY);
                }
            }

            // ����� ���� Ÿ���� �����Ѵ�.
            AStar_wait.RemoveAt(0);
        }
    }

    // �������� ��, �Ʒ� ���
    // Y�� ���� Ȧ���� ���� ¦���� ���� ���� ����� �޶����⿡ �̸� �����Ͽ� ����Ѵ�.
    void HYJ_AStar_Calc_UpDown(
        List<HYJ_Character_AStar> _tiles,
        //
        HYJ_Character_AStar_TYPE _type, int _range, int _x, int _y)
    {
        // ���� ���    //
        int clacX = _x;
        if ((_y % 2) == 0)
        {
            clacX -= 1;
        }
        if (clacX >= 0)
        {
            if (_range < _tiles[_y].HYJ_Data_GetTile(clacX))
            {
                HYJ_AStar_Calc_Setting(
                    _tiles,
                    _type, _range, clacX, _y);
            }
        }

        // ������ ���   //
        clacX = _x + 1;
        if ((_y % 2) == 0)
        {
            clacX -= 1;
        }
        // �ش� ������ �ִ�X���� ã�Ƴ���.
        int fieldX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
        if ((_y % 2) == 1)
        {
            fieldX += 1;
        }
        // ���
        if (clacX < fieldX)
        {
            if (_range < _tiles[_y].HYJ_Data_GetTile(clacX))
            {
                HYJ_AStar_Calc_Setting(
                    _tiles,
                    _type, _range, clacX, _y);
            }
        }
    }

    void HYJ_AStar_Calc_Setting(
        List<HYJ_Character_AStar> _tiles,
        //
        HYJ_Character_AStar_TYPE _type, int _range, int _x, int _y)
    {
        bool isDoing = true;

        // ��ֹ��� ã�ƾ��� �� Ÿ�� ���� ������Ʈ�� �ִٸ� ��꿡�� �����Ѵ�.
        if(_type == HYJ_Character_AStar_TYPE.FIELD)
        {
            HYJ_Battle_Tile element
                = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    //
                    HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE,
                    //
                    _x, _y);

            if(element.HYJ_Basic_onUnit != null)
            {
                isDoing = false;
            }
        }

        if (isDoing)
        {
            // �Ÿ����� �Է��ϰ�
            // ����� Ÿ�Ϸ� �Է��Ѵ�.
            _tiles[_y].HYJ_Data_SetTile(_x, _range);
            AStar_wait.Add(_x + "_" + _y);
        }
    }

    //////////  Default Method  //////////
    void HYJ_Astar_Init()
    {
        AStar_tiles_distance = new List<HYJ_Character_AStar>();
        HYJ_Astar_Init_AStar(AStar_tiles_distance);
        AStar_tiles_field = new List<HYJ_Character_AStar>();
        HYJ_Astar_Init_AStar(AStar_tiles_field);
        AStar_tiles_enemy = new List<HYJ_Character_AStar>();
        HYJ_Astar_Init_AStar(AStar_tiles_enemy);

        AStar_wait = new List<string>();
    }

    void HYJ_Astar_Init_AStar(List<HYJ_Character_AStar> _tiles)
    {
        int tilesCount = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT);
        for (int i = 0; i < tilesCount; i++)
        {
            _tiles.Add(new HYJ_Character_AStar((int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT, i)));
        }
    }
}

#endregion

#region ACTION

public enum HYJ_Character_COMMAND
{
    AUTO,   // �ڵ�

    //
    ATTACK  // ������
}

public enum HYJ_Character_ACTION
{
    IDLE,   // ���
    //
    WALK_START,     // �̵��غ�
    WALK_SETTING,   //
    WALK,           // �̵�
    //
    ATTACK  // ����
}

partial class HYJ_Character
{
    [Header("==================================================")]
    [Header("ACTION")]
    [SerializeField] HYJ_Character_COMMAND Action_command;
    [SerializeField] HYJ_Character_ACTION Action_action;
    [SerializeField] object Action_target;

    //
    Vector3 Action_moveStartPos;
    Vector3 Action_moveArrivePos;
    float Action_moveTimer;
    float Action_moveTimerMax;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_Action_Init()
    {
        Action_command = HYJ_Character_COMMAND.AUTO;
        Action_action = HYJ_Character_ACTION.IDLE;
    }

    void HYJ_Action_Update()
    {
        switch(Action_action)
        {
            case HYJ_Character_ACTION.IDLE:         HYJ_Action_Update_ACTION_IDLE();    break;
            // WALK
            case HYJ_Character_ACTION.WALK_START:   HYJ_Action_Update_ACTION_WALK_START();      break;
            case HYJ_Character_ACTION.WALK_SETTING: HYJ_Action_Update_ACTION_WALK_SETTING();    break;
            case HYJ_Character_ACTION.WALK:         HYJ_Action_Update_ACTION_WALK();            break;
            // ATTACK
            case HYJ_Character_ACTION.ATTACK:       HYJ_Action_Update_ACTION_ATTACK();  break;
        }
    }

    // ������� ��
    void HYJ_Action_Update_ACTION_IDLE()
    {
        switch (Action_command)
        {
            case HYJ_Character_COMMAND.AUTO:
                {
                    HYJ_AStar_Calc(AStar_tiles_distance);

                    //
                    HYJ_Character targetObj = null;
                    int targetRange = 10000;

                    // ���� ����� ���� ã��
                    {
                        int tileY = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT);
                        for (int y = 0; y < tileY; y++)
                        {
                            int tileX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT, y);
                            for (int x = 0; x < tileX; x++)
                            {
                                HYJ_Character element
                                    = (HYJ_Character)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                                        //
                                        HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_CHARACTER,
                                        //
                                        x, y);

                                if ((element != null) && (Info_faction != element.HYJ_Info_faction))
                                {
                                    int astarRange = AStar_tiles_distance[y].HYJ_Data_GetTile(x);
                                    if (targetRange > astarRange)
                                    {
                                        targetObj = element;
                                        targetRange = astarRange;
                                    }
                                }
                            }
                        }
                    }

                    // ã�� ���� �������� �� ���� ������
                    // ���������� ����� �����ؼ� �밭 1�� �մϴ�.
                    // ��Ÿ� �ȿ� ������ �ѱ��м���.
                    Action_target = targetObj;
                    if (targetRange <= 1)
                    {
                        Action_action = HYJ_Character_ACTION.ATTACK;
                    }
                    // ��Ÿ� �ۿ� ������ �ִ� �Ÿ��� ã�ư��ô�.
                    else
                    {
                        Action_action = HYJ_Character_ACTION.WALK_START;
                    }
                }
                break;
        }
    }

    // WALK
    void HYJ_Action_Update_ACTION_WALK_START()
    {
        // x, y ��ǥ
        // z    ��Ÿ�
        Vector3 targetPos = new Vector3(10000, 10000, 10000);

        // �� �̵� ���
        HYJ_AStar_Calc(AStar_tiles_field);

        // �� �Ÿ� ���
        HYJ_AStar_Calc(AStar_tiles_enemy);

        // �ִ� �Ÿ� ���
        int tileY = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT);
        for (int y = 0; y < tileY; y++)
        {
            int tileX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT, y);
            for (int x = 0; x < tileX; x++)
            {
                if ((AStar_tiles_field[y].HYJ_Data_GetTile(x) == 1) && (AStar_tiles_enemy[y].HYJ_Data_GetTile(x) < targetPos.z))
                {
                    targetPos.x = x;
                    targetPos.y = y;
                    targetPos.z = AStar_tiles_enemy[y].HYJ_Data_GetTile(x);
                }
            }
        }

        //
        Action_target = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE,
            //
            (int)targetPos.x, (int)targetPos.y);

        // ĳ������ Ÿ�� ��ġ ����
        HYJ_Battle_Tile tile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER, this);
        tile.HYJ_Basic_onUnit = null;

        tile = (HYJ_Battle_Tile)Action_target;
        tile.HYJ_Basic_onUnit = this;

        Action_action = HYJ_Character_ACTION.WALK_SETTING;
    }

    void HYJ_Action_Update_ACTION_WALK_SETTING()
    {
        //
        Action_moveStartPos = this.transform.localPosition;
        Action_moveArrivePos = ((HYJ_Battle_Tile)Action_target).transform.localPosition;

        // Ÿ�̸� ����
        Action_moveTimer = 0.0f;

        float distance = Mathf.Sqrt(MathF.Pow(Action_moveStartPos.x - Action_moveArrivePos.x, 2) + MathF.Pow(Action_moveStartPos.y - Action_moveArrivePos.y, 2) + MathF.Pow(Action_moveStartPos.z - Action_moveArrivePos.z, 2));
        Action_moveTimerMax = distance / 1.0f;

        //
        Action_action = HYJ_Character_ACTION.WALK;
    }

    void HYJ_Action_Update_ACTION_WALK()
    {
        Action_moveTimer += Time.deltaTime;

        if (Action_moveTimer >= Action_moveTimerMax)
        {
            this.transform.localPosition = Action_moveArrivePos;
            Action_action = HYJ_Character_ACTION.IDLE;
        }
        else
        {
            this.transform.localPosition = Vector3.Lerp(Action_moveStartPos, Action_moveArrivePos, Action_moveTimer / Action_moveTimerMax);
        }
    }

    // ATTACK
    void HYJ_Action_Update_ACTION_ATTACK()
    {
    }
}

#endregion
