using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_DataBase : MonoBehaviour
{
    [SerializeField] int Basic_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    // Default_phase
    // 초기화가 끝났는지 확인
    object HYJ_Basic_GetIsInitialize(params object[] _args)
    {
        bool res = false;

        //
        if (Basic_phase == -1)
        {
            res = true;
        }

        return res;
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_phase = 0;

        HYJ_Relic_Start();
    }

    // Update is called once per frame
    // 어드레서블 도입을 위해 초기화를 업데이트에서 진행
    // 어드레서블은 호출한 프레임에서 바로 불러지지 않기에 지속적으로 체크가 필요함.
    void Update()
    {
        switch(Basic_phase)
        {
            case 0:
                {
                    if(HYJ_Relic_Init())
                    {
                        Basic_phase = 1;
                    }
                }
                break;
            case 1:
                {
                    if (HYJ_Potion_Init())
                    {
                        Basic_phase = 2;
                    }
                }
                break;
            case 2:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BASIC__GET_IS_INITIALIZE, HYJ_Basic_GetIsInitialize);

                    Basic_phase = -1;
                }
                break;
        }
    }
}

#region RELIC

partial class HYJ_DataBase
{
    [SerializeField] List<HYJ_Item> Relic_datas;
    [SerializeField] int Relic_phase;

    //////////  Getter & Setter //////////
    object HYJ_Relic_GetDataCount(params object[] _args)
    {
        return Relic_datas.Count;
    }

    object HYJ_Relic_GetDataName(params object[] _args)
    {
        int count = (int)_args[0];

        return Relic_datas[count].HYJ_Data_name;
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_Relic_Start()
    {
        Relic_phase = 0;
    }

    // 초기화
    bool HYJ_Relic_Init()
    {
        switch (Relic_phase)
        {
            case 0:
                {
                    Relic_datas = new List<HYJ_Item>();

                    Relic_phase = 1;
                }
                break;
            case 1:
                {
                    List<Dictionary<string, object>> data = CSVReader.Read("HYJ/Relic_csv");

                    //
                    for (int i = 0; i < data.Count; i++)
                    {
                        Relic_datas.Add(new HYJ_Item(data[i]));
                    }

                    Relic_phase = 2;
                }
                break;
            case 2:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_COUNT,   HYJ_Relic_GetDataCount  );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_NAME,    HYJ_Relic_GetDataName   );

                    Relic_phase = 3;
                }
                break;
            case 3:
                {
                    Relic_phase = -1;
                }
                break;
        }

        return (Relic_phase == -1);
    }
}

#endregion

#region POTION

partial class HYJ_DataBase
{
    [SerializeField] List<HYJ_Item> Potion_datas;
    [SerializeField] int Potion_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    object HYJ_Potion_GetDataFromName(params object[] _args)
    {
        HYJ_Item res = null;

        //
        string name = (string)_args[0];

        for(int i = 0; i < Potion_datas.Count; i++)
        {
            if(Potion_datas[i].HYJ_Data_name.Equals(name))
            {
                res = Potion_datas[i];
                break;
            }
        }

        return res;
    }

    //////////  Default Method  //////////
    void HYJ_Potion_Start()
    {
        Potion_phase = 0;
    }

    bool HYJ_Potion_Init()
    {
        switch (Potion_phase)
        {
            case 0:
                {
                    Potion_datas = new List<HYJ_Item>();

                    Potion_phase = 1;
                }
                break;
            case 1:
                {
                    List<Dictionary<string, object>> data = CSVReader.Read("HYJ/Potion_csv");

                    //
                    for (int i = 0; i < data.Count; i++)
                    {
                        Potion_datas.Add(new HYJ_Item(data[i]));
                    }

                    Potion_phase = 2;
                }
                break;
            case 2:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___POTION__GET_DATA_FROM_NAME, HYJ_Potion_GetDataFromName);

                    Potion_phase = 3;
                }
                break;
            case 3:
                {
                    Potion_phase = -1;
                }
                break;
        }

        return (Potion_phase == -1);
    }
}

#endregion

#region UNIT

partial class HYJ_DataBase
{
    [SerializeField] List<HYJ_Item> Unit_datas;
    [SerializeField] int Unit_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    bool HYJ_Unit_Init()
    {
        switch (Unit_phase)
        {
            case 0:
                {
                    Unit_datas = new List<HYJ_Item>();

                    Unit_phase = 1;
                }
                break;
            case 1:
                {
                    List<Dictionary<string, object>> data = CSVReader.Read("HYJ/Relic_csv");

                    //
                    for (int i = 0; i < data.Count; i++)
                    {
                        Unit_datas.Add(new HYJ_Item(data[i]));
                    }

                    Unit_phase = 2;
                }
                break;
            case 2:
                {
                    Unit_phase = -1;
                }
                break;
        }

        return (Unit_phase == -1);
    }
}

#endregion