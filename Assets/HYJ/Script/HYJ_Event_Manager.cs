using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public partial class HYJ_Event_Manager : MonoBehaviour
{
    [SerializeField] public Text eventName;     //좌측상단 이벤트 이름 텍스트
    [SerializeField] public Text script;        //이벤트 텍스트 텍스트
    [SerializeField] public Text choice1;       //선택지 1 텍스트
    [SerializeField] public Text choice2;       //선택지 2 텍스트
    [SerializeField] private Button resultButton; // 결과 버튼

    [SerializeField] int Basic_initialize;

    // 이벤트 선택지에 따라 ScriptBridge에 메서드로 보낼 데이터를 담는 변수
    private JHW_EventSelectedData tempData;

    // 랜덤으로 등장하게 될 이벤트 ID
    private int randomEventID = -1;

    // 이벤트 현재 위치(번호)
    private int eventIndex;

    // 이벤트 선택 후 결과 스크립트
    private string resultScript;

    // 이벤트 버튼 오브젝트들
    private GameObject eventButton1;
    private GameObject eventButton2;
    private GameObject eventResultButton;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);
        //
        return null;
    }

    bool isFirst = true;
    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        // 이벤트 클릭시 이벤트ID 랜덤으로 뽑고, 그 ID에 따른 사진/스크립트 변경
        randomEventID = JHW_setRandomEventID();
        JHW_displayEventText();

        //
        if (isFirst)
            isFirst= false;
        else
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;

        JHW_Event_start();
    }

    // Update is called once per frame
    void Update()
    {
        switch (Basic_initialize)
        {
            case -1: break;
            //
            case 0:
                {
                    Camera camera
                        = (Camera)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.MASTER___UI__GET_CAMERA,
                            0);

                    if (camera != null && JHW_Event_Init())
                    {
                        this.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = camera;

                        Basic_initialize = 1;
                    }

                    // 버튼 게임오브젝트 불러오기
                    eventButton1 = GameObject.Find("EventButton1");
                    eventButton2 = GameObject.Find("EventButton2");
                    eventResultButton = GameObject.Find("ResultButton");
                }
                break;
            case 1:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON, HYJ_ActiveOn);

                    this.HYJ_SetActive(false);

                    Basic_initialize = -1;

                    // 결과버튼비활성화
                    eventResultButton.SetActive(false);
                }
                break;
        }
    }
}

// Event_csv , EventSelect_csv 읽어오기
#region Event/EventSelect 데이터 읽기

partial class HYJ_Event_Manager
{
    [SerializeField] List<JHW_Event> Event_datas;
    [SerializeField] List<JHW_EventSelect> EventSelect_datas;
    [SerializeField] int Event_phase;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    public void JHW_Event_start()
    {
        Event_phase = 0;
    }

    // 초기화
    bool JHW_Event_Init()
    {
        switch (Event_phase)
        {
            case 0:
                {
                    Event_datas = new List<JHW_Event>();
                    EventSelect_datas = new List<JHW_EventSelect>();

                    Event_phase = 1;
                }
                break;
            case 1:
                {
                    // csv 읽어들이기
                    List<Dictionary<string, object>> eventData = CSVReader.Read("HYJ/Event_csv");
                    List<Dictionary<string, object>> eventSelectData = CSVReader.Read("HYJ/EventSelect_csv");

                    for (int i = 0; i < eventData.Count; i++)
                    {
                        Event_datas.Add(new JHW_Event(eventData[i]));
                    }

                    for (int i = 0; i < eventSelectData.Count; i++)
                    {
                        EventSelect_datas.Add(new JHW_EventSelect(eventSelectData[i]));
                    }

                    // csv 초기화
                    InitCSVFile();

                    Event_phase = 2;
                }
                break;
            case 2:
                {
                    // 메서드 등록

                    // 이벤트이펙트db 데이터 얻는 메서드
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.EVENT___DATA__GET_SELECTED_EVENT, JHW_GetSelectedData);
                    // 이벤트이펙트db 데이터들 모두 얻는 메서드
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.EVENT___DATA__GET_ALL_SELECTED_EVENT, JHW_GetAllSelectedData);
                    // 이벤트이펙트db 초기화 메서드
                    // HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.EVENT___DATA__EVENT_DB_INIT, JHW_EventDatabaseInit);

                    Event_phase = 3;
                }
                break;
            case 3:
                {
                    Event_phase = -1;
                }
                break;
        }

        return (Event_phase == -1);
    }
}

