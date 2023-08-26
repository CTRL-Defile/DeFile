using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� �ܺ����� �޼���� �̰����� �����մϴ�.
// ��������Ʈ�� ���Ͽ� ��� Ŭ�������� ��� �� ������ �����մϴ�.
public partial class HYJ_ScriptBridge
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    public HYJ_ScriptBridge()
    {
        HYJ_Event_Start();
    }

}

// �̱���
partial class HYJ_ScriptBridge
{
    static HYJ_ScriptBridge Static_instance;

    //////////  Getter & Setter //////////
    public static HYJ_ScriptBridge HYJ_Static_instance
    {
        get
        {
            if (Static_instance == null)
            {
                Static_instance = new HYJ_ScriptBridge();
            }
            return Static_instance;
        }
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    //void HYJ_Static_Start()
    //{
    //    Static_instance = this;
    //}

}

// ��ɾ� ����
public enum HYJ_ScriptBridge_EVENT_TYPE
{
    MASTER___SCENE__CHANGE,

    MASTER___UI__GET_CAMERA,

    //
    PLAYER___BASIC__GET_UPDATE_PHASE,

    PLAYER___BASIC__GET_GOLD,
    PLAYER___BASIC__GOLD_PLUS,
    PLAYER___BASIC__GOLD_MINUS,
    PLAYER___BASIC__GOLD_IS_ENOUGH,
    PLAYER___BASIC__GOLD_INTEREST,

    PLAYER___BASIC__EXP_INCREASE,
    PLAYER___BASIC__EXP_DECREASE,
    PLAYER___BASIC__GET_EXP,
    PLAYER___BASIC__LEVEL_INCREASE,
    PLAYER___BASIC__GET_LEVEL,

    PLAYER___BASIC__MAX_HP,
    PLAYER___BASIC__CURRENT_HP,
    PLAYER___BASIC__HP_INCREASE,

    PLAYER___UNIT__GET_BUY_UNIT_DATA,
    PLAYER___UNIT__GET_BUY_UNIT_COUNT,
    PLAYER___UNIT__GET_WAIT_UNIT_DATA,
    PLAYER___UNIT__GET_FIELD_UNIT_DATA,
    /// <summary>
    /// �������� ������ �����Ѵ�.
    /// </summary>
    PLAYER___UNIT__BUY_FROM_BATTLE,
    PLAYER___UNIT__DATA_UPDATE,
    PLAYER___UNIT__GET_PLAYER_UNIT_DATABASE,
    PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE,

    /// <summary>
    /// �����ϰ� �ִ� ������ ����Ʈ�� ��ȯ�Ѵ�.
    /// </summary>
    PLAYER___ITEM__GET_RELICS,
    /// <summary>
    /// ����ϰ� �ִ� ������ ����Ʈ�� ��ȯ�Ѵ�.
    /// </summary>
    PLAYER___ITEM__GET_RELICS_EQUIP,
    PLAYER___ITEM__INSERT,
    /// <summary>
    /// �������� ����Ѵ�.<br/>
    /// �谳���� : int - �κ��丮 ��ȣ, int - ��� ��ȣ
    /// </summary>
    PLAYER___ITEM__EQUIP,

    PLAYER___BUFF__SETTING,

    PLAYER___BUFF__GET_TOTAL_BUFFS,

    PLAYER___BUFF__GET_DEBUFF_FROM_COUNT,
    PLAYER___BUFF__GET_DEBUFF_COUNT,

    /// <summary>
    /// ���������� ����� ������, ����/������� Ÿ�̸Ӹ� 1 ���̰� Ÿ�̸Ӱ� ���� ������ �����ش�.
    /// </summary>
    PLAYER___BUFF__END_STAGE,
    PLAYER___BUFF__UNIT_BUFFS,
    PLAYER___BUFF__INSERT_BY_EVENT,

