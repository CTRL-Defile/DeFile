using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using System;
using UnityEngine.UI;

// �������� ������ csv���Ͽ��� ������ DB�� �����ϱ� ���� ����ϴ� Ŭ����
// �÷��̾ �� ���� ��ü�� ������ �ִ� ���� �ƴϰ�, DB�̸��̳� ��ȣ�� ������ �ʿ��� ������ �̰����� ������ �����ͼ� Ȱ���մϴ�.
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
    [SerializeField] Sprite Data_sprite;

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

    //
    public Sprite HYJ_Data_sprite { get { return Data_sprite;   } set { Data_sprite = value;    } }

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
