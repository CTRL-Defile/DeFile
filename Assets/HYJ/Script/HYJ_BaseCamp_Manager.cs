using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public partial class HYJ_BaseCamp_Manager : MonoBehaviour
{
    [SerializeField] int Basic_initialize;

    [SerializeField] private int actionCntMax = 10; // 최대행동개수
    [SerializeField] private int actionCnt = 0; // 현재 행동개수

    [SerializeField] private Text gaugeText; // 행동개수 보일 텍스트
    [SerializeField] private Transform gaugeImgs; // 게이지 아이콘 이미지들

    [SerializeField] private int restStack = 0; // 휴식 시 다음 휴식에 필요한 행동개수 증가하기 위한 변수
    [SerializeField] private int removeStack = 0; // 유닛삭제 시 다음 휴식에 필요한 행동개수 증가하기 위한 변수

    int randomSelectUnitNumber1; // 선택된 유닛 인덱스 번호1
    int randomSelectUnitNumber2; // 선택된 유닛 인덱스 번호2
    int randomSelectUnitNumber3; // 선택된 유닛 인덱스 번호3

    GameObject unitList1; // 유닛리스트 1 (이미지)
    GameObject unitList2; // 유닛리스트 2 (이미지)
    GameObject unitList3; // 유닛리스트 3 (이미지)

    Dictionary<string, object> selectedUnit1; // 랜덤하게 선택된 유닛 데이터1
    Dictionary<string, object> selectedUnit2; // 랜덤하게 선택된 유닛 데이터1
    Dictionary<string, object> selectedUnit3; // 랜덤하게 선택된 유닛 데이터1

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

    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch(Basic_initialize)
        {
            case -1:    break;
            //
            case 0:
                {
                    gaugeText = GameObject.Find("GaugeText").transform.GetChild(0).GetComponent<Text>(); // 게이지 텍스트
                    gaugeImgs = GameObject.Find("Gauge").transform.GetChild(1); // 게이지 이미지 아이콘
                    this.actionCnt = this.actionCntMax; // 행동개수를 최대 행동개수로설정
                    ChangeGaugeUI(); // UI 변경

                    Camera camera
                        = (Camera)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                            HYJ_ScriptBridge_EVENT_TYPE.MASTER___UI__GET_CAMERA,
                            0);

                    if (camera != null)
                    {
                        this.transform.Find("Canvas").GetComponent<Canvas>().worldCamera = camera;

                        Basic_initialize = 1;
                    }
                }
                break;
            case 1:
                {
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON, HYJ_ActiveOn);

                    this.HYJ_SetActive(true);

                    Basic_initialize = -1;
                }
                break;
        }
    }
}

// 메서드
partial class HYJ_BaseCamp_Manager {
    // 정비
    public void JHW_BaseCamp_Maintenance()
    {
        GameObject.Find("BaseCamp").transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false); // 베이스캠프 선택지 뜨는 이미지 비활성화
        GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject.SetActive(true); // 베이스캠프 유닛 선택하는 이미지 활성화

        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/UnitDataBase");

