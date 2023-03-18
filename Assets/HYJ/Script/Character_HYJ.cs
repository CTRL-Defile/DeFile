using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Character
{
    [Header("======================================= STATUS =======================================")]
    [SerializeField] protected CTRL_Character_Data Status_saveData; // 저장용 클래스

    // 이름
    [SerializeField] protected int Status_idx;
    [SerializeField] protected int Status_id;
    [SerializeField] protected string Status_name;
    [SerializeField] protected string Status_name_kor;
    [SerializeField] protected string Status_name_eng;

    // 속성
    [SerializeField] protected string Status_script;    // 스토리 번호
    [SerializeField] protected string Status_race;      // 종족
    [SerializeField] protected string Status_job;       // 특성

    // 조합
    [SerializeField] protected int Status_mix;  // 등급
    [SerializeField] protected int Status_Cost; // 코스트

    // 
    [SerializeField] protected string Status_atkType; // 공격방식
    [SerializeField] protected float Status_MaxHP;  // 최대체력
    [SerializeField] protected float Status_HP;     // 현재체력
    [SerializeField] protected float Status_MaxMP;  // 최대마나
    [SerializeField] protected float Status_MP;     // 현재마나
    [SerializeField] protected float Status_startMp;// 시작마나

    [SerializeField] protected float Status_moveSpeed;   // 이속

    [SerializeField] protected float Status_atkPhysics;  // 물공
    [SerializeField] protected float Status_atkSpell;    // 마공
    [SerializeField] protected float Status_atkSpeed;    // 공속

    [SerializeField] protected float Status_defence;            // 물방
    [SerializeField] protected float Status_spellRegistance;    // 마방

    // 치명타
    [SerializeField] protected float Status_critValue;  // 치명타 수치
    [SerializeField] protected float Status_critPer;    // 치명타 확률

    // 스킬
    [SerializeField] protected int Data_spell0; // 일반 공격 번호
    [SerializeField] protected int Data_spell1; // 스킬 번호

    //////////  Getter & Setter //////////
    public CTRL_Character_Data HYJ_Status_saveData
    {
        get { return Status_saveData;   }
        set { Status_saveData = value;  }
    }
    virtual public int Character_Status_Index { get { return Status_idx; } }
    virtual public int Character_Status_ID { get { return Status_id; } }
    virtual public string Character_Status_name { get { return Status_name; } }
    virtual public string Character_Status_name_eng { get { return Status_name_eng; } }

    virtual public float Character_Status_maxHp { get { return Status_MaxHP; } }
    virtual public float Character_Status_startMp { get { return Status_startMp; } }

    virtual public float Character_Status_atkPhysics { get { return Status_atkPhysics;} }
    virtual public float Character_Status_atkSpell { get { return Status_atkSpell; } }

    virtual public float Character_Status_defence { get { return Status_defence; } }
    virtual public float Character_Status_spellRegistance { get { return Status_spellRegistance; } }

    virtual public float Character_Status_critPer { get { return Status_critPer; } }
    virtual public float Character_Status_critValue { get { return Status_critValue; } }

    virtual public int Character_Status_spell0 { get { return Data_spell0; } }
    virtual public int Character_Status_spell1 { get { return Data_spell1; } }

    //////////  Method          //////////

    public void HYJ_Status_SettingData(Dictionary<string, object> _data)
    {
        // 이름
        Status_idx = (int)_data["Index"];
        Status_id = (int)_data["ID"];
        Status_name = (string)_data["NAME"];
        Status_name_kor = (string)_data["NAME_KOR"];
        Status_name_eng = (string)_data["NAME_ENG"];

        // 속성
        Status_script = (string)_data["SCRIPT_KOR"];
        Status_race = (string)_data["RACE"];
        Status_job = (string)_data["JOB"];

        // 조합
        Status_mix = (int)_data["MIX"];

        // 코스트
        Status_Cost = (int)_data["COST"];

        // 능력치
        Status_atkType = (string)_data["ATK_TYPE"];
        Status_MaxHP = (float)_data["MAX_HP"];
        Status_MaxMP = (float)_data["MAX_MP"];
        Status_startMp = (float)_data["START_MP"];

        // 공격
        Status_atkPhysics = (float)_data["ATK_PHYSICS"];
        Status_atkSpell = (float)_data["ATK_SPELL"];
        Status_atkSpeed = (float)_data["ATK_SPEED"];

        // 방어
        Status_defence = (float)_data["DEFENCE"];
        Status_spellRegistance = (float)_data["SPELL_REGISTANCE"];

        // 치명타
        Status_critPer = (float)_data["CRIT_PERCENT"];
        Status_critValue = (float)_data["CRIT_VALUE"];

        // 스킬
        Data_spell0 = (int)_data["SPELL_0"];
        Data_spell1 = (int)_data["SPELL_1"];

        // Current HP 초기화
        Status_HP = Status_MaxHP;
    }

    //////////  Default Method  //////////
}


[Serializable]
public class CTRL_Character_Data : IDisposable
{
    public string Data_ID;
    public int Data_COST;
    public List<HYJ_Item> Data_items;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void Dispose()
    {

    }

    //////////  Default Method  //////////
    public CTRL_Character_Data(string _ID)
    {
        Data_ID = _ID;
    }
}