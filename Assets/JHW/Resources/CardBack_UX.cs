using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CardBack_UX : MonoBehaviour
{
    // 뒷면을 누른 횟수. 이 변수는 점검 클릭 시 또는 reroll 버튼 클릭 시 0으로 초기화합니다
    public static int checkBackCardCnt;

    // 카드 뒷면 클릭시
    public void CardBack_Click()
    {
        this.transform.DOLocalRotate(new Vector3(0, 90, 0), 0.3f, RotateMode.Fast);
        this.transform.parent.GetChild(0).DOLocalRotate(new Vector3(0, 0, 0), 0.3f, RotateMode.Fast).SetDelay(0.3f);
        // 뒷면 클릭 3번일 때 reroll 버튼 활성화
        if (++checkBackCardCnt >= 3) Check_reroll_able();
    }

    // 카드 전부 뒷면 누르면 reroll 버튼 활성화
    public void Check_reroll_able()
    {
        // reroll 버튼 활성화
        GameObject.Find("RerollButton").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.Find("RerollButton").transform.GetChild(0).GetComponent<Image>().DOFade(0f, 0f);
        GameObject.Find("RerollButton").transform.GetChild(0).GetComponent<Image>().DOFade(1f, 0.5f).SetDelay(0.5f);
        GameObject.Find("RerollButton").transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().DOFade(0f, 0f);
        GameObject.Find("RerollButton").transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().DOFade(1f, 0.5f).SetDelay(0.5f);
    }
}