        // 유닛 랜덤 인덱스 중복제거 뽑기
        bool[] dataFlag = new bool[unitDatas.Count];
        for (int i = 0; i < unitDatas.Count; i++) dataFlag[i] = false;
        randomSelectUnitNumber1 = Random.Range(0, unitDatas.Count);
        dataFlag[randomSelectUnitNumber1] = true;
        do { randomSelectUnitNumber2 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber2] == true);
        dataFlag[randomSelectUnitNumber2] = true;
        do { randomSelectUnitNumber3 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber3] == true);

        // 랜덤하게 선택된 유닛
        selectedUnit1 = unitDatas[randomSelectUnitNumber1];
        selectedUnit2 = unitDatas[randomSelectUnitNumber2];
        selectedUnit3 = unitDatas[randomSelectUnitNumber3];

        GameObject unitList = GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject;
        unitList1 = unitList.transform.GetChild(0).gameObject;
        unitList2 = unitList.transform.GetChild(1).gameObject;
        unitList3 = unitList.transform.GetChild(2).gameObject;

        // 유닛카드에 보여지는 텍스트(여러 수치들) 변경
        ChangeCardUI();
    }

    // 휴식
    public void JHW_BaseCamp_Rest()
    {
        int minusActionCnt = 2 + this.restStack; // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) return; // 행동개수 부족하면 실행X 

        actionCnt -= (2 + this.restStack++); // 휴식 사용 시 다음 휴식에 필요한 행동개수 증가 및 행동개수 감소

        // 10만큼 체력회복
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__HP_INCREASE, 10);

        ChangeGaugeUI(); // Gauge UI 변경
    }

    // 삭제버튼 on/off
    public void OnOffDeleteButton(GameObject gameObject)
    {
        unitList1.transform.GetChild(1).gameObject.SetActive(false);
        unitList2.transform.GetChild(1).gameObject.SetActive(false);
        unitList3.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    // Gauge UI 변경
    public void ChangeGaugeUI()
    {
        this.gaugeText.text = this.actionCnt + "/" + this.actionCntMax;
        for (int i = 0; i < this.actionCntMax; i++)
        {
            this.gaugeImgs.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }
        for (int i = 0; i < this.actionCnt; i++)
        {
            this.gaugeImgs.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
    }


    // 리롤
    public void Reroll()
    {
        int minusActionCnt = 1; // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) return; // 행동개수 부족하면 실행X 

        actionCnt -= minusActionCnt; // 휴식 사용 시 다음 휴식에 필요한 행동개수 증가 및 행동개수 감소

        ChangeGaugeUI(); // 게이지 UI 변경

        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/UnitDataBase");

        // 유닛 랜덤 인덱스 중복제거 뽑기
        bool[] dataFlag = new bool[unitDatas.Count];
        for (int i = 0; i < unitDatas.Count; i++) dataFlag[i] = false;
        randomSelectUnitNumber1 = Random.Range(0, unitDatas.Count);
        dataFlag[randomSelectUnitNumber1] = true;
        do { randomSelectUnitNumber2 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber2] == true);
        dataFlag[randomSelectUnitNumber2] = true;
        do { randomSelectUnitNumber3 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber3] == true);
        dataFlag[randomSelectUnitNumber3] = true;

        // 랜덤하게 선택된 유닛
        selectedUnit1 = unitDatas[randomSelectUnitNumber1];
        selectedUnit2 = unitDatas[randomSelectUnitNumber2];
        selectedUnit3 = unitDatas[randomSelectUnitNumber3];

        GameObject unitList = GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject;
        unitList1 = unitList.transform.GetChild(0).gameObject;
        unitList2 = unitList.transform.GetChild(1).gameObject;
        unitList3 = unitList.transform.GetChild(2).gameObject;

        // 카드 내에 수치 및 이름 설정
        ChangeCardUI();

        //삭제버튼 안보이게
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.gameObject.SetActive(false);
    }

    // 카드 내에 수치 및 이름 설정
    public void ChangeCardUI()
    {
        //카드1
        unitList1.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["NameKor"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["MaxHP"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["MaxMP"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["AttackPower"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(4).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["SpellPower"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(5).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["Defence"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(6).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["SpellResistance"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(7).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["CriticalChance"].ToString();
        unitList1.transform.GetChild(0).transform.GetChild(2).transform.GetChild(8).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit1["CriticalMultiplier"].ToString();

        //카드2
        unitList2.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["NameKor"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["MaxHP"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["MaxMP"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["AttackPower"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(4).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["SpellPower"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(5).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["Defence"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(6).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["SpellResistance"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(7).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["CriticalChance"].ToString();
        unitList2.transform.GetChild(0).transform.GetChild(2).transform.GetChild(8).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit2["CriticalMultiplier"].ToString();

        //카드3
        unitList3.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["NameKor"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["MaxHP"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["MaxMP"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(3).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["AttackPower"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(4).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["SpellPower"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(5).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["Defence"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(6).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["SpellResistance"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(7).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["CriticalChance"].ToString();
        unitList3.transform.GetChild(0).transform.GetChild(2).transform.GetChild(8).transform.GetChild(0).transform.gameObject.GetComponent<Text>().text = selectedUnit3["CriticalMultiplier"].ToString();
    }

    public void DeleteUnit(int number)
    {
        int deleteLineNumber=0; //삭제할 UnitDataBase 라인 인덱스
        if (number == 1) deleteLineNumber = randomSelectUnitNumber1+3;
        if (number == 2) deleteLineNumber = randomSelectUnitNumber2+3;
        if (number == 3) deleteLineNumber = randomSelectUnitNumber3+3;

        string[] lines;

        // 유닛 데이터 읽어오기
        lines = File.ReadAllLines("Assets/Resources/DataBase/UnitDataBase.csv");
        // 유닛 데이터 쓰기
        StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/UnitDataBase1.csv");
        // 삭제되는 유닛 라인은 가장 마지막 라인으로 대체
        lines[deleteLineNumber] = lines[lines.Length-1];
        for (int i = 0; i < lines.Length-1; i++) {outStream.WriteLine(lines[i].ToString());}
        outStream.Close();
    }

    public void DeleteCard(int deleteNumber)
    {
        int minusActionCnt = 1+(this.removeStack++); // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) return; // 행동개수 부족하면 실행X 

        // 유닛 삭제
        DeleteUnit(deleteNumber);

        actionCnt -= minusActionCnt; // 행동개수 삭제

        GameObject.Find("BaseCamp").transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true); // 베이스캠프 UI 활성화
        GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject.SetActive(false); // 유닛 편성 UI 비활성화

        //삭제버튼 안보이게
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.gameObject.SetActive(false);

        // 게이지 이미지 on/off
        ChangeGaugeUI();
    }
}
