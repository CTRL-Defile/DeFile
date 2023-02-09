using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    GameObject m_Target;

    [SerializeField]
    Vector3 m_CurPos = Vector3.zero;
    [SerializeField]
    float m_Speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        
	}

    // Update is called once per frame
    void Update()
    {
		transform.position = Vector3.MoveTowards(transform.position, m_Target.transform.position, m_Speed * Time.deltaTime);
	}

	void SetUpProjectile(GameObject obj, float speed = 5.0f)
	{
		m_Target = obj;
        m_Speed = speed;
	}
}
