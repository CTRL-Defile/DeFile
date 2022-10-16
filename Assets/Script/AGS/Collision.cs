using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Collision : MonoBehaviour
{
	[SerializeField]
	HYJ_Character HostObject;
	[SerializeField]
	GameObject Target;

	Collider Waeponcollider;
	private void OnTriggerEnter(Collider other)
	{		
		if (other.gameObject.tag == "HitArea")
		{
			Target = other.gameObject;
			other.gameObject.GetComponentInParent<Transform>().GetComponentInParent<Transform>().gameObject.GetComponentInParent<HYJ_Character>().HitProcess(HostObject.Stat_Attack);
		}
	}

	// Start is called before the first frame update
	void Start()
    {
		Waeponcollider = GetComponent<SphereCollider>();
		Waeponcollider.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }


}
