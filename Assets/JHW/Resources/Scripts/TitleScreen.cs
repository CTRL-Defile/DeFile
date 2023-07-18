using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

partial class TitleScreen : MonoBehaviour
{
    // Ÿ��Ʋȭ��

    void Start()
    {
        // Ÿ��Ʋȭ�� Ȱ��ȭ
        this.transform.GetChild(0).gameObject.SetActive(true);
        // Ÿ��Ʋȭ�� ���� ȭ�� ���̵� �� �����
        this.transform.GetChild(0).GetChild(1).GetComponent<Image>().DOFade(0f, 1f).SetDelay(1f);
        // � ȭ�� 2�ʵ� ��Ȱ��ȭ
        Invoke("blackScreenOff", 2f);
        // ��Ʈ
        InitText();
    }


    void blackScreenOff()
    {
        this.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
    }

    // ���� ���� ��ư
    public void GameStartButton()
    {
        // �κ�ȭ�� ���̵�
        this.transform.GetChild(0).GetChild(0).GetComponent<Image>().DOFade(0f, 2f);

        // De'file �ؽ�Ʈ ���̵�
        this.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().DOFade(0f, 2f);
        // ��ư disable
        this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false); // �÷��̹�ư
        this.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false); // ���Ӽ����ư

        

        // ���̽�ķ�� ���� UX,
        GameObject.Find("BaseCampBackgroundCanvas").GetComponent<Basecamp_background>().StartBaseCamp_BackGround_UX();
        GameObject.Find("BasecampUI").GetComponent<Basecamp_ux>().StartBaseCamp_Button_UX();
        //2�� �� Ÿ��Ʋȭ�� OFF
        Invoke("Basecamp_disable", 2f);

        // ����
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__BGM_NAME, JHW_SoundManager.BGM_list.temp_BGM);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.UI_CLICK1);
    }

    void Basecamp_disable()
    {
        this.gameObject.SetActive(false);
    }
}

partial class TitleScreen
{
    // ��Ʈ
    [Header("=== Font ===")]
    [SerializeField] public TMPro.TMP_FontAsset FONT_JSONG;
    [SerializeField] public TMPro.TMP_FontAsset FONT_CHOSUN;

    [SerializeField] List<TextMeshProUGUI> EventTextList_JSONG;
    [SerializeField] List<TextMeshProUGUI> EventTextList_CHOSUN;

    private void InitText()
    {
        for (int i = 0; i < EventTextList_JSONG.Count; i++)
        {
            EventTextList_JSONG[i].font = FONT_JSONG;
        }

        for (int i = 0; i < EventTextList_CHOSUN.Count; i++)
        {
            EventTextList_CHOSUN[i].font = FONT_CHOSUN;
        }
    }

}