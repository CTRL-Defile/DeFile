using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Master : MonoBehaviour
{
    [SerializeField] int Basic_initialize;

    //////////  Getter & Setter //////////

    //////////  Method          //////////

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
                    HYJ_UI_Init();
                    Basic_initialize = -1;
                }
                break;
        }
    }
}

#region UI

partial class HYJ_Master : MonoBehaviour
{
    [Header("==================================================")]
    [Header("UI")]
    [SerializeField] List<Camera> UI_cameras;

    //////////  Getter & Setter //////////
    Camera HYJ_UI_Get_Camera(int _count)
    {
        return UI_cameras[_count];
    }

    object HYJ_UI_Get_Camera_Bridge(params object[] _args)
    {
        int count = (int)_args[0];

        //
        return HYJ_UI_Get_Camera(count);
    }

    //////////  Method          //////////

    //////////  Default Method  //////////
    void HYJ_UI_Init()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.MASTER___UI__GET_CAMERA, HYJ_UI_Get_Camera_Bridge);
    }
}

#endregion