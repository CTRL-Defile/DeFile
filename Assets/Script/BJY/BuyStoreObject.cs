using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuyStoreObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    float distance = 0;
    float doubleClick = 0f, doubleClickDelta = 0.25f;
    Camera _storeCamera;
    Vector3 _initialPos, _currentPosWorld, _endPos;

    void Start(){
        if(_storeCamera == null){
            _storeCamera = GameObject.Find("StoreCamera").GetComponent<Camera>();
        }
        _endPos.y = 0.0f;
        _endPos.z = - 2.0f;

    }

    //드래그
    public void OnBeginDrag(PointerEventData eventData){   
        _initialPos = transform.position;
        distance = Vector3.Distance(transform.position, _storeCamera.transform.position);
    }

    public void OnDrag(PointerEventData eventData){
        Vector3 _currentPos = eventData.position;

        if(this.gameObject.layer != 0){
            _currentPosWorld = _storeCamera.ScreenToWorldPoint(new Vector3(_currentPos.x,_currentPos.y,distance));
        
            if(_currentPosWorld.z != 0.5f){
                _currentPosWorld.z = 0.5f;
            }
            if(_currentPosWorld.y < 4.5f){
                StoreCamera.StoreCameraOff();
                this.gameObject.layer = 0;
            }
        }
        else {
            _currentPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(_currentPos.x,_currentPos.y,distance));
            
            if(_currentPosWorld.y != 0.0f){
                _currentPosWorld.y = 0.0f;
            }
            if(_currentPosWorld.z != -2.0f){
                _currentPosWorld.z = -2.0f;
            }
        }
        
        transform.position = _currentPosWorld;
    }

    public void OnEndDrag(PointerEventData eventData){
        if(_currentPosWorld.y > 4.5f && this.gameObject.layer != 0){
            transform.position = _initialPos;
        }
        else if(this.gameObject.layer == 0){
            if(!MapManager.IsFullWaitingBoard()){
                _endPos.x = (float) Math.Round(_currentPosWorld.x);
                _endPos = MapManager.ClearWaitingPosition(_endPos);
                
                if(MapManager.DragPutWaitingBoard(_endPos))
                    PutStorePlayer();
                else{
                    CanceledPutStorePlayer();
                }
            }
            else{
                Debug.Log("Full!!! - On Drag");  
                CanceledPutStorePlayer();          
            }
        }
    }

    //클릭
    void OnMouseUp(){
        if((Time.time - doubleClick) < doubleClickDelta){
            doubleClick = 0f;
            OnMouseDoubleClick();
        }
        else
            doubleClick = Time.time;
    }

    void OnMouseDoubleClick(){
        if(MapManager.ClickPutWaitingBoard(ref _endPos)){
            PutStorePlayer();
        }
        else
            Debug.Log("Full! - Double Click");
    }

    void PutStorePlayer(){
        int id = StoreManager.ReturnStorePlayerID(_initialPos);
        UnitManager.Instance.CallUnit(id, _endPos,eTag.Ally);
        Destroy(this.gameObject);
    }

    void CanceledPutStorePlayer(){
        transform.position = _initialPos;
        this.gameObject.layer = 6;
    }

}