    PLAYER___REPUTATION__GET_VALUE,
    //PLAYER___REPUTATION__PLUS_VALUE,
    //PLAYER___REPUTATION__MINUS_VALUE,

    PLAYER___MAP__GET_LEVEL,
    PLAYER___MAP__GET_PLAYER_POS,
    PLAYER___MAP__SET_PLAYER_POS,
    PLAYER___MAP__GET_MAP_DATAS,
    PLAYER___MAP__GET_STAGE,
    PLAYER___MAP__SET_STAGE,
    PLAYER___MAP__GET_STAGE_DATAS,
    /// <summary>
    /// ���� ���������� ����.<br/>
    /// �Ű����� : bool - ���� ���� ����
    /// </summary>
    PLAYER___MAP__MAP_SETTING,

    /// <summary>
    /// �÷��̾� �����͸� �����Ѵ�.
    /// </summary>
    PLAYER___FILE__SAVE,

    //
    /// <summary>
    /// �������� ������ �����´�.
    /// </summary>
    MAP___CHEAPTER__GET_LEVEL,
    /// <summary>
    /// �������� ����Ʈ�� �޾ƿ´�.
    /// </summary>
    MAP___CHEAPTER__GET_STAGES,
    MAP___CHEAPTER__SELECT_RESET,
    MAP___CHEAPTER__MOVE_CENTER,
    MAP___CHANGE__PLAYER_POSITION,
    MAP___ACTIVE__ACTIVE_ON,
    MAP___GET__SELECTED_ROAD,
    MAP___GET__BLACKSCREEN,
    MAP___GET__LOCATIONMARKER,

    //
    BASE_CAMP___ACTIVE__ACTIVE_ON,

    //
    EVENT___ACTIVE__ACTIVE_ON,
    /// <summary>
    /// �̺�Ʈ ������ ����Ʈ�� �����´�.
    /// </summary>
    EVENT___DATA__GET_LIST,
    EVENT___DATA__GET_SELECTED_EVENT,
    EVENT___DATA__GET_ALL_SELECTED_EVENT,

    //
    SHOP___ACTIVE__ACTIVE_ON,
    SHOP___RELIC__BUY,
    SHOP___UNIT__BUY,
    SHOP___POTION__BUY,
    SHOP___BUFF__BUY,
    SHOP___DEBUFF__BUY,

    //
    BATTLE___BASIC__GET_PHASE,
    BATTLE___BASIC__GET_STAGE_TYPE,
    BATTLE___BASIC__SET_STAGE_TYPE,
    BATTLE___BASIC__GET_IS_WIN,
    BATTLE___ACTIVE__ACTIVE_ON,
    BATTLE___ACTIVE__SHOP_UI,

    /// <summary>
    /// �÷��̾� �����͸� �������� �ʵ��� ���ֵ��� ������Ʈ
    /// </summary>
    BATTLE___FIELD__CHARACTER_FIXED,
	BATTLE___FIELD_GRAPH_NODE_INIT,
	BATTLE___FIELD_GET_GRAPH,
	BATTLE___FIELD_GET_TILES,
    BATTLE___FIELD__GET_FIELD_X,
    BATTLE___FIELD__GET_FIELD_Y,
    BATTLE___FIELD__GET_STAND_TILES,
    BATTLE___FIELD__GET_STAND_X,
    BATTLE___FIELD__GET_TILE,
    BATTLE___FIELD__GET_TILE_FROM_CHARACTER,
    BATTLE___FIELD__GET_CHARACTER,
    BATTLE___FIELD__GET_TILES_COUNT,
    BATTLE___FIELD__GET_TILES_GET_COUNT,
    BATTLE___FIELD__GET_XY_FROM_CHARACTER,
    BATTLE___FIELD__COUNT_ALLY_ONTILE,
	BATTLE___FIELD__GET_TILE_IN_GRAPH,

    BATTLE__SYNERGY_UPDATE,

