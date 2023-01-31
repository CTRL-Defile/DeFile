using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Basecamp_ux : MonoBehaviour
{
    GameObject title;
    GameObject Btn1;
    GameObject Btn2;
    GameObject Btn3;
    GameObject Btn4;

    bool isActiveAble = false;

    // Start is called before the first frame update
    void Start()
    {
        title = this.transform.GetChild(0).gameObject;
        Btn1 = this.transform.GetChild(1).gameObject;
        Btn2 = this.transform.GetChild(2).gameObject;
        Btn3 = this.transform.GetChild(3).gameObject;
        Btn4 = this.transform.GetChild(4).gameObject;

        // 초기에 사이즈 0
        title.transform.DOScale(0f, 0f);
        Btn1.transform.DOScale(0f, 0f);
        Btn2.transform.DOScale(0f, 0f);
        Btn3.transform.DOScale(0f, 0f);
        Btn4.transform.DOScale(0f, 0f);

        // ux 시작
        var sequence = DOTween.Sequence().SetAutoKill(false); // 이거 setAutoKill 없으면 다시실행못함
        sequence.Insert(1f, title.transform.DOScale(1f, 0.5f)).SetEase(Ease.OutCubic);
        sequence.Insert(2f, Btn1.transform.DOScale(1f, 0.5f));
        sequence.Insert(2.3f, Btn2.transform.DOScale(1f, 0.5f));
        sequence.Insert(2.6f, Btn3.transform.DOScale(1f, 0.5f));
        sequence.Insert(2.9f, Btn4.transform.DOScale(1f, 0.5f));

        // ux 끝나면 버튼 누를 수 있게
        Invoke("changeBtnAble", 3f);
    }

    void changeBtnAble()
    {
        isActiveAble = !isActiveAble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
