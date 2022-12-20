using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    [SerializeField]
    public Vector3 oriPos, curBlkPos;
    Ray m_TileRay, m_UnitRay;
    bool isHeld = false;
    private void Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_POSITION, LSY_Set_blkPos);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DRAG___UNIT__SET_ORIGINAL, LSY_Set_oriPos);
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

    void FixedUpdate()
    {
        RaycastHit tile_hit = CastRay("Tile"), unit_hit = CastRay("Unit");

        // Tile Ray
        if (tile_hit.collider != null)
        {
            // Tile Detected..
            if (tile_hit.collider != selectedTile && selectedTile != null)
                selectedTile.GetComponent<HYJ_Battle_Tile>().LSY_Set_Cast(0);

            selectedTile = tile_hit.collider.gameObject;
            selectedTile.GetComponent<HYJ_Battle_Tile>().LSY_Set_Cast(1);
            m_TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.yellow);
        }
        else
        {
            if (selectedTile != null)
            {
                selectedTile.GetComponent<HYJ_Battle_Tile>().LSY_Set_Cast(0);
                selectedTile = null;
            }
            m_TileRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.white);
        }

        // Unit Ray
        if (isHeld)
        {
            m_UnitRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(m_TileRay.origin, m_TileRay.direction * 1000, Color.green);
        }

        if (Input.GetMouseButtonDown(0))
        {
            isHeld = true;
            if (selectedObject == null && unit_hit.collider != null)
            {
                switch (unit_hit.collider.gameObject.tag)
                {
                    case "Ally":
                        //case "HitArea":
                        selectedObject = unit_hit.collider.gameObject;
                        oriPos = selectedObject.transform.position;
                        break;
                }

                Cursor.visible = false;
            }
        }

        if (selectedObject != null) // Draging...
        {
            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y + 0.25f, worldPosition.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            Cursor.visible = true;

            if (selectedObject != null)
            {
                if (selectedTile != null && selectedTile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit == null)
                {
                    selectedObject.transform.position = selectedTile.transform.position;
                }
                else
                {
                    selectedObject.transform.position = oriPos;
                }
                selectedObject = null;

            }

        }

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
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, 1000, layerMask);

        return hit;
    }





}
