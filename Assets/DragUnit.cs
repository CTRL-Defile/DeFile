using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DragUnit : MonoBehaviour
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
    [SerializeField] Vector3 oriPos;
    Ray ray;
    bool isHeld = false;
    private void Start()
    {
        oriPos = this.gameObject.transform.position;
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
            Debug.Log("bt down");
            isHeld = true;
            if (selectedObject == null)
            {
                RaycastHit hit = CastRay();

                if(hit.collider != null)
                {
                    //if(!hit.collider.CompareTag("Ally"))
                    //{
                    //    return;
                    //}

                    selectedObject = hit.collider.gameObject;
                    Cursor.visible = false;
                }
            }
            //else
            //{
            //    Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            //    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            //    selectedObject.transform.position = new Vector3(worldPosition.x, 0f, worldPosition.z);

            //    selectedObject = null;
            //    Cursor.visible = true;
            //}
        }

        if(Input.GetMouseButtonUp(0))
        {
            Debug.Log("bt up");
            isHeld = false;
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y + 0f, worldPosition.z);

            selectedObject = null;
            Cursor.visible = true;

        }

        if (selectedObject!=null)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.transform.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, oriPos.y + 0.25f, worldPosition.z);
            Debug.Log(oriPos.y);
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
        int layerMask = 1 << LayerMask.NameToLayer("Store");
        Physics.Raycast(worldMousePosNear, worldMousePosFar - worldMousePosNear, out hit, 1000, layerMask);

        return hit;
    }


}
