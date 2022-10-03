using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Flags]
public enum SkillTag
{
    Strike = 1 << 0, 
    Spell = 1 << 1, 
    Buff = 1 << 2, 
    Debuff = 1 << 3, 
    Chain = 1 << 4, 
    Pierce = 1 << 5
}

public enum SkillUnitGroup
{
    Ally, Enemy
}

public enum SkillUnitSortingBase
{
    Random = 1 << 29,
    Ascending = 1 << 30,
    Decending = 1 << 31,

    Distance = 1 << 0,
    HealthPoint = 1 << 1,

    DistanceAscending = Distance | Ascending,
    DistanceDecending = Distance | Decending, 
    HealthPointAscending = HealthPoint | Ascending, 
    HealthPointDecending = HealthPoint | Decending,
}

public enum SkillCenterPoint
{
    OfStartTime = 1 << 30,
    OfJudgmentTime = 1 << 31,

    Target = 1 << 0,
    TargetLocation = 1 << 1,

    TargetLocationOfStartTime = TargetLocation | OfStartTime,
    TargetLocationOfJudgmentTime = TargetLocation | OfJudgmentTime
}

public enum SkillAreaForm
{
    Area = 1 << 1,
    Projectile = 1 << 2,

    CircleForm = 1 << 11,
    SectorForm = 1 << 12,
    SquareForm = 1 << 13,

    Circle = Area | CircleForm, 
    Sector = Area | SectorForm, 
    Square = Area | SquareForm
}

public enum SkillEffectType
{
    Physic = 1 << 30, 
    Magic = 1 << 31,

    Damage = 1 << 0,
    Recovery = 1 << 1,

    PhysicDamage = Physic | Damage,
    MagicDamage = Magic | Damage,
    PhysicRecovery = Physic | Recovery,
    MagicRecovery = Magic | Recovery,
}

public enum SkillApplyStatus
{
    AttackPower, SpellPower
}


public enum SkillAddEffectType
{
    DamageOverTime
}


public struct SkillData
{
    #region 스킬 데이터 변수
    private int _id;
    private string _nameKor;
    private SkillTag _skillTag;
    private SkillTag _skillCannotTag;

    private SkillUnitGroup _targetGroup;
    private int _targetRange;
    private SkillUnitSortingBase _targetSortingBase;

    private float _skillRange;
    private float _judgmentTime;
    private float _afterDelay;

    private SkillCenterPoint _skillCenterPoint;
    private float _skillDuration;
    private float _skillRepeatCycle;

    private SkillAreaForm _areaForm;
    private float _areaLength;
    private float _areaWidth;
    private SkillUnitGroup _judgmentGroup;
    private int _maximumJudgmentTargetCount;
    private SkillUnitSortingBase _judgmentTargetSortingBase;

    private SkillEffectType _effectType;
    private float _baseAmount;
    private SkillApplyStatus _applyStatus;
    public float _coefficientAmount;



    public int id { get => _id; }
    public string nameKor { get => _nameKor; }
    public SkillTag skillTag { get => _skillTag; }
    public SkillTag skillCannotTag { get => _skillCannotTag; }


    public SkillUnitGroup targetGroup { get => _targetGroup; }
    public int targetRange { get => _targetRange; }
    public SkillUnitSortingBase targetSortingBase { get => _targetSortingBase; }


    public float skillRange { get => _skillRange; }
    public float judgmentTime { get => _judgmentTime; }
    public float afterDelay { get => _afterDelay; }


    public SkillCenterPoint skillCenterPoint { get => _skillCenterPoint; }
    public float skillDuration { get => _skillDuration; }
    public float skillRepeatCycle { get => _skillRepeatCycle; }

    public SkillAreaForm areaForm { get => _areaForm; }
    public float areaLength { get => _areaLength; }
    public float areaWidth{ get => _areaWidth; }
    public SkillUnitGroup judgmentGroup { get => _judgmentGroup; }
    public int maximumJudgmentTargetCount { get => _maximumJudgmentTargetCount; }
    public SkillUnitSortingBase judgmentTargetSortingBase { get => _judgmentTargetSortingBase; }

    public SkillEffectType effectType { get => _effectType; }
    public float baseAmount { get => _baseAmount; }
    public SkillApplyStatus applyStatus { get => _applyStatus; }
    public float coefficientAmount { get => _coefficientAmount; }

    #endregion



