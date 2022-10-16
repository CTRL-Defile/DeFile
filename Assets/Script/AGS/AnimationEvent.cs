using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField]
    Collider Collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetEnableCollider()
	{
		Collider.enabled = true;
	}

	public void SetDisableCollider()
	{
		Collider.enabled = false;
	}
}
