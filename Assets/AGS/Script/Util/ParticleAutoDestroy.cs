using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour {

    public enum DESTROY_TYPE
    {
        Destroy,
        Inactive,
    }
    [SerializeField]
    DESTROY_TYPE m_destroy = DESTROY_TYPE.Inactive; //풀에 환원하는게 원칙이라 기본값 Inactive
    [SerializeField] 
    float m_lifeTime; //안꺼지고 Loop도는 파티클들때문에 안꺼지는 파티클을 제어하기위해 LifeTime설정 버프 (지속시간) 같은거에서 사용
                      //LifeTime이있다면 이 변수로 제어  없다면 그냥 isPlaying으로 제어
    float m_curLifeTime;
    ParticleSystem[] m_particles;
    
    void DestroyParticles()
    {
        switch(m_destroy)
        {
            case DESTROY_TYPE.Destroy:
                Destroy(gameObject);
                break;
            case DESTROY_TYPE.Inactive:
                gameObject.SetActive(false);
                break;
        }
    }



	// Use this for initialization
	void Start () {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        if (m_lifeTime > 0) //Inactive 경우인데 Pool과 연동해야됨
        {
            m_curLifeTime += Time.deltaTime;
            if (m_curLifeTime >= m_lifeTime)
            {
                DestroyParticles();
                m_curLifeTime = 0f;
            }
        }
        else
        {
            bool isPlay = false;
            for (int i = 0; i < m_particles.Length; i++)
            {
                if (m_particles[i].isPlaying) // 파티클 재생중인지 체크가능
                {
                    isPlay = true;
                    break;
                }
            }
            if (!isPlay)
            {
                DestroyParticles();
            }
        }
	}
}