    public SkillData SetSkillData(Dictionary<string, object> _skillData)
    {
        SetValue(ref _id, _skillData["ID"]);
        _nameKor = _skillData["NameKor"].ToString();
        /*
        _elemental = (SkillElemental)System.Enum.Parse(typeof(SkillElemental), _skillData["Elemental"].ToString());
        _type = (SkillType)System.Enum.Parse(typeof(SkillType), _skillData["Type"].ToString());*/

        _targetGroup = (SkillUnitGroup)System.Enum.Parse(typeof(SkillUnitGroup), _skillData["TargetGroup"].ToString());
        SetValue(ref _targetRange, _skillData["TargetRange"]);
        if (!_skillData["TargetSortingBase"].ToString().Equals("-"))
            _targetSortingBase = (SkillUnitSortingBase)System.Enum.Parse(typeof(SkillUnitSortingBase), _skillData["TargetSortingBase"].ToString());

        SetValue(ref _skillRange, _skillData["SkillRange"]);
        SetValue(ref _judgmentTime, _skillData["JudgmentTime"]);
        SetValue(ref _afterDelay, _skillData["AfterDelay"]);

        if (!_skillData["SkillCenterPoint"].ToString().Equals("-")) 
            _skillCenterPoint = (SkillCenterPoint)System.Enum.Parse(typeof(SkillCenterPoint), _skillData["SkillCenterPoint"].ToString());
        SetValue(ref _skillDuration, _skillData["SkillDuration"]);
        SetValue(ref _skillRepeatCycle, _skillData["SkillRepeatCycle"]);

        if (!_skillData["AreaForm"].ToString().Equals("-")) 
            _areaForm = (SkillAreaForm)System.Enum.Parse(typeof(SkillAreaForm), _skillData["AreaForm"].ToString());
        SetValue(ref _areaLength, _skillData["AreaLength"]);
        SetValue(ref _areaWidth, _skillData["AreaWidth"]);
        if (!_skillData["JudgmentGroup"].ToString().Equals("-")) 
            _judgmentGroup = (SkillUnitGroup)System.Enum.Parse(typeof(SkillUnitGroup), _skillData["JudgmentGroup"].ToString());
        SetValue(ref _maximumJudgmentTargetCount, _skillData["MaximumJudgmentTargetCount"]);
        if (!_skillData["JudgmentTargetSortingBase"].ToString().Equals("-"))
            _judgmentTargetSortingBase = (SkillUnitSortingBase)System.Enum.Parse(typeof(SkillUnitSortingBase), _skillData["JudgmentTargetSortingBase"].ToString());

        if (!_skillData["EffectType"].ToString().Equals("-"))
            _effectType = (SkillEffectType)System.Enum.Parse(typeof(SkillEffectType), _skillData["EffectType"].ToString());
        SetValue(ref _baseAmount, _skillData["BaseAmount"]);
        if (_skillData["ApplyStatus"].ToString().Equals("-"))
            _applyStatus = (SkillApplyStatus)System.Enum.Parse(typeof(SkillApplyStatus), _skillData["ApplyStatus"].ToString());
        SetValue(ref _coefficientAmount, _skillData["CoefficientAmount"]);

        return this;
    }

    private void SetValue(ref int variableName, object dataName)
    {
        if (!dataName.ToString().Equals("-"))
            variableName = (int)dataName;
    }
    private void SetValue(ref float variableName, object dataName)
    {
        if (!dataName.ToString().Equals("-"))
            variableName = (float)dataName;
    }
    private void SetValue(ref GameObject variableName, object dataName)
    {
        if (!dataName.ToString().Equals("-"))
        {
            variableName = Resources.Load("DataBase/" + dataName.ToString()) as GameObject;
        }
    }

}

public class SkillDataBase : MonoBehaviour
{
    private Dictionary<int, SkillData> skillDB = new Dictionary<int, SkillData>();

    private void Awake()
    {
        List<Dictionary<string, object>> SkillDataDialog = CSVReader.Read("DataBase/SkillDataBase");

        for (int i = 0; i < SkillDataDialog.Count; i++)
        {
            var tempSkillData = new SkillData();
            tempSkillData = new SkillData().SetSkillData(SkillDataDialog[i]);
            skillDB.Add((int)SkillDataDialog[i]["ID"], tempSkillData);
        }

        //필요없는건 정리 정리
        for (int i = 0; i < SkillDataDialog.Count; i++)
        {
            SkillDataDialog[i].Clear();
        }
        SkillDataDialog.Clear();
    }

    public SkillData GetSkillData(int _id)
    {
        return skillDB[_id];
    }    
}
