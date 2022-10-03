using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

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

// 캐릭터의 정보
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
    [SerializeField] HYJ_Character_FACTION Info_faction;    // 캐릭터가 속한 진영
    [SerializeField] int Info_DBNumber;                     // 캐릭터의 DB 번호

    //////////  Getter & Setter //////////
    public HYJ_Character_FACTION HYJ_Info_faction
    {
        get { return Info_faction; }
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
}

#endregion

// 캐릭터의 능력치
// 버프나 아이템, 전투 등의 외부요소로 인해 변하는 수치들을 적재해주기 위해 사용되는 변수들
#region STATUS

partial class HYJ_Character
{
    [Header("==================================================")]
    [Header("STATUS")]
    [SerializeField] int Status_HP;     // 체력
    [SerializeField] int Status_MaxHP;  // 최대체력
    [SerializeField] int Status_MP;     // 마나
    [SerializeField] int Status_MaxMP;  // 최대마나

    [SerializeField] int Status_atk;    // 공격력
    [SerializeField] int Status_magic;  // 마력

    [SerializeField] int Status_atkSpeed;   // 공속

    [SerializeField] int Status_critValue;  // 치명타 수치
    [SerializeField] int Status_critPer;    // 치명타 확률

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
}

#endregion

// AStar관련
#region ASTAR

public enum HYJ_Character_AStar_TYPE
{
    DISTANCE,   // 거리만 특정할 때
    FIELD       // 지도 위를 측정할 때
}

// AStar의 정보를 담은 클래스
[Serializable]
public class HYJ_Character_AStar
{
    [SerializeField] List<int> tiles;   // 거리값

    //////////  Getter & Setter //////////
    public int HYJ_Data_GetTile(int _count) { return tiles[_count]; }   // 거리값을 받아온다.
    public void HYJ_Data_SetTile(int _count, int _value) { tiles[_count] = _value; }    // 거리값을 입력한다.

    //
    public int HYJ_Data_GetTileCount() { return tiles.Count; }  // 해당 Y라인의 X갯수를 구한다.

    //////////  Method          //////////
    // 계산 시작 전 초기화
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
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_distance;    // 단순히 거리만 계산하는 AStar
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_field;       // 필드 내에 장애물까지 계산에 넣은 AStar
    [SerializeField] List<HYJ_Character_AStar> AStar_tiles_enemy;       // 적을 기준으로 계산한 AStar(필드 기준)
    List<string> AStar_wait;    // 앞으로 계산해야하는 타일의 번호모음

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    void HYJ_AStar_Calc(
        List<HYJ_Character_AStar> _tiles)
    {
        // 계산을 위한 AStar의 종류에 따라 계산할 방식을 정한다.    //
        HYJ_Character_AStar_TYPE type = HYJ_Character_AStar_TYPE.DISTANCE;

        if((_tiles == AStar_tiles_distance))
        {
            type = HYJ_Character_AStar_TYPE.DISTANCE;
        }
        else
        {
            type = HYJ_Character_AStar_TYPE.FIELD;
        }

        // 수치를 초기화  //
        int tilesCount = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT);
        for (int i = 0; i < tilesCount; i++)
        {
            _tiles[i].HYJ_Data_Reset();
        }

        // 시작점을 0으로 //
        Vector2 coordinate = new Vector2();
        // 캐릭터의 AStar를 계산할 때에는 자기 자신의 위치를 0으로 한다.
        if ((_tiles == AStar_tiles_distance) || (_tiles == AStar_tiles_field))
        { coordinate = (Vector2)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER, this); }
        // 적을 계산할 때는 목표가 되는 적의 위치를 0으로 한다.
        else if (_tiles == AStar_tiles_enemy) { coordinate = (Vector2)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER, (HYJ_Character)Action_target); }

        _tiles[(int)coordinate.y].HYJ_Data_SetTile((int)coordinate.x, 0);
        AStar_wait.Add((int)coordinate.x + "_" + (int)coordinate.y);

        // AStar_wait이 0이 될 때까지 반복한다.
        while (AStar_wait.Count > 0)
        {
            string[] strs = AStar_wait[0].Split('_');
            int whileX = int.Parse(strs[0]);
            int whileY = int.Parse(strs[1]);

            int range = _tiles[whileY].HYJ_Data_GetTile(whileX) + 1;
            int fieldX = 0;

            // 계산시작
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

            // 계산이 끝난 타일을 제거한다.
            AStar_wait.RemoveAt(0);
        }
    }

    // 기준점의 위, 아래 계산
    // Y의 값이 홀수일 때와 짝수일 때에 따라 계산이 달라지기에 이를 보정하여 계산한다.
    void HYJ_AStar_Calc_UpDown(
        List<HYJ_Character_AStar> _tiles,
        //
        HYJ_Character_AStar_TYPE _type, int _range, int _x, int _y)
    {
        // 왼쪽 계산    //
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

        // 오른쪽 계산   //
        clacX = _x + 1;
        if ((_y % 2) == 0)
        {
            clacX -= 1;
        }
        // 해당 라인의 최대X값을 찾아낸다.
        int fieldX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
        if ((_y % 2) == 1)
        {
            fieldX += 1;
        }
        // 계산
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

        // 장애물을 찾아야할 때 타일 위에 오브젝트가 있다면 계산에서 제외한다.
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
            // 거리값을 입력하고
            // 계산할 타일로 입력한다.
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
    AUTO,   // 자동

    //
    ATTACK  // 일점사
}

public enum HYJ_Character_ACTION
{
    IDLE,   // 대기
    //
    WALK_START,     // 이동준비
    WALK_SETTING,   //
    WALK,           // 이동
    //
    ATTACK  // 공격
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

    // 대기중일 때
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

                    // 가장 가까운 적을 찾자
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

                    // 찾은 적을 바탕으로 뭘 할지 정하자
                    // 무기정보를 우얄지 안정해서 대강 1로 합니다.
                    // 사거리 안에 있으면 뚜까패세요.
                    Action_target = targetObj;
                    if (targetRange <= 1)
                    {
                        Action_action = HYJ_Character_ACTION.ATTACK;
                    }
                    // 사거리 밖에 있으면 최단 거리로 찾아갑시다.
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
        // x, y 좌표
        // z    사거리
        Vector3 targetPos = new Vector3(10000, 10000, 10000);

        // 내 이동 계산
        HYJ_AStar_Calc(AStar_tiles_field);

        // 적 거리 계산
        HYJ_AStar_Calc(AStar_tiles_enemy);

        // 최단 거리 계산
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

        // 캐릭터의 타일 위치 변경
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

        // 타이머 세팅
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