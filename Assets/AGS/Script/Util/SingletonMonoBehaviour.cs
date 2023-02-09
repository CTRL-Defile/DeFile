using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    static public T Instance { get; private set; }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = (T)this;            
            OnAwake();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Use this for initialization
    void Start () {
		if(Instance == (T)this)
        {
            OnStart();
        }
	}
	
    virtual protected void OnAwake()
    {

    }

    virtual protected void OnStart()
    {

    }


	// Update is called once per frame
	void Update () {
		
	}
}
