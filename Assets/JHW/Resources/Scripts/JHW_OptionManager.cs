using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHW_OptionManager : MonoBehaviour
{
    public void optionOnOff(bool arg)
    {
        GameObject optionPopup = GameObject.Find("OptionPopup").transform.GetChild(0).gameObject;
        optionPopup.SetActive(arg);


        // 사운드
        if(optionPopup.activeSelf==true) // 옵션 오픈 사운드
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.OPTION_OPEN);
        else // 옵션 클로즈 사운드
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.SOUNDMANAGER___PLAY__SFX_NAME, JHW_SoundManager.SFX_list.OPTION_CLOSE);
    }
}
