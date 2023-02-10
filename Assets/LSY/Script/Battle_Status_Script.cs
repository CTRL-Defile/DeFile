using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Battle_Status_Script : MonoBehaviour
{
    public Canvas parentCanvas;

    [SerializeField]
    public List<Sprite> Imagelist;
    [SerializeField]
    Transform unitImage;
    [SerializeField]
    TextMeshProUGUI unitName, skillName, skillDes, maxHp, startMp, phyAttk, defence, spellAttk, spellDefence, critChance, critMulti;

    public void Awake()
    {
        parentCanvas = GameObject.Find("Battle_Canvas").GetComponent<Canvas>();
    }
    private void Start()
    {
        //unitName = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        //skillName = transform.GetChild()

    }

    private void OnEnable()
    {
        Vector2 movePos, realPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
        out movePos);

        realPos = parentCanvas.transform.TransformPoint(movePos);
        realPos.x += 100;
        realPos.y += 200;


        transform.position = realPos;
    }

    public void Set_Status(GameObject obj)
    {
        Character obj_char = obj.GetComponent<Character>();

        int _idx = obj_char.Character_Status_Index;
        unitName.text = obj_char.Character_Status_name;
        maxHp.text = "최대 HP : " + obj_char.Character_Status_maxHp.ToString("F2");
        startMp.text = "시작 MP : " + obj_char.Character_Status_startMp.ToString("F2");
        phyAttk.text = "물리공격력 : " + obj_char.Character_Status_atkPhysics.ToString("F2");
        spellAttk.text = "주문공격력 : " + obj_char.Character_Status_atkSpell.ToString("F2");
        defence.text = "방어력 : " + obj_char.Character_Status_defence.ToString("F2");
        spellDefence.text = "주문저항력 : " + obj_char.Character_Status_spellRegistance.ToString("F2");
        critChance.text = "치명타확률 : " + obj_char.Character_Status_critPer.ToString("F2");
        critMulti.text = "치명타배율 : " + obj_char.Character_Status_critValue.ToString("F2");

        //transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = Imagelist[_id];
        unitImage.GetComponent<Image>().sprite = Imagelist[_idx];

    }


}