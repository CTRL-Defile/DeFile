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
    //-------------------------------------------------------------------
    // Field
    //-------------------------------------------------------------------
    [SerializeField]
    BATTLE_PHASE m_Basic_phase;
	[SerializeField]
    protected Vector3 char_ori_Pos; // Update/MouseUp 에서 갱신
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
	protected UI_StatusBar m_StatusBar = null;

	//-------------------------------------------------------------------
	// Property
	//-------------------------------------------------------------------
	public Vector3 LSY_Character_OriPos { get { return char_ori_Pos; } set { char_ori_Pos = value; } }
	public GameObject Target { get { return m_Target; } set { m_Target = value; } }
	public GameObject PreTarget { get { return m_PreTarget; } set { m_PreTarget = value; } }
	public bool Dead { get { return IsDead; } set { IsDead = value; } }
	//public int CurPosIndex { get { return m_CurPosIndex; } set { m_CurPosIndex = value; } }
	public UI_StatusBar STATUS_BAR { get { return m_StatusBar; } set { m_StatusBar = value; } }

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

        // HP 테스트 용 초기화
        Status_MaxMP = 60.0f;
		Status_MP = 0.0f;


		switch (m_AnimEvent.Anim_Type)
		{
			case ANIM_TYPE.BEAR:
				Status_atk = 5.0f;
				Status_MaxHP = 100.0f;
				Status_HP = Status_MaxHP;
				break;
			case ANIM_TYPE.ORC:
				Status_atk = 7.0f;
				Status_MaxHP = 120.0f;
				Status_HP = Status_MaxHP;
				break;
			case ANIM_TYPE.EVIL:
				Status_atk = 9.0f;
				Status_MaxHP = 140.0f;
				Status_HP = Status_MaxHP;
				break;
			case ANIM_TYPE.GOBLIN_T:
				Status_MaxHP = 160.0f;
				Status_HP = Status_MaxHP;
				Status_atk = 11.0f;
				break;
			case ANIM_TYPE.GOBLIN_N:
				Status_MaxHP = 160.0f;
				Status_HP = Status_MaxHP;
				Status_atk = 13.0f;
				break;
			case ANIM_TYPE.WERERAT:
				Status_MaxHP = 180.0f;
				Status_HP = Status_MaxHP;
				Status_atk = 15.0f;
				break;
		}
        
		Status_moveSpeed = 5.0f;
		// Stat_MoveSpeed = UnityEngine.Random.Range(1.0f, 8.0f);
		//CurPosIndex = 0;
	}

	private void Awake()
	{
		m_animator = GetComponentInChildren<Animator>();
		m_PathFinder = GetComponent<PathFinder>();
		m_AnimEvent = GetComponentInChildren<AnimationEvent>();
		m_StatusBar = GetComponentInChildren<UI_StatusBar>();
	}

	private void LateUpdate()
    {
        m_Basic_phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
        // char_ori_Pos 는 다음 전투 혹은 드래그 드랍 시 되돌아가는 위치이므로, 전투 상태일 때는 초기화할 필요가 없음.
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

// 캐릭터의 능력치
// 버프나 아이템, 전투 등의 외부요소로 인해 변하는 수치들을 적재해주기 위해 사용되는 변수들
#region STATUS

public partial class Character
{
    //-------------------------------------------------------------------
    // Field
    //-------------------------------------------------------------------
    [Header("======================================= STATUS =======================================")]
	[Space (10f)]	

	[SerializeField] protected float Status_HP;     // 체력
    [SerializeField] protected float Status_MaxHP;  // 최대체력
	[Space(10f)]

	[SerializeField] protected float Status_MP;     // 마나
    [SerializeField] protected float Status_MaxMP;  // 최대마나
	[Space(10f)]

	[SerializeField] protected float Status_atk;    // 공격력
    [SerializeField] protected int Status_magic;  // 마력
	[Space(10f)]

	[SerializeField] protected int Status_atkSpeed;   // 공속
	[SerializeField] protected float Status_moveSpeed;   // 공속
	[Space(10f)]

	[SerializeField] protected int Status_critValue;  // 치명타 수치
    [SerializeField] protected int Status_critPer;    // 치명타 확률
    [Space(10f)]

	[SerializeField] protected int Status_Cost;

    //-------------------------------------------------------------------
    // Property
    //-------------------------------------------------------------------
    public float Stat_HP { get { return Status_HP; } set { Status_HP = value; } }
	public float Stat_MP { get { return Status_MP; } set { Status_MP = value; } }
	public float Stat_MaxHP { get { return Status_MaxHP; } set { Status_MaxHP = value; } }
	public float Stat_MaxMP { get { return Status_MaxMP; } set { Status_MaxMP = value; } }
	public float Stat_Attack { get { return Status_atk; } set { Status_atk = value; } }
	public float Stat_MoveSpeed { get { return Status_moveSpeed; } set { Status_moveSpeed = value; } }
	public int Stat_Cost { get { return Status_Cost; } set { Status_Cost = value; } }

	//-------------------------------------------------------------------
	// Method
	//-------------------------------------------------------------------
	virtual public void HitProcess(float Attack)
    {
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
		else if(Status_HP <= 0.0f && State != STATE.DIE)
		{
			List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);
			BattleGraph[on_Tile.GetComponent<HYJ_Battle_Tile>().GraphIndex].Marking = false;
			//m_PathFinder.InitMarking();			
			on_Tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;
			State = STATE.DIE;			
		}
		
		// Dissolve Shader 적용 예정
		// 일단 조절 값 없어서 Die 애니메이션 끝났을 때 IsDead true로
		if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
		  m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
		{
			on_Tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;
			gameObject.SetActive(false);
			IsDead = true;
		}
		
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT_DIE, this.gameObject);
	}

	virtual public void BattleProcess()
	{
		if (null == Target)
			State = STATE.IDLE;

		if (PreTarget != Target)
		{			
			m_PathFinder.InitCloseNodes();
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
						State = STATE.IDLE;
						m_PathFinder.InitCloseNodes();
						m_PathFinder.InitMarking();
						break;
					case STATE.SKILL_IDLE:
						Dir = Target.transform.position - transform.position;
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Dir), 5.0f * Time.deltaTime);

						if (Target.GetComponent<Character>().State == STATE.SKILL)
						{
							m_PathFinder.InitCloseNodes();
							m_PathFinder.InitMarking();
							State = STATE.IDLE;
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
		m_animator.SetBool("Run Forward", false);
	}

	virtual protected void UpdateRun()
	{
		m_animator.SetBool("Skill", false);
		m_animator.SetBool("Run Forward", true);
	}

	virtual protected void UpdateDie()
	{
		m_animator.SetBool("Run Forward", false);
		m_animator.SetBool("Skill", false);
		m_animator.SetTrigger("Die");		
	}

	virtual protected void UpdateSkill()
	{
		m_animator.SetBool("Skill", true);
		m_animator.SetBool("Run Forward", false);
	}

	virtual public void CharacterInit()
	{
		this.State = STATE.IDLE;
		this.Target = null;
		this.PreTarget = null;
	}
}

#endregion