    BATTLE___UNIT__STAND_TO_FIELD,
    BATTLE___UNIT__FIELD_TO_STAND,
    BATTLE___UNIT__TO_TRASH,
    BATTLE___UNIT__TO_SACRIFICED,
    BATTLE___UNIT__SACRIFICED_TO_POOL,
    BATTLE___UNIT_DIE,
    BATTLE___UNIT__FIND_FIELD,
    BATTLE___UNIT__FIND_STAND,
    BATTLE___UNIT__GET_FIELD_UNIT,
    BATTLE___UNIT__GET_STAND_UNIT,
    BATTLE___UNIT__GET_ENEMY_UNIT,
    BATTLE___UNIT__GET_SACRIFICED_UNIT,

    BATTLE___COUNT__FIELD_UNIT,

    //
    DATABASE___BASIC__GET_IS_INITIALIZE,

    DATABASE___RELIC__GET_DATA_COUNT,
    DATABASE___RELIC__GET_DATA_NAME,
    DATABASE___RELIC__GET_DATA_FROM_NAME,

    DATABASE___POTION__GET_DATA_FROM_NAME,

    DATABASE___UNIT__GET_PHASE,
    DATABASE___UNIT__GET_UNIT_PREFAB,
    DATABASE___UNIT__GET_DATA_COUNT,
    DATABASE___UNIT__GET_DATABASE_CSV,
    DATABASE___UNIT__GET_DATA_FROM_ID,
    DATABASE___UNIT__GET_DATA_NAME,

    DATABASE___SKILL__GET_DATA,

    DATABASE___BUFF__GET_DATA,
    DATABASE___BUFF__GET_COUNT,
    DATABASE___BUFF__GET_KEYS,

    DATABASE___DEBUFF__GET_DATA,
    DATABASE___DEBUFF__GET_COUNT,
    DATABASE___DEBUFF__GET_KEYS,

    //
    DRAG___UNIT__SET_POSITION,
    DRAG___UNIT__SET_ORIGINAL,
    DRAG___INIT,

    //
    TOPBAR___HP__VIEW_HP,
    TOPBAR___LEVEL__VIEW_LEVEL,
    TOPBAR___GOLD__VIEW_GOLD,
    TOPBAR___BATTLE__VIEW_POWER,
    TOPBAR___BUFF__VIEW,

    //
    SOUNDMANAGER___PLAY__BGM_NAME,
    SOUNDMANAGER___PLAY__SFX_NAME,
    SOUNDMANAGER___PLAY__SFX_STOP,
	SOUNDMANAGER___PLAY__BGM_STOP,

	SOUNDMANAGER___PLAY__SFX_ISPLAYING,

    //
    /// <summary>
    /// �κ� Ȱ��ȭ�Ѵ�.<b/r>
    /// �Ű����� : bool - Ȱ��ȭ ����
    /// </summary>
    LOBBY___BASIC__ACTIVE_ON
}

public delegate object HYJ_ScriptBridge_Event(params object[] _args);

partial class HYJ_ScriptBridge
{

    Dictionary<HYJ_ScriptBridge_EVENT_TYPE, HYJ_ScriptBridge_Event> Event_events;

    //////////  Getter & Setter //////////

    // ��ϵ� �޼��带 ������ �� ���
    public object HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE _type, params object[] _args)
    {
        object res = null;

        if (Event_events.ContainsKey(_type))
        {
            res = Event_events[_type].Invoke(_args);
        }

        return res;
    }

    // �޼��带 ����� �� ���
    public void HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE _type, HYJ_ScriptBridge_Event _event)
    {
        if(Event_events.ContainsKey(_type))
        {
            Event_events[_type] = _event;
        }
        else
        {
            Event_events.Add(_type, _event);
        }
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_Event_Start()
    {
        Event_events = new Dictionary<HYJ_ScriptBridge_EVENT_TYPE, HYJ_ScriptBridge_Event>();
    }

}