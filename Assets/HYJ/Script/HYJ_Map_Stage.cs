using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using UnityEngine.UI;
using DG.Tweening;

public enum HYJ_Map_Stage_TYPE
{
    NONE,
    BASE_CAMP,
    BATTLE_NORMAL,  // 30%
    BATTLE_ELITE,   // 30%
    BATTLE_BOSS,
    SHOP,
    EVENT           // 40%
}

public partial class HYJ_Map_Stage : MonoBehaviour
{
    //////////  Getter & Setter //////////

    //////////  Method          //////////

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    //
    public void HYJ_Init(
        Transform _parent,
        int _x, int _y)
    {
        Transform parent = this.transform.parent;
        //parent.parent = _parent;
        parent.SetParent(_parent);
        parent.localScale = new Vector3(1, 1, 1);

        // >> ���� - ��ġ ��������
        //parent.localPosition = new Vector3(_x * 150, _y * 150, 0); // < ���� �ִ� ��ũ��Ʈ
        parent.localPosition = new Vector3(_x * 150 + Random.Range(-40f,40f), _y * 170 + Random.Range(-30f,30f), 0);
        // >> ���� - end

        HYJ_Stage_Init();
        HYJ_UI_Init();
    }
}

partial class HYJ_Map_Stage
{
    [System.Serializable]
    public class SaveData
    {
        public HYJ_Map_Stage_TYPE Stage_type;
        public string Stage_reward;
        public bool isSelected = false; // ������ ������ ������������ �����ϴ� ����

        //////////  Getter & Setter //////////

        //////////  Method          //////////

        //////////  Default Method  //////////
    }
    [SerializeField] SaveData Stage_data;
    [SerializeField] int Stage_power;
    [SerializeField] List<HYJ_Map_Stage> Stage_roots;
    [SerializeField] List<GameObject> Stage_icons;

    // �ʿ� �������� �������� ��ġ
    public int stage_x;
    public int stage_y;

    //////////  Getter & Setter //////////
    public SaveData HYJ_Stage_saveData { get { return Stage_data;   }   set { Stage_data = value;   }   }

    public HYJ_Map_Stage_TYPE HYJ_Stage_type
    {
        get { return Stage_data.Stage_type; }
        set
        {
            Stage_data.Stage_type = value;

            //
            for(int i = 0; i < Stage_icons.Count; i++)
            {
                Stage_icons[i].SetActive(false);
            }

            Stage_icons[(int)Stage_data.Stage_type].SetActive(true);
        }
    }

    public List<HYJ_Map_Stage> HYJ_Stage_roots { get { return Stage_roots;  }   }

    //////////  Method          //////////
    public void HYJ_Stage_SettingType(int _level)
    {
        if(_level <= 4)
        {
            int rand = Random.Range(0, 100);

            int randCount = 40;
            if(rand < randCount)
            {
                HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_NORMAL;
            }
            else
            {
                randCount += 20;
                if (rand < randCount)
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.BATTLE_ELITE;
                }
                else
                {
                    HYJ_Stage_type = HYJ_Map_Stage_TYPE.EVENT;
                }
            }
        }
    }

    //
    public void HYJ_Stage_AddRoot(HYJ_Map_Stage _root)
    {
        Stage_roots.Add(_root);
    }

    public void HYJ_Stage_Select()
    {
        bool isSelected = Stage_data.isSelected;

        // ������ ������ �������������� ��Ÿ���� ���� false -> true
        Stage_data.isSelected = true;

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.PLAYER___MAP__SET_PLAYER_POS,
            //
            stage_x, stage_y);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__SELECT_RESET);

        Transform parent = this.transform.parent;
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
            HYJ_ScriptBridge_EVENT_TYPE.MAP___CHEAPTER__MOVE_CENTER,
            parent.localPosition.x, parent.localPosition.y, parent.localPosition.z);

        this.transform.DOScale(1.1f, RoadUX_Duration).SetEase(Ease.OutBack);

        // �÷��̾ ������ ��ư�� �����ϴ� �� ������, ( ���� �������� ������� ã������  (stage_y!=0) )
        if (stage_y != 0) {
            selectedRoad = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__SELECTED_ROAD, stage_x, stage_y);
           // StartCoroutine(Road_ON_UX(selectedRoad.transform.childCount));
        }
        // �÷��̾� ��ġ ���� ( ���� �������̸� ����X, �Ŵ������� ������ ���� )
        if (stage_y != 0) HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___CHANGE__PLAYER_POSITION, stage_x, stage_y);
        // ������ ������ ���������
        for (int i = 0; i < Stage_icons.Count; i++) // ������ �������� active ���ִ� �� Ž���մϴ�. ���� ���� �������� active �������� �� active �� Ÿ���� UX ����
            if (Stage_icons[i].activeSelf) {
                GameObject targetObject = Stage_icons[i].transform.GetChild(0).gameObject;
                //targetObject.SetActive(false);

                Sequence IconSequence;
                IconSequence = DOTween.Sequence();
                IconSequence.OnStart(() => targetObject.GetComponent<Image>().color = new Color(1f,1f,1f,1f)); // ó���� �⺻ �����ܿ��� �����ð��� ��������� ����
                IconSequence.Insert(RoadUX_Duration-0.5f, targetObject.GetComponent<Image>().DOFade(0f,0.5f))
                    .Join(targetObject.transform.DOScale(1.5f,0.5f))
                    .OnComplete(()=> targetObject.SetActive(false));
                 
                
                break;
            }

        //
        for (int i = 0; i < Stage_roots.Count; i++)
        {
            Stage_roots[i].HYJ_UI_Active(true);
        }

        // ���� - UX�� ���� ���� �Ʒ� ������ �Ʒ� UX�� �Ű� �ξ����ϴ�.

        //switch(Stage_type)
        //{
        //    case HYJ_Map_Stage_TYPE.BASE_CAMP:
        //        {
        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON,   true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,         false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.SHOP:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON,    true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.EVENT:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON,   true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //    case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
        //    case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
        //    case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
        //        {
        //            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

        //            //
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  true);
        //            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON,     false);
        //        }
        //        break;
        //}
        StartCoroutine(Delay_ChangeStage(!isSelected));

    }

    public void HYJ_Stage_Select_0(bool _isStage)
    {

    }

    //////////  Default Method  //////////
    void HYJ_Stage_Init()
    {
        Stage_roots = new List<HYJ_Map_Stage>();
    }
}

