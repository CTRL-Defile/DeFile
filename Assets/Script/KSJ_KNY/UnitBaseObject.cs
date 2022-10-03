using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
[RequireComponent(typeof(UnitStatus))]

public class UnitBaseObject : MonoBehaviour
{
    Transform tr;
    GameObject go;
    UnitController _unitController;

    void Awake()
    {
        go = gameObject;
        tr = go.GetComponent<Transform>();
    }

    public void SetUnitObject(int unitID, Vector3 pos, float lookDirection, eTag tag, Transform parent)
    {
        if (!go.activeSelf)
            go.SetActive(true);

        go.tag = tag.ToString();
        tr.parent = parent;
        tr.position = pos;
        tr.localRotation = Quaternion.Euler(0.0f, lookDirection, 0.0f);

        GetComponent<UnitStatus>().SetUnitID(unitID);
    }

    public void ReturnUnitObject()
    {
        UnitManager.Instance.ReturnUnitModelingObject(_unitController.unitModelingObject.GetComponent<UnitModelingObject>());
        UnitManager.Instance.ReturnUnit(this, GetComponent<UnitController>());
        go.SetActive(false);
    }

    public GameObject GetGameObject() { return go; }
    public Transform GetTransform() { return tr; }
}
