using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

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
        HYJ_UnitSkill_Start();
        HYJ_Buff_Start();
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BASIC__GET_IS_INITIALIZE, HYJ_Basic_GetIsInitialize);
    }

    // Update is called once per frame
    // 어드레서블 도입을 위해 초기화를 업데이트에서 진행
    // 어드레서블은 호출한 프레임에서 바로 불러지지 않기에 지속적으로 체크가 필요함.
    void Update()
    {
        switch(Basic_phase)
        {
            case 0: { if (HYJ_Relic_Init()      )   { Basic_phase = 1;  } } break;
            case 1: { if (HYJ_Potion_Init()     )   { Basic_phase = 2;  } } break;
            case 2: { if (HYJ_Unit_Init()       )   { Basic_phase = 3;  } } break;
            case 3: { if (HYJ_UnitSkill_Init()  )   { Basic_phase = 4;  } } break;
            case 4: { if (HYJ_Buff_Init()       )   { Basic_phase = 5;  } } break;
            case 5: {                               { Basic_phase = -1; } } break;
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

    object HYJ_Relic_GetDataFromName(params object[] _args)
    {
        HYJ_Item res = null;

        //
        string strName = (string)_args[0];

        //
        for (int i = 0; i < Relic_datas.Count; i++)
        {
            if(Relic_datas[i].HYJ_Data_name.Equals(strName))
            {
                res = Relic_datas[i];
                break;
            }
        }

        //
        return res;
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
                    Relic_phase = 3;

                    Addressables.LoadAssetAsync<GameObject>("Assets/CTRL_RELIC/RelicData.prefab").Completed +=
                        (_handle) =>
                        {
                            Transform elements = _handle.Result.transform;

                            for(int i = 0; i < Relic_datas.Count; i++)
                            {
                                Transform target = elements.Find(Relic_datas[i].HYJ_Data_name);
                                if(target != null)
                                {
                                    Relic_datas[i].HYJ_Data_sprite = target.Find("Image").GetComponent<Image>().sprite;
                                }
                            }

                            //
                            Relic_phase = 4;
                        };
                }
                break;
            case 3:
                {

                }
                break;
            case 4:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_COUNT,   HYJ_Relic_GetDataCount  );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_NAME,    HYJ_Relic_GetDataName   );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___RELIC__GET_DATA_FROM_NAME,   HYJ_Relic_GetDataFromName   );

                    Relic_phase = 5;
                }
                break;
            case 5:
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
    GameObject Unit_datas; // UnitData.prefab
    int Unit_phase;
    List<List<Dictionary<string, object>>> Unit_csv = new List<List<Dictionary<string, object>>>();
    List<List<Dictionary<string, object>>> Player_Unit_csv = new List<List<Dictionary<string, object>>>();

    //////////  Getter & Setter //////////

    object LSY_GetUnitPrefab(params object[] _args)
    {
        return Unit_datas;
    }
    object LSY_GetPhase(params object[] _args)
    {
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
        string csv_path = "DataBase/DB_Using_Character";

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

#region UNIT_SKILL

partial class HYJ_DataBase
{
    [SerializeField] Dictionary<int, HYJ_CharacterSkill> UnitSkill_datas;
    [SerializeField] int UnitSkill_phase;

    //////////  Getter & Setter //////////
    object HYJ_UnitSkill_GetData(params object[] _args)
    {
        int id = (int)_args[0];
        return UnitSkill_datas[id];
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_UnitSkill_Start()
    {
        UnitSkill_phase = 0;
    }

    // 초기화
    bool HYJ_UnitSkill_Init()
    {
        switch (UnitSkill_phase)
        {
            case 0:
                {
                    UnitSkill_datas = new Dictionary<int, HYJ_CharacterSkill>();

                    UnitSkill_phase = 1;
                }
                break;
            case 1:
                {
                    List<Dictionary<string, object>> data = CSVReader.Read("DataBase/DB_Skill");

                    //
                    for (int i = 0; i < data.Count; i++)
                    {
                        UnitSkill_datas.Add((int)data[i]["ID"], new HYJ_CharacterSkill(data[i]));
                    }

                    UnitSkill_phase = 2;
                }
                break;
            case 2:
                {
                    //List<Dictionary<string, object>> data = CSVReader.Read("HYJ/Unit_Skill_Effect_csv");

                    //
                    //for (int i = 0; i < data.Count; i++)
                    //{
                    //    HYJ_CharacterSkillEffect element = new HYJ_CharacterSkillEffect(data[i]);
                    //    UnitSkill_datas[element.HYJ_Data_skillId].HYJ_Data_AddEffect(element);
                    //}

                    UnitSkill_phase = 3;
                }
                break;
            case 3:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___SKILL__GET_DATA, HYJ_UnitSkill_GetData);

                    UnitSkill_phase = 4;
                }
                break;
            case 4:
                {
                    UnitSkill_phase = -1;
                }
                break;
        }

        return (UnitSkill_phase == -1);
    }
}

