using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Projectile : MonoBehaviour
{
	[SerializeField]
	GameObject m_HostObject;
    [SerializeField]
    GameObject m_Target;

    [SerializeField]
    Vector3 m_CurPos = Vector3.zero;
    [SerializeField]
    float m_Speed = 10.0f;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == m_Target)
		{			
			//m_Target.GetComponentInParent<Character>().HitProcess(m_HostObject.GetComponent<Character>().Stat_Attack);			
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
	}

	// Update is called once per frame
    void Update()
    {
		if(null != m_Target)
		{
			float Offset = m_Target.GetComponent<CapsuleCollider>().height / 3.0f;
			Vector3 TargetPos = new Vector3(m_Target.transform.position.x, m_Target.transform.position.y + Offset, m_Target.transform.position.z);
			transform.position = Vector3.MoveTowards(transform.position, TargetPos, m_Speed * Time.deltaTime);

			if(transform.position == TargetPos)
			{
				m_Target.GetComponentInParent<Character>().HitProcess(m_HostObject.GetComponent<Character>().Character_Status_atkPhysics);

				ParticleSystem[] particles;
				particles = GetComponentsInChildren<ParticleSystem>();
				foreach(var particle in particles)
				{
					particle.Stop();
				}
				m_Target = null;
			}
		}
	}

	public void SetUpProjectile(GameObject hostObj, GameObject target, float speed = 10.0f)
	{
		m_HostObject = hostObj;
		m_Target = target;
        m_Speed = speed;
	}
}
