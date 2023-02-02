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
	public Slider EffectHPBarSlider;

	[SerializeField]
    GameObject LineParent;

    [SerializeField]
    bool m_bEffect = false;

    float PreMaxHP = 0.0f;

    Canvas canvas;

	// Start is called before the first frame update
	void Start()
    {
		Trans = GetComponent<RectTransform>();
        Target_Obj = gameObject.GetComponentInParent<Character>();

        if(Bar == STATUS_BAR.HPBar)
        {
			EffectHPBarSlider = gameObject.transform.GetChild(1).GetComponent<Slider>();
			HPBarSlider = gameObject.transform.GetChild(2).GetComponent<Slider>();
		}
        else
        {
			HPBarSlider = gameObject.transform.GetChild(0).GetComponent<Slider>();
		}

        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;


        //FillImage.color = new Color(250, 62, 62, 255);
        //FillImage.color = new Color(51, 183, 46, 255);

    }

    // Update is called once per frame
    void Update()
    {       
        switch (Bar)
        {
            case STATUS_BAR.HPBar:
                float CurHp = Target_Obj.Stat_HP / Target_Obj.Stat_MaxHP;
				HPBarSlider.value = CurHp;
                            
                Invoke("HpEffectFunc", 1.0f);
                if(true == m_bEffect)
                {
					EffectHPBarSlider.value = Mathf.Lerp(EffectHPBarSlider.value, CurHp, Time.deltaTime * 1.5f);

                    if (HPBarSlider.value >= EffectHPBarSlider.value - 0.01f)
                    {
						m_bEffect = false;
                        EffectHPBarSlider.value = HPBarSlider.value;
					}                        
				}				    
                break;
            case STATUS_BAR.MPBar:
                float CurMp = Target_Obj.Stat_MP / Target_Obj.Stat_MaxMP;
                HPBarSlider.value = CurMp;
                break;
            default:
                break;
        }

    }

	private void LateUpdate()
	{
		Transform Rootparent = transform.parent;        

		if (Bar == STATUS_BAR.HPBar)
        {           
            transform.position = Rootparent.position + Vector3.up * (Rootparent.GetComponent<CapsuleCollider>().bounds.size.y + 0.6f);
			//Trans.position = new Vector3(Target_Obj.transform.position.x, Target_Obj.transform.position.y + 3.1f, Target_Obj.transform.position.z);

            if(PreMaxHP != Target_Obj.Stat_MaxHP)
			    SetHpLine();
		}
            
        else if(Bar == STATUS_BAR.MPBar)
        {			
			transform.position = Rootparent.position + Vector3.up * (Rootparent.GetComponent<CapsuleCollider>().bounds.size.y + 0.4f) + Vector3.forward * -0.1f;
		}

		PreMaxHP = Target_Obj.Stat_MaxHP;
		transform.rotation = Camera.main.transform.rotation;
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

    public void SetHpLine()
    {
        float scaleX = 1250.0f / Target_Obj.Stat_MaxHP;
		LineParent.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(false);
        foreach(Transform child in LineParent.transform)
        {
            child.gameObject.transform.localScale = new Vector3(scaleX, 1.0f, 1.0f);
        }
		LineParent.GetComponent<HorizontalLayoutGroup>().gameObject.SetActive(true);
	}

    void HpEffectFunc()
    {
        m_bEffect = true;
	}
}