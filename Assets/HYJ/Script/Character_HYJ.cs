using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class Character
{
    [Header("======================================= STATUS =======================================")]
    [SerializeField] protected CTRL_Character_Data Status_saveData; // ����� Ŭ����

    // �̸�
    [SerializeField] protected int Status_idx;
    [SerializeField] protected int Status_id;
    [SerializeField] protected string Status_name;
    [SerializeField] protected string Status_name_kor;
    [SerializeField] protected string Status_name_eng;

    // �Ӽ�
    [SerializeField] protected string Status_script;    // ���丮 ��ȣ
    [SerializeField] protected string Status_race;      // ����
    [SerializeField] protected string Status_job;       // Ư��

    // ����
    [SerializeField] protected int Status_mix;  // ���
    [SerializeField] protected int Status_Cost; // �ڽ�Ʈ

    // 
    [SerializeField] protected string Status_atkType; // ���ݹ��
    [SerializeField] protected float Status_MaxHP;  // �ִ�ü��
    [SerializeField] protected float Status_HP;     // ����ü��
    [SerializeField] protected float Status_MaxMP;  // �ִ븶��
    [SerializeField] protected float Status_MP;     // ���縶��
    [SerializeField] protected float Status_startMp;// ���۸���

    [SerializeField] protected float Status_moveSpeed;   // �̼�

    [SerializeField] protected float Status_Damage;  // ������

    [SerializeField] protected float Status_atkPhysics;  // ����
    [SerializeField] protected float Status_atkSpell;    // ����
    [SerializeField] protected float Status_atkSpeed;    // ����

    [SerializeField] protected float Status_defence;            // ����
    [SerializeField] protected float Status_spellRegistance;    // ����

    // ġ��Ÿ
    [SerializeField] protected float Status_critValue;  // ġ��Ÿ ��ġ
    [SerializeField] protected float Status_critPer;    // ġ��Ÿ Ȯ��

    // ��ų
    [SerializeField] protected int Data_spell0; // �Ϲ� ���� ��ȣ
    [SerializeField] protected int Data_spell1; // ��ų ��ȣ

    // �ó��� ����
    [SerializeField] protected int Synergy_Stat1; // �ó��� ����_1


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
    virtual public float Character_Status_Damage { get { return Status_Damage; } }

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
        // �̸�
        Status_idx = (int)_data["Index"];
        Status_id = (int)_data["ID"];
        Status_name = (string)_data["NAME"];
        Status_name_kor = (string)_data["NAME_KOR"];
        Status_name_eng = (string)_data["NAME_ENG"];

        // �Ӽ�
        Status_script = (string)_data["SCRIPT_KOR"];
        Status_race = (string)_data["RACE"];
        Status_job = (string)_data["JOB"];

        // ����
        Status_mix = (int)_data["MIX"];

        // �ڽ�Ʈ
        Status_Cost = (int)_data["COST"];

        // �ɷ�ġ
        Status_atkType = (string)_data["ATK_TYPE"];
        Status_MaxHP = (float)_data["MAX_HP"];
        Status_MaxMP = (float)_data["MAX_MP"];
        Status_startMp = (float)_data["START_MP"];

        // ����
        Status_atkPhysics = (float)_data["ATK_PHYSICS"];
        Status_atkSpell = (float)_data["ATK_SPELL"];
        Status_atkSpeed = (float)_data["ATK_SPEED"];

        // ���
        Status_defence = (float)_data["DEFENCE"];
        Status_spellRegistance = (float)_data["SPELL_REGISTANCE"];

        // ġ��Ÿ
        Status_critPer = (float)_data["CRIT_PERCENT"];
        Status_critValue = (float)_data["CRIT_VALUE"];

        // ��ų
        Data_spell0 = (int)_data["SPELL_0"];
        Data_spell1 = (int)_data["SPELL_1"];

        // Current HP �ʱ�ȭ
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