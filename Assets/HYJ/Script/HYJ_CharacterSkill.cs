using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

#region HYJ_CharacterSkill

public enum HYJ_CharacterSkill_ATTACK_TYPE
{
    NONE
}

public enum HYJ_CharacterSkill_SEARCH_TEAM
{
    ALLY,
    ENEMY
}

public enum HYJ_CharacterSkill_SEARCH_CONDITION
{
    NONE
}

//
public enum HYJ_CharacterSkill_AOE_FIGURE
{
    NONE
}

//
public enum HYJ_CharacterSkill_TARGET_RACE
{
    NONE
}

public enum HYJ_CharacterSkill_TARGET_CONDITION
{
    NONE
}

//
public enum HYJ_CharacterSkill_DAMAGE_TYPE
{
    PHYSICS
}

public enum HYJ_CharacterSkill_DAMAGE_STATUS
{
    NONE
}

//
public class HYJ_CharacterSkill :IDisposable
{
    [SerializeField] int    Data_ID;
    [SerializeField] string Data_name;
    [SerializeField] string Data_nameKor;

    //
    [SerializeField] HYJ_CharacterSkill_ATTACK_TYPE         Data_attackType;
    [SerializeField] HYJ_CharacterSkill_SEARCH_TEAM         Data_searchTeam;
    [SerializeField] int                                    Data_searchRange;
    [SerializeField] HYJ_CharacterSkill_SEARCH_CONDITION    Data_searchCondition;

    //
    [SerializeField] float  Data_range;
    [SerializeField] float  Data_delayPre;      // ����
    [SerializeField] float  Data_delayAfter;    // �ĵ�

    [SerializeField] bool   Data_isTargeting;   // Ÿ���ÿ���
    [SerializeField] float  Data_duration;      // ���ӽð�
    [SerializeField] float  Data_repeatCycle;   // �ݺ��ֱ�

    //
    [SerializeField] HYJ_CharacterSkill_AOE_FIGURE  Data_aoeFigure;
    [SerializeField] float                          Data_aoeDegree;
    [SerializeField] float                          Data_aoeArea;

    //
    [SerializeField] HYJ_CharacterSkill_TARGET_RACE         Data_targetRace;
    [SerializeField] int                                    Data_targetMax;
    [SerializeField] HYJ_CharacterSkill_TARGET_CONDITION    Data_targetCondition;

    //
    [SerializeField] HYJ_CharacterSkill_DAMAGE_TYPE     Data_damageType;
    [SerializeField] float                              Data_damageValue;
    [SerializeField] HYJ_CharacterSkill_DAMAGE_STATUS   Data_damageStatus;
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
    /// <summary>�̸�</summary>
    public string   HYJ_Data_name       { get { return Data_name; } }
    /// <summary>�ѱ� �̸�</summary>
    public string   HYJ_Data_nameKor    { get { return Data_nameKor; } }

    //
    /// <summary>���� Ÿ��</summary>
    public HYJ_CharacterSkill_ATTACK_TYPE       HYJ_Data_attackType         { get { return Data_attackType; } }
    /// <summary>�ǾƱ���</summary>
    public HYJ_CharacterSkill_SEARCH_TEAM       HYJ_Data_searchTeam         { get { return Data_searchTeam; } }
    /// <summary>Ž�� ��Ÿ�</summary>
    public int                                  HYJ_Data_searchRange        { get { return Data_searchRange; } }
    /// <summary>Ž�� ����</summary>
    public HYJ_CharacterSkill_SEARCH_CONDITION  HYJ_Data_searchCondition    { get { return Data_searchCondition; } }

    //
    /// <summary>��Ÿ�</summary>
    public float    HYJ_Data_range      { get { return Data_range; } }
    /// <summary>��������</summary>
    public float    HYJ_Data_delayPre   { get { return Data_delayPre; } }
    /// <summary>�ĵ�����</summary>
    public float    HYJ_Data_delayAfter { get { return Data_delayAfter; } }

    //
    /// <summary>Ÿ����</summary>
    public bool     HYJ_Data_isTargeting    { get { return Data_isTargeting; } }
    /// <summary>���ӽð�</summary>
    public float    HYJ_Data_duration       { get { return Data_duration; } }
    /// <summary>�ݺ��ֱ�</summary>
    public float    HYJ_Data_repeatCycle    { get { return Data_repeatCycle; } }

    //
    /// <summary>��������</summary>
    public HYJ_CharacterSkill_AOE_FIGURE    HYJ_Data_aoeFigure  { get { return Data_aoeFigure; } }
    /// <summary>��������</summary>
    public float                            HYJ_Data_aoeDegree  { get { return Data_aoeDegree; } }
    /// <summary>��������</summary>
    public float                            HYJ_Data_aoeArea    { get { return Data_aoeArea; } }

