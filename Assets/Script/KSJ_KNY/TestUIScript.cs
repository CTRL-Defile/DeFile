using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestUIScript : MonoBehaviour
{
    [SerializeField] Image hp;
    [SerializeField] Image mp;

    UnitStatus unitStatus;

    private void Start()
    {
        unitStatus = GetComponentInParent<UnitStatus>();
    }
    // Update is called once per frame
    void Update()
    {
        if(!transform.rotation.Equals(Camera.main.transform.rotation))
        {
            transform.rotation = Camera.main.transform.rotation;
        }
        if(unitStatus.nowHP.Equals(0.0f))
        {
            gameObject.SetActive(false);
        }

        hp.fillAmount = unitStatus.nowHP / unitStatus.maxHP;
        mp.fillAmount = unitStatus.nowMP / unitStatus.maxMP;
    }
}
