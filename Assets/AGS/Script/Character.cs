using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using UnityEngine.EventSystems;
using static AnimationEvent;
using UnityEngine.UIElements;
using Newtonsoft.Json.Serialization;
using UnityEditor;
[System.Serializable]

public partial class Character : MonoBehaviour
{
	public enum MODEL_TYPE { BEAR, ORC, EVIL, GOBLIN_N, GOBLIN_T, WERERAT };

	//-------------------------------------------------------------------
	// Field
	//-------------------------------------------------------------------
	[SerializeField]
    BATTLE_PHASE m_Basic_phase;
	[SerializeField]
    protected Vector3 char_ori_Pos; // Update/MouseUp ���� ����
    [SerializeField]
	protected GameObject on_Tile;
	[SerializeField]
	protected bool IsDead = false;
	[SerializeField]
	protected GameObject m_Target = null;
	[SerializeField]
	protected GameObject m_PreTarget = null;
	//[SerializeField]
	//protected int m_CurPosIndex = 0;
	[SerializeField]
	protected PathFinder m_PathFinder = null;
	[SerializeField]
	protected AnimationEvent m_AnimEvent = null;
	[SerializeField]
	protected UI_StatusBar m_StatusHpBar = null;	
	protected UI_StatusBar m_StatusMpBar = null;
	[SerializeField]
	protected GameObject[] m_WeaponEffect;
	[SerializeField]
	MODEL_TYPE m_Modeltype;	
	[SerializeField]
	protected HYJ_CharacterSkill m_Spell_0;
	[SerializeField]
	protected HYJ_CharacterSkill m_Spell_1;

	[SerializeField]
	protected bool m_IsInRange = false;
	protected bool m_IsOneShot = false;
	//-------------------------------------------------------------------
	// Property
	//-------------------------------------------------------------------
	public Vector3 LSY_Character_OriPos { get { return char_ori_Pos; } set { char_ori_Pos = value; } }
	public GameObject Target { get { return m_Target; } set { m_Target = value; } }
	public GameObject PreTarget { get { return m_PreTarget; } set { m_PreTarget = value; } }
	public bool Dead { get { return IsDead; } set { IsDead = value; } }
	//public int CurPosIndex { get { return m_CurPosIndex; } set { m_CurPosIndex = value; } }
	public UI_StatusBar STATUS_HPBAR { get { return m_StatusHpBar; } set { m_StatusHpBar = value; } }
	public UI_StatusBar STATUS_MPBAR { get { return m_StatusMpBar; } set { m_StatusMpBar = value; } }
	public MODEL_TYPE Model_Type { get { return m_Modeltype; } set { m_Modeltype = value; } }

	public HYJ_CharacterSkill Spell_0 { get { return m_Spell_0; } }
	public HYJ_CharacterSkill Spell_1 { get { return m_Spell_1; } }
	public bool InRange { get { return m_IsInRange; } set { m_IsInRange = value; } }
	//-------------------------------------------------------------------
	// Method
	//-------------------------------------------------------------------
	public void LSY_Character_Set_OnTile(GameObject tile)
	{
		on_Tile = tile;
	}
	public GameObject LSY_Character_Get_OnTile() { return on_Tile; }

    // Start is called before the first frame update
    void Start()
    {
		char_ori_Pos = new Vector3();
        m_Basic_phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);

		Status_HP = Status_MaxHP;

#if false
		// ��ų �ѹ� ���;���
		//int Spell0_Idx = 1;
		//int Spell1_Idx = 3;

		//switch (m_Modeltype)
		//{
		//	case MODEL_TYPE.BEAR:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell0_Idx);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);				
		//		break;
		//	case MODEL_TYPE.ORC:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell0_Idx);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);
		//		break;
		//	case MODEL_TYPE.EVIL:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, 2);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);
		//		break;
		//	case MODEL_TYPE.GOBLIN_N:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell0_Idx);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);
		//		break;
		//	case MODEL_TYPE.GOBLIN_T:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell0_Idx);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);
		//		break;
		//	case MODEL_TYPE.WERERAT:
		//		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell0_Idx);
		//		m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Spell1_Idx);
		//		break;
		//	default:
		//		break;
		//}
#endif