#endregion

#region BUFF

partial class HYJ_DataBase
{
    int Buff_phase;
    Dictionary<int, CTRL_Buff> Buff_datas;

    //////////  Getter & Setter //////////
    object HYJ_Buff_GetData(params object[] _args)
    {
        int id = (int)_args[0];
        return Buff_datas[id];
    }

    object HYJ_Buff_GetCount(params object[] _args)
    {
        return Buff_datas.Count;
    }

    object HYJ_Buff_GetKeys(params object[] _args)
    {
        List<int> res = new List<int>(Buff_datas.Keys);

        return res;
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_Buff_Start()
    {
        Buff_phase = 0;
    }

    // 초기화
    bool HYJ_Buff_Init()
    {
        switch (Buff_phase)
        {
            case 0:
                {
                    Buff_datas      = new Dictionary<int, CTRL_Buff>();

                    HYJ_DeBuff_Init(Buff_phase);

                    Buff_phase = 1;
                }
                break;
            case 1:
                {
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_PlayerExp_csv"    );
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_PlayerGold_csv"   );
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_PlayerHp_csv"     );

                    HYJ_Buff_Init_1_BuffSetting(    "Buff_UnitChance_csv"               );
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_UnitStatusFromCharacter_csv"  );
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_UnitStatusFromField_csv"      );
                    HYJ_Buff_Init_1_BuffSetting(    "Buff_UnitStatusFromRace_csv"       );

                    Buff_phase = 2;
                }
                break;
            case 2:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_DATA,  HYJ_Buff_GetData    );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_COUNT, HYJ_Buff_GetCount   );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___BUFF__GET_KEYS,  HYJ_Buff_GetKeys    );

                    HYJ_DeBuff_Init(Buff_phase);

                    Buff_phase = 3;
                }
                break;
            case 3:
                {
                    Buff_phase = -1;
                }
                break;
        }

        return (Buff_phase == -1);
    }

    void HYJ_Buff_Init_1_BuffSetting(string _csv)
    {
        List<Dictionary<string, object>> data = CSVReader.Read("HYJ/" + _csv);

        //
        List<int> debuffCounts = new List<int>();
        for (int i = 0; i < data.Count; i++)
        {
            if ((int)data[i]["ratio_value"] < 0)
            {
                debuffCounts.Add(i);
            }
            else
            {
                int dataIndex = (int)data[i]["index"];
                Buff_datas.Add(dataIndex, new CTRL_Buff(data[i]));
            }
        }

        HYJ_DeBuff_Init(Buff_phase, data, debuffCounts);
    }
}

#endregion

#region DEBUFF

partial class HYJ_DataBase
{
    Dictionary<int, CTRL_Buff> DeBuff_datas;

    //////////  Getter & Setter //////////
    object HYJ_DeBuff_GetData(params object[] _args)
    {
        int id = (int)_args[0];
        return DeBuff_datas[id];
    }

    object HYJ_DeBuff_GetCount(params object[] _args)
    {
        return DeBuff_datas.Count;
    }

    object HYJ_DeBuff_GetKeys(params object[] _args)
    {
        List<int> res = new List<int>(DeBuff_datas.Keys);

        return res;
    }

    //////////  Method          //////////

    //////////  Default Method  //////////

    // 초기화
    void HYJ_DeBuff_Init(params object[] _args)
    {
        int phase = (int)_args[0];
        List<Dictionary<string, object>> data = null;
        List<int> debuffCounts = null;
        if (_args.Length > 1)
        {
            data            = (List<Dictionary<string, object>>)_args[1];
            debuffCounts    = (List<int>)_args[2];
        }


        //
        switch (phase)
        {
            case 0:
                {
                    DeBuff_datas    = new Dictionary<int, CTRL_Buff>();
                }
                break;
            case 1:
                {
                    //
                    for (int i = 0; i < debuffCounts.Count; i++)
                    {
                        DeBuff_datas.Add((int)data[debuffCounts[i]]["index"], new CTRL_Buff(data[debuffCounts[i]]));
                    }
                }
                break;
            case 2:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_DATA,  HYJ_DeBuff_GetData    );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_COUNT, HYJ_DeBuff_GetCount   );
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.DATABASE___DEBUFF__GET_KEYS,  HYJ_DeBuff_GetKeys    );
                }
                break;
        }
    }
}

#endregion