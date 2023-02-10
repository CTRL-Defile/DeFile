using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Collision : MonoBehaviour
{
	[SerializeField]
	Character HostObject;
	[SerializeField]
	GameObject Target;

	Collider Waeponcollider;
	private void OnTriggerEnter(Collider other)
	{
		BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
		if (phase == BATTLE_PHASE.PHASE_COMBAT)
		{			
			if(null != other.GetComponentInParent<Character>() &&
			   HostObject.UnitType != other.GetComponentInParent<Character>().UnitType)
			{
				if (other.gameObject.tag == "HitArea" && other.GetComponentInParent<Character>() != HostObject)
				{
					Target = other.gameObject;
					other.GetComponentInParent<Character>().HitProcess(HostObject.Stat_Attack);
				}
			}
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		Waeponcollider = GetComponent<Collider>();
		Waeponcollider.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }


}
