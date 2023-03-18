using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionMark_Collection : MonoBehaviour
{
    public void QuestionMark_MouseOver()
    {
        this.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void QuestionMark_MouseExit()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
    }
}
