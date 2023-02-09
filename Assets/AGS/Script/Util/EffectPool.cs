using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : SingletonMonoBehaviour<EffectPool>
{	
    [SerializeField]
    private GameObject[] effects;

	Dictionary<EFFECT_TYPE, List<EffectPoolUnit>> m_dicEffectPool = new Dictionary<EFFECT_TYPE, List<EffectPoolUnit>>();
	int m_presetSize = 1; //몇개 생길지모르지만 기본적으로 1개 만들어놈

	void LoadEffect()
    {
        int size = effects.Length;
		EFFECT_TYPE effect_type = EFFECT_TYPE.EFFECT_SPARK;

		for (int i = 0; i < size; i++)
        {
			var Types = Enum.GetValues(typeof(EFFECT_TYPE));

			effect_type = (EFFECT_TYPE)Types.GetValue(i);

			List<EffectPoolUnit> listObjectPool = new List<EffectPoolUnit>(); //인스턴스 이팩트하나당 1개의 풀
			m_dicEffectPool[effect_type] = listObjectPool;
				
			GameObject obj = Instantiate(effects[i]);
			obj.layer = LayerMask.NameToLayer("TransparentFX");

			EffectPoolUnit objectPoolUnit = obj.GetComponent<EffectPoolUnit>();
			if (objectPoolUnit == null)
			{
				obj.AddComponent<EffectPoolUnit>();
			}

			if (obj.GetComponent<ParticleAutoDestroy>() == null)
			{
				obj.AddComponent<ParticleAutoDestroy>();
			}

			obj.transform.SetParent(transform);

			EFFECT_TYPE type = obj.GetComponent<EffectPoolUnit>().EffectType;

			obj.GetComponent<EffectPoolUnit>().SetObjectPool(type, this);
			if (obj.activeSelf)
			{
				//아직 이 이팩트는 풀에들어가있는 상태가아님 엑티브끄면 OnDisable 이벤트가 동작됨
				obj.SetActive(false);
			}
			else
			{
				AddPoolUnit(type, obj.GetComponent<EffectPoolUnit>());
			}
		}

	}
	public GameObject Create(EFFECT_TYPE effectType)
	{
		return Create(effectType, Vector3.zero, Quaternion.identity);
	}
	public GameObject Create(EFFECT_TYPE effectType, Vector3 pos)
	{
		return Create(effectType, pos, Quaternion.identity);
	}

	public GameObject Create(EFFECT_TYPE effectType, Vector3 position, Quaternion rotation)
	{
		List<EffectPoolUnit> listObjectPool = m_dicEffectPool[effectType];
		if (listObjectPool == null)
		{
			return null;
		}

		if (listObjectPool.Count > 0)
		{
			if (listObjectPool[0] != null && listObjectPool[0].IsReady())//0번도 준비가 안되면 나머지는 무조건 안되있기떄문에 0번검사
			{
				EffectPoolUnit unit = listObjectPool[0];
				listObjectPool.Remove(listObjectPool[0]);
				unit.transform.position = position;
				unit.transform.rotation = rotation;
				StartCoroutine(Coroutine_SetActive(unit.gameObject));
				return unit.gameObject;
			}
		}

		GameObject obj = Instantiate(effects[(int)effectType]);
		obj.layer = LayerMask.NameToLayer("TransparentFX");

		EffectPoolUnit objectPoolUnit = obj.GetComponent<EffectPoolUnit>();
		if (objectPoolUnit == null)
		{
			obj.AddComponent<EffectPoolUnit>();
		}
		if (obj.GetComponent<ParticleAutoDestroy>() == null)
		{
			obj.AddComponent<ParticleAutoDestroy>();
		}
		obj.GetComponent<EffectPoolUnit>().SetObjectPool(effectType, this);
		StartCoroutine(Coroutine_SetActive(obj));
		return obj;
	}

	IEnumerator Coroutine_SetActive(GameObject obj)
	{
		yield return new WaitForEndOfFrame();
		obj.SetActive(true);
	}

	public void AddPoolUnit(EFFECT_TYPE effectType, EffectPoolUnit unit)
	{
		List<EffectPoolUnit> listObjectPool = m_dicEffectPool[effectType];
		if (listObjectPool != null)
		{
			listObjectPool.Add(unit);
		}
	}

	// Use this for initialization
	protected override void OnStart()
	{
		base.OnStart();
		LoadEffect();
	}
	// Update is called once per frame
	void Update()
	{

	}
}
