using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreCamera : MonoBehaviour
{
    static Camera _storeCamera;
    static bool storeCameraOn;
    Button _manaBtn;

    void Start()
    {
        if(_storeCamera == null){
            _storeCamera = GameObject.Find("StoreCamera").GetComponent<Camera>();
            _storeCamera.enabled = false;
        }
        
        storeCameraOn = false;
        _manaBtn = this.transform.GetComponent<Button>();
        if(_manaBtn != null){
            _manaBtn.onClick.AddListener(ManaBtnClick);
        }
        
    }

    void ManaBtnClick(){
        if(!storeCameraOn)
            StoreCameraOn();
        else
            StoreCameraOff();
    }

    void StoreCameraOn(){
        _storeCamera.enabled = true;
        _storeCamera.depth = 2;
        storeCameraOn = true;
    }

    public static void StoreCameraOff(){
        _storeCamera.depth = -1;
        _storeCamera.enabled = false;
        storeCameraOn = false;
    }

}
