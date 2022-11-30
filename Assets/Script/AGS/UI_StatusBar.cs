using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusBar : MonoBehaviour
{
    enum STATUS_BAR
    {
        HPBar, MPBar
    }

    RectTransform Trans;

    [SerializeField]
    STATUS_BAR Bar;

    [SerializeField]
    Image FillImage;

    [SerializeField]
    public Character Target_Obj;
    public Slider HPBarSlider;

	// Start is called before the first frame update
	void Start()
    {
		Trans = GetComponent<RectTransform>();
        Target_Obj = gameObject.GetComponentInParent<Character>();
        HPBarSlider = GetComponentInChildren<Slider>();

        //FillImage.color = new Color(250, 62, 62, 255);
        //FillImage.color = new Color(51, 183, 46, 255);
        if (Bar == STATUS_BAR.HPBar)
            FillImage.color = new Color(0.2f, 0.718f, 0.18f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        Trans.rotation = Camera.main.transform.rotation;

        switch (Bar)
        {
            case STATUS_BAR.HPBar:
                float CurHp = Target_Obj.Stat_HP / Target_Obj.Stat_MaxHP;
                HPBarSlider.value = CurHp;
                break;
            case STATUS_BAR.MPBar:
                float CurMp = Target_Obj.Stat_MP / Target_Obj.Stat_MaxHP;
                HPBarSlider.value = CurMp;
                break;
            default:
                break;
        }

    }
}