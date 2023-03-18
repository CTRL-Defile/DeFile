using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AnimationEvent : MonoBehaviour
{

	[SerializeField]
    Collider Collider;

	// Projectile
	[SerializeField]
	EFFECT_TYPE Effect_Type;
	[SerializeField]
	Transform FirePos;

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

	public void CreateProjectile()
	{
		GameObject obj = EffectPool.Instance.Create(Effect_Type, FirePos.position);
		if (obj.GetComponent<Projectile>() == null)
			obj.AddComponent<Projectile>();
		obj.GetComponent<Projectile>().SetUpProjectile(transform.parent.gameObject, gameObject.transform.parent.GetComponent<Character>().Target);
	}

	public void SetProjectileOption(EFFECT_TYPE type, Vector3 Pos)
	{
		Effect_Type = type;
		FirePos.position = Pos;
	}

	public void SetDisableCollider()
	{
		Collider.enabled = false;
	}

	public void TargetSkillHit()
	{
		Debug.Log("[AnimEvent] " + this.transform.parent.name);
		GameObject HostObj = transform.parent.gameObject;
		GameObject Target = HostObj.GetComponent<Character>().Target;
		if(HostObj != null && Target != null)
		{
			Target.GetComponent<Character>().HitProcess(HostObj.GetComponent<Character>().Spell_1.HYJ_Data_damageValue);
			HostObj.GetComponent<Character>().Stat_MP = 0.0f;
		}
	}
}
