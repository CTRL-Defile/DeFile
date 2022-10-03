using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentObject : MonoBehaviour
{
    bool isTrial = true;

    Transform tr;
    GameObject go;
    JudgmentObjectPool judgmentObjectPoolMgr;

    private int skillID;
    private int casterInstanceID;
    private int targetInstanceID;

    private List<int> judgmentTargetsInstanceID = new List<int>();
    private IEnumerator _judgmentDelay;

    private float effectAmount;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        go = gameObject;
        judgmentObjectPoolMgr = GetComponentInParent<JudgmentObjectPool>();
    }

    //���ʷ� ����� ���� ��쿡�� �ڵ����� ��Ȱ��ȭ (���� Pool���� ��)
    //�� �ܿ��� ���� ����� �ξ��� Ÿ�ٸ���Ʈ �ʱ�ȭ

    public void ActiveSkill(int skillID, int casterInstanceID, int targetInstanceID)
    {
        Debug.Log("���̿���");
        go.SetActive(true);

        this.skillID = skillID;
        this.casterInstanceID = casterInstanceID;
        this.targetInstanceID = targetInstanceID;

        if (SkillManager.Instance.GetSkillData(skillID).judgmentTime.Equals(0.0f))
        {
            FindJudgmentTarget();
        }
        else
        {
            UnitManager.Instance.GetUnitController(casterInstanceID).SetUsingJudgmentObject(this);
            _judgmentDelay = JudgmentDelay();
            StartCoroutine(_judgmentDelay);
        }
    }

    public void DisableSkill()
    {
        if(go.activeSelf)
            go.SetActive(false);
    }

    //���� �����̸�ŭ ��� �� ���� ����
    IEnumerator JudgmentDelay()
    {
        effectAmount = JudgmentCalculater.Instance.EffectAmountCalculater(skillID, casterInstanceID);

        if (SkillManager.Instance.GetSkillData(skillID).skillCenterPoint.HasFlag(SkillCenterPoint.OfStartTime))
        {
            if (SkillManager.Instance.GetSkillData(skillID).skillCenterPoint.HasFlag(SkillCenterPoint.TargetLocation))
                tr.position = UnitManager.Instance.GetUnitPosition(targetInstanceID);
            else
                Debug.Log("error : can't cope this skillcenterpoint ");

            yield return new WaitForSeconds(SkillManager.Instance.GetSkillData(skillID).judgmentTime);

            FindJudgmentTarget();            
        }
        else if(SkillManager.Instance.GetSkillData(skillID).skillCenterPoint.HasFlag(SkillCenterPoint.OfJudgmentTime))
        {
            yield return new WaitForSeconds(SkillManager.Instance.GetSkillData(skillID).judgmentTime);

            if (SkillManager.Instance.GetSkillData(skillID).skillCenterPoint.HasFlag(SkillCenterPoint.TargetLocation))
                tr.position = UnitManager.Instance.GetUnitPosition(targetInstanceID);
            else
                Debug.Log("error : can't cope this skillcenterpoint ");

            FindJudgmentTarget();
        }
        else if (SkillManager.Instance.GetSkillData(skillID).skillCenterPoint.HasFlag(SkillCenterPoint.Target))
        {
            tr.position = UnitManager.Instance.GetUnitPosition(targetInstanceID);

            if (skillID.Equals(2222))
                Debug.Log($"222�ð� {SkillManager.Instance.GetSkillData(skillID).judgmentTime}");
            yield return new WaitForSeconds(SkillManager.Instance.GetSkillData(skillID).judgmentTime);

            CalculationJudgment();
        }
    }

    //��ų���� �����ؼ� ���� �� Ÿ�� ����
    private void FindJudgmentTarget()
    {
        switch (SkillManager.Instance.GetSkillData(skillID).areaForm)
        {
            case SkillAreaForm.Circle:
                var tempJudgmentTargets = Physics2D.OverlapCircleAll(transform.position, SkillManager.Instance.GetSkillData(skillID).areaLength);
                for (int i = 0; i < tempJudgmentTargets.Length; i++)
                {
                    if (!tempJudgmentTargets[i].gameObject.GetInstanceID().Equals(casterInstanceID))
                    {
                        judgmentTargetsInstanceID.Add(tempJudgmentTargets[i].gameObject.GetInstanceID());
                    }
                }
                CalculationJudgments(skillID, judgmentTargetsInstanceID);
                break;
            default:
                break;
        }
    }


    private void CalculationJudgment()
    {
        JudgmentCalculater.Instance.CalculationJudgment(skillID, casterInstanceID, targetInstanceID, effectAmount);
        gameObject.SetActive(false);
    }
    

    private void CalculationJudgments(int _id, List<int> targetInstanceIDList)
    {
        for(int i = 0; i < targetInstanceIDList.Count; i++)
        {
            JudgmentCalculater.Instance.CalculationJudgment(skillID, casterInstanceID, targetInstanceIDList[i], effectAmount);
        }
        gameObject.SetActive(false);
    }
    

    private void OnDisable()
    {
        if(isTrial)
        {
            isTrial = false;
            return;
        }

        if (UnitManager.Instance.GetUnitController(casterInstanceID).nowUsingJudgmentObject.Equals(this))
            UnitManager.Instance.GetUnitController(casterInstanceID).ResetJudgmentObject();

        if (_judgmentDelay != null)
        {
            StopCoroutine(_judgmentDelay);
            _judgmentDelay = null;
        }

        skillID = 0;
        casterInstanceID = 0;
        effectAmount = 0.0f;

        judgmentObjectPoolMgr.EnqueueObject(this);
    }
}
