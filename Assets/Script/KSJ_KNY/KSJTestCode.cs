using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSJTestCode : MonoBehaviour
{
    [System.Serializable]
    public struct UnitSetting
    {
        public int ID;
        public Vector3 pos;
        public eTag tag;
    }

    [SerializeField] private UnitSetting[] UnitArray;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < UnitArray.Length; i++)
        {
            UnitManager.Instance.CallUnit(UnitArray[i].ID, UnitArray[i].pos, UnitArray[i].tag);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
