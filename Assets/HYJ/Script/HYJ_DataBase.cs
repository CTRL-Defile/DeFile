using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
    //void Start()
    private void Awake()
    {
        Basic_phase = 0;

        HYJ_Relic_Start();
        HYJ_Unit_Start();
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BASIC__GET_IS_INITIALIZE, HYJ_Basic_GetIsInitialize);
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
                    if (HYJ_Unit_Init())
                    {
                        Basic_phase = 3;
                    }
                }
                break;
            case 3:
                {
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BASIC__GET_IS_INITIALIZE, HYJ_Basic_GetIsInitialize);

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
    [SerializeField] GameObject Unit_datas; // UnitData.prefab
    [SerializeField] int Unit_phase;
    [SerializeField] List<List<Dictionary<string, object>>> Unit_csv = new List<List<Dictionary<string, object>>>();

    //////////  Getter & Setter //////////

    object LSY_GetUnitPrefab(params object[] _args)
    {
        return Unit_datas;
    }
    object LSY_GetPhase(params object[] _args)
    {
        int a;
        if (Unit_phase == -1)
            a=0;
        return Unit_phase;
    }
    object HYJ_Unit_GetDataCount(params object[] _args)
    {
        return Unit_datas.transform.childCount;
    }

    object HYJ_Unit_GetDataFromID(params object[] _args)
    {

        int id = (int)_args[0];

        if (Unit_datas.transform.Find("" + id))
            return Unit_datas.transform.Find("" + id).gameObject;
        else
            return null;
    }

    List<List<Dictionary<string, object>>> LSY_Unit_GetDB_CSV(params object[] _args)
    {
        return Unit_csv;
    }

    object HYJ_Unit_GetDataName(params object[] _args)
    {
        int count = (int)_args[0];

        return Unit_datas.transform.GetChild(count).name;
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_Unit_Start()
    {
        Unit_phase = 0;
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_PHASE, LSY_GetPhase);

    }

    bool HYJ_Unit_Init()
    {
        switch (Unit_phase)
        {
            case 0:
                {
                    Unit_phase = 1;

                    //
                    Addressables.LoadAssetAsync<GameObject>("Assets/CTRL_UNIT/UnitData.prefab").Completed +=
                        (_handle) =>
                        {
                            Unit_datas = _handle.Result;

                            //
                            Unit_phase = 2;
                        };
                }
                break;
            case 1:
                {

                }
                break;
            case 2:
                {
                    string csv_path = "DataBase/DB_Using_Character";
                    //Unit_csv.AddRange(CSVReader.Read(csv_path));
                    //Unit_csv[1] = CSVReader.Read(csv_path+"_2");
                    //Unit_csv[2] = CSVReader.Read(csv_path+"_3");

                    for (int i=0; i<3; i++)
                    {
                        List<Dictionary<string, object>> tmp = CSVReader.Read(csv_path + "_" + (i+1).ToString());
                        Unit_csv.Add(new List<Dictionary<string, object>>());

                        int len = tmp.Count;
                        for (int k=0; k<len; k++)
                        {
                            Unit_csv[i].Add(tmp[k]);
                        }

                    }





                    for (int i = 0; i < Unit_csv[0].Count; i++)
                    {
                        Unit_csv[0][i]["Index"] = i;
                        Unit_csv[1][i]["Index"] = i;
                        Unit_csv[2][i]["Index"] = i;

                        var Unit_trans = Unit_datas.transform.Find("" + (int)Unit_csv[0][i]["Index"]);
                        if (Unit_trans)
                            Unit_trans.GetComponent<Character>().HYJ_Status_SettingData(Unit_csv[0][i]);
                        else
                            Debug.Log((int)Unit_csv[0][i]["Index"] + " is NULL Unit object in ADDRESSABLE");

                        //Unit_datas.transform.Find("" + (int)Unit_csv[i]["ID"]).GetComponent<Character>().HYJ_Status_SettingData(Unit_csv[i]);
                        //Unit_datas.transform.Find(data[i]["ID"].ToString()).GetComponent<Character>().HYJ_Status_SettingData(data[i]);
                    }
                    Unit_phase = 3;
                }
                break;
            case 3:
                {
                    //HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID, HYJ_Unit_GetDataFromID);
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_UNIT_PREFAB,   LSY_GetUnitPrefab       );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV,  LSY_Unit_GetDB_CSV      );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_COUNT,    HYJ_Unit_GetDataCount   );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,  HYJ_Unit_GetDataFromID  );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_NAME,     HYJ_Unit_GetDataName    );
                    Unit_phase = -1;
                }
                break;
        }

        return (Unit_phase == -1);
    }

    private void Addressable_Completed(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = handle.Result;
        }
    }


}

#endregion