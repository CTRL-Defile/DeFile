using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusBar : MonoBehaviour
{
    public enum STATUS_BAR
    {
        HPBar, MPBar
    }

    public enum STATUS_HP_COLOR
    {
        RED = 0,
        GREEN = 1,        
    }


    RectTransform Trans;

    [SerializeField]
    STATUS_BAR Bar;

    [SerializeField]
    Image FillImage;

    [SerializeField]
    STATUS_HP_COLOR HPColor;

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

    public void SetHPColor(STATUS_HP_COLOR color)
    {
        switch(color)
        {
            case STATUS_HP_COLOR.RED:
				FillImage.color = new Color(0.98f, 0.243f, 0.243f, 1f);
				break;
            case STATUS_HP_COLOR.GREEN:
				FillImage.color = new Color(0.2f, 0.718f, 0.18f, 1f);
				break;
            default: 
                break;
		}
    }
}