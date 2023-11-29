using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character_Status
{
    [SerializeField] protected float Status_moveSpeed;   // 이속

    // 공격
    [SerializeField] protected float Status_atkPhysics;  // 물공
    [SerializeField] protected float Status_atkSpell;    // 마공
    [SerializeField] protected float Status_atkSpeed;    // 공속

    // 방어
    [SerializeField] protected float Status_defence;            // 물방
    [SerializeField] protected float Status_spellRegistance;    // 마방

    // 치명타
    [SerializeField] protected float Status_critValue;  // 치명타 수치
    [SerializeField] protected float Status_critPer;    // 치명타 확률

    //////////  Getter & Setter //////////
    public float Character_Status_moveSpeed { get { return Status_moveSpeed;    }   set { Status_atkSpeed = value;  }   }

    // 공격
    public float Character_Status_atkPhysics    { get { return Status_atkPhysics;   }   }
    public float Character_Status_atkSpell      { get { return Status_atkSpell;     }   }
    public float Character_Status_atkSpeed      { get { return Status_atkSpeed;     }   }

    // 방어
    public float Character_Status_defence           { get { return Status_defence;          }   }
    public float Character_Status_spellRegistance   { get { return Status_spellRegistance;  }   }

    // 치명타
    public float Character_Status_critPer   { get { return Status_critPer;      }   }
    public float Character_Status_critValue { get { return Status_critValue;    }   }
    
    //////////  Method          //////////
    public void HYJ_Status_Reset()
    {
        Status_moveSpeed = 0.0f;
        
        // 공격
        Status_atkPhysics   = 0.0f;
        Status_atkSpell     = 0.0f;
        Status_atkSpeed     = 0.0f;

        // 방어
        Status_defence          = 0.0f;
        Status_spellRegistance  = 0.0f;

        // 치명타
        Status_critPer      = 0.0f;
        Status_critValue    = 0.0f;
    }

    public void HYJ_Status_DB(Dictionary<string, object> _data)
    {
        HYJ_Status_Reset();

        // 공격
        Status_atkPhysics   = (float)_data["ATK_PHYSICS"];
        Status_atkSpell     = (float)_data["ATK_SPELL"];
        Status_atkSpeed     = (float)_data["ATK_SPEED"];

        // 방어
        Status_defence          = (float)_data["DEFENCE"];
        Status_spellRegistance  = (float)_data["SPELL_REGISTANCE"];

        // 치명타
        Status_critPer      = (float)_data["CRIT_PERCENT"];
        Status_critValue    = (float)_data["CRIT_VALUE"];
    }

    public void HYJ_Status_Buff(List<CTRL_Buff> _buffs, Character _character, Character_Status _db)
    {
        HYJ_Status_Reset();
        for (int i = 0; i < _buffs.Count; i++)
        {
            string[] strs = _buffs[i].CTRL_Basic_applyType.ToString().Split('/');
            if (    strs[0].Equals(_character.Character_Status_race.ToString()) ||
                    strs[0].Equals(_character.Character_Status_name))
            {
                switch (strs[1])
                {
                    case "physicAtk":
                        {
                            switch(_buffs[i].Basic_ratioType)
                            {
                                case CTRL_Buff.RATIO_TYPE.plat:     { Status_atkPhysics += _buffs[i].CTRL_Basic_ratioValue;                                     }   break;
                                case CTRL_Buff.RATIO_TYPE.percent:  { Status_atkPhysics += _db.Character_Status_atkPhysics * _buffs[i].CTRL_Basic_ratioValue;   }   break;
                            }
                        }
                        break;
                }
            }
        }
    }

    public void HYJ_Status_Item()
    {

    }

    //////////  Default Method  //////////
    public Character_Status()
    {
        HYJ_Status_Reset();
    }
}