		m_Spell_0 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Data_spell0);
        m_Spell_1 = (HYJ_CharacterSkill)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, Data_spell1);


        Status_moveSpeed = 5.0f;
		// Stat_MoveSpeed = UnityEngine.Random.Range(1.0f, 8.0f);
		//CurPosIndex = 0;
	}

	private void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();
		m_PathFinder = GetComponent<PathFinder>();
		m_AnimEvent = GetComponentInChildren<AnimationEvent>();
		m_StatusHpBar = transform.GetChild(1).GetComponent< UI_StatusBar>();
		m_StatusMpBar = transform.GetChild(2).GetComponent<UI_StatusBar>();
	}

	private void LateUpdate()
    {
        m_Basic_phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
        // char_ori_Pos �� ���� ���� Ȥ�� �巡�� ��� �� �ǵ��ư��� ��ġ�̹Ƿ�, ���� ������ ���� �ʱ�ȭ�� �ʿ䰡 ����.
		if (m_Basic_phase == BATTLE_PHASE.PHASE_PREPARE && 
			this.on_Tile != null && 
			this.on_Tile.GetComponent<HYJ_Battle_Tile>().tile_Available == HYJ_Battle_Tile.Tile_Available.Available)
			char_ori_Pos = on_Tile.transform.position;

		DieProcess();

		if (true == IsDead)
			return;

		if (State != STATE.DIE && Target != null && on_Tile != null)
		{
			//m_PathFinder.StartPathFinding(on_Tile.GetComponent<HYJ_Battle_Tile>().GraphIndex, Target.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex);
			m_PathFinder.MoveOnPath();			
		}

		BattleProcess();
		ChangeState();
	}
}

// ĳ������ �ɷ�ġ
// ������ ������, ���� ���� �ܺο�ҷ� ���� ���ϴ� ��ġ���� �������ֱ� ���� ���Ǵ� ������
#region STATUS

public partial class Character
{
    //-------------------------------------------------------------------
    // Field
    //-------------------------------------------------------------------
 //   [Header("======================================= STATUS =======================================")]
	//[Space (10f)]
	//[SerializeField] protected float Status_HP;     // ü��
	//[SerializeField] protected float Status_MP;     // ����

	//[Space(10f)]
	//[SerializeField] protected float Status_moveSpeed;   // �̼�

    //-------------------------------------------------------------------
    // Property
    //-------------------------------------------------------------------
    public float Stat_HP { get { return Status_HP; } set { Status_HP = value; } }
	public float Stat_MP { get { return Status_MP; } set { Status_MP = value; } }
	public float Stat_MaxHP { get { return Status_MaxHP; } set { Status_MaxHP = value; } }
	public float Stat_MaxMP { get { return Status_MaxMP; } set { Status_MaxMP = value; } }
	//public float Stat_Attack { get { return Status_atkPhysics; } set { Status_atkPhysics = value; } }
 //   public float Stat_Attack_Spell { get { return Status_atkSpell; } set { Status_atkSpell = value; } }
    public float Stat_MoveSpeed { get { return Status_moveSpeed; } set { Status_moveSpeed = value; } }
	public int Stat_Cost { get { return Status_Cost; } set { Status_Cost = value; } }
	public int Stat_Synergy1 { get { return Synergy_Stat1; } set { Synergy_Stat1 = value; } }

	//-------------------------------------------------------------------
	// Method
	//-------------------------------------------------------------------

	virtual public void Synergy_Atk()
	{
		Status_Damage = Status_atkPhysics + Synergy_Stat1;
	}

	virtual public void HitProcess(float Attack)
    {
		//���⼭ Hit Effect �߻�
		CapsuleCollider Col = GetComponent<CapsuleCollider>();
		Vector3 Pos = new Vector3(transform.position.x, transform.position.y + (Col.height / 3.0f), transform.position.z);
		EffectPool.Instance.Create(EFFECT_TYPE.EFFECT_SPARK, Pos);				

		if (Status_HP >= Attack)
            Status_HP -= Attack;
        else if (Status_HP < Attack)
        {
			Status_HP = 0.0f;
		}
    }

    virtual public void DieProcess()
    {
		if (Status_HP > 0.0f)
			return;
		else if(Status_HP <= 0.0f && false == m_IsOneShot)
		{
			List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);
			BattleGraph[on_Tile.GetComponent<HYJ_Battle_Tile>().GraphIndex].Marking = false;
			//m_PathFinder.InitMarking();			
			on_Tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;
			State = STATE.DIE;
			HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT_DIE, this.gameObject);

