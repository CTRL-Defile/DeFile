using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

#region HYJ_CharacterSkill

public enum CharacterSkill_SLOT
{
    Basic,
    Skill
}

public enum CharacterSkill_ATTACK_TYPE
{
    Atk,
    Spell
}

public enum CharacterSkill_BUFF
{
    None
}

public enum CharacterSkill_IMPOSSIBLE_TAG
{
    NONE,

    CHAIN,
    PIERCING,
    DIVIDE
}

public enum CharacterSkill_SKILL_ACTIVE
{
    ACTIVE,
    PASSIVE
}

public enum CharacterSkill_SEARCH_TEAM
{
    ALLY,
    ENEMY
}

public enum CharacterSkill_SEARCH_CONDITION
{
    None
}

//
public enum CharacterSkill_AOE_FIGURE
{
    NONE,
    CIRCLE,
    SECTOR,
    SQUARE,
    BULLET_SECTOR,
    BULLET_SQUARE
}

//
public enum CharacterSkill_TARGET_RACE
{
    ALLY,
    ENEMY
}

public enum CharacterSkill_TARGET_CONDITION
{
    None
}

//
public enum CharacterSkill_DAMAGE_TYPE
{
    PHYSICS,
    MAGIC,
    DOT,
    FIX
}

public enum CharacterSkill_DAMAGE_STATUS
{
    HP,
    DAMAGE_PHYSICS,
    DAMAGE_MAGIC,
    ATTACK_SPEED,
    DEFENCE_PHYSICS,
    DEFENCE_MAGIC,
    CRITICAL_PERCENT,
    CRITICAL_VALUE
}

//
public class HYJ_CharacterSkill :IDisposable
{
    [SerializeField] int    Data_ID;
    [SerializeField] string Data_name;
    //[SerializeField] string Data_nameKor;

    CharacterSkill_SLOT Data_slot;  // 기본, 스킬 여부

    //
    CharacterSkill_ATTACK_TYPE Data_attackType;
    CharacterSkill_BUFF Data_buff;
    //CharacterSkill_IMPOSSIBLE_TAG Data_impossibleTags;
    string Data_impossibleTags;
    int Data_skillActive;
    int Data_searchTeam;
    float Data_searchRange;
    CharacterSkill_SEARCH_CONDITION Data_searchCondition;

    //
    [SerializeField] float  Data_range;
    [SerializeField] float  Data_delayPre;      // 선딜
    [SerializeField] float  Data_delayAfter;    // 후딜

    [SerializeField] int   Data_isTargeting;   // 타게팅여부
    [SerializeField] float  Data_duration;      // 지속시간
    [SerializeField] float  Data_repeatCycle;   // 반복주기

    //
    int Data_aoeFigure;
    [SerializeField] float                          Data_aoeDegree;
    [SerializeField] float                          Data_aoeArea;

    //
    int Data_targetRace;
    [SerializeField] int                                 Data_targetMax;
    [SerializeField] CharacterSkill_TARGET_CONDITION     Data_targetCondition;

    //
    int Data_damageType;
    [SerializeField] float                              Data_damageValue;
    int Data_damageStatus;
    [SerializeField] float                              Data_damageRatio;

    //
    [SerializeField] int Data_chain;
    [SerializeField] int Data_piercing;
    [SerializeField] int Data_divide;

    //
    [SerializeField] List<HYJ_CharacterSkillEffect> Data_effects;

    //////////  Getter & Setter //////////
    //
    /// <summary>ID</summary>
    public int      HYJ_Data_ID         { get { return Data_ID; } }
    /// <summary>이름</summary>
    public string   HYJ_Data_name       { get { return Data_name; } }
    /// <summary>한글 이름</summary>
    //public string   HYJ_Data_nameKor    { get { return Data_nameKor; } }

    //
    /// <summary>기본, 스킬 여부</summary>
    public CharacterSkill_SLOT HYJ_Data_slot { get { return Data_slot; } }

