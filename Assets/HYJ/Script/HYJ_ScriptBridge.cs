using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 모든 외부접근 메서드는 이곳에서 관리합니다.
// 델리게이트를 통하여 모든 클래스에서 등록 및 접근이 가능합니다.
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

// 싱글턴
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

// 명령어 모음
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
    /// 전투에서 유닛을 구매한다.
    /// </summary>
    PLAYER___UNIT__BUY_FROM_BATTLE,
    PLAYER___UNIT__DATA_UPDATE,
    PLAYER___UNIT__GET_PLAYER_UNIT_DATABASE,
    PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE,

    /// <summary>
    /// 보유하고 있는 유물의 리스트를 반환한다.
    /// </summary>
    PLAYER___ITEM__GET_RELICS,
    /// <summary>
    /// 장비하고 있는 유물의 리스트를 반환한다.
    /// </summary>
    PLAYER___ITEM__GET_RELICS_EQUIP,
    PLAYER___ITEM__INSERT,
    /// <summary>
    /// 아이템을 장비한다.<br/>
    /// 배개변수 : int - 인벤토리 번호, int - 장비 번호
    /// </summary>
    PLAYER___ITEM__EQUIP,

    PLAYER___BUFF__SETTING,

    PLAYER___BUFF__GET_TOTAL_BUFFS,

    PLAYER___BUFF__GET_DEBUFF_FROM_COUNT,
    PLAYER___BUFF__GET_DEBUFF_COUNT,

    /// <summary>
    /// 스테이지가 종료될 때마다, 버프/디버프의 타이머를 1 줄이고 타이머가 끝난 버프를 날려준다.
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
    /// 지도 저장정보를 갱신.<br/>
    /// 매개변수 : bool - 정보 삭제 여부
    /// </summary>
    PLAYER___MAP__MAP_SETTING,

    /// <summary>
    /// 플레이어 데이터를 저장한다.
    /// </summary>
    PLAYER___FILE__SAVE,

    //
    /// <summary>
    /// 스테이지 레벨을 가져온다.
    /// </summary>
    MAP___CHEAPTER__GET_LEVEL,
    /// <summary>
    /// 스테이지 리스트를 받아온다.
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
    /// 이벤트 데이터 리스트를 가져온다.
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
    /// 플레이어 데이터를 바탕으로 필드의 유닛들을 업데이트
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
    /// 로비를 활성화한다.<b/r>
    /// 매개변수 : bool - 활성화 여부
    /// </summary>
    LOBBY___BASIC__ACTIVE_ON
}

public delegate object HYJ_ScriptBridge_Event(params object[] _args);

partial class HYJ_ScriptBridge
{

    Dictionary<HYJ_ScriptBridge_EVENT_TYPE, HYJ_ScriptBridge_Event> Event_events;

    //////////  Getter & Setter //////////

    // 등록된 메서드를 가져올 때 사용
    public object HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE _type, params object[] _args)
    {
        object res = null;

        if (Event_events.ContainsKey(_type))
        {
            res = Event_events[_type].Invoke(_args);
        }

        return res;
    }

    // 메서드를 등록할 때 사용
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