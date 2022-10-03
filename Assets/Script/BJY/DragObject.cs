using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Button _manaButton;
    public Sprite _sellImage;
    Sprite _originImage;

    float distance = 0;
    Vector3 _initialPos, _currentPos, _currentPosWorld;
    static Vector3 _storePos;
    RaycastHit hit;
    Ray ray;

     void Start()
    {
        MapManager.InitBoard();
        if(_manaButton == null){
            _manaButton = GameObject.Find("Mana").GetComponent<Button>();
            _originImage = _manaButton.GetComponent<Image>().sprite;
            _storePos = _manaButton.transform.position;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData){    
        _initialPos = transform.position;
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        _manaButton.GetComponent<Image>().sprite = _sellImage;
    }

    public void OnDrag(PointerEventData eventData){
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit, 1000f, 1 << 30)){
            _currentPosWorld =  hit.point;
        }

        if(_currentPosWorld.y != 0.0f)
            _currentPosWorld.y = 0.0f;
        
        transform.position = _currentPosWorld;
    }

    public void OnEndDrag(PointerEventData eventData){
        _manaButton.GetComponent<Image>().sprite = _originImage;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit, 1000f, 1 << 7)){
            _currentPosWorld = hit.transform.position;
            _currentPosWorld.y += 0.5f;
            transform.position = _currentPosWorld;
            Debug.Log(hit.transform.position);
        }
        else{
            transform.position = _initialPos;            
            Debug.Log("Nope!! - Cant Access"); 
        }


        //object pos가 마나버튼 위일 때
        if(IsOnManaButton(Camera.main.WorldToScreenPoint(_currentPosWorld))){
            SellObject();
            if(MapManager.IsInChessBoard(_initialPos)){
                MapManager.TakeOutChessBoard(_initialPos);
            }
            else if(MapManager.IsInWaitingBoard(_initialPos)){
                MapManager.TakeOutWaitingBoard(_initialPos);
                MapManager.SubWaitingBoardCount();
            }
        }
        //object pos를 판 위에 놓을 때
        //else if(!MapManager.IsInChessBoard(_currentPosWorld)&&!MapManager.IsInWaitingBoard(_currentPosWorld)){
        //}
        /*else if(MapManager.IsInChessBoard(_currentPosWorld)&&MapManager.IsInChessBoard(_initialPos)){
            _currentPosWorld.z = (float) Math.Round(_currentPosWorld.z);

            if(_initialPos != _currentPosWorld){
                if(MapManager.ChangePosChessBoard(_initialPos, _currentPosWorld)){
                    transform.position = _currentPosWorld;
                    Debug.Log("Change Pos"); 
                }
            }
            else{
                transform.position = _initialPos;
            }
        }
        else if(MapManager.IsInChessBoard(_currentPosWorld)&&MapManager.IsInWaitingBoard(_initialPos)){
            _currentPosWorld.z = (float) Math.Round(_currentPosWorld.z);

            if(MapManager.PutChessBoard(_currentPosWorld)){
                transform.position = _currentPosWorld;
                Debug.Log("Put!!"); 
            }
            else{
                transform.position = _initialPos;
            }

            MapManager.TakeOutWaitingBoard(_initialPos);
            MapManager.SubWaitingBoardCount();
        }
        else if(MapManager.IsInWaitingBoard(_currentPosWorld)&&MapManager.IsInChessBoard(_initialPos)){
            if(!MapManager.IsFullWaitingBoard()){
                _currentPosWorld = MapManager.ClearWaitingPosition(_currentPosWorld);

                if(MapManager.DragPutWaitingBoard(_currentPosWorld))
                    _currentPosWorld.z = -2.0f;
                    transform.position = _currentPosWorld;
                    Debug.Log("Back!!"); 
            }
            else{
                Debug.Log("Nope!! - FULL"); 
                transform.position = _initialPos;
            }

            MapManager.TakeOutChessBoard(_initialPos);
        }
        else if(MapManager.IsInWaitingBoard(_currentPosWorld)&&MapManager.IsInWaitingBoard(_initialPos)){
            _currentPosWorld = MapManager.ClearWaitingPosition(_currentPosWorld);
            _currentPosWorld.z = -2.0f;

            if(_initialPos != _currentPosWorld){
                if(MapManager.ChangePosWaitingBoard(_initialPos, _currentPosWorld)){
                    transform.position = _currentPosWorld;
                    Debug.Log("Change Pos"); 
                }
            }
            else{
                transform.position = _initialPos;
            }
        }*/
        
    }

    void SellObject(){
        Destroy(this.gameObject);
    }

    public static bool IsOnManaButton(Vector3 position){
        if((position.x<=_storePos.x+40)&&(position.x>=_storePos.x-60))
            if((position.y<=_storePos.y+130)&&(position.y>=_storePos.y+70))
                return true;
        return false;
    }


}
