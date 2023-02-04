using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] bool isHovering = false, isDraging = false, isSwap = false;
    MeshRenderer m_mesh;
    Material m_material;
    Color ori_Color;
    public enum Tile_Available
    {
        Available,
        Non_Available,
        Available_END
    }
    public enum Tile_Type
    {
        Stand,
        Field,
        Trash,
        Tile_END
    }
    [SerializeField] public Tile_Available tile_Available = Tile_Available.Non_Available;
    [SerializeField] public Tile_Type tile_type;

    //////////  Getter & Setter //////////
    public int GraphIndex { get { return m_GraphIdx; } set { m_GraphIdx = value; } }
    public Tile_Type TileType { get { return tile_type; } set { tile_type = value; } }
    public Tile_Available TileAvailable { get { return tile_Available; } set { tile_Available = value; } }
	//////////  Method          //////////
	public GameObject HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }
    public void LSY_Set_Hover(bool tf)
    {
        isHovering = tf;
    }

    public void LSY_Set_Drag(bool tf)
    {
        isDraging = tf;
    }
    public void Set_Swap(bool tf)
    {
        isSwap = tf;
    }

    //////////  Default Method  //////////
    void Start()
    {
        m_mesh = this.GetComponent<MeshRenderer>();
        m_material = this.GetComponent<Material>();
        //ori_Color = m_material.color;
        ori_Color = m_mesh.material.color;
        Tile_Position = this.transform.localPosition;

    }
    void Update()
    {
        if (isDraging)
        {
            //if (this.tile_Available == Tile_Available.Available)
            if (TileAvailable == Tile_Available.Available)
                m_mesh.enabled = true;
        }
        else
        {
            //m_child.SetActive(false);
            m_mesh.enabled = false;
        }

        if (isHovering)
        {
            //if (m_child.gameObject.activeSelf == false)
            //m_child.SetActive(true);
            //m_child.GetComponent<SpriteRenderer>().color = Color.red;
            if (TileAvailable == Tile_Available.Available)
            {
                m_mesh.enabled = true;
                //m_material.color = Color.white;
                m_mesh.material.color = Color.white;
            }
            //this.transform.position = new Vector3(Tile_Position.x, Tile_Position.y + 0.25f, Tile_Position.z);

        }
        else
        {
            if (!isDraging)
            {
                //m_child.SetActive(false);
                m_mesh.enabled = false;
            }
            m_mesh.material.color = ori_Color;
            //m_child.GetComponent<SpriteRenderer>().color = Color.white;
            //this.transform.position = new Vector3(Tile_Position.x, Tile_Position.y, Tile_Position.z);

        }
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
        Character other_Script = other.GetComponent<Character>();
        if (other_Script != null)   // 다른 collider 검출 방지
        {
            //Debug.Log("[Tile] " + other + " " + this);
            if (other_Script.m_UnitType == Character.Unit_Type.Ally || other_Script.m_UnitType == Character.Unit_Type.Enemy)
            {
                BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
                if (phase == BATTLE_PHASE.PHASE_COMBAT && !this.CompareTag("TrashTile"))
                {
                    if (Basic_onUnit == null)
                    {
                        Basic_onUnit = other.gameObject;
                        other_Script.LSY_Character_Set_OnTile(this.gameObject);
                    }
                }
                else
                {
                    switch (other_Script.m_UnitType)
                    {
                        case Character.Unit_Type.Ally:
                            Ally_Enter(other);
                            break;

                        case Character.Unit_Type.Enemy:
                            Enemy_Enter(other);
                            break;
                    }
                }

            }
        }

    }

	private void OnTriggerStay(Collider other)
	{
		Character other_Script = other.GetComponent<Character>();
		if (other_Script != null)   // 다른 collider 검출 방지
		{
			//Debug.Log("[Tile] " + other + " " + this);
			if (other_Script.m_UnitType == Character.Unit_Type.Ally || other_Script.m_UnitType == Character.Unit_Type.Enemy)
			{
				BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
				if (phase == BATTLE_PHASE.PHASE_COMBAT && !this.CompareTag("TrashTile"))
				{
					if (Basic_onUnit == null &&
						other.GetComponent<Character>().State != Character.STATE.DIE)
					{
						Basic_onUnit = other.gameObject;
						other_Script.LSY_Character_Set_OnTile(this.gameObject);
					}
				}

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
            Character other_Script = other.GetComponent<Character>();
            if (other_Script != null)
            {
                switch (other_Script.m_UnitType)
                {
                    case Character.Unit_Type.Ally:
                        Ally_Exit(other);
                        break;

                    case Character.Unit_Type.Enemy:
                        Enemy_Exit(other);
                        break;
                }

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
        Debug.Log(this.name + " is enter " + other.name);
        if (tile_Available == Tile_Available.Non_Available)
        {
            Debug.Log("[BattleTile] NonAvailable Tile Enter");
            //other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
        }
        else
        {
            HYJ_Battle_Tile.Tile_Type _Type;
            if (other.GetComponent<Character>().LSY_Character_Get_OnTile() != null)
                _Type = other.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().TileType;
            else
                _Type = Tile_Type.Stand;


            //string Tag_UnitTile;
            //if (other.GetComponent<Character>().LSY_Character_Get_OnTile() == null)
            //    // 첫 생성(구매) 시 Stand로 설정
            //    Tag_UnitTile = "StandTile";
            //else
            //    Tag_UnitTile = other.GetComponent<Character>().LSY_Character_Get_OnTile().tag;

            int Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);
            int cnt_FieldUnit = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___COUNT__FIELD_UNIT);
            // stand->stand, stand->field, field->field, field->stand & [trash]
            switch (_Type)
            {
                case Tile_Type.Stand:
                    if (this.tile_type == Tile_Type.Stand)
                    {
                        Move_Unit(other);
                    }
                    else if (this.tile_type == Tile_Type.Field)
                    {
                        if (cnt_FieldUnit < Player_Lv + 1)
                        {
                            Debug.Log(Player_Lv + " Lv... and " + cnt_FieldUnit + " of Ally is on tile." + this.name + " " + other.name);
                            Move_Unit(other);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__STAND_TO_FIELD, other.gameObject);
                        }
                        else if (isSwap)
                        {
                            Debug.Log("[Tile] It's swapping now..");
                            isSwap = false;
                            Debug.Log(Player_Lv + " Lv... and " + cnt_FieldUnit + " of Ally is on tile." + this.name + " " + other.name);
                            Move_Unit(other);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__STAND_TO_FIELD, other.gameObject);
                        }
                        else
                        {
                            // 필드 위 유닛 개수 초과 시,
                            Debug.Log("!! Count over");
                            //other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;
                            other.transform.position = other.GetComponent<Character>().LSY_Character_Get_OnTile().transform.position;
                        }
                    }
                    else if (this.tile_type == Tile_Type.Trash)
                    {
                        //Debug.Log("Trash Collider Enter");
                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, _Type, other.gameObject);
                    }
                    break;

                case Tile_Type.Field:
                    if (this.tile_type == Tile_Type.Field)
                    {
                        Move_Unit(other);
                    }
                    else if (this.tile_type == Tile_Type.Stand)
                    {
                        Debug.Log(Player_Lv + " Lv... and " + cnt_FieldUnit + " of Ally is on tile. " + this.name + " " + other.name);
                        Move_Unit(other);
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__FIELD_TO_STAND, other.gameObject);
                    }
                    else if (this.tile_type == Tile_Type.Trash)
                    {
                        //Debug.Log("Trash Collider Enter");
                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH, _Type, other.gameObject);
                    }
                    break;

            }
        }

        // TODO
        // Player에 연결, 시너지, 2성 갱신 타이밍 -> tile의 triggerEnter (특정 phase 일 때..)
        // 전투 중에서도 2성 갱신이 되어야함 (stand 에서만)

        // 영재가 추가
        BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
        if (phase == BATTLE_PHASE.PHASE_PREPARE || phase == BATTLE_PHASE.PHASE_COMBAT_OVER)
        {
            // Player에 유닛 데이터 업데이트 -> 내부에 시너지 업데이트도 포함됨
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__DATA_UPDATE);
        }
    }
    private void Ally_Exit(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Character.Unit_Type _Type = other.GetComponent<Character>().UnitType;
            switch (_Type)
            {
                case Character.Unit_Type.Ally:
                    //detectedUnit.Remove(other.gameObject);
                    //other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                    Basic_onUnit = null;
                    break;

                    //case "HitArea":
                    //    break;
            }

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
        if (other.GetComponent<Character>() != null)
        {
            Character.Unit_Type _Type = other.GetComponent<Character>().UnitType;
            switch (_Type)
            {
                case Character.Unit_Type.Enemy:
                    //detectedUnit.Remove(other.gameObject);
                    Basic_onUnit = null;
                    break;

                //case "HitArea":
                //    break;
            }

        }
    }


    private void Move_Unit(Collider other)
    {
        if (other.GetComponent<Character>() != null)
        {
            Character.Unit_Type _Type = other.GetComponent<Character>().UnitType;
            switch (_Type)
            {
                case Character.Unit_Type.Ally:
                    if (Basic_onUnit == null /* && detectedUnit.Count == 0 */ )
                    {
                        Debug.Log(this.name + " isEmpty, " + other.name + " is now onUnit");
                        Basic_onUnit = other.gameObject;

                        other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(this.gameObject);
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

                        //other.gameObject.transform.position = this.transform.parent.transform.parent.transform.parent.GetComponent<LSY_DragUnit>().oriPos;

                        //other.gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(null);

                        //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL);
                    }

                    break;
                //case "HitArea":
                //    break;
            }
        }
    }



}