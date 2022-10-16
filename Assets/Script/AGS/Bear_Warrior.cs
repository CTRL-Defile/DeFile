using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bear_Warrior : HYJ_Character
{
	[SerializeField]
	private Animator animator;

	// Start is called before the first frame update
	void Start()
    {
		// HP 테스트 용 초기화
		Status_MaxHP = 100.0f;
		Status_HP = 100.0f;
		Status_atk = 10.0f;

		animator = GetComponentInChildren<Animator>();
		Basic_phase = 0;

		HYJ_Action_Init();
	}

    // Update is called once per frame
    void Update()
    {
		switch (Basic_phase)
		{
			case -1:
				{
					HYJ_Action_Update();
					Animation_Update();
				}
				break;
			//
			case 0:
				{
					int battlePhase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);

					if (battlePhase == -1)
					{
						HYJ_Astar_Init();

						//
						Basic_phase = -1;
					}
				}
				break;
		}
	}

	private void Animation_Update()
	{
		if(true == AnimParam.Idle)
		{
			animator.SetBool("Run Forward", AnimParam.Run);
			animator.ResetTrigger("Chop Attack");
			animator.SetTrigger("Idle");
		}
		else if(true == AnimParam.Attack)
		{
			animator.SetBool("Run Forward", AnimParam.Run);
			animator.ResetTrigger("Idle");
			animator.SetTrigger("Chop Attack");
		}
		else if(true == AnimParam.Run)
		{
			if(false == animator.GetBool("Run Forward"))
			{
				animator.SetBool("Run Forward", AnimParam.Run);
				animator.ResetTrigger("Idle");
				animator.ResetTrigger("Chop Attack");
			}
		}
	}

}
