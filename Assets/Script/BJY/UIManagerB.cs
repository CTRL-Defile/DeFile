using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManagerB : MonoBehaviour
{
    private const int _maxTimeValue = 1;
    private const float _maxTime = 0.1f * 300;
    private string _stageStatus;
    public Slider timeBar;
    public Text timeText;
    float limitTime;

    // Start is called before the first frame update
    void Start()
    {
        setInitalTime("Ready");
    }

    // Update is called once per frame
    void Update()
    {
        if(limitTime > 0)
            setTime();
        else if(limitTime == 0 && _stageStatus == "Ready"){
            setInitalTime("Battle");
        }
        else if(limitTime == 0 && _stageStatus == "Battle"){
            setInitalTime("BattleExtension");
        }
    }

    void setTime(){
        limitTime -= Time.deltaTime;
        timeText.text = Math.Round(limitTime).ToString();
        timeBar.value -= _maxTimeValue/_maxTime * Time.deltaTime;

    }

    void setInitalTime(string s){
        _stageStatus = s;
        switch(_stageStatus){
            case "Ready":
            case "Battle":
                limitTime = _maxTime;
                break;
            case "BattleExtension":
                limitTime = 0.1f * 100;
                break;

        }

        timeBar.value = _maxTimeValue;

    }

}
