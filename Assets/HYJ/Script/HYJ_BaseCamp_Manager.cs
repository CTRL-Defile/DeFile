using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    // 휴식에서 감소시킬 행동개수
    private int rest_minusActionCnt = 2;


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

        //if (_isActive == false)
        //{
        //    BaseCamp_ExitButton_OnClick(); // 베이스캠프 나갈때 ux
        //}
        
        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;
        DOTween.Init();
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
    public void JHW_BaseCamp_Maintanance()
    {
        

        // 베이스캠프 유닛 선택하는 이미지 활성화
        GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject.SetActive(true);

        // 베이스캠프 선택지 고르는거 비활성화
        this.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/DB_Using_Character");

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

        // ux
        // 카드1
        GameObject.Find("UnitCard1").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard1").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드2
        GameObject.Find("UnitCard2").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard2").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드3
        GameObject.Find("UnitCard3").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard3").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                GameObject.Find("UnitCard1").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360);
                GameObject.Find("UnitCard1").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack);
                GameObject.Find("CardBack1").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f);
            })
            .Insert(0.2f, GameObject.Find("UnitCard2").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(GameObject.Find("UnitCard2").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
            .Join(GameObject.Find("CardBack2").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f))
            .Insert(0.4f, GameObject.Find("UnitCard3").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(GameObject.Find("UnitCard3").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
            .Join(GameObject.Find("CardBack3").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f)
            );

        // 유닛카드에 보여지는 텍스트(여러 수치들) 변경
        ChangeCardUI();

        // ux
        GameObject.Find("RerollButton").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("RerollButton").transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0f);
        GameObject.Find("RerollButton").transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.5f).SetDelay(2f);
        GameObject.Find("RerollButton").transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().DOFade(0f, 0f);
        GameObject.Find("RerollButton").transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().DOFade(1f, 0.5f).SetDelay(2f);
    }

    // 휴식
    public void JHW_BaseCamp_Rest()
    {
        if (isClicked == true) return; // 이미 누른 상태면 실행X

        if (this.actionCnt < rest_minusActionCnt) {
            ActionCntAlert();
            return; 
        }  // 행동개수 부족시 ux 및 리턴

        if (isRestAble == false) return; // 쉴수없으면 실행X

        // 휴식 사용 시 다음 휴식에 필요한 행동개수 증가 및 행동개수 감소
        rest_minusActionCnt += this.restStack++;
        actionCnt -= rest_minusActionCnt; 
        
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

        // ux
        if (gameObject.name == "1")
        {
            // 카드1
            GameObject.Find("UnitCard1").transform.DOScale(1.05f, .5f);
            GameObject.Find("UnitCard1").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // 카드2
            GameObject.Find("UnitCard2").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard2").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드3
            GameObject.Find("UnitCard3").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard3").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "2")
        {
            // 카드1
            GameObject.Find("UnitCard1").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard1").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드2
            GameObject.Find("UnitCard2").transform.DOScale(1.05f, .5f);
            GameObject.Find("UnitCard2").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // 카드3
            GameObject.Find("UnitCard3").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard3").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "3")
        {
            // 카드1
            GameObject.Find("UnitCard1").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard1").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드2
            GameObject.Find("UnitCard2").transform.DOScale(0.9f, .5f);
            GameObject.Find("UnitCard2").transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드3
            GameObject.Find("UnitCard3").transform.DOScale(1.05f, .5f);
            GameObject.Find("UnitCard3").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        }
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
        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.REROLL);

        int minusActionCnt = 1; // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // 행동개수 부족하면 실행X 

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


        // ux
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                GameObject.Find("UnitCard1").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360);
                GameObject.Find("UnitCard1").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack);
            })
            .Insert(0.1f, GameObject.Find("UnitCard2").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(GameObject.Find("UnitCard2").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
            .Insert(0.2f, GameObject.Find("UnitCard3").transform.DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(GameObject.Find("UnitCard3").transform.GetChild(0).transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack)
            );

        // 카드1
        GameObject.Find("UnitCard1").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard1").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드2
        GameObject.Find("UnitCard2").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard2").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드3
        GameObject.Find("UnitCard3").transform.DOScale(1f, .5f);
        GameObject.Find("UnitCard3").transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
    }

    // 카드 내에 수치 및 이름 설정
    public void ChangeCardUI()
    {
        

        //카드1
        GameObject.Find("ChampLabel1").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["NAME_KOR"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_MaxHP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["MAX_HP"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_MaxMP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["MAX_MP"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_PhyAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["ATK_PHYSICS"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_SpellAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["ATK_SPELL"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_Defence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["DEFENCE"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_SpellDefence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["SPELL_REGISTANCE"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_CritChance").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["CRIT_PERCENT"].ToString();
        GameObject.Find("StatusLabel1/BaseCamp_CritMulti").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["CRIT_VALUE"].ToString();

        //카드2
        GameObject.Find("ChampLabel2").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["NAME_KOR"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_MaxHP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["MAX_HP"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_MaxMP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["MAX_MP"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_PhyAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["ATK_PHYSICS"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_SpellAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["ATK_SPELL"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_Defence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["DEFENCE"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_SpellDefence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["SPELL_REGISTANCE"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_CritChance").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["CRIT_PERCENT"].ToString();
        GameObject.Find("StatusLabel2/BaseCamp_CritMulti").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["CRIT_VALUE"].ToString();

        //카드3
        GameObject.Find("ChampLabel3").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["NAME_KOR"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_MaxHP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["MAX_HP"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_MaxMP").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["MAX_MP"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_PhyAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["ATK_PHYSICS"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_SpellAttk").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["ATK_SPELL"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_Defence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["DEFENCE"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_SpellDefence").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["SPELL_REGISTANCE"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_CritChance").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["CRIT_PERCENT"].ToString();
        GameObject.Find("StatusLabel3/BaseCamp_CritMulti").transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["CRIT_VALUE"].ToString();

    }

    public void DeleteUnit(int number)
    {
        int deleteLineNumber=0; //삭제할 UnitDataBase 라인 인덱스
        if (number == 1) deleteLineNumber = randomSelectUnitNumber1+3;
        if (number == 2) deleteLineNumber = randomSelectUnitNumber2+3;
        if (number == 3) deleteLineNumber = randomSelectUnitNumber3+3;

        string[] lines;

        // 유닛 데이터 읽어오기
        lines = File.ReadAllLines("Assets/Resources/DataBase/DB_Using_Character.csv");
        // 유닛 데이터 쓰기
        StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase.csv");
        // 삭제되는 유닛 라인은 가장 마지막 라인으로 대체
        lines[deleteLineNumber] = lines[lines.Length-1];
        for (int i = 0; i < lines.Length-1; i++) {outStream.WriteLine(lines[i].ToString());}
        outStream.Close();
    }

    public void DeleteCard(int deleteNumber)
    {
        int minusActionCnt = 1+(this.removeStack++); // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // 행동개수 부족하면 실행X 

        // 유닛 삭제
        DeleteUnit(deleteNumber);

        actionCnt -= minusActionCnt; // 행동개수 삭제

        // ux
        GameObject.Find("RerollButton/Image").GetComponent<Image>().DOFade(0f, 0.5f);
        GameObject.Find("RerollButton/Image/Text").GetComponent<Text>().DOFade(0f, 0.5f);

        // 카드 일단 뒤로 보낸다
        GameObject.Find("UnitCard1").transform.GetChild(0).transform.DOLocalMoveY(-1000f, 1f);
        GameObject.Find("UnitCard2").transform.GetChild(0).transform.DOLocalMoveY(-1000f, 1f);
        GameObject.Find("UnitCard3").transform.GetChild(0).transform.DOLocalMoveY(-1000f, 1f);

        GameObject.Find("BaseCamp").transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true); // 베이스캠프 UI 활성화
        GameObject.Find("UnitList").transform.GetChild(0).transform.gameObject.SetActive(false); // 유닛 편성 UI 비활성화

        //삭제버튼 안보이게
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(1).transform.GetChild(1).transform.gameObject.SetActive(false);
        GameObject.Find("UnitList").transform.GetChild(0).transform.GetChild(2).transform.GetChild(1).transform.gameObject.SetActive(false);

        // 게이지 이미지 on/off
        ChangeGaugeUI();

        //ux
        GameObject g = this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //GameObject.Find("BaseCamp/Canvas").transform.GetChild(1).GetComponent<Image>().DOFade(1f, 0.5f);
        g.transform.parent.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.7f).SetEase(Ease.InSine);
        g.transform.parent.GetChild(0).gameObject.transform.GetComponent<Image>().DOFade(0.66f, 0.8f);
        g.transform.parent.GetChild(1).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(2).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(3).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(4).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);


    }
}

// UX
partial class HYJ_BaseCamp_Manager
{
    private bool isMouseEntered = false;
    private bool isClicked = false;
    private bool isMouseDown = false;
    private int cardNumber = 0;

    private bool isRestAble = true;
    private bool isMaintananceAble = true;

    private Stack<GameObject> actionCntStack = new Stack<GameObject>();

    private GameObject info;

    public void Button_OnMouseEnter(GameObject g)
    {
        // 눌러진상태면 실행x
        if (isClicked) return;
        // 실행할 수 없으면 실행x
        if (g == GameObject.Find("RestButton") && isRestAble == false)
        {
            return;
        }

        // 게이지
        GameObject gauge = GameObject.Find("Gauge/Image");

        // 마우스 올려진상태면 크게/색변경
        g.transform.DOScale(new Vector3(1.1f, 1.1f,1f), .2f);
        //g.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0.8f), 0.1f);
        isMouseEntered = true;

        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BUTTON_MOUSEOVER);

        // 정보창
        info = GameObject.Find("Info_Canvas");

        //info.transform.position = g.transform.position; info.transform.GetComponent<RectTransform>().rect.x+=50; // 위치조정
        info.transform.GetComponent<Image>().DOFade(1, 0.2f); // 이미지 페이드
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(1, 0.2f); // 텍스트 페이드
        info.transform.SetParent(g.transform.parent);
        Vector3 newPos = g.transform.position;
        newPos.y += 1f;
        info.transform.transform.position = newPos;

        // 휴식일 경우
        if (g == GameObject.Find("RestButton"))
        {
            if (actionCnt > 0)
            {
                info.transform.GetChild(0).transform.GetComponent<Text>().text = "행동개수 -" + (rest_minusActionCnt + restStack) + "\n체력 +10";
                for (int i = rest_minusActionCnt + restStack; i > 0; i--)
                {
                    if (actionCnt - i < 0) break;
                    if (gauge.transform.GetChild(actionCnt - i) == null) break;
                    gauge.transform.GetChild(actionCnt - i).DOScale(new Vector3(1.25f, 1.25f,1f), 0.2f);
                    gauge.transform.GetChild(actionCnt - i).GetChild(0).GetComponent<Image>().DOFade(0.7f, 0.2f);
                    actionCntStack.Push(gauge.transform.GetChild(actionCnt - i).gameObject);
                }
            }
        }
        // 정비일 경우
        if (g == GameObject.Find("MaintananceButton"))
        {
            GameObject.Find("InfoText").transform.GetComponent<Text>().text = "가진 유닛 3개 중\n1개를 버립니다.";
        }

        //// 마우스 올려져있을때 실행하는거
        //StartCoroutine(OnMouseOver(g));
    }

    private IEnumerator OnMouseOver(GameObject g)
    {
        while (isMouseEntered && !isClicked)
        {
            //Vector3 point = Camera.main.ScreenToViewportPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
            //Debug.Log(point.x + "," + point.y);
            if (g == GameObject.Find("RestButton"))
            {
                if(isMouseDown==false && isRestAble==true) g.GetComponent<Image>().DOColor(new Color(1f, 0.5f, 1f, 1f), 0.1f);
                else
                {
                    if (isMouseDown == true) StopCoroutine(OnMouseOver(g));
                    if (isRestAble == true)
                    {
                        g.transform.DOScale(new Vector3(1.05f, 1.05f), 0f);
                        g.GetComponent<Image>().DOFade(0.7f, 0f);
                    }
                }
                
                //info.transform.position = new Vector3(point.x* Camera.main.aspect , point.y*Camera.main.orthographicSize);
                yield return new WaitForSeconds(0.02f);
            }
            else yield return new WaitForSeconds(0f);
        }
    }

    public void Button_OnMouseExit(GameObject g)
    {


        // 실행할 수 없으면 실행x
        if (g == GameObject.Find("RestButton") && isRestAble == false) return;

        g.transform.DOScale(new Vector3(1f, 1f,1f), 0.2f);
        isMouseEntered = false;

        info.transform.GetComponent<Image>().DOFade(0, 0.2f);
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(0, 0.2f);
        //g.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1f), 0.2f).SetDelay(0.001f);

        // 행동개수 ux
        while (actionCntStack.Count != 0)
        {
            GameObject actionCntUX = actionCntStack.Pop();
            actionCntUX.transform.DOScale(new Vector3(1f, 1f,1f), 0.2f);
            actionCntUX.transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.2f);
        }
        //StopCoroutine(OnMouseOver(g));
    }

    public void Button_OnMouseDown()
    {
        isMouseDown = true;
    }

    public void Button_OnMouseUp()
    {
        isMouseDown = false;
    }

    public void Button_OnMouseClick(GameObject g)
    {
        // 클릭한상태면 리턴
        if (isClicked) return;
        isClicked = true;
        Invoke("clickFlagChange", 1f);

        // 휴식 할 수 없으면 리턴
        if (g == GameObject.Find("RestButton") && isRestAble == false) { ActionCntAlert(); return; }
        // 점검 할 수 없으면 리턴
        if (g == GameObject.Find("MaintananceButton") && isMaintananceAble == false) return;

        // 점검 클릭시 1초 뒤 점검ㄱ
        if (g == GameObject.Find("MaintananceButton")) {
            Invoke("JHW_BaseCamp_Maintanance", 1f);

            //ux
            g.transform.parent.gameObject.transform.DOScale(new Vector3(0f, 0f,1f), 0.7f).SetEase(Ease.InSine);
            g.transform.parent.GetChild(0).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(1).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(2).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(3).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(4).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
        }

        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BASECAMP_OPEN_UNIT_DELETE_TITLE);

        // ux 오브젝트
        GameObject uxObject = Instantiate(g, g.transform.position, g.transform.rotation);
        uxObject.GetComponent<Button>().enabled = false;
        uxObject.GetComponent<EventTrigger>().enabled = false;
        uxObject.transform.SetParent(g.transform);
        uxObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
        uxObject.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 1f);
        uxObject.GetComponent<Image>().DOFade(0f, 1f);
        uxObject.transform.GetChild(0).gameObject.GetComponent<Text>().DOFade(0f, 1f);
        Destroy(uxObject, 1f);
        

        g.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
        g.GetComponent<Image>().DOFade(1f, 0f);

        // 행동개수 검사 및 ux
        CheckActionCntAndChangeUX(g);

        // 행동개수 ux
        while (actionCntStack.Count != 0)
        {
            GameObject actionCntUX = actionCntStack.Pop();
            actionCntUX.transform.DOScale(new Vector3(1f, 1f), 0.2f);

            GameObject actionCntTempUX = Instantiate(actionCntUX, actionCntUX.transform.position, actionCntUX.transform.rotation);
            actionCntTempUX.transform.SetParent(actionCntUX.transform);
            actionCntTempUX.transform.DOScale(new Vector3(1f, 1f), 0f);
            actionCntTempUX.transform.DOScale(new Vector3(1.7f, 1.7f), 1f);
            actionCntTempUX.GetComponent<Image>().DOFade(0f, 1f);
            Destroy(actionCntTempUX, 1f);
        }
    }

    private void clickFlagChange()
    {
        isClicked = !isClicked;
    }

    bool isAlert = false;
    private void ActionCntAlert()
    {
        if (isAlert) return;

        // ux 
        GameObject alert = GameObject.Find("ActionCntAlert");
        alert.transform.GetChild(0).gameObject.SetActive(true);

        // 초기 alert 설정
        alert.transform.localPosition = new Vector3(-1000, -600);
        alert.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0);
        alert.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0.5f, 0f);

        // ux 적용
        Vector3 newVector = new Vector3(Input.mousePosition.x - Screen.width / 2 , Input.mousePosition.y - Screen.height / 2);
        alert.transform.localPosition = newVector;

        alert.transform.DOShakePosition(0.3f, 10f);
        alert.transform.GetComponent<Image>().DOColor(new Color(1f, 0f, 0f, 0.5f), 0f);
        alert.transform.GetChild(0).GetComponent<Image>().DOFade(0.6f, 0.5f);
        alert.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0.7f, 0.5f);

        alert.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(1f);
        alert.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0f, 0.5f).SetDelay(1f);
    }

    private void AlertActive() {
        GameObject.Find("ActionCntAlert").transform.GetChild(0).gameObject.SetActive(false); 
    }
    // 행동개수 알림 클릭시
    public void alertButtonClicked(GameObject g)
    {
        g.transform.GetChild(0).gameObject.SetActive(false);
    }

    // 휴식 - 행동개수를 세고 그에 맞춰 UX 조정
    private void CheckActionCntAndChangeUX(GameObject g)
    {
        if (actionCnt >= rest_minusActionCnt + restStack) isRestAble = true;
        else
        {
            isRestAble = false;
            g.GetComponent<Image>().DOFade(0.5f, 0.5f);
        }
    }

    // 캠프 나가기 버튼
    public void BaseCamp_ExitButton_OnEnter(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void BaseCamp_ExitButton_OnExit(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void BaseCamp_ExitButton_OnClick()
    {
    }

    // 점검 - 스탯정보
    bool isStatusMouseOver = false;
    public void Maintanance_Status_OnEnter(GameObject g)
    {
        isStatusMouseOver = true;

        GameObject statusInfo = GameObject.Find("StatusInfoLabel");
        statusInfo.transform.GetChild(0).transform.gameObject.SetActive(true);

        string infoText = "";

        switch (g.name)
        {
            case "BaseCamp_MaxHP": infoText = "최대 HP"; break;
            case "BaseCamp_MaxMP": infoText = "최대 MP"; break;
            case "BaseCamp_PhyAttk": infoText = "물리공격력"; break;
            case "BaseCamp_Defence": infoText = "방어력"; break;
            case "BaseCamp_SpellAttk": infoText = "주문공격력"; break;
            case "BaseCamp_SpellDefence": infoText = "주문저항력"; break;
            case "BaseCamp_CritChance": infoText = "치명타확률"; break;
            case "BaseCamp_CritMulti": infoText = "치명타배율"; break;
        }

        //statusInfo.transform.SetParent(g.transform);
        statusInfo.transform.localPosition = g.transform.localPosition;
        statusInfo.transform.DOScale(1f, 1f).SetEase(Ease.OutExpo);
        statusInfo.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText;
        StartCoroutine("Maintanance_Status_OnOver");

        
    }

    IEnumerator Maintanance_Status_OnOver()
    {
        while (isStatusMouseOver)
        {
            GameObject statusInfo = GameObject.Find("StatusInfoLabel");

            Vector3 newVector = new Vector3(Input.mousePosition.x - Screen.width / 2 + 60, Input.mousePosition.y - Screen.height / 2 +20);
            statusInfo.transform.DOLocalMove(newVector,1f);

            yield return new WaitForSeconds(0.001f);
        }
    }

    public void Maintanance_Status_OnExit(GameObject g)
    {
        isStatusMouseOver = false;

        GameObject statusInfo = GameObject.Find("StatusInfoLabel");
        statusInfo.transform.DOScale(0f, 1f).SetEase(Ease.OutExpo);
        StopCoroutine("Maintanance_Status_OnOver");
    }

    
}