    //
    /// <summary>ȿ����������</summary>
    public HYJ_CharacterSkill_TARGET_RACE       HYJ_Data_targetRace         { get { return Data_targetRace; } }
    /// <summary>�ִ��󰳼�</summary>
    public float                                HYJ_Data_targetMax          { get { return Data_targetMax; } }
    /// <summary>ȿ���������</summary>
    public HYJ_CharacterSkill_TARGET_CONDITION  HYJ_Data_targetCondition    { get { return Data_targetCondition; } }

    //
    /// <summary>��������</summary>
    public HYJ_CharacterSkill_DAMAGE_TYPE   HYJ_Data_damageType     { get { return Data_damageType; } }
    /// <summary>�⺻���ط�</summary>
    public float                            HYJ_Data_damageValue    { get { return Data_damageValue; } }
    /// <summary>���ط� �ɷ�ġ</summary>
    public HYJ_CharacterSkill_DAMAGE_STATUS HYJ_Data_damageStatus   { get { return Data_damageStatus; } }
    /// <summary>��ų ���</summary>
    public float                            HYJ_Data_damageRatio    { get { return Data_damageRatio; } }

    //
    /// <summary>��ų ����</summary>
    public int  HYJ_Data_chain      { get { return Data_chain; } }
    /// <summary>��ų ����</summary>
    public int  HYJ_Data_piercing   { get { return Data_piercing; } }
    /// <summary>��ų �и�</summary>
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
        Data_nameKor    = (string)_data["NAME_KOR"];

        Data_attackType         = (HYJ_CharacterSkill_ATTACK_TYPE)Enum.Parse(       typeof(HYJ_CharacterSkill_ATTACK_TYPE),         (string)_data["ATTACK_TYPE"]);
        Data_searchTeam         = (HYJ_CharacterSkill_SEARCH_TEAM)Enum.Parse(       typeof(HYJ_CharacterSkill_SEARCH_TEAM),         (string)_data["SEARCH_TEAM"]);
        Data_searchRange        = (int)_data["SEARCH_RANGE"];
        Data_searchCondition    = (HYJ_CharacterSkill_SEARCH_CONDITION)Enum.Parse(  typeof(HYJ_CharacterSkill_SEARCH_CONDITION),    (string)_data["SEARCH_CONDITION"]);

        Data_range      = (float)_data["RANGE"];
        Data_delayPre   = (float)_data["DELAY_PRE"];
        Data_delayAfter = (float)_data["DELAY_AFTER"];

        Data_isTargeting    = bool.Parse((string)_data["TARGETING"]);
        Data_duration       = (float)_data["DURATION"];
        Data_repeatCycle    = (float)_data["REPEAT_CYCLE"];

        Data_aoeFigure  = (HYJ_CharacterSkill_AOE_FIGURE)Enum.Parse(    typeof(HYJ_CharacterSkill_AOE_FIGURE),  (string)_data["AOE_FIGURE"]);
        Data_aoeDegree  = (float)_data["AOE_DEGREE"];
        Data_aoeArea    = (float)_data["AOE_AREA"];

        Data_targetRace         = (HYJ_CharacterSkill_TARGET_RACE)Enum.Parse(       typeof(HYJ_CharacterSkill_TARGET_RACE),         (string)_data["TARGET_RACE"]);
        Data_targetMax          = (int)_data["ID"];
        Data_targetCondition    = (HYJ_CharacterSkill_TARGET_CONDITION)Enum.Parse(  typeof(HYJ_CharacterSkill_TARGET_CONDITION),    (string)_data["TARGET_CONDITION"]);

        Data_damageType     = (HYJ_CharacterSkill_DAMAGE_TYPE)Enum.Parse(   typeof(HYJ_CharacterSkill_DAMAGE_TYPE),     (string)_data["DAMAGE_TYPE"]);
        Data_damageValue    = (float)_data["DAMAGE_VALUE"];
        Data_damageStatus   = (HYJ_CharacterSkill_DAMAGE_STATUS)Enum.Parse( typeof(HYJ_CharacterSkill_DAMAGE_STATUS),   (string)_data["DAMAGE_STATUS"]);
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
    /// <summary>ȿ������</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_TYPE       Data_effectType;
    /// <summary>�ߵ�����</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_TRIGGER    Data_effectTrigger;
    /// <summary>�⺻���ط�</summary>
    [SerializeField] float                                      Data_effectValue;
    /// <summary>��� �ɷ�ġ</summary>
    [SerializeField] HYJ_CharacterSkillEffect_EFFECT_STATUS     Data_effectStatus;
    /// <summary>��ų ���</summary>
    [SerializeField] float                                      Data_effectRatio;
    /// <summary>���ӽð�</summary>
    [SerializeField] float                                      Data_effectDuration;
    /// <summary>�ݺ��ֱ�</summary>
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