partial class HYJ_Map_Stage
{
    Button UI_btn;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_UI_Active(bool _isActive)
    {
        // ��Ȱ��ȭ�� ��ư�� �������ϰ�, Ȱ��ȭ�� ��ư�� �����ϰ�
        int stageIdx = 0;
        switch (this.Stage_data.Stage_type)
        {
            case HYJ_Map_Stage_TYPE.BASE_CAMP:
                stageIdx = 1;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
                stageIdx = 2;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
                stageIdx = 3;
                break;
            case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
                stageIdx = 4;
                break;
            case HYJ_Map_Stage_TYPE.SHOP:
                stageIdx = 5;
                break;
            case HYJ_Map_Stage_TYPE.EVENT:
                stageIdx = 6;
                break;
        }

        if (!_isActive) // ��ư ��Ȱ��ȭ��
        {
            Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);

            // ������ �̹� ������ ��ư�ϰ�� ���� ���� �� ũ�� ����
            if (Stage_data.isSelected)
            {
                Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
                // ��ư Ȱ��ȭ/��Ȱ��ȭ ���� ����
                UI_btn.interactable = _isActive;
                return;
            }

            Stage_icons[stageIdx].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
            this.transform.DOScale(0.7f, 0.5f);
        }
        else // ��ư Ȱ��ȭ��
        {
            Stage_icons[stageIdx].GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            Stage_icons[stageIdx].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            this.transform.DOScale(1.5f, 0.5f);
        }

        // ��ư Ȱ��ȭ/��Ȱ��ȭ ���� ����
        UI_btn.interactable = _isActive;

    }

    //////////  Default Method  //////////
    void HYJ_UI_Init()
    {
        UI_btn = gameObject.GetComponent<Button>();
        UI_btn.interactable = false;
    }
}

// UX
partial class HYJ_Map_Stage
{
    GameObject selectedRoad = null; // UX ������ �߹ٴ� �ڱ� ������Ʈ
    GameObject pathLightMarker = null; // UX ������ �� �� ��Ŀ ������Ʈ
    int footPrintCnt = 0; // UX ������ �߹ٴ� �ڱ� ����
    int curFootPrintCnt = 0;
    float RoadUX_Duration = 1.5f; // �� UX ���ӽð�

    //IEnumerator Road_ON_UX(int _footPrintCnt)
    //{
    //    for (int i = 0; i < _footPrintCnt * 3; i++)
    //    {
    //        selectedRoad.transform.GetChild(i / 3).GetChild(i % 3).GetChild(0).gameObject.SetActive(true);
    //        //yield return new WaitForSeconds(0.5f);
    //    }
    //    yield return null;
    //}