public enum HYJ_Character_REPUTATION_RACE
{
    ELF = 0,
    HUMAN,
    DWARF,
    GOBLIN,
    WILD
}

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
    [SerializeField] protected string                           Status_script;  // 스토리 번호
    [SerializeField] protected HYJ_Character_REPUTATION_RACE    Status_race;    // 종족
    [SerializeField] protected string                           Status_job;     // 특성

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

    //[SerializeField] protected float Status_moveSpeed;   // 이속

    [SerializeField] protected float Status_Damage;  // 데미지

    [SerializeField] Character_Status Status_DB;
    [SerializeField] Character_Status Status_Buff;
    [SerializeField] Character_Status Status_Item;

    // 스킬
    [SerializeField] protected int Data_spell0; // 일반 공격 번호
    [SerializeField] protected int Data_spell1; // 스킬 번호

    // 시너지 벨류
    [SerializeField] protected int Synergy_Stat1; // 시너지 스텟_1


    //////////  Getter & Setter //////////
    public CTRL_Character_Data HYJ_Status_saveData
    {
        get { return Status_saveData;   }
        set { Status_saveData = value;  }
    }
    virtual public int Character_Status_Index       { get { return Status_idx; } }
    virtual public int Character_Status_ID          { get { return Status_id; } }
    virtual public string Character_Status_name     { get { return Status_name; } }
    virtual public string Character_Status_name_eng { get { return Status_name_eng; } }

    virtual public HYJ_Character_REPUTATION_RACE    Character_Status_race   { get { return Status_race; } }

    virtual public float Character_Status_maxHp     { get { return Status_MaxHP; } }
    virtual public float Character_Status_startMp   { get { return Status_startMp; } }
    virtual public float Character_Status_Damage    { get { return Status_Damage; } }
    
    virtual public float Character_Status_moveSpeed { get { return Status_DB.Character_Status_moveSpeed;   } }

    virtual public float Character_Status_atkPhysics
    {
        get
        {
            return  Status_DB.Character_Status_atkPhysics +
                    Status_Buff.Character_Status_atkPhysics;
        }
    }
    virtual public float Character_Status_atkSpell
    {
        get
        {
            return  Status_DB.Character_Status_atkSpell +
                    Status_Buff.Character_Status_atkSpell;
        }
    }
    virtual public float Character_Status_atkSpeed      { get { return Status_DB.Character_Status_atkSpeed;     } }

    virtual public float Character_Status_defence           { get { return Status_DB.Character_Status_defence;          } }
    virtual public float Character_Status_spellRegistance   { get { return Status_DB.Character_Status_spellRegistance;  } }

    virtual public float Character_Status_critPer   { get { return Status_DB.Character_Status_critPer;      } }
    virtual public float Character_Status_critValue { get { return Status_DB.Character_Status_critValue;    } }

    virtual public int Character_Status_spell0 { get { return Data_spell0; } }
    virtual public int Character_Status_spell1 { get { return Data_spell1; } }

    //////////  Method          //////////

    // DB에서 사용.
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
        Status_race = (HYJ_Character_REPUTATION_RACE)Enum.Parse(typeof(HYJ_Character_REPUTATION_RACE), (string)_data["RACE"]);
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

        Status_DB.HYJ_Status_DB(_data);

        // 스킬
        Data_spell0 = (int)_data["SPELL_0"];
        Data_spell1 = (int)_data["SPELL_1"];

        // Current HP 초기화
        Status_HP = Status_MaxHP;
    }

    // 인게임에서 사용.
    // 유닛의 능력치를 조정.
    public void HYJ_Status_SettingBuff(List<CTRL_Buff> _buffs)
    {
        Status_Buff.HYJ_Status_Buff(_buffs, this, Status_DB);
    }

    //////////  Default Method  //////////
    void HYJ_Status_Start()
    {
        Status_DB   = new Character_Status();
        Status_Buff = new Character_Status();
    }
}


[Serializable]
public class CTRL_Character_Data : IDisposable
{
    public string Data_ID;
    public Character.Unit_Star Data_star;
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
        Data_star = Character.Unit_Star.ONE;
    }
}