			// Sound : ���� ���
			HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.UNIT_DEATH);

			m_IsOneShot = true;
		}
		
		// Dissolve Shader ���� ����
		// �ϴ� ���� �� ��� Die �ִϸ��̼� ������ �� IsDead true��		
		if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
		  m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
		{
			on_Tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;
			//gameObject.SetActive(false);

			// Dissolve ���̴� SetUp
			if(IsDead == false)
			{
                gameObject.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_DISSOLVE);				
			}				

			// ���� ����Ʈ OFF
			int EffectCnt = m_WeaponEffect.Length;
			if (EffectCnt != 0)
			{
				foreach (GameObject Effect in m_WeaponEffect)
				{
					if(Effect.activeSelf == true)
						Effect.SetActive(false);
				}
			}

			IsDead = true;			
		}
	}

	virtual public void BattleProcess()
	{
		if (State == STATE.DIE)
			return;

		if (null == Target)
			State = STATE.IDLE;

		if (PreTarget != Target)
		{			
			m_PathFinder.InitCloseNodes();
			m_IsInRange = false;
			m_PathFinder.CheckRange();
		}
			

		if (null != Target)
		{
			float Dist = Vector3.Magnitude(transform.position - Target.transform.position);
			Vector3 Dir = Vector3.zero;
			float Angle = 0.0f;

			if (2.5f >= Dist)
			{
				switch (State)
				{
					case STATE.IDLE:
						State = STATE.SKILL_IDLE;
						break;
					case STATE.SKILL:
						Dir = Target.transform.position - transform.position;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Dir), 5.0f * Time.deltaTime);
						Angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Dir));
						break;
					case STATE.SKILL_IDLE:
						Dir = Target.transform.position - transform.position;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Dir), 5.0f * Time.deltaTime);
						Angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Dir));

						if (Angle <= 5.0f)
							State = STATE.SKILL;
						break;
					default:
						break;
				}
			}
			else if(2.5f < Dist)
			{
				switch (State) 
				{
					case STATE.IDLE:						
						break;
					case STATE.SKILL:	
						if(true == m_IsInRange)
						{

						}
						else
						{
							State = STATE.IDLE;
							m_PathFinder.InitCloseNodes();
							m_PathFinder.InitMarking();
						}
						break;
					case STATE.SKILL_IDLE:
						if (true == m_IsInRange)
						{
							Dir = Target.transform.position - transform.position;
							transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Dir), 5.0f * Time.deltaTime);
							Angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(Dir));

							if (Angle <= 5.0f)
								State = STATE.SKILL;
						}
						else
						{
							if (Target.GetComponent<Character>().State == STATE.SKILL)
							{
								m_PathFinder.InitCloseNodes();
								m_PathFinder.InitMarking();
								State = STATE.IDLE;
							}
						}					
						break;
					default:
						break;
				}
			}

			if (Target.GetComponent<Character>().Stat_HP <= 0/* || (PreTarget != Target && State != STATE.SKILL_IDLE )*/)
			{
				State = STATE.IDLE;
				m_PathFinder.InitCloseNodes();
				//m_PathFinder.InitMarking();
			}
		}
		MoveProcess();
	}

	virtual public void MoveProcess()
	{

	}

	//////////  Default Method  //////////
}

#endregion

#region STATE

public partial class Character
{
    public enum Unit_Type
    {
        Ally,
        Enemy,
		Unit_END
    }
    public enum STATE
	{
		IDLE,
		RUN,
		DIE,
		SKILL,
		SKILL_IDLE,
		STATE_END
	}

	//-------------------------------------------------------------------
	// Field
	//-------------------------------------------------------------------
	[Header("======================================= STATE =======================================")]
	[Space(10f)]

	[SerializeField]
	protected Animator m_animator;

	[SerializeField]
	public Unit_Type m_UnitType = Unit_Type.Unit_END;
	[SerializeField]
	protected STATE m_state = STATE.IDLE;
	public STATE State { get { return m_state; } set { m_state = value; } }
	public Unit_Type UnitType { get { return m_UnitType; } set { m_UnitType = value; } }

	//-------------------------------------------------------------------
	// Property
	//-------------------------------------------------------------------



