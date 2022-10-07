using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HYJ_BaseCamp_Manager : MonoBehaviour
{
    [SerializeField] int Basic_initialize;

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        //
        return null;
    }

    public void HYJ_SetActive(bool _isActive)
    {
        this.gameObject.SetActive(_isActive);

        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        Basic_initialize = 0;
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

                    this.HYJ_SetActive(true);

                    Basic_initialize = -1;
                }
                break;
        }
    }
}
