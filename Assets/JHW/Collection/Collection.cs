using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class Collection : MonoBehaviour
{
    [SerializeField] public GameObject Battle_Unit_Ref; // 전투에서 유닛 구매시 참조하는 것

    [SerializeField] public GameObject Collection_Beast; // 야수 도감 목록
    [SerializeField] public GameObject Collection_Undead; // 언데드 도감 목록
    [SerializeField] public GameObject Collection_Demon; // 마족 도감 목록
    [SerializeField] public GameObject Collection_Spirit; // 정령 도감 목록
    [SerializeField] public GameObject Collection_Goblin; // 고블린 도감 목록
    [SerializeField] public GameObject Collection_Dwarp; // 드워프 도감 목록
    [SerializeField] public GameObject Collection_DarkElf; // 다크엘프 도감 목록
    [SerializeField] public GameObject Collection_HighElf; // 하이엘프 도감 목록
    [SerializeField] public GameObject Collection_Angel; // 천족 도감 목록
    [SerializeField] public GameObject Collection_Human; // 인간 도감 목록

    [SerializeField] public GameObject UnlockCountTexts; // 해금 횟수 텍스트
    [SerializeField] public GameObject CollectionInfo; // 도감 정보창
    [SerializeField] public bool[] collectionID; // 컬렉션 ID 저장

    private int unitCnt = 0; // 유닛 구매 시 유닛 수 변동 감지하는 변수
    private int unlockUnitStarOne = 0; // 1성 유닛 해금 수
    private int unlockUnitStarTwo = 0; // 2성 유닛 해금 수
    private int unlockUnitStarThree = 0; // 3성 유닛 해금 수

    // 유닛 데이터
    private List<Dictionary<string, object>> unitDatas;

    void Start()
    {
        collectionID = new bool[100];
        unitDatas = CSVReader.Read("DataBase/DB_Using_Character_1");
    }

    void Update()
    {
        // W키 누르면 활성화/비활성화
        if (Input.GetKeyDown(KeyCode.W))
        {
            if(this.transform.GetChild(0).gameObject.activeSelf==true) this.transform.GetChild(0).gameObject.SetActive(false);
            else this.transform.GetChild(0).gameObject.SetActive(true);
            isMouseOvered = false;
        }

        // Unit_parent에 유닛 올라가면 도감 체크 (유닛 구매시 도감 체크)
        if(unitCnt != Battle_Unit_Ref.transform.childCount)
        {
            unitCnt = Battle_Unit_Ref.transform.childCount;
            Character CollectionCharacter;
            for (int i = 0; i < unitCnt; i++)
            {
                CollectionCharacter = Battle_Unit_Ref.transform.GetChild(i).gameObject.GetComponent<Character>();

                Debug.Log(CollectionCharacter.Character_Status_ID);
                // 이미 도감 활성화상태면 도감 갱신X
                if (collectionID[CollectionCharacter.Character_Status_ID] == true) continue;

                // 도감 갱신
                collectionID[CollectionCharacter.Character_Status_ID] = true;

                GameObject target = getCollectionUnitByID(CollectionCharacter.Character_Status_ID);

                // 이미지 오픈
                target.transform.GetChild(0).gameObject.SetActive(true);
                // 별 이미지 (1/2/3성)
                target.transform.GetChild(1).GetChild((CollectionCharacter.Character_Status_ID-1) % 3).GetChild(0).gameObject.SetActive(true);
                // 해금 수 증가
                switch((CollectionCharacter.Character_Status_ID - 1) % 3){
                    case 0:
                        unlockUnitStarOne++;
                        UnlockCountTexts.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "1성 해금 : " +unlockUnitStarOne.ToString()+"개";
                        break;
                    case 1:
                        unlockUnitStarTwo++;
                        UnlockCountTexts.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2성 해금 : " + unlockUnitStarTwo.ToString() + "개";
                        break;
                    case 2:
                        unlockUnitStarThree++;
                        UnlockCountTexts.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "3성 해금 : " + unlockUnitStarThree.ToString() + "개";
                        break;
                }
            }
        }

        if (isMouseOvered) // 유닛 도감에 마우스 올려져 있으면
        {
            GameObject target = getCollectionUnitByID(mouseOverUnitID);
            if (target == null) return; // 만약 찾는 도감 내 유닛이 null 이면 실행 X

            // 유닛 info창 위치조정 - x,y는 마우스 위치에, z는 타겟위치(도감 내 유닛)에
            Vector3 newVector = new Vector3(Input.mousePosition.x + CollectionInfo.GetComponent<RectTransform>().rect.width / 2, Input.mousePosition.y - CollectionInfo.GetComponent<RectTransform>().rect.height / 2, target.transform.position.z);
            CollectionInfo.transform.position = newVector;
        }
        else CollectionInfo.transform.position = new Vector3(-999f, -999f); // 마우스 올려져있지 않으면 다른데로 보낸다

    }
}