    //
    /// <summary>공격 타입</summary>
    public CharacterSkill_ATTACK_TYPE       HYJ_Data_attackType         { get { return Data_attackType; } }
    /// <summary>버프 적용 여부</summary>
    public CharacterSkill_BUFF Get_Data_buff { get { return Data_buff; } }
    // Data_impossibleTags
    /// <summary>불가능 태그</summary>
    //public HYJ_CharacterSkill_IMPOSSIBLE_TAG    HYJ_Data_GetImpossibleTag(int _num) { return Data_impossibleTags[_num]; }
    /// <summary>불가능 태그 갯수</summary>
    //public int                                  HYJ_Data_GetImpossibleTagCount() { return Data_impossibleTags.Count; }

    public string Get_Data_ImpossibleTags { get { return Data_impossibleTags; } }

    /// <summary>액티브, 패시브 여부</summary>
    public int HYJ_Data_skillActive        { get { return Data_skillActive; } }
    /// <summary>피아구분</summary>
    public int HYJ_Data_searchTeam         { get { return Data_searchTeam; } }
    /// <summary>탐색 사거리</summary>
    public float                                  HYJ_Data_searchRange        { get { return Data_searchRange; } }
    /// <summary>탐색 조건</summary>
    public CharacterSkill_SEARCH_CONDITION  HYJ_Data_searchCondition    { get { return Data_searchCondition; } }

    //
    /// <summary>사거리</summary>
    public float    HYJ_Data_range      { get { return Data_range; } }
    /// <summary>선딜레이</summary>
    public float    HYJ_Data_delayPre   { get { return Data_delayPre; } }
    /// <summary>후딜레이</summary>
    public float    HYJ_Data_delayAfter { get { return Data_delayAfter; } }

    //
    /// <summary>타겟팅</summary>
    public int     HYJ_Data_isTargeting    { get { return Data_isTargeting; } }
    /// <summary>지속시간</summary>
    public float    HYJ_Data_duration       { get { return Data_duration; } }
    /// <summary>반복주기</summary>
    public float    HYJ_Data_repeatCycle    { get { return Data_repeatCycle; } }

    //
    /// <summary>범위형태</summary>
    public int HYJ_Data_aoeFigure  { get { return Data_aoeFigure; } }
    /// <summary>범위길이</summary>
    public float                            HYJ_Data_aoeDegree  { get { return Data_aoeDegree; } }
    /// <summary>범위넓이</summary>
    public float                            HYJ_Data_aoeArea    { get { return Data_aoeArea; } }

    //
    /// <summary>효과적용진영</summary>
    public int HYJ_Data_targetRace         { get { return Data_targetRace; } }
    /// <summary>최대대상개수</summary>
    public float                                HYJ_Data_targetMax          { get { return Data_targetMax; } }
    /// <summary>효과대상조건</summary>
    public CharacterSkill_TARGET_CONDITION  HYJ_Data_targetCondition    { get { return Data_targetCondition; } }

    //
    /// <summary>피해유형</summary>
    public int HYJ_Data_damageType     { get { return Data_damageType; } }
    /// <summary>기본피해량</summary>
    public float                            HYJ_Data_damageValue    { get { return Data_damageValue; } }
    /// <summary>피해량 능력치</summary>
    public int HYJ_Data_damageStatus   { get { return Data_damageStatus; } }
    /// <summary>스킬 계수</summary>
    public float                            HYJ_Data_damageRatio    { get { return Data_damageRatio; } }

    //
    /// <summary>스킬 연쇄</summary>
    public int  HYJ_Data_chain      { get { return Data_chain; } }
    /// <summary>스킬 관통</summary>
    public int  HYJ_Data_piercing   { get { return Data_piercing; } }
    /// <summary>스킬 분리</summary>
    public int  HYJ_Data_divide     { get { return Data_divide; } }

    //////////  Method          //////////
    public void Dispose()
    {

    }

    public void HYJ_Data_AddEffect(HYJ_CharacterSkillEffect _effect)
    {
        if(Data_effects == null)
        {
            Data_effects = new List<HYJ_CharacterSkillEffect>();
        }

        Data_effects.Add(_effect);
    }

