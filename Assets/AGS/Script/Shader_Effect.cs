using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shader_Effect : MonoBehaviour
{
    public enum EFFECT_MODE { MODE_DEFAULT, MODE_PHASE, MODE_DISSOLVE, MODE_RIM, MODE_END };
	public enum PHASE_DISSOLVE_MODE { MODE_RIM_PHASE = 4, MODE_RIM_DISSOLVE = 5 };
	public enum RIM_TYPE { RIM_SILVER, RIM_GOLD, RIM_END };

    [SerializeField]
    private Renderer[] _renderer = new Renderer[2];

    [SerializeField]
    private Material[] _materials;

    [SerializeField]
    private float TimeEffect = 5.0f;

    [SerializeField]
    private float TimeAcc = 0.0f;	

	[SerializeField]
    EFFECT_MODE EffectMode;

    [SerializeField]
    private float Splitval = -0.1f;

	[SerializeField]
	Texture2D BaseTex;

    [SerializeField]
    private bool Effect_State = true;
	private bool SetTex = false;	
	private Color Rim_Color;

	// Start is called before the first frame update
	void Start()
    {
		//_renderer.material = MtrPhase;		

		Set_EffectMode(EFFECT_MODE.MODE_PHASE);		
	}

    // Update is called once per frame
    void Update()
    {
		if(SetTex == false)
		{
			if (_renderer[0] != null && BaseTex != null)
				_renderer[0].material.SetTexture("_Base_Map", BaseTex);

			if (_renderer[1] != null && BaseTex != null)
				_renderer[1].material.SetTexture("_Base_Map", BaseTex);

			SetTex = !SetTex;
		}


		BATTLE_PHASE phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);
		if (phase == BATTLE_PHASE.PHASE_PREPARE ||
			phase == BATTLE_PHASE.PHASE_COMBAT ||
			phase == BATTLE_PHASE.PHASE_COMBAT_OVER)
		{
			// Phase -0.05 ~ 2.5 // Dissolve -0.1 ~ 1.2			
			if (Effect_State)
			{
				switch (EffectMode)
				{
					case EFFECT_MODE.MODE_PHASE:
						TimeEffect = 1.0f;
						TimeAcc += Time.deltaTime;
						Splitval = Mathf.Lerp(-0.05f, 5.0f, TimeAcc / TimeEffect);
						Set_SplitValue(Splitval);
						if (Splitval >= 5.0f)
						{
							Effect_State = false;
							TimeAcc = 0.0f;

							if(GetComponent<Character>().UnitStar == Character.Unit_Star.ONE)
								Set_EffectMode(EFFECT_MODE.MODE_DEFAULT);
							else
								Set_EffectMode(EFFECT_MODE.MODE_RIM);
						}
						break;
					case EFFECT_MODE.MODE_DISSOLVE:
						TimeEffect = 0.5f;
						TimeAcc += Time.deltaTime;
						Splitval = Mathf.Lerp(1.2f, -0.1f, TimeAcc / TimeEffect);
						Set_SplitValue(Splitval);
						if (Splitval <= -0.1f)
						{
							Effect_State = false;
							TimeAcc = 0.0f;							
						}
						break;
					default:
						break;
				}

			}
		}
			
	}

    public void Set_EffectMode(EFFECT_MODE mode)
    {
		EffectMode = mode;

		if(mode == EFFECT_MODE.MODE_PHASE || mode == EFFECT_MODE.MODE_DISSOLVE)		
			Effect_State = true;

		Character.Unit_Star Star = GetComponent<Character>().UnitStar;

		switch (mode)
        {
			case EFFECT_MODE.MODE_DEFAULT:
				if (_renderer[0] != null)
					_renderer[0].material = _materials[(int)EFFECT_MODE.MODE_DEFAULT];
				if (_renderer[1] != null)
					_renderer[1].material = _materials[(int)EFFECT_MODE.MODE_DEFAULT];
				break;
			case EFFECT_MODE.MODE_PHASE:
				if (_renderer[0] != null)
				{
					if (Star == Character.Unit_Star.TWO)
					{
						_renderer[0].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_PHASE];
						Set_RimColor(RIM_TYPE.RIM_SILVER);						
					}
					else if(Star == Character.Unit_Star.THREE)
					{
						_renderer[0].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_PHASE];
						Set_RimColor(RIM_TYPE.RIM_GOLD);						
					}
					else
						_renderer[0].material = _materials[(int)EFFECT_MODE.MODE_PHASE];

					_renderer[0].material.SetTexture("_Base_Map", BaseTex);
				}
					
				if (_renderer[1] != null)
				{
					if (Star == Character.Unit_Star.TWO)
					{
						_renderer[1].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_PHASE];
						Set_RimColor(RIM_TYPE.RIM_SILVER);						
					}
					else if (Star == Character.Unit_Star.THREE)
					{
						_renderer[1].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_PHASE];
						Set_RimColor(RIM_TYPE.RIM_GOLD);						
					}
					else
						_renderer[1].material = _materials[(int)EFFECT_MODE.MODE_PHASE];

					_renderer[1].material.SetTexture("_Base_Map", BaseTex);
				}
				break;
			case EFFECT_MODE.MODE_DISSOLVE:
				if (_renderer[0] != null)
				{
					if (Star == Character.Unit_Star.TWO)
					{
						_renderer[0].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_DISSOLVE];
						Set_RimColor(RIM_TYPE.RIM_SILVER);						
					}
					else if (Star == Character.Unit_Star.THREE)
					{
						_renderer[0].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_DISSOLVE];
						Set_RimColor(RIM_TYPE.RIM_GOLD);						
					}
					else
						_renderer[0].material = _materials[(int)EFFECT_MODE.MODE_DISSOLVE];

					_renderer[0].material.SetTexture("_Base_Map", BaseTex);
				}
					
				if (_renderer[1] != null)
				{
					if (Star == Character.Unit_Star.TWO)
					{
						_renderer[1].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_DISSOLVE];
						Set_RimColor(RIM_TYPE.RIM_SILVER);						
					}
					else if (Star == Character.Unit_Star.THREE)
					{
						_renderer[1].material = _materials[(int)PHASE_DISSOLVE_MODE.MODE_RIM_DISSOLVE];
						Set_RimColor(RIM_TYPE.RIM_GOLD);						
					}
					else
						_renderer[1].material = _materials[(int)EFFECT_MODE.MODE_DISSOLVE];

					_renderer[1].material.SetTexture("_Base_Map", BaseTex);
				}
				break;
			case EFFECT_MODE.MODE_RIM:
				if (_renderer[0] != null)
				{
					_renderer[0].material = _materials[(int)EFFECT_MODE.MODE_RIM];				
					_renderer[0].material.SetTexture("_Base_Map", BaseTex);

					if (Star == Character.Unit_Star.TWO)
					{
						Set_RimColor(RIM_TYPE.RIM_SILVER);
					}
					else if (Star == Character.Unit_Star.THREE)
					{
						Set_RimColor(RIM_TYPE.RIM_GOLD);
					}
				}

				if (_renderer[1] != null)
				{
					_renderer[1].material = _materials[(int)EFFECT_MODE.MODE_RIM];
					_renderer[1].material.SetTexture("_Base_Map", BaseTex);

					if (Star == Character.Unit_Star.TWO)
					{
						Set_RimColor(RIM_TYPE.RIM_SILVER);
					}
					else if (Star == Character.Unit_Star.THREE)
					{
						Set_RimColor(RIM_TYPE.RIM_GOLD);
					}
				}
				break;
			default:
                break;
		}
    }

    void Set_SplitValue(float val)
    {
        if (EFFECT_MODE.MODE_DEFAULT == EffectMode || EFFECT_MODE.MODE_END == EffectMode)
            return;

        if(_renderer[0] != null)
            _renderer[0].material.SetFloat("_Split_Value", val);

        if (_renderer[1] != null)
		    _renderer[1].material.SetFloat("_Split_Value", val);
	}

	void Set_RimColor(RIM_TYPE color)
	{
		if (EFFECT_MODE.MODE_DEFAULT == EffectMode || EFFECT_MODE.MODE_END == EffectMode)
			return;

		// Lim Color Gold == 200 159 35 or 218 165 32, Silver 192 192 192
		switch (color)
		{
			case RIM_TYPE.RIM_SILVER:
				Rim_Color = new Color(0.753f, 0.753f, 0.753f);
				break;
			case RIM_TYPE.RIM_GOLD:
				Rim_Color = new Color(0.784f, 0.624f, 0.137f);
				break;
			default:
				break;
		}

		if (_renderer[0] != null)
		{
			_renderer[0].material.SetColor("_RimColor", Rim_Color);
		}
		if (_renderer[1] != null)
		{
			_renderer[1].material.SetColor("_RimColor", Rim_Color);
		}
	}
}