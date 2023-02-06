using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    // 타이틀화면

    void Start()
    {
        // 타이틀화면 활성화
        this.transform.GetChild(0).gameObject.SetActive(true);
        // 타이틀화면 검은 화면 페이드 후 사라짐
        this.transform.GetChild(0).GetChild(1).GetComponent<Image>().DOFade(0f, 1f).SetDelay(1f);
        // 까만 화면 2초뒤 비활성화
        Invoke("blackScreenOff", 2f);
    }


    void blackScreenOff()
    {
        this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    // 게임 시작 버튼
    public void GameStartButton()
    {
        // 로비화면 페이드
        this.transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(0f, 2f);

        // 버튼 disable
        this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false); // 플레이버튼
        this.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false); // 게임설명버튼

        // 베이스캠프 입장 UX,
        GameObject.Find("BaseCampBackgroundCanvas").GetComponent<Basecamp_background>().StartBaseCamp_BackGround_UX();
        GameObject.Find("BasecampUI").GetComponent<Basecamp_ux>().StartBaseCamp_Button_UX();
        //2초 뒤 타이틀화면 OFF
        Invoke("Basecamp_disable", 2f);
    }

    void Basecamp_disable()
    {
        this.gameObject.SetActive(false);
    }
}