    //////////  Default Method  //////////
    public HYJ_CharacterSkill(Dictionary<string, object> _data)
    {
        Data_ID         = (int)_data["ID"];
        Data_name       = (string)_data["NAME"];
        //Data_nameKor    = (string)_data["NAME_KOR"];

        Data_slot   = (CharacterSkill_SLOT)Enum.Parse(  typeof(CharacterSkill_SLOT),    (string)_data["SLOT"]);

        Data_attackType         = (CharacterSkill_ATTACK_TYPE)Enum.Parse(       typeof(CharacterSkill_ATTACK_TYPE),         (string)_data["ATTACK_TYPE"]        );
        Data_buff = (CharacterSkill_BUFF)Enum.Parse(              typeof(CharacterSkill_BUFF),                (string)_data["BUFF"]               );

        //Data_impossibleTags = new List<HYJ_CharacterSkill_IMPOSSIBLE_TAG>();
        //string[] impossibleTags_Strs = ((string)_data["IMPOSSIBLE_TAG"]).Split('|');
        //for(int i = 0; i < impossibleTags_Strs.Length; i++)
        //{
        //    Data_impossibleTags.Add((HYJ_CharacterSkill_IMPOSSIBLE_TAG)Enum.Parse(typeof(HYJ_CharacterSkill_IMPOSSIBLE_TAG), impossibleTags_Strs[i]));
        //}

        Data_impossibleTags = (string)_data["IMPOSSIBLE_TAG"];

        //Data_skillActive        = (CharacterSkill_SKILL_ACTIVE)Enum.Parse(      typeof(CharacterSkill_SKILL_ACTIVE),        (string)_data["SKILL_ACTIVE"]       );
        Data_skillActive = (int)_data["SKILL_ACTIVE"];
        //Data_searchTeam         = (CharacterSkill_SEARCH_TEAM)Enum.Parse(       typeof(CharacterSkill_SEARCH_TEAM),         (string)_data["SEARCH_TEAM"]        );
        Data_searchTeam = (int)_data["SEARCH_TEAM"];
        Data_searchRange        = (float)_data["SEARCH_RANGE"];
        Data_searchCondition    = (CharacterSkill_SEARCH_CONDITION)Enum.Parse(  typeof(CharacterSkill_SEARCH_CONDITION),    (string)_data["SEARCH_CONDITION"]   );

        Data_range      = (float)_data["RANGE"];
        Data_delayPre   = (float)_data["DELAY_PRE"];
        Data_delayAfter = (float)_data["DELAY_AFTER"];

        Data_isTargeting    = (int)_data["TARGETING"];
        Data_duration       = (float)_data["DURATION"];
        Data_repeatCycle    = (float)_data["REPEAT_CYCLE"];

        //Data_aoeFigure  = (HYJ_CharacterSkill_AOE_FIGURE)Enum.Parse(    typeof(HYJ_CharacterSkill_AOE_FIGURE),  (string)_data["AOE_FIGURE"]);
        Data_aoeFigure = (int)_data["AOE_FIGURE"];
        Data_aoeDegree  = (float)_data["AOE_DEGREE"];
        Data_aoeArea    = (float)_data["AOE_AREA"];

        //Data_targetRace         = (HYJ_CharacterSkill_TARGET_RACE)Enum.Parse(       typeof(HYJ_CharacterSkill_TARGET_RACE),         (string)_data["TARGET_RACE"]);
        Data_targetRace = (int)_data["TARGET_RACE"];
        Data_targetMax          = (int)_data["ID"];
        Data_targetCondition    = (CharacterSkill_TARGET_CONDITION)Enum.Parse(  typeof(CharacterSkill_TARGET_CONDITION),    (string)_data["TARGET_CONDITION"]);

        //Data_damageType     = (HYJ_CharacterSkill_DAMAGE_TYPE)Enum.Parse(   typeof(HYJ_CharacterSkill_DAMAGE_TYPE),     (string)_data["DAMAGE_TYPE"]);
        Data_damageType = (int)_data["DAMAGE_TYPE"];

        Data_damageValue = (float)_data["DAMAGE_VALUE"];
        //Data_damageStatus   = (HYJ_CharacterSkill_DAMAGE_STATUS)Enum.Parse( typeof(HYJ_CharacterSkill_DAMAGE_STATUS),   (string)_data["DAMAGE_STATUS"]);
        Data_damageStatus = (int)_data["DAMAGE_STATUS"];
        Data_damageRatio    = (float)_data["DAMAGE_RATIO"];

        Data_chain      = (int)_data["CHAIN"];
        Data_piercing   = (int)_data["PIERCING"];
        Data_divide     = (int)_data["DIVIDE"];
    }
}

