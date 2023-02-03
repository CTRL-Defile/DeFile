using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
	public enum ANIM_TYPE { BEAR, ORC, EVIL, GOBLIN_N, GOBLIN_T, WERERAT };

	[SerializeField]
    Collider Collider;

    [SerializeField]
    ANIM_TYPE type;

	public ANIM_TYPE Anim_Type { get { return type; } set { type = value; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(type)
        {
            case ANIM_TYPE.BEAR:				
				break;
			case ANIM_TYPE.ORC:
				break;
			case ANIM_TYPE.EVIL:
				break;
			case ANIM_TYPE.GOBLIN_N:
				break;
			case ANIM_TYPE.GOBLIN_T:
				break;
			default:
				break;
		}
    }

	public void SetEnableCollider()
	{
		Collider.enabled = true;
	}

	public void AddMP()
	{
		gameObject.GetComponentInParent<Character>().Stat_MP += 10.0f;
	}

	public void SetDisableCollider()
	{
		Collider.enabled = false;
	}
}
