using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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

	public void AddMP()
	{
		gameObject.GetComponentInParent<Character>().Stat_MP += 10.0f;
	}

	public void CreateProjectile(EFFECT_TYPE type)
	{

		//EffectPool.Instance.Create(type, Pos);
	}

	public void SetDisableCollider()
	{
		Collider.enabled = false;
	}
}