#endregion

// EventEffectSelectedDataBase_csv 저장하기
#region EventSelectedDataBase.csv
partial class HYJ_Event_Manager
{
    string filepath = Path.GetFullPath("Assets/Resources/DataBase");
    string fileName = "EventSelectedDataBase.csv";

    List<string[]> data = new List<string[]>();
    public string[] csvDataLine;

    // CSV 초기화
    public void InitCSVFile()
    {
        string filepath = Path.GetFullPath("Assets/Resources/DataBase");
        string fileName = "EventSelectedDataBase.csv";

        System.IO.File.Delete(filepath + "\\" + fileName);

        csvDataLine = new string[13];
        csvDataLine[0] = "ID";
        csvDataLine[1] = "NAME";
        csvDataLine[2] = "NAME_KOR";
        csvDataLine[3] = "SCRIPT_KOR";
        csvDataLine[4] = "SCENE";
        csvDataLine[5] = "CONDITION";
        csvDataLine[6] = "VALUE_0";
        csvDataLine[7] = "VALUE_1";
        csvDataLine[8] = "SELECT";
        csvDataLine[9] = "SELECT_PAY";
        csvDataLine[10] = "SELECT_PAY_COUNT";
        csvDataLine[11] = "SELECT_EFFECT";
        csvDataLine[12] = "SELECT_EFFECT_COUNT";
        data.Add(csvDataLine);

        csvDataLine = new string[13];
        csvDataLine[0] = "Int";
        csvDataLine[1] = "String";
        csvDataLine[2] = "String";
        csvDataLine[3] = "String";
        csvDataLine[4] = "String";
        csvDataLine[5] = "String";
        csvDataLine[6] = "Int";
        csvDataLine[7] = "Int";
        csvDataLine[8] = "String";
        csvDataLine[9] = "String";
        csvDataLine[10] = "Int";
        csvDataLine[11] = "String";
        csvDataLine[12] = "String";
        data.Add(csvDataLine);

        csvDataLine = new string[13];
        csvDataLine[0] = "ID";
        csvDataLine[1] = "이름";
        csvDataLine[2] = "한글 이름";
        csvDataLine[3] = "한글 스크립트";
        csvDataLine[4] = "등장위치";
        csvDataLine[5] = "등장조건";
        csvDataLine[6] = "최소수치";
        csvDataLine[7] = "최대수치";
        csvDataLine[8] = "선택지";
        csvDataLine[9] = "선택지 자원";
        csvDataLine[10] = "지불할 수치";
        csvDataLine[11] = "효과";
        csvDataLine[12] = "효과 수치";
        data.Add(csvDataLine);

        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = data[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }
        StreamWriter outStream = System.IO.File.AppendText(filepath + "\\" + fileName);
        outStream.Write(sb.ToString(), Encoding.UTF8);
        outStream.Close();
    }
    public void SaveCSVFile(JHW_EventSelect eventSelectResult)
    {
        string filepath = Path.GetFullPath("Assets/Resources/DataBase");
        string fileName = "EventSelectedDataBase.csv";

        data.Clear();

        csvDataLine = new string[13];
        csvDataLine[0] = randomEventID.ToString();                                // ID
        csvDataLine[1] = Event_datas[randomEventID].Data_NAME;                    // 이름
        csvDataLine[2] = Event_datas[randomEventID].Data_NAME_KOR;                // 한글 이름
        csvDataLine[3] = Event_datas[randomEventID].Data_SCRIPT_KOR;              // 한글 스크립트
        csvDataLine[4] = Event_datas[randomEventID].Data_SCENE;                   // 등장위치
        csvDataLine[5] = Event_datas[randomEventID].Data_CONDITION;               // 등장조건
        csvDataLine[6] = Event_datas[randomEventID].Data_VALUE_0.ToString();      // 최소수치
        csvDataLine[7] = Event_datas[randomEventID].Data_VALUE_1.ToString();      // 최대수치
        csvDataLine[8] = eventSelectResult.Data_SELECT;                           // 선택지
        csvDataLine[9] = eventSelectResult.Data_SELECT_PAY;                       // 지불자원 (마나,체력 등)
        csvDataLine[10] = eventSelectResult.Data_SELECT_PAY_COUNT.ToString();     // 지불 수치
        csvDataLine[11] = eventSelectResult.Data_SELECT_EFFECT0;                  // 효과
        csvDataLine[12] = eventSelectResult.Data_SELECT_EFFECT0_COUNT;            // 효과 수치
        data.Add(csvDataLine);

        string[][] output = new string[data.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = data[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }

        StreamWriter outStream = System.IO.File.AppendText(filepath + "\\" + fileName);
        outStream.Write(sb.ToString());
        outStream.Close();
        //AssetDatabase.Refresh(); // EventSelectedDataBase.csv 가 자동빌드되지 않으므로 메서드로 최신 데이터가 안읽히는 경우 발생, 이를 막기위해 Refresh
    }

}
#endregion

#region 타입 모음
// Event_csv 타입
# region Event_generic
public class JHW_Event
{
    public int Data_ID; // 이벤트 아이디
    public string Data_NAME; // 이름 (JoinHightElf)
    public string Data_NAME_KOR; // 한글 이름 (하이엘프 부족 조우)
    public string Data_SCRIPT_KOR; // 한글 스크립트 (굶주리고 있는 하이엘프 종족 부족을 발견하였습니다.)
    public string Data_SCENE; // 등장위치 (EVENT)
    public string Data_CONDITION; // 등장조건 
    public int Data_VALUE_0; // 최소수치
    public int Data_VALUE_1; // 최대수치 
    public string Data_SELECTED1_SCRIPT_KOR;
    public string Data_SELECTED2_SCRIPT_KOR;

