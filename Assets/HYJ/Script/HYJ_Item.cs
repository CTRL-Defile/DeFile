using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;

// 아이템의 정보를 csv파일에서 가져와 DB에 적재하기 위해 사용하는 클래스
// 플레이어가 이 정보 전체를 가지고 있는 것이 아니고, DB이름이나 번호를 가지고 필요할 때마다 이곳에서 정보를 가져와서 활용합니다.
[Serializable]
public class HYJ_Item : IDisposable
{
    [SerializeField] string Data_name;
    [SerializeField] string Data_type;
    [SerializeField] string Data_effect;
    [SerializeField] int Data_valueMin;
    [SerializeField] int Data_valueMax;
    [SerializeField] int Data_limit;
    [SerializeField] int Data_tier;

    //////////  Getter & Setter //////////
    //
    public string HYJ_Data_name { get { return Data_name; } }

    //
    public string HYJ_Data_type { get { return Data_type; } }

    //
    public int HYJ_Data_valueMin { get { return Data_valueMin; } }

    public int HYJ_Data_valueMax { get { return Data_valueMax; } }

    //
    public int HYJ_Data_limit { get { return Data_limit; } }

    //////////  Method          //////////
    public void Dispose()
    {

    }

    //////////  Default Method  //////////
    public HYJ_Item(Dictionary<string, object> _data)
    {
        Data_name       = (string)_data["NAME"];
        Data_type       = (string)_data["TYPE"];
        Data_effect     = (string)_data["EFFECT"];
        Data_valueMin   = (int)_data["VALUE_MIN"];
        Data_valueMax   = (int)_data["VALUE_MAX"];
        try
        {
            Data_limit = (int)_data["LIMIT"];
        }
        catch (Exception e)
        {
            Data_limit = -1;
        }

        try
        {
            Data_tier = (int)_data["TIER"];
        }
        catch(Exception e)
        {
            Data_tier = -1;
        }
    }
}
