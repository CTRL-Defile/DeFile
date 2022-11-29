using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Character
{
    [Header("======================================= HYJ_STATUS =======================================")]
    [SerializeField] protected CTRL_Character_Data Status_saveData; // 저장용 클래스

    [SerializeField] protected string Status_name;
    // 속성
    [SerializeField] protected string Status_race;
    [SerializeField] protected string Status_job;
    [SerializeField] protected string Status_affiliation;
    // 조합
    [SerializeField] protected int Status_mix;

    [SerializeField] protected int Status_startMp;

    [SerializeField] protected int Status_atkRange;
    [SerializeField] protected int Status_atkPhysics;
    [SerializeField] protected int Status_atkSpell;
    [SerializeField] protected int Status_defence;
    [SerializeField] protected int Status_spellRegistance;

    // 스킬
    [SerializeField] protected int Data_spell0;
    [SerializeField] protected int Data_spell1;

    //////////  Getter & Setter //////////
    public CTRL_Character_Data HYJ_Status_saveData
    {
        get { return Status_saveData;   }
        set { Status_saveData = value;  }
    }

    //////////  Method          //////////

    public void HYJ_Status_SettingData(Dictionary<string, object> _data)
    {
        Status_name = (string)_data["NAME"];
        // 속성
        Status_race = (string)_data["RACE"];
        Status_job = (string)_data["JOB"];
        Status_affiliation = (string)_data["AFFILIATION"];
        // 조합
        Status_mix = (int)_data["MIX"];
        // 능력치
        Status_MaxHP = (int)_data["MAX_HP"];
        Status_MaxMP = (int)_data["MAX_MP"];
        Status_startMp = (int)_data["START_MP"];
        Status_atkRange = (int)_data["ATK_RANGE"];
        Status_atkPhysics = (int)_data["ATK_PHYSICS"];
        Status_atkSpell = (int)_data["ATK_SPELL"];
        Status_atkSpeed = (int)((float)_data["ATK_SPEED"]);
        Status_defence = (int)_data["DEFENCE"];
        Status_spellRegistance = (int)_data["SPELL_REGISTANCE"];
        Status_critPer = (int)((float)_data["CRIT_PERCENT"]);
        Status_critValue = (int)((float)_data["CRIT_VALUE"]);
        // 스킬
        Data_spell0 = (int)_data["SPELL_0"];
        Data_spell1 = (int)_data["SPELL_1"];
        // 코스트
        Status_Cost = (int)_data["COST"];
    }

    //////////  Default Method  //////////
}