    // 생성자
    public JHW_Event(Dictionary<string, object> _data)
    {
        Data_ID = (int)_data["ID"];
        Data_NAME = (string)_data["NAME"];
        Data_NAME_KOR = (string)_data["NAME_KOR"];
        Data_SCRIPT_KOR = (string)_data["SCRIPT_KOR"];
        Data_SCENE = (string)_data["SCENE"];
        Data_CONDITION = (string)_data["CONDITION"];
        Data_VALUE_0 = (int)_data["VALUE_0"];
        Data_VALUE_1 = (int)_data["VALUE_1"];
        Data_SELECTED1_SCRIPT_KOR = (string)_data["SELECTED1_SCRIPT_KOR"];
        Data_SELECTED2_SCRIPT_KOR = (string)_data["SELECTED2_SCRIPT_KOR"];
    }
}
#endregion

// EventSelect_csv 타입
#region EventSelect_generic
public class JHW_EventSelect
{
    public int Data_ID; // ID
    public int Data_EVENT_ID; // 이벤트ID 
    public string Data_SELECT; // 선택지 (돕는다, 무시한다)
    public string Data_SELECT_PAY; // 지불할 자원 (MANA,NONE,HP...)
    public int Data_SELECT_PAY_COUNT; // 지불할 수치 (10,0 ... )
    public string Data_SELECT_EFFECT0; // 효과
    public string Data_SELECT_EFFECT0_COUNT; // 효과 수치 
    public string Data_SELECT_EFFECT1; // 효과
    public string Data_SELECT_EFFECT1_COUNT; // 효과 수치
    public string Data_SELECT_EFFECT2; // 효과
    public string Data_SELECT_EFFECT2_COUNT; // 효과 수치

