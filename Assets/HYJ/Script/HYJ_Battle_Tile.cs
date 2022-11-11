using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Battle_Tile : MonoBehaviour
{
    //(11/4) 상윤 수정 사항 : 기존 타일은 HYJ_Character 스크립트를 갖고 있는 유닛을 감지하였으나, 모든 유닛이 동일한 스크립트를 갖고 있지 않음.
    // 상속받은 스크립트를 감지하는 방법은 알 수 없으므로 태그로 구분하던지, 일단은 GameObject형으로 수정.

    //[SerializeField] HYJ_Character Basic_onUnit;    // 타일위에 올라가 있는 유닛
    [SerializeField] GameObject Basic_onUnit;   // GameObject 형으로 교체 -> 유닛이 모두 HYJ_Character 스크립트를 갖고 있지 않음.

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public GameObject HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }
    //public HYJ_Character HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }


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
    [Header("==================================================")]
    [Header("Unit Detect")]

    [SerializeField]
    private List<GameObject> detectedUnit = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(Basic_onUnit == null && detectedUnit.Count == 0 && other.CompareTag("Ally"))
        {
            Debug.Log("isEmpty");
            detectedUnit.Add(other.gameObject);
            //Basic_onUnit = other.GetComponent<HYJ_Character>();
            Basic_onUnit = other.gameObject;

            other.gameObject.transform.position = this.gameObject.transform.position;
        }
        else
        {
            Debug.Log("isOverlap");
            //other.gameObject.transform.position = other.gameObject.GetComponent<HYJ_Character>().LSY_Unit_Position;
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