using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    GameObject[] PlayerArray = new GameObject[5];
    GameObject[] StorePlayerArray = new GameObject[5];
    private static int[] _unitIDArray = new int[5];
    public Button _rerollBtn;
    Vector3 _newPos;
    System.Random random = new System.Random();

    //storeplayer 하위에 모델링 생성, 

    void Start()
    {
        _newPos.y = 7.5f;
        _newPos.z = 1.0f;

        for(int i = 0; i<5; i++){
            PlayerArray[i] = Resources.Load("BJYPrefab/StorePlayer(" + (i+1) + ")") as GameObject;
        }

        if(_rerollBtn != null){
            _rerollBtn.onClick.AddListener(RerollBtnClick);
        }

        CreateStorePlayer();
    }

    void RerollBtnClick(){
        if(PlayerManager.GetMana()>=2){
            DestroyAllStorePlayer();
            CreateStorePlayer();
            PlayerManager.ReduceMana(2);
        }
        else{
            Debug.Log("Mana 부족 - reroll");
        }
    }

    void CreateStorePlayer(){
        for(int i = 0; i<5; i++){
            int unitID = random.Next(10001,10005);
            _unitIDArray[i] = unitID;
            _newPos.x = -5.0f + 2*i;
            StorePlayerArray[i] = Instantiate(PlayerArray[i]) as GameObject;
            StorePlayerArray[i].transform.position = _newPos;
            StorePlayerArray[i].SetActive(true);

            var a = UnitManager.Instance.CallUnitModelingObject(unitID, StorePlayerArray[i].transform).GetComponentsInChildren<Transform>();
            for(int j=0;j<a.Length;j++){
                a[j].gameObject.layer = 6;
                a[j].rotation = Quaternion.Euler(0,-180,0);
            }
        
        }
    }

    void DestroyAllStorePlayer(){
        foreach(GameObject toDestroy in StorePlayerArray)
            Destroy(toDestroy);
    }

    public static int ReturnStorePlayerID(Vector3 position){
        int i = (int) Math.Abs(5.0f + position.x)/2;
        return _unitIDArray[i];
    }

    

}
