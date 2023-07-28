using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_Lobby_Manager : MonoBehaviour
{
    [SerializeField] GameObject Basic_gameObj;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public void HYJ_Basic_BtnStart()
    {
        HYJ_Basic_ActiveOn(false);

        Basic_gameObj.SetActive(true);
    }

    //
    object HYJ_Basic_ActiveOn_script(params object[] _args)
    {
        bool isActive = (bool)_args[0];
        HYJ_Basic_ActiveOn(isActive);

        //
        return true;
    }

    void HYJ_Basic_ActiveOn(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);
    }


    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.LOBBY___BASIC__ACTIVE_ON, HYJ_Basic_ActiveOn_script);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