    private void Road_UX()
    {
        if (curFootPrintCnt < footPrintCnt)
        {
            GameObject targetRoad = selectedRoad.transform.GetChild(curFootPrintCnt / 3).GetChild(curFootPrintCnt % 3).GetChild(0).gameObject;
            targetRoad.SetActive(true);

            Sequence RoadSequence;
            RoadSequence = DOTween.Sequence();
            RoadSequence.OnStart( () => targetRoad.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f) ); // ó���� ���� ���ڱ�
            RoadSequence.Append(targetRoad.GetComponent<Image>().DOFade(1f, 0.2f)) // 0.2�� �� ����� ���ڱ����� ����
                .Join( targetRoad.transform.DOScale(1.3f,0.2f)); // ���ڱ� ũ�� ����
            curFootPrintCnt++;

            Invoke("Road_UX", RoadUX_Duration/footPrintCnt);
        }
    }

    IEnumerator Delay_ChangeStage(bool _isStage)
    {
        if (selectedRoad != null && curFootPrintCnt==0)
        {
            // �ٸ� ���������� �����ϴ°� ���� ���� ȭ�鿡 ���� ȭ�� active
            GameObject blackScreen = null;
            if (_isStage)
            {
                blackScreen = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__BLACKSCREEN);
                blackScreen.SetActive(true);
            }

            // active�Ǿ��ִ� ���ڱ� ����ŭ ���ڱ� ��������� �Ǵ� UX ����
            while (selectedRoad.transform.GetChild(footPrintCnt/3).gameObject.activeSelf == true)
            {
                footPrintCnt+=3;
            }

            curFootPrintCnt = 0;
            Road_UX();

            if (_isStage)
            {
                // �÷��̾� ��ġ ǥ���ϴ� ��Ŀ UX
                GameObject StageLightMarker = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___GET__LOCATIONMARKER);
                StageLightMarker.transform.parent.DOScale(0.8f, RoadUX_Duration).SetEase(Ease.OutQuart);
                if (pathLightMarker == null)
                {
                    pathLightMarker = Instantiate(StageLightMarker);
                    pathLightMarker.transform.SetParent(StageLightMarker.transform.parent);
                    pathLightMarker.transform.localPosition = Vector3.zero;
                    pathLightMarker.transform.localScale = new Vector3(0.3f, 0.3f);
                    pathLightMarker.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.6f);
                    pathLightMarker.transform.DOMove(new Vector3(this.transform.position.x, this.transform.position.y), RoadUX_Duration+0.5f).SetEase(Ease.OutCubic);
                    pathLightMarker.GetComponent<Image>().DOFade(0f, RoadUX_Duration).SetEase(Ease.InExpo);
                }
                Sequence IconSequence;
                IconSequence = DOTween.Sequence();
                IconSequence.OnStart(() => { StageLightMarker.transform.localPosition = Vector3.zero; StageLightMarker.transform.localScale = Vector3.one; })
                    .Join(StageLightMarker.transform.DORotate(new Vector3(0, 0, 360), RoadUX_Duration, RotateMode.FastBeyond360).SetEase(Ease.InCubic))
                    .Join(StageLightMarker.transform.DOScale(0f,RoadUX_Duration-0.5f).SetEase(Ease.InBack));

                //pathLightMarker.transform.DOLocalMove(new Vector3(this.transform.localPosition.x, this.transform.localPosition.y), RoadUX_Duration);//.OnComplete(()=>Destroy(newLightMarker));


                // UX ������
                yield return new WaitForSeconds(RoadUX_Duration+0.5f); // �� UX ���ӽð�+0.5 ��ŭ ��ٸ� �Ŀ� �������� ����

                // ������ ���� ���������� ����� ���ڱ� UX ���� ���� ux���� �ʱ�ȭ
                footPrintCnt = 0;
                curFootPrintCnt = 0;
                curFootPrintCnt = 0;

                // �ٸ� ���������� �����ϴ°� ���� ���� ȭ�鿡 ���� ȭ�� ����
                if (blackScreen != null)
                {
                    blackScreen.SetActive(false);
                }

                // ���� �÷��̾��� �������� ��ġ üũ�ϴ� ������(��Ŀ) �̵� 
                StageLightMarker.transform.parent = this.transform.parent;
                StageLightMarker.transform.SetAsFirstSibling();
                StageLightMarker.transform.localPosition = Vector3.zero;
                StageLightMarker.transform.localScale = Vector3.one;

                switch (Stage_data.Stage_type)
                {
                    case HYJ_Map_Stage_TYPE.BASE_CAMP:
                        {
                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BASE_CAMP___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.SHOP:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SHOP___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.EVENT:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.EVENT___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                    case HYJ_Map_Stage_TYPE.BATTLE_NORMAL:
                    case HYJ_Map_Stage_TYPE.BATTLE_ELITE:
                    case HYJ_Map_Stage_TYPE.BATTLE_BOSS:
                        {
                            //HYJ_Stage_type = HYJ_Map_Stage_TYPE.NONE;

                            //
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON, true);
                            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, false);
                        }
                        break;
                }
            }
        }
    }
}