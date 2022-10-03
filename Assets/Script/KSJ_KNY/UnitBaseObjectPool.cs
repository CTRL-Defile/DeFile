using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBaseObjectPool : MonoBehaviour
{
    Transform tr;
    GameObject go;

    [SerializeField] float startCreateCount;

    Queue<UnitBaseObject> unitBaseObjectQueue = new Queue<UnitBaseObject>();

    GameObject unitBaseObject;

    GameObject tempObject;

    void Awake()
    {
        unitBaseObjectQueue.Clear();
        unitBaseObject = Resources.Load("UnitPrefab/Unit/Unit") as GameObject;

        for (int i = 0; i < startCreateCount; i++)
        {
            unitBaseObjectQueue.Enqueue(Instantiate(unitBaseObject, transform).GetComponent<UnitBaseObject>());
        }

    }


    public void CallUnitBaseObject(int unitID, Vector3 pos, float lookDirection, eTag tag, Transform parent)
    {
        if(ChkUnitBaseObjectQueue())
        {
            tempObject = Instantiate(unitBaseObject, transform);
            unitBaseObjectQueue.Enqueue(tempObject.AddComponent<UnitBaseObject>());

            tempObject = null;
        }

        unitBaseObjectQueue.Peek().name = UnitManager.Instance.GetUnitData(unitID).nameKor;
        unitBaseObjectQueue.Dequeue().SetUnitObject(unitID, pos, lookDirection, tag, parent);
    }

    private bool ChkUnitBaseObjectQueue()
    {
        return unitBaseObjectQueue.Count.Equals(0);
    }

    public void EnqueueObject(UnitBaseObject unitBaseObject)
    {
        unitBaseObject.GetTransform().parent = tr;
        unitBaseObjectQueue.Enqueue(unitBaseObject);
    }
}
