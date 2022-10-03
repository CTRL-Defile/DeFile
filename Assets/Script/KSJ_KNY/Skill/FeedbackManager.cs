using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{

    private static FeedbackManager s_instance = null;
    public static FeedbackManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<FeedbackManager>();
            }
            return s_instance;
        }
    }


    public void Damage(int casterInstanceID, int judgmentTargetInstanceID, float amount, SkillEffectType skillEffectType, bool mpRecovery)
    {
        UnitManager.Instance.GetUnitFeedback(judgmentTargetInstanceID).Damage(amount, skillEffectType);

        if (mpRecovery)
            UnitManager.Instance.GetUnitFeedback(casterInstanceID).RecoveryMP(10.0f);
    }

    public void RecoveryHP(int casterInstanceID, int judgmentTargetInstanceID, float amount, SkillEffectType skillDamageType, bool mpRecovery)
    {
        UnitManager.Instance.GetUnitFeedback(judgmentTargetInstanceID).RecoveryHP(amount);

        if (mpRecovery)
            UnitManager.Instance.GetUnitFeedback(casterInstanceID).RecoveryMP(10.0f);
    }

}
