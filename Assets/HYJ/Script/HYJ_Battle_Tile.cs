using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Battle_Tile : MonoBehaviour
{
    [SerializeField] HYJ_Character Basic_onUnit;    // 타일위에 올라가 있는 유닛

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public HYJ_Character HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}


partial class HYJ_Battle_Tile : MonoBehaviour
{
    //[Header("==================================================")]
    //[Header("Unit Detect")]

    [SerializeField]
    private List<GameObject> detectedUnit = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(Basic_onUnit == null && detectedUnit.Count == 0)
        {
            Debug.Log("isEmpty");
            detectedUnit.Add(other.gameObject);
            Basic_onUnit = other.GetComponent<HYJ_Character>();

            other.gameObject.transform.position = this.gameObject.transform.position;
        }
        else
        {
            Debug.Log("isOverlap");
            other.gameObject.transform.position = other.gameObject.GetComponent<HYJ_Character>().LSY_Unit_Position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        detectedUnit.Remove(other.gameObject);
        Basic_onUnit = null;
    }

    private GameObject DetectUnitObject()
    {
        GameObject near_obj = null;

        detectedUnit.ForEach ((obj) =>
        {
            if (near_obj == null)
            {
                near_obj = obj;
            }
            else if (Vector3.Distance(near_obj.transform.position, transform.position) >
            Vector3.Distance(obj.transform.position, transform.position))
            {
                near_obj = obj;
            }
        });

        return near_obj;
    }

}