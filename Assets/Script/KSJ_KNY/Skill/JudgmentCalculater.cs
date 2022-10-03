using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentCalculater : MonoBehaviour
{
    //singleton
    private static JudgmentCalculater mInstance;
    public static JudgmentCalculater Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<JudgmentCalculater>();
            }
            return mInstance;
        }
    }
    
    public void CalculationJudgment(int skillID, int casterInstanceID, int judgmentTargetInstanceID, float Amount)
    {
        #region 기본효과 적용
        //효과량이 0 이상이면 적용
        if (!Amount.Equals(0.0f))
        {
            if (SkillManager.Instance.GetSkillData(skillID).effectType.HasFlag(SkillEffectType.Recovery))
            {                
                FeedbackManager.Instance.RecoveryHP(casterInstanceID, judgmentTargetInstanceID, Amount,
                    SkillManager.Instance.GetSkillData(skillID).effectType, UnitManager.Instance.GetUnitStatus(casterInstanceID).normalSkillID.Equals(skillID));

                return;
            }

            if (UnitManager.Instance.GetUnitStatus(casterInstanceID).criticalChance > 0.0f && Random.Range(0.0f, 1.0f) >= UnitManager.Instance.GetUnitStatus(casterInstanceID).criticalChance)
            {
                Debug.Log("치명타!!!!!!");
                FeedbackManager.Instance.Damage(casterInstanceID, judgmentTargetInstanceID, Amount * UnitManager.Instance.GetUnitStatus(casterInstanceID).criticalMultiplier,
                    SkillManager.Instance.GetSkillData(skillID).effectType, UnitManager.Instance.GetUnitStatus(casterInstanceID).normalSkillID.Equals(skillID));
            }
            else
            {
                FeedbackManager.Instance.Damage(casterInstanceID, judgmentTargetInstanceID, Amount,
                    SkillManager.Instance.GetSkillData(skillID).effectType, UnitManager.Instance.GetUnitStatus(casterInstanceID).normalSkillID.Equals(skillID));
            }            
        }
        #endregion


    }



    public float EffectAmountCalculater(int skillID, int casterInstanceID)
    {
        switch (SkillManager.Instance.GetSkillData(skillID).applyStatus)
        {
            case SkillApplyStatus.AttackPower:
                return (UnitManager.Instance.GetUnitStatus(casterInstanceID).attackPower * SkillManager.Instance.GetSkillData(skillID).coefficientAmount) 
                    + SkillManager.Instance.GetSkillData(skillID).baseAmount;

            case SkillApplyStatus.SpellPower:
                return (UnitManager.Instance.GetUnitStatus(casterInstanceID).spellPower * SkillManager.Instance.GetSkillData(skillID).coefficientAmount)
                    + SkillManager.Instance.GetSkillData(skillID).baseAmount;

            default:
                Debug.Log("error : This applystatus type is notting");
                return 0.0f;
        }
    }
}
