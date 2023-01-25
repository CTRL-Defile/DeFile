using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LSY_Battle_Synergy : MonoBehaviour
{
    TextMeshProUGUI Synergy_Name, Synergy_Unit_Number, Synergy_Activation;
    public enum Synergy_Level
    {
        Red,
        Green,
        Gray,
        Level_END
    }
    public enum Synergy_Tribe
    {
        One,
        Two,
        Three,
        Dot,
        Tribe_END
    }

    //
    [SerializeField] private Synergy_Level synergy_lv;
    [SerializeField] private Synergy_Tribe synergy_tr;


    //
    public Synergy_Level Get_synergy_lv { get { return synergy_lv; } }
    public Synergy_Tribe Get_synergy_tr { get { return synergy_tr; } }


    void Start()
    {
        if (synergy_tr != Synergy_Tribe.Dot)
        {
            Synergy_Name = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            Synergy_Unit_Number = this.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        
    }

    public void LSY_Set_Synergy()
    {

    }




}
