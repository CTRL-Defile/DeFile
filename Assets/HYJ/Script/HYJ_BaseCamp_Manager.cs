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

    private GameObject info; // 정보창

    // 휴식에서 감소시킬 행동개수
    private int rest_minusActionCnt = 2;


    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__SET_STAGE, HYJ_Map_Stage_TYPE.BASE_CAMP);
        List<string> playerStageDatas = (List<string>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE_DATAS);
        playerStageDatas.Clear();
        playerStageDatas.Add(actionCnt.ToString());
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___FILE__SAVE);

        //
        return null;
    }

    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        if (_isActive == false)
        {
			HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__BGM_STOP, JHW_SoundManager.BGM_list.temp_BGM);
			// 사운드
			HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BOOKSHELF_WHIP);
            //BaseCamp_ExitButton_OnClick(); // 베이스캠프 나갈때 ux
        }

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);

        if (!_isActive)
        {
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BUFF__END_STAGE);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___FILE__SAVE);
        }
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;
        InitText();
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
                    gaugeText = this.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>(); // 게이지 텍스트
                    gaugeImgs = this.transform.GetChild(0).GetChild(1).GetChild(1); // 게이지 이미지 아이콘
                    info = this.transform.GetChild(0).GetChild(6).gameObject; // 정보창

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

                    Basic_initialize = 2;
                }
                break;
            case 2:
                {
                    object playerPhase = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_UPDATE_PHASE);
                    if(playerPhase != null)
                    {
                        if(((HYJ_Player.UPDATE_PHASE)playerPhase).Equals(HYJ_Player.UPDATE_PHASE.UPDATE))
                        {
                            HYJ_Map_Stage_TYPE stageType = (HYJ_Map_Stage_TYPE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE);

                            if(stageType.Equals(HYJ_Map_Stage_TYPE.BASE_CAMP))
                            {
                                List<string> playerStageDatas = (List<string>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE_DATAS);
                                actionCnt = int.Parse(playerStageDatas[0]);
                                ChangeGaugeUI();

                                this.HYJ_SetActive(true);
                            }
                            else
                            {
                                this.HYJ_SetActive(false);
                            }
                        }
                    }

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

        
        // reroll 버튼 비활성화
        this.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(4).GetChild(0).gameObject.SetActive(false);

        // 베이스캠프 유닛 선택하는 이미지 활성화
        this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject.SetActive(true);

        // 행동포인트 이미지
        this.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);

        // 베이스캠프 나가기 이미지 비활성화
        this.transform.GetChild(0).transform.GetChild(2).transform.gameObject.SetActive(false);

        int _lv = 1;
        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/DB_Using_Character_" + _lv.ToString());

        // 카드 뒷면 뒤집은 횟수 0으로 초기화
        CardBack_UX.checkBackCardCnt = 0;

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

        GameObject unitList = this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject;
        unitList1 = unitList.transform.GetChild(0).gameObject;
        unitList2 = unitList.transform.GetChild(1).gameObject;
        unitList3 = unitList.transform.GetChild(2).gameObject;

        // ux
        // 카드1
        unitList1.transform.DOScale(1f, .5f);
        unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드2
        unitList2.transform.DOScale(1f, .5f);
        unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드3
        unitList3.transform.DOScale(1f, .5f);
        unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                unitList1.transform.GetChild(2).DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360); // 카드뒷면
                unitList1.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack);
                //GameObject.Find("CardBack1").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f);
            })
            .Insert(0.2f, unitList2.transform.GetChild(2).DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(unitList2.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack))
            //.Join(GameObject.Find("CardBack2").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f))
            .Insert(0.4f, unitList3.transform.GetChild(2).DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360))
            .Join(unitList3.transform.DOLocalMoveY(0, 1f).SetEase(Ease.OutBack)
            //.Join(GameObject.Find("CardBack3").GetComponent<Image>().DOFade(0f, 0.5f).SetDelay(0.5f)
            );

        // 유닛카드에 보여지는 텍스트(여러 수치들) 변경
        ChangeCardUI();



        // 카드 앞면 안보이게
        unitList1.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        unitList2.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        unitList3.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        // 카드 뒷면 활성화
        unitList1.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        unitList2.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        unitList3.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    // 휴식
    public void JHW_BaseCamp_Rest()
    {
        if (isClicked == true) return; // 이미 누른 상태면 실행X

        if (this.actionCnt < rest_minusActionCnt +restStack) {
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

        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.RECOVER);

        //
        List<string> playerStageDatas = (List<string>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE_DATAS);
        playerStageDatas.Clear();
        playerStageDatas.Add(actionCnt.ToString());
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___FILE__SAVE);
    }

    // 삭제버튼 on/off
    public void OnOffDeleteButton(GameObject gameObject)
    {
        if (isRerolling) return;

        unitList1.transform.GetChild(1).gameObject.SetActive(false);
        unitList2.transform.GetChild(1).gameObject.SetActive(false);
        unitList3.transform.GetChild(1).gameObject.SetActive(false);

        gameObject.SetActive(true);

        // ux
        if (gameObject.name == "1")
        {
            // 카드1
            unitList1.transform.DOScale(1.05f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // 카드2
            unitList2.transform.DOScale(0.9f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드3
            unitList3.transform.DOScale(0.9f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "2")
        {
            // 카드1
            unitList1.transform.DOScale(0.9f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드2
            unitList2.transform.DOScale(1.05f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // 카드3
            unitList3.transform.DOScale(0.9f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "3")
        {
            // 카드1
            unitList1.transform.DOScale(0.9f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드2
            unitList2.transform.DOScale(0.9f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // 카드3
            unitList3.transform.DOScale(1.05f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
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

    private bool isRerolling;
    // 리롤
    public void Reroll()
    {
        // reroll하고있는중인지 여부
        isRerolling = true;
        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.REROLL);

        // 카드 뒷면 뒤집은 횟수 초기화
        CardBack_UX.checkBackCardCnt = 0;

        // reroll 버튼 비활성화
        this.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(4).GetChild(0).gameObject.SetActive(false);

        int minusActionCnt = 1; // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // 행동개수 부족하면 실행X 

        actionCnt -= minusActionCnt; // 휴식 사용 시 다음 휴식에 필요한 행동개수 증가 및 행동개수 감소

        ChangeGaugeUI(); // 게이지 UI 변경

        int _lv = 1;
        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/DB_Using_Character_" + _lv.ToString());

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

        // 카드 내에 수치 및 이름 설정
        Invoke("ChangeCardUI",0.5f);

        //삭제버튼 안보이게
        unitList1.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList2.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList3.transform.GetChild(1).transform.gameObject.SetActive(false);


        // ux
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                unitList1.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast); //카드 앞면
                unitList1.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f); //카드 뒷면
            })
            .Insert(0.2f, unitList2.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast))
            .Join(unitList2.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f))
            .Insert(0.4f, unitList3.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast))
            .Join(unitList3.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f)
            ).OnComplete(()=> { isRerolling = false; });

        // 카드1
        unitList1.transform.DOScale(1f, .5f);
        unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드2
        unitList2.transform.DOScale(1f, .5f);
        unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // 카드3
        unitList3.transform.DOScale(1f, .5f);
        unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
    }

    // 카드 내에 수치 및 이름 설정
    public void ChangeCardUI()
    {


        //카드1
        unitList1.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit1["ID"]);
        unitList1.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["NAME_KOR"].ToString(); // 챔피언 이름
        unitList1.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 HP\n" + selectedUnit1["MAX_HP"].ToString(); // 최대HP
        unitList1.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 MP\n" + selectedUnit1["MAX_MP"].ToString(); // 최대MP
        unitList1.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "물리공격력\n" + selectedUnit1["ATK_PHYSICS"].ToString(); // 물리공격력
        unitList1.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "방어력\n" + selectedUnit1["DEFENCE"].ToString(); // 방어력
        unitList1.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "마법공격력\n" + selectedUnit1["ATK_SPELL"].ToString(); // 마법공격력
        unitList1.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "주문저항력\n" + selectedUnit1["SPELL_REGISTANCE"].ToString(); // 마법저항력
        unitList1.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타확률\n" + selectedUnit1["CRIT_PERCENT"].ToString(); // 크리티컬확률
        unitList1.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타배율\n" + selectedUnit1["CRIT_VALUE"].ToString(); // 크리티컬배율

        //카드2
        unitList2.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit2["ID"]);
        unitList2.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["NAME_KOR"].ToString(); // 챔피언 이름
        unitList2.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 HP\n" + selectedUnit2["MAX_HP"].ToString(); // 최대HP
        unitList2.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 MP\n" + selectedUnit2["MAX_MP"].ToString(); // 최대MP
        unitList2.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "물리공격력\n" + selectedUnit2["ATK_PHYSICS"].ToString(); // 물리공격력
        unitList2.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "방어력\n" + selectedUnit2["DEFENCE"].ToString(); // 방어력
        unitList2.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "마법공격력\n" + selectedUnit2["ATK_SPELL"].ToString(); // 마법공격력
        unitList2.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "주문저항력\n" + selectedUnit2["SPELL_REGISTANCE"].ToString(); // 마법저항력
        unitList2.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타확률\n" + selectedUnit2["CRIT_PERCENT"].ToString(); // 크리티컬확률
        unitList2.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타배율\n" + selectedUnit2["CRIT_VALUE"].ToString(); // 크리티컬배율

        //카드3
        unitList3.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit3["ID"]);
        unitList3.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["NAME_KOR"].ToString(); // 챔피언 이름
        unitList3.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 HP\n" + selectedUnit3["MAX_HP"].ToString(); // 최대HP
        unitList3.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "최대 MP\n" + selectedUnit3["MAX_MP"].ToString(); // 최대MP
        unitList3.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "물리공격력\n" + selectedUnit3["ATK_PHYSICS"].ToString(); // 물리공격력
        unitList3.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "방어력\n" + selectedUnit3["DEFENCE"].ToString(); // 방어력
        unitList3.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "마법공격력\n" + selectedUnit3["ATK_SPELL"].ToString(); // 마법공격력
        unitList3.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "주문저항력\n" + selectedUnit3["SPELL_REGISTANCE"].ToString(); // 마법저항력
        unitList3.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타확률\n" + selectedUnit3["CRIT_PERCENT"].ToString(); // 크리티컬확률
        unitList3.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "치명타배율\n" + selectedUnit3["CRIT_VALUE"].ToString(); // 크리티컬배율
    }

    public void DeleteUnit(int number)
    {
        int deleteLineNumber=0; //삭제할 UnitDataBase 라인 인덱스
        if (number == 1) deleteLineNumber = randomSelectUnitNumber1;
        if (number == 2) deleteLineNumber = randomSelectUnitNumber2;
        if (number == 3) deleteLineNumber = randomSelectUnitNumber3;

        Debug.Log(number + "번 카드 id : " + deleteLineNumber + " 삭제!");
        Player_DB.Instance.idx_delete(deleteLineNumber);

#if false
        string[] lines;

        for (int i = 1; i <= 3; i++)
        {
            // 유닛 데이터 읽어오기
            lines = File.ReadAllLines("Assets/Resources/DataBase/DB_Using_Character_"+i.ToString()+".csv");
            // 유닛 데이터 쓰기
            System.IO.File.Delete("Assets/Resources/DataBase/Player_Unit_DataBase_" + i.ToString() + ".csv");
            StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase_"+i.ToString()+".csv");
            // 삭제되는 유닛 라인은 가장 마지막 라인으로 대체
            //lines[deleteLineNumber] = lines[lines.Length - 1];
            for (int j = 0; j < lines.Length; j++)
            {
                if (j == deleteLineNumber)
                {
                    //Debug.Log(j + " ########## " + lines[j].ToString());
                    continue;
                }
                //Debug.Log(j + " " + lines[j].ToString());
                outStream.WriteLine(lines[j].ToString());
            }
            outStream.Close();
        }
        //Debug.Log("삭제.");
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE);
#endif

    }

    public void DeleteCard(int deleteNumber)
    {
        if (isRerolling) return; // 리롤중이면 실행X

        int minusActionCnt = 1+(this.removeStack++); // 감소시킬 행동개수
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // 행동개수 부족하면 실행X 

        // 유닛 삭제
        DeleteUnit(deleteNumber);

        //카드 뒤집은횟수 0으로 초기화
        CardBack_UX.checkBackCardCnt = 0;

        actionCnt -= minusActionCnt; // 행동개수 삭제

        // 카드 일단 뒤로 보낸다
        unitList1.transform.DOLocalMoveY(-1000f, 1f);
        unitList2.transform.DOLocalMoveY(-1000f, 1f);
        unitList3.transform.DOLocalMoveY(-1000f, 1f);

        this.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true); // 베이스캠프 UI 활성화
        this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject.SetActive(false); // 유닛 편성 UI 비활성화

        //삭제버튼 안보이게
        unitList1.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList2.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList3.transform.GetChild(1).transform.gameObject.SetActive(false);

        // 게이지 이미지 on/off
        ChangeGaugeUI();

        //사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BASECAMP_DELETE_UNIT);

        //ux
        GameObject g = this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        //GameObject.Find("BaseCamp/Canvas").transform.GetChild(1).GetComponent<Image>().DOFade(1f, 0.5f);
        g.transform.parent.gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.7f).SetEase(Ease.InSine);
        g.transform.parent.GetChild(0).gameObject.transform.GetComponent<Image>().DOFade(0.66f, 0.8f);
        g.transform.parent.GetChild(1).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(2).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(3).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);
        g.transform.parent.GetChild(4).gameObject.transform.GetComponent<Image>().DOFade(0.5f, 0.8f);

        // 베이스캠프 나가기 이미지 활성화
        this.transform.GetChild(0).transform.GetChild(2).transform.gameObject.SetActive(true);
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

    

    public void Button_OnMouseEnter(GameObject g)
    {
        // 눌러진상태면 실행x
        if (isClicked) return;
        // 실행할 수 없으면 실행x
        if (g.name=="RestButton" && isRestAble == false)
        {
            return;
        }

        // 게이지
        GameObject gauge = this.transform.GetChild(0).GetChild(1).GetChild(1).gameObject; //게이지 이미지

        // 마우스 올려진상태면 크게/색변경
        g.transform.DOScale(new Vector3(1.1f, 1.1f,1f), .2f);
        //g.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0.8f), 0.1f);
        isMouseEntered = true;

        // 사운드
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BUTTON_MOUSEOVER);

        // 정보창
        info.transform.GetComponent<Image>().DOFade(1, 0.2f); // 이미지 페이드
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(1, 0.2f); // 텍스트 페이드
        info.transform.SetParent(g.transform.parent);
        Vector3 newPos = g.transform.position;
        newPos.y += 1f;
        info.transform.transform.position = newPos;

        // 휴식일 경우 정보창
        if (g.name=="RestButton" && actionCnt >= rest_minusActionCnt + restStack)
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
        // 정비일 경우 정보창
        if (g == GameObject.Find("MaintananceButton"))
        {
            info.transform.GetChild(0).transform.GetComponent<Text>().text = "가진 유닛 3개 중\n1개를 버립니다.";
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


        g.transform.DOScale(new Vector3(1f, 1f,1f), 0.2f);
        isMouseEntered = false;

        // 정보창 페이드
        info.transform.GetComponent<Image>().DOFade(0, 0.2f);
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(0, 0.2f);
        //g.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1f), 0.2f).SetDelay(0.001f);

        // 행동개수 ux
        // 실행할 수 없으면 실행x
        // if (g == GameObject.Find("RestButton") && isRestAble == false) return;
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

        // 점검 할 수 없으면 리턴
        if (g.name=="MaintananceButton" && isMaintananceAble == false) return;

        // 휴식 클릭시
        if (g.name == "RestButton")
        {
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

            // 휴식 - 행동개수 검사
            if (actionCnt >= rest_minusActionCnt + restStack) isRestAble = true;
            else isRestAble = false;
        }

        // 점검 클릭시 1초 뒤 점검ㄱ
        if (g == GameObject.Find("MaintananceButton")) {
            Invoke("JHW_BaseCamp_Maintanance", 1f);

            // 사운드
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BASECAMP_OPEN_UNIT_DELETE_TITLE);

            //ux
            g.transform.parent.gameObject.transform.DOScale(new Vector3(0f, 0f,1f), 0.7f).SetEase(Ease.InSine);
            g.transform.parent.GetChild(0).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(1).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(2).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(3).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(4).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
        }

        
        // 버튼 원래크기로
        g.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);

        
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

    // 캠프 나가기 버튼
    public void BaseCamp_ExitButton_OnEnter(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void BaseCamp_ExitButton_OnExit(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(true);
    }


    //// 점검 - 스탯정보
    //bool isStatusMouseOver = false;
    //public void Maintanance_Status_OnEnter(GameObject g)
    //{
    //    isStatusMouseOver = true;

    //    GameObject statusInfo = GameObject.Find("StatusInfoLabel");
    //    statusInfo.transform.GetChild(0).transform.gameObject.SetActive(true);

    //    string infoText = "";

    //    switch (g.name)
    //    {
    //        case "BaseCamp_MaxHP": infoText = "최대 HP"; break;
    //        case "BaseCamp_MaxMP": infoText = "최대 MP"; break;
    //        case "BaseCamp_PhyAttk": infoText = "물리공격력"; break;
    //        case "BaseCamp_Defence": infoText = "방어력"; break;
    //        case "BaseCamp_SpellAttk": infoText = "주문공격력"; break;
    //        case "BaseCamp_SpellDefence": infoText = "주문저항력"; break;
    //        case "BaseCamp_CritChance": infoText = "치명타확률"; break;
    //        case "BaseCamp_CritMulti": infoText = "치명타배율"; break;
    //    }

    //    //statusInfo.transform.SetParent(g.transform);
    //    statusInfo.transform.localPosition = g.transform.localPosition;
    //    statusInfo.transform.DOScale(1f, 1f).SetEase(Ease.OutExpo);
    //    statusInfo.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = infoText;
    //    StartCoroutine("Maintanance_Status_OnOver");

        
    //}

    //IEnumerator Maintanance_Status_OnOver()
    //{
    //    while (isStatusMouseOver)
    //    {
    //        GameObject statusInfo = GameObject.Find("StatusInfoLabel");

    //        Vector3 newVector = new Vector3(Input.mousePosition.x - Screen.width / 2 + 60, Input.mousePosition.y - Screen.height / 2 +20);
    //        statusInfo.transform.DOLocalMove(newVector,1f);

    //        yield return new WaitForSeconds(0.001f);
    //    }
    //}

    //public void Maintanance_Status_OnExit(GameObject g)
    //{
    //    isStatusMouseOver = false;

    //    GameObject statusInfo = GameObject.Find("StatusInfoLabel");
    //    statusInfo.transform.DOScale(0f, 1f).SetEase(Ease.OutExpo);
    //    StopCoroutine("Maintanance_Status_OnOver");
    //}

    
}

// 폰트 초기화
partial class HYJ_BaseCamp_Manager
{
    [Header("=== Font ===")]
    [SerializeField] public TMPro.TMP_FontAsset FONT_JSONG;
    [SerializeField] public TMPro.TMP_FontAsset FONT_CHOSUN;

    [SerializeField] List<TextMeshProUGUI> BaseCampTextList_JSONG;
    [SerializeField] List<TextMeshProUGUI> BaseCampTextList_CHOSUN;

    private void InitText()
    {
        for (int i = 0; i < BaseCampTextList_JSONG.Count; i++)
        {
            BaseCampTextList_JSONG[i].font = FONT_JSONG;
        }

        for (int i = 0; i < BaseCampTextList_CHOSUN.Count; i++)
        {
            BaseCampTextList_CHOSUN[i].font = FONT_CHOSUN;
        }
    }
    
}