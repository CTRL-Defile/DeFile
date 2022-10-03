using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgmentObjectPool : MonoBehaviour
{
    [SerializeField] float startCreateCount;
    [SerializeField] GameObject judgmentObject;

    Queue<JudgmentObject> judgmentObjectQueue = new Queue<JudgmentObject>();

    void Awake()
    {
        judgmentObjectQueue.Clear();
        judgmentObject = Resources.Load("SkillPrefab/JudgmentObject") as GameObject;
    }

    
    public void CallSkillObject(int skillID, int casterInstanceID, int targetInstanceID)
    {
        if(!ChkjudgmentObjectQueue())
        {
            judgmentObjectQueue.Enqueue(Instantiate(judgmentObject, transform).GetComponent<JudgmentObject>());
        }
        judgmentObjectQueue.Dequeue().ActiveSkill(skillID, casterInstanceID, targetInstanceID);
    }

    private bool ChkjudgmentObjectQueue()
    {
        return !judgmentObjectQueue.Count.Equals(0);
    }

    public void EnqueueObject(JudgmentObject judgmentObject)
    {
        judgmentObjectQueue.Enqueue(judgmentObject);
    }
}
