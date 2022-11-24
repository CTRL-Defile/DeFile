using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using UnityEngine.EventSystems;

public partial class Character : MonoBehaviour
{
	//-------------------------------------------------------------------
	// Field
	//-------------------------------------------------------------------
	[SerializeField]
    protected Vector3 ori_Pos;
	[SerializeField]
	protected GameObject on_Tile;
	[SerializeField]
	protected bool IsDead = false;

	//-------------------------------------------------------------------
	// Property
	//-------------------------------------------------------------------
	public Vector3 LSY_Unit_Position { get { return ori_Pos; } set { ori_Pos = value; } }

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
		ori_Pos = new Vector3();
		// HP 테스트 용 초기화
		Status_MaxHP = 100.0f;
		Status_HP = 50.0f;
		//Status_atk = 10.0f;		
	}

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
	}

	// Update is called once per frame
	void Update()
    {
		if (IsDead)
		{
			// 풀링 할지 말지 생각중
			Destroy(gameObject);
			return;
		}

		if (Input.GetKeyDown(KeyCode.A))
		{
			state = STATE.RUN;
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			state = STATE.IDLE;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			state = STATE.SKILL;
		}
		else if (Input.GetKeyDown(KeyCode.F))
		{
			HitProcess(10.0f);
		}

		ChangeState();

		MoveProcess();
		DieProcess();

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

	[SerializeField] protected int Status_MP;     // 마나
    [SerializeField] protected int Status_MaxMP;  // 최대마나
	[Space(10f)]

	[SerializeField] protected float Status_atk;    // 공격력
    [SerializeField] protected int Status_magic;  // 마력
	[Space(10f)]

	[SerializeField] protected int Status_atkSpeed;   // 공속
	[Space(10f)]

	[SerializeField] protected int Status_critValue;  // 치명타 수치
    [SerializeField] protected int Status_critPer;    // 치명타 확률
    [Space(10f)]

	[SerializeField] protected int Status_Cost;

    //-------------------------------------------------------------------
    // Property
    //-------------------------------------------------------------------
    public float Stat_HP { get { return Status_HP; } set { Status_HP = value; } }
	public float Stat_MaxHP { get { return Status_MaxHP; } set { Status_MaxHP = value; } }
	public float Stat_Attack { get { return Status_atk; } set { Status_atk = value; } }
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
		else
			state = STATE.DIE;

		// Dissolve Shader 적용 예정
		// 일단 조절 값 없어서 Die 애니메이션 끝났을 때 IsDead true로
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Die") &&
		  animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
		IsDead = true;

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
	public enum STATE
	{
		IDLE,
		RUN,
		DIE,
		SKILL,
		STATE_END
	}

	//-------------------------------------------------------------------
	// Field
	//-------------------------------------------------------------------
	[Header("======================================= STATE =======================================")]
	[Space(10f)]

	[SerializeField]
	protected Animator animator;

	[SerializeField]
	protected STATE state = STATE.IDLE;

	//-------------------------------------------------------------------
	// Property
	//-------------------------------------------------------------------



	//-------------------------------------------------------------------
	// Method
	//-------------------------------------------------------------------
	virtual protected void ChangeState()
	{
		switch (state)
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
			default:
					break;
		}
	}

	virtual protected void UpdateIdle()
	{
		animator.ResetTrigger("Skill");
		animator.SetBool("Run Forward", false);
	}

	virtual protected void UpdateRun()
	{
		animator.ResetTrigger("Skill");
		animator.SetBool("Run Forward", true);
	}

	virtual protected void UpdateDie()
	{
		animator.SetBool("Run Forward", false);
		animator.ResetTrigger("Skill");
		animator.SetTrigger("Die");		
	}

	virtual protected void UpdateSkill()
	{		
		animator.SetTrigger("Skill");
	}

}

#endregion
