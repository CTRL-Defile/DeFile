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

    [SerializeField] private int actionCntMax = 10; // �ִ��ൿ����
    [SerializeField] private int actionCnt = 0; // ���� �ൿ����

    [SerializeField] private Text gaugeText; // �ൿ���� ���� �ؽ�Ʈ
    [SerializeField] private Transform gaugeImgs; // ������ ������ �̹�����

    [SerializeField] private int restStack = 0; // �޽� �� ���� �޽Ŀ� �ʿ��� �ൿ���� �����ϱ� ���� ����
    [SerializeField] private int removeStack = 0; // ���ֻ��� �� ���� �޽Ŀ� �ʿ��� �ൿ���� �����ϱ� ���� ����



    int randomSelectUnitNumber1; // ���õ� ���� �ε��� ��ȣ1
    int randomSelectUnitNumber2; // ���õ� ���� �ε��� ��ȣ2
    int randomSelectUnitNumber3; // ���õ� ���� �ε��� ��ȣ3

    GameObject unitList1; // ���ָ���Ʈ 1 (�̹���)
    GameObject unitList2; // ���ָ���Ʈ 2 (�̹���)
    GameObject unitList3; // ���ָ���Ʈ 3 (�̹���)

    Dictionary<string, object> selectedUnit1; // �����ϰ� ���õ� ���� ������1
    Dictionary<string, object> selectedUnit2; // �����ϰ� ���õ� ���� ������1
    Dictionary<string, object> selectedUnit3; // �����ϰ� ���õ� ���� ������1

    private GameObject info; // ����â

    // �޽Ŀ��� ���ҽ�ų �ൿ����
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
			// ����
			HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BOOKSHELF_WHIP);
            //BaseCamp_ExitButton_OnClick(); // ���̽�ķ�� ������ ux
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
                    gaugeText = this.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>(); // ������ �ؽ�Ʈ
                    gaugeImgs = this.transform.GetChild(0).GetChild(1).GetChild(1); // ������ �̹��� ������
                    info = this.transform.GetChild(0).GetChild(6).gameObject; // ����â

                    this.actionCnt = this.actionCntMax; // �ൿ������ �ִ� �ൿ�����μ���

                    ChangeGaugeUI(); // UI ����

                    

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

// �޼���
partial class HYJ_BaseCamp_Manager {
    // ����
    public void JHW_BaseCamp_Maintanance()
    {

        
        // reroll ��ư ��Ȱ��ȭ
        this.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(4).GetChild(0).gameObject.SetActive(false);

        // ���̽�ķ�� ���� �����ϴ� �̹��� Ȱ��ȭ
        this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject.SetActive(true);

        // �ൿ����Ʈ �̹���
        this.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);

        // ���̽�ķ�� ������ �̹��� ��Ȱ��ȭ
        this.transform.GetChild(0).transform.GetChild(2).transform.gameObject.SetActive(false);

        int _lv = 1;
        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/DB_Using_Character_" + _lv.ToString());

        // ī�� �޸� ������ Ƚ�� 0���� �ʱ�ȭ
        CardBack_UX.checkBackCardCnt = 0;

