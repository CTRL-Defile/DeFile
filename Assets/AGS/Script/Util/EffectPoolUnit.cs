using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //C# 시간관리사용하기위해 추가

public enum EFFECT_TYPE
{
    EFFECT_SPARK,
    EFFECT_FIREBALL,
    EFFECT_END
}

public class EffectPoolUnit : MonoBehaviour {


    public float m_delay = 1f; //풀에환원되고 적어도1초 지난것들 사용해야됨
    DateTime m_inactiveTime; //시관 관리 다됨 달력도 만들수있음 //Active껏을때의 시간 꺼졌을때부터 1초지난거체크위해 사용
    EffectPool m_objectPool;

    [SerializeField]
	private EFFECT_TYPE m_effectType = EFFECT_TYPE.EFFECT_END;

    public EFFECT_TYPE EffectType { get { return m_effectType; } set { m_effectType = value; } }

    public void SetObjectPool(EFFECT_TYPE effectType, EffectPool objectPool)
    {
		m_effectType = effectType; //어떤이팩트
        m_objectPool = objectPool; //어떤풀에서관리하는 이팩트
        ResetParent();
    }

    public bool IsReady()
    {
        if(!gameObject.activeSelf) //꺼져있으면 풀에 들어가있음을의미
        {
            TimeSpan timeSpan = DateTime.Now - m_inactiveTime; //현재시간 - 엑티브 껏을때 시간 //timeSpan으로 값이나옴
            if (timeSpan.TotalSeconds > m_delay)  //시간을 전체 시 / 분 / 초 로 나눠서 받을수있다. timeSpan.TotalSeconds 초로만 반환을 했을때 1초보다 크면
            {
                //엑티브가 꺼진지 1초이상 지나면 이펙트 여러개터트려도 문제가 발생안되서 1초조건걸음
                return true;
            }
        }
        return false;
    }

    public void ResetParent()
    {
        transform.SetParent(m_objectPool.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    private void OnDisable()
    {
        m_inactiveTime = DateTime.Now;
        m_objectPool.AddPoolUnit(m_effectType, this); //풀에다 넣어줌
    }

    private void OnDestroy()
    {
        //m_objectPool.RemovePoolUnit(m_effectName, this);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
