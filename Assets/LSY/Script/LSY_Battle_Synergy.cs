using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LSY_Battle_Synergy : MonoBehaviour
{
    TextMeshProUGUI Synergy_Name, Synergy_Unit_Number, Synergy_Activation;
    private enum Synergy_Level
    {
        Red,
        Green,
        Gray,
        Level_END
    }
    private enum Synergy_Tribe
    {
        Bear,
        Orc,
        Tribe_END
    }

    [SerializeField] private Synergy_Level synerge_lv;
    [SerializeField] private Synergy_Tribe synerge_tr;


    void Start()
    {
        Synergy_Name = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        Synergy_Unit_Number = this.transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        
    }

    public void LSY_Set_Synergy()
    {

    }




}