        // ���� ���� �ε��� �ߺ����� �̱�
        bool[] dataFlag = new bool[unitDatas.Count];
        for (int i = 0; i < unitDatas.Count; i++) dataFlag[i] = false;
        randomSelectUnitNumber1 = Random.Range(0, unitDatas.Count);
        dataFlag[randomSelectUnitNumber1] = true;
        do { randomSelectUnitNumber2 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber2] == true);
        dataFlag[randomSelectUnitNumber2] = true;
        do { randomSelectUnitNumber3 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber3] == true);

        // �����ϰ� ���õ� ����
        selectedUnit1 = unitDatas[randomSelectUnitNumber1];
        selectedUnit2 = unitDatas[randomSelectUnitNumber2];
        selectedUnit3 = unitDatas[randomSelectUnitNumber3];

        GameObject unitList = this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject;
        unitList1 = unitList.transform.GetChild(0).gameObject;
        unitList2 = unitList.transform.GetChild(1).gameObject;
        unitList3 = unitList.transform.GetChild(2).gameObject;

        // ux
        // ī��1
        unitList1.transform.DOScale(1f, .5f);
        unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // ī��2
        unitList2.transform.DOScale(1f, .5f);
        unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // ī��3
        unitList3.transform.DOScale(1f, .5f);
        unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                unitList1.transform.GetChild(2).DOLocalRotate(new Vector3(0, -360, 0), 1, RotateMode.FastBeyond360); // ī��޸�
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

        // ����ī�忡 �������� �ؽ�Ʈ(���� ��ġ��) ����
        ChangeCardUI();



        // ī�� �ո� �Ⱥ��̰�
        unitList1.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        unitList2.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        unitList3.transform.GetChild(0).transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        // ī�� �޸� Ȱ��ȭ
        unitList1.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        unitList2.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        unitList3.transform.GetChild(2).transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    // �޽�
    public void JHW_BaseCamp_Rest()
    {
        if (isClicked == true) return; // �̹� ���� ���¸� ����X

        if (this.actionCnt < rest_minusActionCnt +restStack) {
            ActionCntAlert();
            return; 
        }  // �ൿ���� ������ ux �� ����

        if (isRestAble == false) return; // ���������� ����X

        // �޽� ��� �� ���� �޽Ŀ� �ʿ��� �ൿ���� ���� �� �ൿ���� ����
        rest_minusActionCnt += this.restStack++;
        actionCnt -= rest_minusActionCnt; 
        
        // 10��ŭ ü��ȸ��
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__HP_INCREASE, 10);

        ChangeGaugeUI(); // Gauge UI ����

        // ����
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.RECOVER);

        //
        List<string> playerStageDatas = (List<string>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__GET_STAGE_DATAS);
        playerStageDatas.Clear();
        playerStageDatas.Add(actionCnt.ToString());
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___FILE__SAVE);
    }

    // ������ư on/off
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
            // ī��1
            unitList1.transform.DOScale(1.05f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // ī��2
            unitList2.transform.DOScale(0.9f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // ī��3
            unitList3.transform.DOScale(0.9f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "2")
        {
            // ī��1
            unitList1.transform.DOScale(0.9f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // ī��2
            unitList2.transform.DOScale(1.05f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
            // ī��3
            unitList3.transform.DOScale(0.9f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
        }
        if (gameObject.name == "3")
        {
            // ī��1
            unitList1.transform.DOScale(0.9f, .5f);
            unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // ī��2
            unitList2.transform.DOScale(0.9f, .5f);
            unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(0.6f, .5f);
            // ī��3
            unitList3.transform.DOScale(1.05f, .5f);
            unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        }
    }

    // Gauge UI ����
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
    // ����
    public void Reroll()
    {
        // reroll�ϰ��ִ������� ����
        isRerolling = true;
        // ����
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.REROLL);

        // ī�� �޸� ������ Ƚ�� �ʱ�ȭ
        CardBack_UX.checkBackCardCnt = 0;

        // reroll ��ư ��Ȱ��ȭ
        this.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(4).GetChild(0).gameObject.SetActive(false);

        int minusActionCnt = 1; // ���ҽ�ų �ൿ����
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // �ൿ���� �����ϸ� ����X 

        actionCnt -= minusActionCnt; // �޽� ��� �� ���� �޽Ŀ� �ʿ��� �ൿ���� ���� �� �ൿ���� ����

        ChangeGaugeUI(); // ������ UI ����

        int _lv = 1;
        List<Dictionary<string, object>> unitDatas = CSVReader.Read("DataBase/DB_Using_Character_" + _lv.ToString());

        // ���� ���� �ε��� �ߺ����� �̱�
        bool[] dataFlag = new bool[unitDatas.Count];
        for (int i = 0; i < unitDatas.Count; i++) dataFlag[i] = false;
        randomSelectUnitNumber1 = Random.Range(0, unitDatas.Count);
        dataFlag[randomSelectUnitNumber1] = true;
        do { randomSelectUnitNumber2 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber2] == true);
        dataFlag[randomSelectUnitNumber2] = true;
        do { randomSelectUnitNumber3 = Random.Range(0, unitDatas.Count); } while (dataFlag[randomSelectUnitNumber3] == true);
        dataFlag[randomSelectUnitNumber3] = true;

        // �����ϰ� ���õ� ����
        selectedUnit1 = unitDatas[randomSelectUnitNumber1];
        selectedUnit2 = unitDatas[randomSelectUnitNumber2];
        selectedUnit3 = unitDatas[randomSelectUnitNumber3];

        // ī�� ���� ��ġ �� �̸� ����
        Invoke("ChangeCardUI",0.5f);

        //������ư �Ⱥ��̰�
        unitList1.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList2.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList3.transform.GetChild(1).transform.gameObject.SetActive(false);


        // ux
        Sequence cardSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                unitList1.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast); //ī�� �ո�
                unitList1.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f); //ī�� �޸�
            })
            .Insert(0.2f, unitList2.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast))
            .Join(unitList2.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f))
            .Insert(0.4f, unitList3.transform.GetChild(0).DOLocalRotate(new Vector3(0, -90, 0), 0.3f, RotateMode.Fast))
            .Join(unitList3.transform.GetChild(2).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f)
            ).OnComplete(()=> { isRerolling = false; });

        // ī��1
        unitList1.transform.DOScale(1f, .5f);
        unitList1.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // ī��2
        unitList2.transform.DOScale(1f, .5f);
        unitList2.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
        // ī��3
        unitList3.transform.DOScale(1f, .5f);
        unitList3.transform.GetChild(0).GetComponent<RawImage>().DOFade(1f, .5f);
    }

    // ī�� ���� ��ġ �� �̸� ����
    public void ChangeCardUI()
    {


        //ī��1
        unitList1.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit1["ID"]);
        unitList1.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit1["NAME_KOR"].ToString(); // è�Ǿ� �̸�
        unitList1.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� HP\n" + selectedUnit1["MAX_HP"].ToString(); // �ִ�HP
        unitList1.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� MP\n" + selectedUnit1["MAX_MP"].ToString(); // �ִ�MP
        unitList1.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit1["ATK_PHYSICS"].ToString(); // �������ݷ�
        unitList1.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "����\n" + selectedUnit1["DEFENCE"].ToString(); // ����
        unitList1.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit1["ATK_SPELL"].ToString(); // �������ݷ�
        unitList1.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ֹ����׷�\n" + selectedUnit1["SPELL_REGISTANCE"].ToString(); // �������׷�
        unitList1.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��ŸȮ��\n" + selectedUnit1["CRIT_PERCENT"].ToString(); // ũ��Ƽ��Ȯ��
        unitList1.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��Ÿ����\n" + selectedUnit1["CRIT_VALUE"].ToString(); // ũ��Ƽ�ù���

        //ī��2
        unitList2.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit2["ID"]);
        unitList2.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit2["NAME_KOR"].ToString(); // è�Ǿ� �̸�
        unitList2.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� HP\n" + selectedUnit2["MAX_HP"].ToString(); // �ִ�HP
        unitList2.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� MP\n" + selectedUnit2["MAX_MP"].ToString(); // �ִ�MP
        unitList2.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit2["ATK_PHYSICS"].ToString(); // �������ݷ�
        unitList2.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "����\n" + selectedUnit2["DEFENCE"].ToString(); // ����
        unitList2.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit2["ATK_SPELL"].ToString(); // �������ݷ�
        unitList2.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ֹ����׷�\n" + selectedUnit2["SPELL_REGISTANCE"].ToString(); // �������׷�
        unitList2.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��ŸȮ��\n" + selectedUnit2["CRIT_PERCENT"].ToString(); // ũ��Ƽ��Ȯ��
        unitList2.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��Ÿ����\n" + selectedUnit2["CRIT_VALUE"].ToString(); // ũ��Ƽ�ù���

        //ī��3
        unitList3.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<UnitCardImage>().ChangeImage((int)selectedUnit3["ID"]);
        unitList3.transform.GetChild(0).GetChild(0).transform.GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = selectedUnit3["NAME_KOR"].ToString(); // è�Ǿ� �̸�
        unitList3.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� HP\n" + selectedUnit3["MAX_HP"].ToString(); // �ִ�HP
        unitList3.transform.GetChild(0).GetChild(2).GetChild(2).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ִ� MP\n" + selectedUnit3["MAX_MP"].ToString(); // �ִ�MP
        unitList3.transform.GetChild(0).GetChild(2).GetChild(3).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit3["ATK_PHYSICS"].ToString(); // �������ݷ�
        unitList3.transform.GetChild(0).GetChild(2).GetChild(4).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "����\n" + selectedUnit3["DEFENCE"].ToString(); // ����
        unitList3.transform.GetChild(0).GetChild(2).GetChild(5).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�������ݷ�\n" + selectedUnit3["ATK_SPELL"].ToString(); // �������ݷ�
        unitList3.transform.GetChild(0).GetChild(2).GetChild(6).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "�ֹ����׷�\n" + selectedUnit3["SPELL_REGISTANCE"].ToString(); // �������׷�
        unitList3.transform.GetChild(0).GetChild(2).GetChild(7).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��ŸȮ��\n" + selectedUnit3["CRIT_PERCENT"].ToString(); // ũ��Ƽ��Ȯ��
        unitList3.transform.GetChild(0).GetChild(2).GetChild(8).GetChild(1).transform.gameObject.GetComponent<TMP_Text>().text = "ġ��Ÿ����\n" + selectedUnit3["CRIT_VALUE"].ToString(); // ũ��Ƽ�ù���
    }

    public void DeleteUnit(int number)
    {
        int deleteLineNumber=0; //������ UnitDataBase ���� �ε���
        if (number == 1) deleteLineNumber = randomSelectUnitNumber1;
        if (number == 2) deleteLineNumber = randomSelectUnitNumber2;
        if (number == 3) deleteLineNumber = randomSelectUnitNumber3;

        Debug.Log(number + "�� ī�� id : " + deleteLineNumber + " ����!");
        Player_DB.Instance.idx_delete(deleteLineNumber);

#if false
        string[] lines;

        for (int i = 1; i <= 3; i++)
        {
            // ���� ������ �о����
            lines = File.ReadAllLines("Assets/Resources/DataBase/DB_Using_Character_"+i.ToString()+".csv");
            // ���� ������ ����
            System.IO.File.Delete("Assets/Resources/DataBase/Player_Unit_DataBase_" + i.ToString() + ".csv");
            StreamWriter outStream = System.IO.File.CreateText("Assets/Resources/DataBase/Player_Unit_DataBase_"+i.ToString()+".csv");
            // �����Ǵ� ���� ������ ���� ������ �������� ��ü
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
        //Debug.Log("����.");
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__UPDATE_PLAYER_UNIT_DATABASE);
#endif

    }

    public void DeleteCard(int deleteNumber)
    {
        if (isRerolling) return; // �������̸� ����X

        int minusActionCnt = 1+(this.removeStack++); // ���ҽ�ų �ൿ����
        if (this.actionCnt < minusActionCnt) { ActionCntAlert(); return; } // �ൿ���� �����ϸ� ����X 

        // ���� ����
        DeleteUnit(deleteNumber);

        //ī�� ������Ƚ�� 0���� �ʱ�ȭ
        CardBack_UX.checkBackCardCnt = 0;

        actionCnt -= minusActionCnt; // �ൿ���� ����

        // ī�� �ϴ� �ڷ� ������
        unitList1.transform.DOLocalMoveY(-1000f, 1f);
        unitList2.transform.DOLocalMoveY(-1000f, 1f);
        unitList3.transform.DOLocalMoveY(-1000f, 1f);

        this.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true); // ���̽�ķ�� UI Ȱ��ȭ
        this.transform.GetChild(0).GetChild(3).GetChild(0).transform.gameObject.SetActive(false); // ���� �� UI ��Ȱ��ȭ

        //������ư �Ⱥ��̰�
        unitList1.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList2.transform.GetChild(1).transform.gameObject.SetActive(false);
        unitList3.transform.GetChild(1).transform.gameObject.SetActive(false);

        // ������ �̹��� on/off
        ChangeGaugeUI();

        //����
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

        // ���̽�ķ�� ������ �̹��� Ȱ��ȭ
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
        // ���������¸� ����x
        if (isClicked) return;
        // ������ �� ������ ����x
        if (g.name=="RestButton" && isRestAble == false)
        {
            return;
        }

        // ������
        GameObject gauge = this.transform.GetChild(0).GetChild(1).GetChild(1).gameObject; //������ �̹���

        // ���콺 �÷������¸� ũ��/������
        g.transform.DOScale(new Vector3(1.1f, 1.1f,1f), .2f);
        //g.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0.8f), 0.1f);
        isMouseEntered = true;

        // ����
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BUTTON_MOUSEOVER);

        // ����â
        info.transform.GetComponent<Image>().DOFade(1, 0.2f); // �̹��� ���̵�
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(1, 0.2f); // �ؽ�Ʈ ���̵�
        info.transform.SetParent(g.transform.parent);
        Vector3 newPos = g.transform.position;
        newPos.y += 1f;
        info.transform.transform.position = newPos;

        // �޽��� ��� ����â
        if (g.name=="RestButton" && actionCnt >= rest_minusActionCnt + restStack)
        {
            if (actionCnt > 0)
            {
                info.transform.GetChild(0).transform.GetComponent<Text>().text = "�ൿ���� -" + (rest_minusActionCnt + restStack) + "\nü�� +10";
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
        // ������ ��� ����â
        if (g == GameObject.Find("MaintananceButton"))
        {
            info.transform.GetChild(0).transform.GetComponent<Text>().text = "���� ���� 3�� ��\n1���� �����ϴ�.";
        }

        //// ���콺 �÷��������� �����ϴ°�
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

        // ����â ���̵�
        info.transform.GetComponent<Image>().DOFade(0, 0.2f);
        info.transform.GetChild(0).gameObject.transform.GetComponent<Text>().DOFade(0, 0.2f);
        //g.GetComponent<Image>().DOColor(new Color(1, 1, 1, 1f), 0.2f).SetDelay(0.001f);

        // �ൿ���� ux
        // ������ �� ������ ����x
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
        // Ŭ���ѻ��¸� ����
        if (isClicked) return;
        isClicked = true;
        Invoke("clickFlagChange", 1f);

        // ���� �� �� ������ ����
        if (g.name=="MaintananceButton" && isMaintananceAble == false) return;

        // �޽� Ŭ����
        if (g.name == "RestButton")
        {
            // ux ������Ʈ
            GameObject uxObject = Instantiate(g, g.transform.position, g.transform.rotation);
            uxObject.GetComponent<Button>().enabled = false;
            uxObject.GetComponent<EventTrigger>().enabled = false;
            uxObject.transform.SetParent(g.transform);
            uxObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0f);
            uxObject.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 1f);
            uxObject.GetComponent<Image>().DOFade(0f, 1f);
            uxObject.transform.GetChild(0).gameObject.GetComponent<Text>().DOFade(0f, 1f);
            Destroy(uxObject, 1f);

            // �޽� - �ൿ���� �˻�
            if (actionCnt >= rest_minusActionCnt + restStack) isRestAble = true;
            else isRestAble = false;
        }

        // ���� Ŭ���� 1�� �� ���ˤ�
        if (g == GameObject.Find("MaintananceButton")) {
            Invoke("JHW_BaseCamp_Maintanance", 1f);

            // ����
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.BASECAMP_OPEN_UNIT_DELETE_TITLE);

            //ux
            g.transform.parent.gameObject.transform.DOScale(new Vector3(0f, 0f,1f), 0.7f).SetEase(Ease.InSine);
            g.transform.parent.GetChild(0).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(1).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(2).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(3).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
            g.transform.parent.GetChild(4).gameObject.transform.GetComponent<Image>().DOFade(0f, 0.8f);
        }

        
        // ��ư ����ũ���
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

        // �ʱ� alert ����
        alert.transform.localPosition = new Vector3(-1000, -600);
        alert.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0);
        alert.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(0.5f, 0f);

        // ux ����
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
    // �ൿ���� �˸� Ŭ����
    public void alertButtonClicked(GameObject g)
    {
        g.transform.GetChild(0).gameObject.SetActive(false);
    }

    // ķ�� ������ ��ư
    public void BaseCamp_ExitButton_OnEnter(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(false);
    }
    public void BaseCamp_ExitButton_OnExit(GameObject g)
    {
        g.transform.GetChild(1).gameObject.SetActive(true);
    }


    //// ���� - ��������
    //bool isStatusMouseOver = false;
    //public void Maintanance_Status_OnEnter(GameObject g)
    //{
    //    isStatusMouseOver = true;

    //    GameObject statusInfo = GameObject.Find("StatusInfoLabel");
    //    statusInfo.transform.GetChild(0).transform.gameObject.SetActive(true);

    //    string infoText = "";

    //    switch (g.name)
    //    {
    //        case "BaseCamp_MaxHP": infoText = "�ִ� HP"; break;
    //        case "BaseCamp_MaxMP": infoText = "�ִ� MP"; break;
    //        case "BaseCamp_PhyAttk": infoText = "�������ݷ�"; break;
    //        case "BaseCamp_Defence": infoText = "����"; break;
    //        case "BaseCamp_SpellAttk": infoText = "�ֹ����ݷ�"; break;
    //        case "BaseCamp_SpellDefence": infoText = "�ֹ����׷�"; break;
    //        case "BaseCamp_CritChance": infoText = "ġ��ŸȮ��"; break;
    //        case "BaseCamp_CritMulti": infoText = "ġ��Ÿ����"; break;
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

// ��Ʈ �ʱ�ȭ
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