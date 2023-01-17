using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JHW_OptionManager : MonoBehaviour
{
    public void optionOnOff(bool arg)
    {
        GameObject.Find("OptionPopup").transform.GetChild(0).gameObject.SetActive(arg);
    }
}
