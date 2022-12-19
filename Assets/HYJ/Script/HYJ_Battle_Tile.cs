using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public partial class HYJ_Battle_Tile : MonoBehaviour
{
    //(11/4) 상윤 수정 사항 : 기존 타일은 HYJ_Character 스크립트를 갖고 있는 유닛을 감지하였으나, 모든 유닛이 동일한 스크립트를 갖고 있지 않음.
    // 상속받은 스크립트를 감지하는 방법은 알 수 없으므로 태그로 구분하던지, 일단은 GameObject형으로 수정.

    [SerializeField] GameObject Basic_onUnit = null;   // GameObject 형으로 교체 -> 유닛이 모두 HYJ_Character 스크립트를 갖고 있지 않음.
    [SerializeField] public List<int> Tile_Idx; // Tile의 행/열 정보
    [SerializeField] public Vector3 Tile_Position;  // Tile의 localPosition 저장
    [SerializeField] private int m_GraphIdx = -1;
    public enum Tile_Available
    {
        Available,
        Non_Available
    }
    [SerializeField] public Tile_Available tile_Available = Tile_Available.Non_Available;

    //////////  Getter & Setter //////////
    public int GraphIndex { get { return m_GraphIdx; } set { m_GraphIdx = value; } }

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
		 BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
        if (phase == BATTLE_PHASE.PHASE_COMBAT)
        {
            if(null == Basic_onUnit)
			    Basic_onUnit = other.gameObject;
        }
        else
        {
            switch (other.tag)
            {
                case "Ally":
                    Ally_Enter(other);
                    break;

                case "Enemy":
                    Enemy_Enter(other);
                    break;
            }
        }

	}

	private void OnTriggerExit(Collider other)
    {
		BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
		if (phase == BATTLE_PHASE.PHASE_COMBAT)
		{			
			Basic_onUnit = null;
		}
        else
        {
			switch (other.tag)
			{
				case "Ally":
					Ally_Exit(other);
					break;

				case "Enemy":
					Enemy_Exit(other);
					break;
			}
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

    private void Ally_Enter(Collider other)
    {
        if (tile_Available == Tile_Available.Non_Available)
        {
            other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
        }
        else
        {
            string Tag_UnitTile;
            if (other.GetComponent<Character>().LSY_Character_Get_OnTile() == null)
                Tag_UnitTile = "StandTile";
            else
                Tag_UnitTile = other.GetComponent<Character>().LSY_Character_Get_OnTile().tag;

            int Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);
            int cnt_FieldUnit = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___COUNT__FIELD_UNIT);
            // stand->stand, stand->field, field->field, field->stand & [trash]
            switch (Tag_UnitTile)
            {
                case "StandTile":
                    if (this.tag == "StandTile")
                    {
                        Move_Unit(other);
                    }
                    else if (this.tag == "FieldTile")
                    {
                        if (cnt_FieldUnit < Player_Lv + 1)
                        {
                            Debug.Log(Player_Lv + " Lv... and " + cnt_FieldUnit + " of Ally is on tile.");
                            Move_Unit(other);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__STAND_TO_FIELD, other.gameObject);
                        }
                        else
                        {
                            // 필드 위 유닛 개수 초과 시,
                            other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
                        }
                    }
                    else if (this.tag == "TrashTile")
                    {
                        //Debug.Log("Trash Collider Enter");
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, Tag_UnitTile, other.gameObject);
                    }
                    break;

                case "FieldTile":
                    if (this.tag == "FieldTile")
                    {
                        Move_Unit(other);
                    }
                    else if (this.tag == "StandTile")
                    {
                        Debug.Log(Player_Lv + " Lv... and " + cnt_FieldUnit + " of Ally is on tile.");
                        Move_Unit(other);
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__FIELD_TO_STAND, other.gameObject);
                    }
                    else if (this.tag == "TrashTile")
                    {
                        //Debug.Log("Trash Collider Enter");
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, Tag_UnitTile, other.gameObject);
                    }
                    break;

                //case "TrashTile":
                //    {
                //        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, this.tag, other.gameObject);
                //    }
                //    break;

            }
        }

        // 영재가 추가
        BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
        if (phase == BATTLE_PHASE.PHASE_PREPARE)
        {
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE);
        }
    }
    private void Ally_Exit(Collider other)
    {
        switch (other.tag)
        {
            case "Ally":
                //detectedUnit.Remove(other.gameObject);
                //other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);
                //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                Basic_onUnit = null;
                break;

            case "HitArea":
                break;
        }
    }
    private void Enemy_Enter(Collider other)
    {
        //detectedUnit.Add(other.gameObject);
        Basic_onUnit = other.gameObject;
        other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(this.gameObject);
    }
    private void Enemy_Exit(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                //detectedUnit.Remove(other.gameObject);
                Basic_onUnit = null;
                break;

            case "HitArea":
                break;
        }
    }


    private void Move_Unit(Collider other)
    {
        switch (other.tag)
        {
            case "Ally":
                if (Basic_onUnit == null /* && detectedUnit.Count == 0 */ )
                {
                    Debug.Log(this.name + " isEmpty");
                    //detectedUnit.Add(other.gameObject);
                    Basic_onUnit = other.gameObject;

                    other.gameObject.transform.position = this.gameObject.transform.position; // ->  여기서 pos 변경 시키는거 배제해야하나..?
                    other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(this.gameObject);

                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_POSITION, Tile_Idx);
                    //Debug.Log(other.gameObject.transform.position + "<-unit // tile->" + this.gameObject.transform.position);
                }
                else // 이미 다른 유닛이 있을 경우 == 겹치는 경우
                {
                    Debug.Log(this.name + " isOverlap " + other.name + " ");
                    /*
                    //other.gameObject.transform.position = other.gameObject.GetComponent<Character>().LSY_Unit_Position;
                    // LSY : overlap 시, 기존 자리로 찾아가게 해놨는데 tile의 부모의 부모의 부모가 Battle 컴포넌트라 코드가 좀 더러움. 일단 작동은 하니까.
                    // oriPos를 Character도 프로퍼티로 갖고 있고, DragDrop.cs도 갖고 있는데 Character를 활용하려면 갱신을 계속 해줘야하는데 귀찮고.. (업데이트에 둘 가치는 없어 보이고, 위치 바꿀 때 프로퍼티로 set 하면 되긴 할듯?)
                    // DragDrop으로 지금처럼 하면 편하긴 한데, 이건 DragDrop이 현재 건드리는 객체가 하나여야함. 만약 복수의 유닛을 이동시키면 작동안함. 물론 마우스는 하나니까 문제는 없을거같은데 일단 문제가 있을 순 있음.
                    */
                    other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
                    //other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);

                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                }

                break;
            case "HitArea":
                break;
        }
    }



}