    public JHW_EventSelect(Dictionary<string, object> _data)
    {
        Data_ID = (int)_data["ID"];
        Data_EVENT_ID = (int)_data["EVENT_ID"];
        Data_SELECT = (string)_data["SELECT"];
        Data_SELECT_PAY = (string)_data["SELECT_PAY"];
        Data_SELECT_PAY_COUNT = (int)_data["SELECT_PAY_COUNT"];
        Data_SELECT_EFFECT0 = (string)_data["SELECT_EFFECT0"];
        Data_SELECT_EFFECT0_COUNT = (string)_data["SELECT_EFFECT0_COUNT"];
        Data_SELECT_EFFECT1 = (string)_data["SELECT_EFFECT1"];
        Data_SELECT_EFFECT1_COUNT = (string)_data["SELECT_EFFECT1_COUNT"];
        Data_SELECT_EFFECT2 = (string)_data["SELECT_EFFECT2"];
        Data_SELECT_EFFECT2_COUNT = (string)_data["SELECT_EFFECT2_COUNT"];
    }
}
#endregion

// EventSeletedData 타입
#region EventSelectedData_generic
public class JHW_EventSelectedData
{
    public int Data_ID; // 이벤트 아이디
    public string Data_NAME; // 이름 (JoinHightElf)
    public string Data_NAME_KOR; // 한글 이름 (하이엘프 부족 조우)
    public string Data_SCRIPT_KOR; // 한글 스크립트 (굶주리고 있는 하이엘프 종족 부족을 발견하였습니다.)
    public string Data_SCENE; // 등장위치 (EVENT)
    public string Data_CONDITION; // 등장조건 
    public int Data_VALUE_0; // 최소수치
    public int Data_VALUE_1; // 최대수치 
    public string Data_SELECT; // 선택지
    public string Data_SELECT_PAY; // 선택지 자원
    public int Data_SELECT_PAY_COUNT; // 지불할 수치
    public string Data_SELECT_EFFECT; // 효과
    public string Data_SELECT_EFFECT_COUNT; // 효과 수치

    // 생성자
    public JHW_EventSelectedData(Dictionary<string, object> _data)
    {
        Data_ID = (int)_data["ID"];
        Data_NAME = (string)_data["NAME"];
        Data_NAME_KOR = (string)_data["NAME_KOR"];
        Data_SCRIPT_KOR = (string)_data["SCRIPT_KOR"];
        Data_SCENE = (string)_data["SCENE"];
        Data_CONDITION = (string)_data["CONDITION"];
        Data_VALUE_0 = (int)_data["VALUE_0"];
        Data_VALUE_1 = (int)_data["VALUE_1"];
        Data_SELECT = (string)_data["SELECT"];
        Data_SELECT_PAY = (string)_data["SELECT_PAY"];
        Data_SELECT_PAY_COUNT = (int)_data["SELECT_PAY_COUNT"];
        Data_SELECT_EFFECT = (string)_data["SELECT_EFFECT"];
        Data_SELECT_EFFECT_COUNT = (string)_data["SELECT_EFFECT_COUNT"];
    }
}
#endregion
#endregion

// 메서드들 (랜덤 이벤트ID 발생 / 텍스트 변경 / database 적재 등)
#region 메서드/함수
partial class HYJ_Event_Manager
{
    // 랜덤 이벤트ID 발생 메서드
    private int JHW_setRandomEventID()
    {
        int randomID = Random.Range(0, 3); // 중간 발표 때문에 0~3으로 임시조정
        while (randomEventID == randomID) // 이미 선택된 이벤트면 다시뽑는다
        {
            randomID = Random.Range(0, 3);
        }

        /*int randomID = Random.Range(0, Event_datas.Count);
        while (randomEventID == randomID) // 이미 선택된 이벤트면 다시뽑는다
        {
            randomID = Random.Range(0, Event_datas.Count);
        }*/
        return randomID;
    }

