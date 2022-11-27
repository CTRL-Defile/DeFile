using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

    private GameObject selectedObject;
    [SerializeField] public Vector3 oriPos, curBlkPos;
    Ray ray;
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

    private void Update()
    {
        if(isHeld)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.green);
        }

        if (Input.GetMouseButtonDown(0))
        {

            isHeld = true;
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();

                if(hit.collider != null)
                {

                    if (hit.collider.gameObject.CompareTag("Ally"))
                    {
                        Debug.Log("Ally detected");
                        selectedObject = hit.collider.gameObject;
                        oriPos = selectedObject.transform.position;
                    }
                    else if (hit.collider.gameObject.CompareTag("HitArea"))
                    {
                        Debug.Log("HitArea detected");
                        selectedObject = hit.collider.gameObject;
                        oriPos = selectedObject.transform.position;
                    }

                    Cursor.visible = false;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHeld = false;
            int Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);
            int cnt_Unit_onTile = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__COUNT_ALLY_ONTILE);

            if (selectedObject != null)
            {
                if (cnt_Unit_onTile < Player_Lv + 1)
                {
                    Debug.Log(Player_Lv + " Lv... and " + cnt_Unit_onTile + " of Ally is on tile.");
                    Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
                    selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y, worldPosition.z);

                    // Unit의 Position은 마우스가 들렸을 때 완료짓는 것이 맞다. tile에서 바꾸는건 여러 경우에 의도하지 않은 현상이 있을 것
                    // ex. trigger에 지속적으로 접하는 경우

                    // selectedObject.transform.position = curBlkPos;
                    // Debug.Log("curblk -> " + curBlkPos);
                }
                else
                {
                    selectedObject.transform.position = oriPos;
                }

                selectedObject = null;
            }

            Cursor.visible = true;
        }


        // During drag
        if (selectedObject != null)
        {

            Vector3 screenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y + 0.25f, worldPosition.z);
        }
    }

    private RaycastHit CastRay()
    {
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
        int layerMask = 1 << LayerMask.NameToLayer("Unit");
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, 1000, layerMask);

        return hit;
    }

}
