using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public partial class HYJ_Battle_Tile : MonoBehaviour
{
    //(11/4) 상윤 수정 사항 : 기존 타일은 HYJ_Character 스크립트를 갖고 있는 유닛을 감지하였으나, 모든 유닛이 동일한 스크립트를 갖고 있지 않음.
    // 상속받은 스크립트를 감지하는 방법은 알 수 없으므로 태그로 구분하던지, 일단은 GameObject형으로 수정.

    //[SerializeField] HYJ_Character Basic_onUnit;    // 타일위에 올라가 있는 유닛
    [SerializeField] GameObject Basic_onUnit;   // GameObject 형으로 교체 -> 유닛이 모두 HYJ_Character 스크립트를 갖고 있지 않음.
    [SerializeField] public List<int> Tile_Idx; // Tile의 행/열 정보
    [SerializeField] public Vector3 Tile_Position;  // Tile의 localPosition 저장
    public enum Tile_Available
    {
        Available,
        Non_Available
    }
    [SerializeField] public Tile_Available tile_Available = Tile_Available.Non_Available;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public GameObject HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }

    //////////  Default Method  //////////
    void Start()
    {
        Tile_Position = this.transform.localPosition;
    }

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
        if(tile_Available == Tile_Available.Non_Available)
        {
            // 둘 수 있는 타일인가, 아닌가,, 아니라면 원래 자리로 돌아가라.
            other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
            Debug.Log("isEmpty");
            detectedUnit.Add(other.gameObject);
            Basic_onUnit = other.GetComponent<GameObject>();


        }
        else
        {
            switch (other.tag)
            {
                case "Ally":
                    if (Basic_onUnit == null && detectedUnit.Count == 0/* && other.CompareTag("Ally")*/)
                    {
                        Debug.Log("isEmpty");
                        detectedUnit.Add(other.gameObject);
                        Basic_onUnit = other.gameObject;

                        other.gameObject.transform.position = this.gameObject.transform.position; // ->  여기서 pos 변경 시키는거 배제해야하나..?
                        other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(this.gameObject);

                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_POSITION, Tile_Idx);
                        //Debug.Log(other.gameObject.transform.position + "<-unit // tile->" + this.gameObject.transform.position);
                    }
                    else // 이미 다른 유닛이 있을 경우 == 겹치는 경우
                    {
                        Debug.Log("isOverlap " + other.name + " ");
                        /*
                        //other.gameObject.transform.position = other.gameObject.GetComponent<Character>().LSY_Unit_Position;
                        // LSY : overlap 시, 기존 자리로 찾아가게 해놨는데 tile의 부모의 부모의 부모가 Battle 컴포넌트라 코드가 좀 더러움. 일단 작동은 하니까.
                        // oriPos를 Character도 프로퍼티로 갖고 있고, DragDrop.cs도 갖고 있는데 Character를 활용하려면 갱신을 계속 해줘야하는데 귀찮고.. (업데이트에 둘 가치는 없어 보이고, 위치 바꿀 때 프로퍼티로 set 하면 되긴 할듯?)
                        // DragDrop으로 지금처럼 하면 편하긴 한데, 이건 DragDrop이 현재 건드리는 객체가 하나여야함. 만약 복수의 유닛을 이동시키면 작동안함. 물론 마우스는 하나니까 문제는 없을거같은데 일단 문제가 있을 순 있음.
                        */
                        other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
                        other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);

                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                    }

                    break;
                case "HitArea":
                    break;
            }
        }
        /*
        //if(Basic_onUnit == null && detectedUnit.Count == 0 && other.CompareTag("Ally"))
        //{
        //    Debug.Log("isEmpty");
        //    detectedUnit.Add(other.gameObject);
        //    //Basic_onUnit = other.GetComponent<HYJ_Character>();
        //    Basic_onUnit = other.gameObject;

        //    other.gameObject.transform.position = this.gameObject.transform.position;
        //    Debug.Log(other.gameObject.transform.position + " " + this.gameObject.transform.position);
        //}
        //else
        //{
        //    Debug.Log("isOverlap " + other.name);
        //    other.gameObject.transform.position = other.gameObject.GetComponent<Character>().LSY_Unit_Position;
        //}
        */
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Ally":
                detectedUnit.Remove(other.gameObject);
                other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);
                //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                Basic_onUnit = null;
                break;

            case "HitArea":
                break;

        }
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