    // 이벤트 내의 텍스트들을 랜덤 이벤트ID에 맞춰 변경
    private void JHW_displayEventText()
    {
        eventName.text = Event_datas[randomEventID].Data_NAME_KOR; // 이벤트 좌측상단 이벤트제목
        script.text = Event_datas[randomEventID].Data_SCRIPT_KOR; // 이벤트 우측 내용

        List<JHW_EventSelect> eventSelectResult = EventSelect_datas.FindAll(x => x.Data_EVENT_ID.Equals(randomEventID));// 이벤트셀렉트 선택지1,2
        if (eventSelectResult.Count == 2)
        {
            choice1.text = eventSelectResult[0].Data_SELECT; // 이벤트 버튼1 텍스트
            choice2.text = eventSelectResult[1].Data_SELECT; // 이벤트 버튼2 텍스트
        }
    }

    // 첫번째 선택지 버튼 누를 때 발생하는 함수
    public void ClickButtonOne(bool isClicked)
    {
        // 이벤트 효과
        List<JHW_EventSelect> eventSelectResult = EventSelect_datas.FindAll(x => x.Data_EVENT_ID.Equals(randomEventID));// 이벤트셀렉트 선택지1,2
        SaveCSVFile(eventSelectResult[0]);
        // 결과 선택 후 등장하는 스크립트로 변경
        string eventResultScript = Event_datas[randomEventID].Data_SELECTED1_SCRIPT_KOR.Replace("n", "\n");
        script.text = eventResultScript;
        if (script.text.Length > 100) script.fontSize = 25;
        else script.fontSize = 30;
        Debug.Log(script.text.Length);
        eventButton1.SetActive(false);
        eventButton2.SetActive(false);
        eventResultButton.SetActive(true);
    }

    // 두번째 선택지 버튼 누를 때 
    public void ClickButtonTwo(bool isClicked)
    {
        // 이벤트 효과
        List<JHW_EventSelect> eventSelectResult = EventSelect_datas.FindAll(x => x.Data_EVENT_ID.Equals(randomEventID));// 이벤트셀렉트 선택지1,2
        SaveCSVFile(eventSelectResult[1]);
        // 결과 선택 후 등장하는 스크립트로 변경
        string eventResultScript = Event_datas[randomEventID].Data_SELECTED2_SCRIPT_KOR.Replace("n","\n");
        if (script.text.Length > 100) script.fontSize = 25;
        else script.fontSize = 30;
        script.text = eventResultScript;
        eventButton1.SetActive(false);
        eventButton2.SetActive(false);
        eventResultButton.SetActive(true);
    }

    // 결과창 버튼
    public void ClickResultButton(bool isClicked)
    {
        // 누르면 false 가 리턴됨
        HYJ_SetActive(isClicked);
        // 활성화시킨 버튼 비활성화 / 비활성화시킨 버튼 활성화 / 스크립트 원래크기로
        eventButton1.SetActive(true);
        eventButton2.SetActive(true);
        eventResultButton.SetActive(false);
        script.fontSize = 30;
    }

    public object JHW_GetSelectedData(params object[] _args) // EventSelectedDataBase.csv의 가장 최근 데이터 리턴
    {
        List<Dictionary<string, object>> eventSelectedData = CSVReader.Read("DataBase/EventSelectedDataBase");
        JHW_EventSelectedData recentSelectedEvent = new(eventSelectedData[^1]);
        return recentSelectedEvent;
    }

    public object JHW_GetAllSelectedData(params object[] _args) // EventselectedDataBase.csv에 있는 모든 데이터 리스트 리턴
    {
        List<Dictionary<string, object>> eventSelectedData = CSVReader.Read("DataBase/EventSelectedDataBase");

        List<JHW_EventSelectedData> EventSelectedDataList = new List<JHW_EventSelectedData>();
        for (int i = 0; i < eventSelectedData.Count; i++)
        {
            EventSelectedDataList.Add(new JHW_EventSelectedData(eventSelectedData[i]));
        }
        return EventSelectedDataList;
    }

    // csv 초기화
    //public object JHW_EventDatabaseInit(params object[] _args)
    //{
    //    InitCSVFile();
    //    return null;
    //}
}
#endregion
