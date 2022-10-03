using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitModelingObjectPool : MonoBehaviour
{
    Transform tr;
    GameObject go;
    
    public bool activeSelf { get => go.activeSelf; }

    Dictionary<string, Queue<UnitModelingObject>> modelingObjectsDic = new Dictionary<string, Queue<UnitModelingObject>>(); 

    private GameObject tempObject;

    void Awake()
    {
        go = gameObject;
        tr = GetComponent<Transform>();
        modelingObjectsDic.Clear();
    }

    public GameObject CallUnitModelingObject(int unitID, Transform parent)
    {
        if (!modelingObjectsDic.ContainsKey(UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName))
            modelingObjectsDic.Add(UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName, new Queue<UnitModelingObject>());

        if (!ChkModelingObjectQueue(UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName))
        {
            tempObject = Instantiate(UnitManager.Instance.GetUnitData(unitID).unitModelingObjectPrefab, transform);
            tempObject.AddComponent<UnitModelingObject>();
            tempObject.name = UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName;

            modelingObjectsDic[UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName].Enqueue(tempObject.GetComponent<UnitModelingObject>());


            tempObject = null;
        }

        modelingObjectsDic[UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName].Peek().SetUnitObject(parent);

        return modelingObjectsDic[UnitManager.Instance.GetUnitData(unitID).unitModelingObjectName].Dequeue().GetGameObject();
    }

    private bool ChkModelingObjectQueue(string key)
    {
        return !modelingObjectsDic[key].Count.Equals(0);
    }

    public void EnqueueObject(UnitModelingObject unitModelingObject)
    {
        unitModelingObject.GetTransform().parent = tr;
        modelingObjectsDic[unitModelingObject.GetGameObject().name].Enqueue(unitModelingObject);
    }
}