#region METHOD
partial class Collection
{
    private int mouseOverUnitID; // 마우스 올린 유닛 ID
    private bool isMouseOvered; // 마우스 올렸는지 안올렸는지 체크

    // 도감 유닛 마우스올리면 정보창 변경
    [SerializeField]
    public void CollectionUnit_OnMouseEnter(int unitID)
    {
        // 매개변수로 들어온 ID값은 각 유닛의 1성 ID임
        mouseOverUnitID = unitID;
        isMouseOvered = true;

        if (unitID == 0)
        {
            CollectionInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "???";
            CollectionInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "종족";
            CollectionInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "특성";
            CollectionInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 ";
            return;
        }

        if (collectionID[unitID]) // 만약 해금한 유닛이면
        {
            CollectionInfo.transform.GetChild(0).GetChild(unitID/3).gameObject.SetActive(true); // 유닛 초상화 on
            CollectionInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (string)unitDatas[unitID / 3]["NAME"]; // 이름
            CollectionInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (string)unitDatas[unitID / 3]["RACE"]; // 종족
            CollectionInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = (string)unitDatas[unitID / 3]["JOB"]; // 특성
            CollectionInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "대충 설명";
        }

        else // 해금하지 않은 유닛이면
        {
            CollectionInfo.transform.GetChild(0).GetChild(unitID / 3).gameObject.SetActive(false); // 유닛 초상화 off
            CollectionInfo.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "???";
            CollectionInfo.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "종족";
            CollectionInfo.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "특성";
            CollectionInfo.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 스토리설명이나 스킬설명 ";
        }
    }

    // 도감 유닛 마우스 벗어나면 정보창 변경
    [SerializeField]
    public void CollectionUnit_OnMouseExit()
    {
        isMouseOvered = false;
        CollectionInfo.transform.GetChild(0).GetChild(mouseOverUnitID / 3).gameObject.SetActive(false); // 유닛 초상화 off
    }

    // 도감 유닛 검사
    private GameObject getCollectionUnitByID(int unitID)
    {
        GameObject target = null;
        switch (unitID)
        {
            case 1:
            case 2:
            case 3:
                // 야수 005
                target = Collection_Beast.transform.GetChild(1).GetChild(0).gameObject;
                break;
            case 4:
            case 5:
            case 6:
                // 고블린 002
                target = Collection_Goblin.transform.GetChild(1).GetChild(0).gameObject;
                break;
            case 7:
            case 8:
            case 9:
                // 정령 002
                target = Collection_Spirit.transform.GetChild(1).GetChild(0).gameObject;
                break;
            case 10:
            case 11:
            case 12:
                // 고블린 004
                target = Collection_Goblin.transform.GetChild(1).GetChild(1).gameObject;
                break;
            case 13:
            case 14:
            case 15:
                // 고블린 003
                target = Collection_Goblin.transform.GetChild(1).GetChild(2).gameObject;
                break;
            case 16:
            case 17:
            case 18:
                // 야수 003
                target = Collection_Beast.transform.GetChild(1).GetChild(1).gameObject;
                break;
        }
        return target;
    }
}
#endregion