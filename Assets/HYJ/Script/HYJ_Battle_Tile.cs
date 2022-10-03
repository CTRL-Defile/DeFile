using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class HYJ_Battle_Tile : MonoBehaviour
{
    [SerializeField] HYJ_Character Basic_onUnit;    // 타일위에 올라가 있는 유닛

    //////////  Getter & Setter //////////

    //////////  Method          //////////
    public HYJ_Character HYJ_Basic_onUnit { get { return Basic_onUnit; } set { Basic_onUnit = value; } }

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