	//-------------------------------------------------------------------
	// Method
	//-------------------------------------------------------------------
	virtual protected void ChangeState()
	{
		switch (m_state)
		{
			case STATE.IDLE:
					UpdateIdle();
					break;
			case STATE.RUN:
					UpdateRun();
					break;
			case STATE.DIE:
					UpdateDie();
					break;
			case STATE.SKILL:
					UpdateSkill();
					break;
			case STATE.SKILL_IDLE:
					UpdateIdle();
					break;
			default:
					break;
		}
	}

	virtual protected void UpdateIdle()
	{		
		m_animator.SetBool("Skill", false);
		m_animator.SetBool("Skill2", false);
		m_animator.SetBool("Run Forward", false);
	}

	virtual protected void UpdateRun()
	{
		m_animator.SetBool("Skill", false);
		m_animator.SetBool("Skill2", false);
		m_animator.SetBool("Run Forward", true);
	}

	virtual protected void UpdateDie()
	{
		m_animator.SetBool("Run Forward", false);
		m_animator.SetBool("Skill", false);
		m_animator.SetBool("Skill2", false);
		m_animator.SetTrigger("Die");		
	}

	virtual protected void UpdateSkill()
	{
		if (m_Modeltype == MODEL_TYPE.ORC || m_Modeltype == MODEL_TYPE.BEAR)
		{
			if (Status_MP >= Status_MaxMP)
			{
				m_animator.SetBool("Skill", false);
				if (m_animator.GetBool("Skill2") == false)
					m_animator.SetBool("Skill2", true);
			}
			else
			{
				if (m_animator.GetBool("Skill") == false)
					m_animator.SetBool("Skill", true);
				m_animator.SetBool("Skill2", false);
			}
		}
		else
			m_animator.SetBool("Skill", true);

		m_animator.SetBool("Run Forward", false);
	}

	virtual public void CharacterInit()
	{
		this.State = STATE.IDLE;
		this.Target = null;
		this.PreTarget = null;
		m_IsOneShot = false;

		// ���� ����Ʈ�� ON
		int EffectCnt = m_WeaponEffect.Length;
		if (EffectCnt != 0)
		{
			foreach(GameObject Effect in m_WeaponEffect)
			{
				Effect.SetActive(true);
			}
		}

		// Status_Bar UI ON
		m_StatusHpBar.gameObject.SetActive(true);
		m_StatusMpBar.gameObject.SetActive(true);
	}
}

#endregion

#region STAR
public partial class Character
{
	public enum Unit_Star
	{
		ONE = 1,
		TWO = 2,
		THREE = 3,
		STAR_END = 4
	}

	[SerializeField]
    Unit_Star m_star = Unit_Star.ONE;
    public Unit_Star UnitStar { get { return m_star; } set { m_star = value; } }
	public int StarInt()
	{
		switch(m_star)
		{
            case Unit_Star.ONE:
				return 0;
			case Unit_Star.TWO:
				return 1;
			case Unit_Star.THREE:
				return 2;
        }
		return 3;
    }
    virtual public void StarUp(HYJ_Battle_Tile _tile)
	{
        //int tmp = (int)STAR.THREE;

        List<List<Dictionary<string, object>>> Unit_csv = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);

		if (_tile != null)
		{
			if (_tile.GetComponent<HYJ_Battle_Tile>().tile_type == HYJ_Battle_Tile.Tile_Type.Field 
				&& on_Tile.GetComponent<HYJ_Battle_Tile>().tile_type == HYJ_Battle_Tile.Tile_Type.Stand)
				HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__STAND_TO_FIELD, this.gameObject);

            this.LSY_Character_Set_OnTile(_tile.gameObject);
            this.transform.position = _tile.transform.position;

			_tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = this.gameObject;
            //Debug.Log(_tile.name + " 's onUnit : " + _tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit);
        }


        switch (m_star)
		{
			case Unit_Star.ONE:
				m_star = Unit_Star.TWO;
				this.transform.localScale *= 1.05f;
                HYJ_Status_SettingData(Unit_csv[1][Status_idx]);

                // Sound : �⹰ 2��
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.UNIT_2STAR);

                break;
            case Unit_Star.TWO:
                m_star = Unit_Star.THREE;
                this.transform.localScale *= 1.05f;
                HYJ_Status_SettingData(Unit_csv[2][Status_idx]);

                // Sound : �⹰ 2��
                HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.UNIT_3STAR);

                break;
        }

    }
}

#endregion

#region SKILL
public partial class Character
{
	[Header("======================================= SKILL =======================================")]
	[Space(10f)]
	[SerializeField] protected int Skill_Name;
}
#endregion
