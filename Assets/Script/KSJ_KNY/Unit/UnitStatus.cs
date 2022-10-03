using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class UnitStatus : MonoBehaviour
{
    Transform tr;
    GameObject go;

    // Start is called before the first frame update
    private UnitController _unitController;

    [SerializeField] private int _unitID;

    [SerializeField] private float _maxHP;
    [SerializeField] private float _nowHP;
    [SerializeField] private float _maxMP;
    [SerializeField] private float _nowMP;

    [SerializeField] private float _attackPower;
    [SerializeField] private float _spellPower;
    [SerializeField] private float _defence;
    [SerializeField] private float _spellResistance;

    [SerializeField] private float _criticalChance;
    [SerializeField] private float _criticalMultiplier;

    [SerializeField] private int _normalSkillID;
    [SerializeField] private string _normalSkillAnimation;
    [SerializeField] private int _specialSkillID;
    [SerializeField] private string _specialSkillAnimation;

    public int unitID { get => _unitID; }
    public float maxHP { get => _maxHP; }
    public float nowHP { get => _nowHP; }
    public float maxMP { get => _maxMP; }
    public float nowMP { get => _nowMP; }

    public float attackPower { get => _attackPower; }
    public float spellPower { get => _spellPower; }
    public float defence { get => _defence; }
    public float spellResistance { get => _spellResistance; }

    public float criticalChance { get => _criticalChance; }
    public float criticalMultiplier { get => _criticalMultiplier; }

    public int normalSkillID { get => _normalSkillID; }
    public string normalSkillAnimation { get => _normalSkillAnimation; }
    public int specialSkillID { get => _specialSkillID; }
    public string specialSkillAnimation { get => _specialSkillAnimation; }

    private UnitData _unitBaseData;
    [SerializeField] private GameObject _unitModelingObject;
    [SerializeField] private string _moveAnimation;
    [SerializeField] private string _deathAnimation;

    private bool _settingComplete;
    public bool settingComplete { get => _settingComplete; }
    public string moveAnimation { get => _moveAnimation; }
    public string deathAnimation { get => _deathAnimation; }

    void Awake()
    {
        tr = GetComponent<Transform>();
        go = gameObject;

        _unitController = GetComponent<UnitController>();
    }

    public void SetUnitID(int id)
    {
        _unitID = id;
        SetUnitBaseStatus(UnitManager.Instance.GetUnitData(_unitID));
    }

    public void IncreaseHP(float amount)
    {
        if (nowHP + amount > 0)
        {
            if(nowHP + amount >= maxHP)
                _nowHP = maxHP;
            else
                _nowHP += amount;
        }
        else
        {
            _nowHP = 0.0f;
            _unitController.Death();
        }
    }

    public void IncreaseMP(float amount)
    {
        if (nowMP + amount > maxMP)
        {
            _nowMP = maxMP;
        }
        else if (nowMP + amount < 0)
        {
            _nowMP = 0.0f;
        }
        else
        {
            _nowMP += amount;
        }
    }

    public void ResetNowMP(float value)
    {
        if (value > maxMP)
        {
            _nowMP = maxMP;
        }
        else if (value < 0)
        {
            _nowMP = 0.0f;
        }
        else
        {
            _nowMP = value;
        }
    }

    public void SetUnitBaseStatus(UnitData unitData)
    {
        if(!_settingComplete)
        {
            if (!_unitBaseData.id.Equals(unitData.id))
            {
                _unitBaseData = unitData;
            }

            _maxHP = _unitBaseData.maxHP;
            _nowHP = maxHP;

            _maxMP = _unitBaseData.maxMP;
            _nowMP = 0.0f;

            _attackPower = _unitBaseData.attackPower;
            _spellPower = _unitBaseData.spellPower;
            _defence = _unitBaseData.defence;
            _spellResistance = _unitBaseData.spellResistance;

            _criticalChance = _unitBaseData.criticalChance;
            _criticalMultiplier = _unitBaseData.criticalMultiplier;

            _normalSkillID = _unitBaseData.normalSkillID;
            _normalSkillAnimation = _unitBaseData.normalSkillAnimation;
            _specialSkillID = _unitBaseData.specialSkillID;
            _specialSkillAnimation = _unitBaseData.specialSkillAnimation;

            _unitController.SetUnitModelingObject(UnitManager.Instance.CallUnitModelingObject(unitID, GetComponent<Transform>()));
            _moveAnimation = _unitBaseData.moveAnimation;
            _deathAnimation = _unitBaseData.deathAnimation;

            _settingComplete = true;
        }      
    }


}