#endregion

#region HYJ_CharacterSkillEffect

//
public enum HYJ_CharacterSkillEffect_EFFECT_TYPE
{
    NONE
}

public enum HYJ_CharacterSkillEffect_EFFECT_TRIGGER
{
    NONE
}

public enum HYJ_CharacterSkillEffect_EFFECT_STATUS
{
    NONE
}

//
public class HYJ_CharacterSkillEffect : IDisposable
{
    //
    /// <summary>ID</summary>
    [SerializeField] int Data_id;
    /// <summary>SKILL_ID</summary>
    [SerializeField] int Data_skillId;

    //
    /// <summary>효과유형</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_TYPE       Data_effectType;
    /// <summary>발동조건</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_TRIGGER    Data_effectTrigger;
    /// <summary>기본피해량</summary>
    [SerializeField] float                                      Data_effectValue;
    /// <summary>상관 능력치</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_STATUS     Data_effectStatus;
    /// <summary>스킬 계수</summary>
    [SerializeField] float                                      Data_effectRatio;
    /// <summary>지속시간</summary>
    [SerializeField] float                                      Data_effectDuration;
    /// <summary>반복주기</summary>
    [SerializeField] float                                      Data_effectRepeatCycle;


    //////////  Getter & Setter //////////
    //
    public int  HYJ_Data_id         { get { return Data_id; } }
    public int  HYJ_Data_skillId    { get { return Data_skillId; } }

    public HYJ_CharacterSkillEffect_EFFECT_TYPE     HYJ_Data_effectType         { get { return Data_effectType; } }
    public HYJ_CharacterSkillEffect_EFFECT_TRIGGER  HYJ_Data_effectTrigger      { get { return Data_effectTrigger; } }
    public float                                    HYJ_Data_effectValue        { get { return Data_effectValue; } }
    public HYJ_CharacterSkillEffect_EFFECT_STATUS   HYJ_Data_effectStatus       { get { return Data_effectStatus; } }
    public float                                    HYJ_Data_effectRatio        { get { return Data_effectRatio; } }
    public float                                    HYJ_Data_effectDuration     { get { return Data_effectDuration; } }
    public float                                    HYJ_Data_effectRepeatCycle  { get { return Data_effectRepeatCycle; } }

    //////////  Method          //////////
    public void Dispose()
    {

    }

    //////////  Default Method  //////////
    public HYJ_CharacterSkillEffect(Dictionary<string, object> _data)
    {
        //EFFECT_RATIO	EFFECT_DURATION	EFFECT_REPEAT_CYCLE
        //(HYJ_CharacterSkill_AOE_FIGURE)Enum.Parse(    typeof(HYJ_CharacterSkill_AOE_FIGURE),  (string)_data["AOE_FIGURE"]);
        Data_id         = (int)_data["ID"];
        Data_skillId    = (int)_data["SKILL_ID"];

        Data_effectType         = (HYJ_CharacterSkillEffect_EFFECT_TYPE)Enum.Parse(     typeof(HYJ_CharacterSkillEffect_EFFECT_TYPE),       (string)_data["EFFECT_TYPE"]);
        Data_effectTrigger      = (HYJ_CharacterSkillEffect_EFFECT_TRIGGER)Enum.Parse(  typeof(HYJ_CharacterSkillEffect_EFFECT_TRIGGER),    (string)_data["EFFECT_TRIGGER"]);
        Data_effectValue        = (float)_data["EFFECT_VALUE"];
        Data_effectStatus       = (HYJ_CharacterSkillEffect_EFFECT_STATUS)Enum.Parse(   typeof(HYJ_CharacterSkillEffect_EFFECT_STATUS),     (string)_data["EFFECT_STATUS"]);
        Data_effectRatio        = (float)_data["EFFECT_RATIO"];
        Data_effectDuration     = (float)_data["EFFECT_DURATION"];
        Data_effectRepeatCycle  = (float)_data["EFFECT_REPEAT_CYCLE"];
    }
}

#endregion