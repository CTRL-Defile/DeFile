using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LSY_DragUnit : MonoBehaviour
{
    /*
    GameObject objectHitPostion;
    // RaycastHit : 객체와 Ray의 충돌에 대한 결과 정보를 저장하는 구조체,, Raycast함수의 out 파라메터로 사용됨
    RaycastHit hitRay, hitLayerMask;

    private void OnMouseUp()
    {
        this.transform.parent = null;
        Destroy(objectHitPostion);
    }

    private void OnMouseDown()
    {
        // Ray는 직선의 시작점과 방향을 가진 구조체.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Physics.Raycast 함수.. 히트 true/false return, out 파라메터로 RaycastHit return
        if (Physics.Raycast(ray, out hitRay))
        {
            objectHitPostion = new GameObject("HitPosition");
            // RaycastHit.point => 월드에서 Raycasting이 감지된 위치
            // RaycastHit.distance => Ray의 원점에서 충돌 지점까지의 거리
            // RaycastHit.collider.tag => 히트된 대상 객체의 세부정보를 얻을 수도 있음
            // RaycastHit.transform.gameObject => 충돌 객체의 transform에 대한 참조
            objectHitPostion.transform.position = hitRay.point;
            this.transform.SetParent(objectHitPostion.transform);
        }
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);

        int layerMask = 1 << LayerMask.NameToLayer("Store");
        if (Physics.Raycast(ray, out hitLayerMask, Mathf.Infinity, layerMask))
        {
            float H = Camera.main.transform.position.y;
            float h = objectHitPostion.transform.position.y;

            //Vector3 newPos = (hitLayerMask.point * (H - h) + Camera.main.transform.position * h) / H;
            Vector3 newPos = hitLayerMask.point;
            newPos.y = objectHitPostion.transform.position.y;
            //newPos.x = objectHitPostion.transform.position.x;


            objectHitPostion.transform.position = newPos;

        }
    }
    */

    [SerializeField]
    private GameObject selectedObject, selectedTile;
    HYJ_Battle_Tile selectedTile_Script;
    BATTLE_PHASE m_Battle_Phase;

    List<HYJ_Battle_Manager_Line> Field_Tile_List;
    HYJ_Battle_Manager_Line Stand_Tiles;

    [SerializeField]
    public Vector3 oriPos, curBlkPos;

    Ray m_TileRay, m_UnitRay;
    RaycastHit tile_hit, unit_hit;

    [SerializeField] bool isHeld = false, isPhaseChange = false, isRayHit, rightClick = false;
    double Hover_timer = 1.0, Time_Acc = 0.0;

    Transform Battle_Status;

    private void Start()
    {
        Battle_Status = transform.GetChild(1).GetChild(0).GetChild(4);

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_POSITION, LSY_Set_blkPos);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL, LSY_Set_oriPos);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DRAG___INIT, Drag_Init);
    }

    object LSY_Set_blkPos(params object[] _args)
    {
        List<float> tmp = (List<float>)_args[0];

        Vector3 pos = new Vector3(tmp[0], tmp[1], tmp[2]);
        curBlkPos = pos;
        return null;
    }
    object LSY_Set_oriPos(params object[] _args)
    {
        curBlkPos = oriPos;
        return null;
    }
    object Drag_Init(params object[] _args)
    {
        Field_Tile_List = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
        Stand_Tiles = (HYJ_Battle_Manager_Line)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_TILES);

        return null;
    }

    void Update()
    {
        m_Battle_Phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);

        tile_hit = CastRay("Tile");
        // Tile Ray
        if (tile_hit.collider != null)
        {
            // Tile Detected..
            if (tile_hit.collider != selectedTile && selectedTile != null)
            {
                selectedTile.GetComponent<HYJ_Battle_Tile>().LSY_Set_Hover(false);
            }

            selectedTile = tile_hit.collider.gameObject;
            selectedTile_Script = selectedTile.GetComponent<HYJ_Battle_Tile>();
            selectedTile_Script.LSY_Set_Hover(true);
            m_TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.yellow);
        }
        else
        {
            if (selectedTile != null)
            {
                selectedTile_Script.LSY_Set_Hover(false);
                selectedTile = null;
            }
            m_TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.white);
        }

        // Unit Ray
        unit_hit = CastRay("Unit");
        switch (m_Battle_Phase)
        {
            case BATTLE_PHASE.PHASE_PREPARE:
                {
                    if (Input.GetMouseButtonDown(0)) MouseDown();
                    if (Input.GetMouseButtonUp(0))   MouseUp();
                    break;
                }

            case BATTLE_PHASE.PHASE_COMBAT:
                {
                    if (isPhaseChange == false)
                    {
                        if (selectedObject == null)
                            isPhaseChange = true;
                        else
                        {
                            Debug.Log("Go Back");
                            selectedObject.transform.position = selectedObject.GetComponent<Character>().LSY_Character_OriPos;
                            selectedObject = null;
                            //isHeld = false;
                            Set_isHeld(false);
                            Cursor.visible = true;
                            isPhaseChange = true;
                        }
                    }

                    if (selectedTile_Script.tile_type == HYJ_Battle_Tile.Tile_Type.Stand || 
                        selectedTile_Script.tile_type == HYJ_Battle_Tile.Tile_Type.Trash)
                    {
                        //unit_hit = CastRay("Unit");
                        if (Input.GetMouseButtonDown(0)) MouseDown();
                        if (Input.GetMouseButtonUp(0))   MouseUp_COMBAT();
                    }
                    else if (selectedTile_Script.tile_type == HYJ_Battle_Tile.Tile_Type.Field)
                    {
                        if (selectedObject != null && Input.GetMouseButtonUp(0))
                        {
                            Debug.Log("BTNUP with ...");
                            selectedObject.transform.position = selectedObject.GetComponent<Character>().LSY_Character_OriPos;
                            selectedObject = null;
                            Set_isHeld(false);
                            Cursor.visible = true;

                        }
                    }
                    break;
                }

            case BATTLE_PHASE.PHASE_COMBAT_OVER:
                {
                    if (isPhaseChange == true)
                        isPhaseChange = false;

                    if (selectedObject != null)
                    {
                        Debug.Log("Go Back");
                        selectedObject.transform.position = selectedObject.GetComponent<Character>().LSY_Character_OriPos;
                        selectedObject = null;
                        //isHeld = false;
                        Set_isHeld(false);
                        Cursor.visible = true;
                    }
                    break;
                }

        }

        // Unit Ray
        //if (unit_hit.collider != null)
        //{
        //    Hover_timer = 1.0;
        //    Time_Acc += Time.deltaTime;

        //    if (Hover_timer - Time_Acc <= 0.0)
        //    {
        //        rightClick = false;
        //        Set_Status(unit_hit.transform.gameObject);
        //    }

        //    Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.red);
        //}
        //else if (!rightClick)
        //{
        //    Time_Acc = 0.0;
        //    Battle_Status.gameObject.SetActive(false);
        //}

        if (Input.GetMouseButton(1))
        {
            Debug.Log("Right Click");
            if (unit_hit.collider != null)
            {
                rightClick = true;
                Battle_Status.gameObject.SetActive(false);
                Set_Status(unit_hit.transform.gameObject);
            }
            else
            {
                rightClick = false;
                Battle_Status.gameObject.SetActive(false);
            }
            Time_Acc = 0.0;

        }

        if (rightClick)
        {
            if (unit_hit.collider != null)
            {
                Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.red);
                Hover_timer = 1.0;
                Time_Acc += Time.deltaTime;

                if (Hover_timer - Time_Acc <= 0.0)
                {
                    if (rightClick)
                    {
                        rightClick = false;
                        Battle_Status.gameObject.SetActive(false);
                    }
                    Set_Status(unit_hit.transform.gameObject);
                }
            }
        }
        else
        {
            if (unit_hit.collider != null)
            {
                Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.red);
                Hover_timer = 1.0;
                Time_Acc += Time.deltaTime;
                if (Hover_timer - Time_Acc <= 0.0)
                    Set_Status(unit_hit.transform.gameObject);

            }
            else
            {
                Time_Acc = 0.0;
                Battle_Status.gameObject.SetActive(false);
            }
        }


        if (isHeld)
        {
            m_UnitRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Battle_Status.gameObject.SetActive(false);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.green);
        }

        // Mouse Button Method
        
        // 1. BattlePhase -> Prepare : every ally unit control / Combat : stand unit control
        // 2. 

        //if (selectedObject != null) // Draging...
        if (Input.GetMouseButton(0))
        {
            rightClick = false;
            Battle_Status.gameObject.SetActive(false);

            if (selectedObject != null)
            {
                Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y + 0.25f, worldPosition.z);
            }
        }

        //
    }

    private RaycastHit CastRay(string layer)
    {
        int layerMask = 0;
        switch (layer)
        {
            case "Tile":
                layerMask = 1 << LayerMask.NameToLayer("Tile");
                break;
            case "Unit":
                layerMask = 1 << LayerMask.NameToLayer("Unit");
                break;
            case "NULL":
                layerMask = 1 << 10000;
                break;
        }

        Vector3 screenMousePosFar = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.farClipPlane
            );
        Vector3 screenMousePosNear = new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Camera.main.nearClipPlane
            );
        Vector3 worldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        isRayHit = Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, 1000, layerMask);

        return hit;
    }

    private void MouseDown()
    {
        //isHeld = true;

        //Debug.Log("BtnDown collider : " + unit_hit.collider);
        if (selectedObject == null && unit_hit.collider != null)
        {
            Character.Unit_Type _Type = unit_hit.collider.gameObject.GetComponent<Character>().UnitType;
            switch (_Type)
            {
                case Character.Unit_Type.Ally:
                    Set_isHeld(true);
                    //case "HitArea":
                    selectedObject = unit_hit.collider.gameObject;
                    oriPos = selectedObject.transform.position;
                    Cursor.visible = false;
                    break;
            }
        }
    }

    private void MouseUp_COMBAT()
    {
        Set_isHeld(false);
        Cursor.visible = true;

        // 잡고 있는 유닛이 있는가
        if (selectedObject != null)
        {
            Character selectedObject_Script = selectedObject.GetComponent<Character>();
            // 지정된 타일이 있는가
            if (selectedTile != null)
            {
                GameObject _selectedtile_onUnit = selectedTile_Script.HYJ_Basic_onUnit;
                HYJ_Battle_Tile.Tile_Available _available = selectedTile_Script.tile_Available;

                // 지정된 타일의 onUnit 이 비었는가
                if (_selectedtile_onUnit == null)
                {
                    // 둘 수 없는 타일인가
                    if (_available == HYJ_Battle_Tile.Tile_Available.Non_Available)
                    {
                        selectedObject.transform.position = selectedObject.GetComponent<Character>().LSY_Character_OriPos;
                    }
                    // 둘 수 있는 타일이다 -> 위치 변화와 Character의 OnTile 값 갱신
                    else
                    {
                        selectedObject.transform.position = selectedTile.transform.position;
                        selectedObject_Script.LSY_Character_Set_OnTile(selectedTile);
                    }
                }
                // 지정된 타일의 onUnit 이 비어있지 않으며 둘 수 있는 타일이다 -> Swap == 위치 변화와 Character의 OnTile 값 갱신
                else if (_available == HYJ_Battle_Tile.Tile_Available.Available)
                {
                    //  Unit Swap -> selectedObject와 selectedTile.onUnit 의 position 및 onTile 교환
                    Debug.Log("[Drag] SWAP " + _selectedtile_onUnit.name + "<-s tile, s obj-> " + selectedObject.name);

                    GameObject sel_obj_ontile = selectedObject.GetComponent<Character>().LSY_Character_Get_OnTile(); // selectedTile의 onUnit의 Character 스크립트의 onTile을 가져옴

                    Debug.Log("Go " + _selectedtile_onUnit.name + " to " + sel_obj_ontile.name);
                    _selectedtile_onUnit.transform.position = sel_obj_ontile.transform.position;

                    selectedTile.GetComponent<HYJ_Battle_Tile>().Set_Swap(true);
                    Debug.Log("Go " + selectedObject.name + " to " + selectedTile.name);
                    selectedObject.transform.position = selectedTile.transform.position;
                    //selectedObject_Script.LSY_Character_Set_OnTile(selectedTile);


                }
                // 지정된 타일의 onUnit 이 비어있지 않으며 둘 수 없는 타일이다 -> 제자리로 가라 (처음 마우스를 집었던 자리)
                else
                {
                    Debug.Log("btup : " + oriPos);
                    selectedObject.transform.position = oriPos;
                }
            }
            // selectedTile이 없을 때 (타일 밖으로 옮기려고 할 때)
            else
            {
                selectedObject.transform.position = oriPos;
            }
            selectedObject_Script.LSY_Character_OriPos = selectedObject.transform.position;
        }

        selectedObject = null;
    }
    private void MouseUp()
    {
        Set_isHeld(false);
        Cursor.visible = true;

        // 잡고 있는 유닛이 있는가
        if (selectedObject != null)
        {
            Character selectedObject_Script = selectedObject.GetComponent<Character>();
            // 지정된 타일이 있는가
            if (selectedTile != null)
            {
                GameObject _selectedtile_onUnit = selectedTile_Script.HYJ_Basic_onUnit;
                HYJ_Battle_Tile.Tile_Available _available = selectedTile_Script.tile_Available;

                // 지정된 타일의 onUnit 이 비었는가
                if (_selectedtile_onUnit == null)
                {
                    Debug.Log("[Tile] " + selectedTile.name + " is null");
                    // 둘 수 없는 타일인가
                    if (_available == HYJ_Battle_Tile.Tile_Available.Non_Available)
                    {
                        selectedObject.transform.position = selectedObject.GetComponent<Character>().LSY_Character_OriPos;
                    }
                    // 둘 수 있는 타일이다 -> 위치 변화와 Character의 OnTile 값 갱신
                    else
                    {
                        selectedObject.transform.position = selectedTile.transform.position;
                        //selectedObject_Script.LSY_Character_Set_OnTile(selectedTile);
                    }
                }
                // 지정된 타일의 onUnit 이 비어있지 않으며 둘 수 있는 타일이다 -> Swap == 위치 변화와 Character의 OnTile 값 갱신
                else if (_available == HYJ_Battle_Tile.Tile_Available.Available)
                {
                    //  Unit Swap -> selectedObject와 selectedTile.onUnit 의 position 및 onTile 교환
                    Debug.Log("[Drag] SWAP " + _selectedtile_onUnit.name + "<-s tile, s obj-> " + selectedObject.name);

                    GameObject sel_obj_ontile = selectedObject.GetComponent<Character>().LSY_Character_Get_OnTile();

                    Debug.Log("Go " + _selectedtile_onUnit.name + " to " + sel_obj_ontile.name);
                    _selectedtile_onUnit.transform.position = sel_obj_ontile.transform.position;

                    selectedTile.GetComponent<HYJ_Battle_Tile>().Set_Swap(true);
                    Debug.Log("Go " + selectedObject.name + " to " + selectedTile.name);
                    selectedObject.transform.position = selectedTile.transform.position;
                    //selectedObject_Script.LSY_Character_Set_OnTile(selectedTile);

                }
                // 지정된 타일의 onUnit 이 비어있지 않으며 둘 수 없는 타일이다 -> 제자리로 가라 (처음 마우스를 집었던 자리)
                else
                    selectedObject.transform.position = oriPos;
            }
            // selectedTile이 없을 때 (타일 밖으로 옮기려고 할 때)
            else
            {
                selectedObject.transform.position = oriPos;
            }
            selectedObject_Script.LSY_Character_OriPos = selectedObject.transform.position;
        }

        selectedObject = null;
    }

    public void Set_isHeld(bool tf)
    {
        isHeld = tf;

        int field_cnt = Field_Tile_List.Count;
        for (int i = 0; i < field_cnt; i++)
        {
            int field_x = Field_Tile_List[i].HYJ_Data_GetCount();
            for (int j = 0; j < field_x; j++)
            {
                Field_Tile_List[i].HYJ_Data_Tile(j).LSY_Set_Drag(tf);
            }
        }

        int stand_cnt = Stand_Tiles.HYJ_Data_GetCount();
        for (int i = 0; i < stand_cnt; i++)
        {
            //Debug.Log(Stand_Tiles.HYJ_Data_Tile(i) + " set Drag");
            Stand_Tiles.HYJ_Data_Tile(i).LSY_Set_Drag(tf);
        }

    }

    public void Set_Status(GameObject obj)
    {
        //Debug.Log(obj.name);
        Battle_Status.gameObject.SetActive(true);

        Battle_Status.GetComponent<Battle_Status_Script>().Set_Status(obj);

        //Battle_Status.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = obj.GetComponent<Character>().Character_Status_name;


    }

    //
}
