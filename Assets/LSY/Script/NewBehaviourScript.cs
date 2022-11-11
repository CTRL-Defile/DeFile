using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void Start()
    {
        Debug.Log("start");
        func();
    }

    void func()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("csv_sample");
        Debug.Log(data.Count);

        for (int i = 0; i < data.Count; i++)
        {
            Debug.Log(data[i]["NAME"].ToString());
        }
    }


}
