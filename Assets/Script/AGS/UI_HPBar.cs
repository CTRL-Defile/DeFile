using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : MonoBehaviour
{
    enum GameObjects
    {
        HPBar
    }

    RectTransform Trans;

    public HYJ_Character Target_Obj;
    public Slider HPBarSlider;

	// Start is called before the first frame update
	void Start()
    {
		Trans = GetComponent<RectTransform>();
        Target_Obj = gameObject.GetComponentInParent<HYJ_Character>();
        HPBarSlider = GetComponentInChildren<Slider>();
	}

    // Update is called once per frame
    void Update()
    {
        Trans.rotation = Camera.main.transform.rotation;


        float CurHp = Target_Obj.Stat_HP / Target_Obj.Stat_MaxHP;

        HPBarSlider.value = CurHp;

	}
}
