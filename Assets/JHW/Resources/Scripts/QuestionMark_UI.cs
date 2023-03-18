using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionMark_UI : MonoBehaviour
{
    public void QuestionMark_MouseOver()
    {
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        this.transform.parent.GetChild(1).gameObject.SetActive(true);
    }

    public void QuestionMark_MouseExit()
    {
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
        this.transform.parent.GetChild(1).gameObject.SetActive(false);
    }
}
