using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eTag
{
    Ally, Enemy
}

[RequireComponent(typeof(TargetFinder))]
[RequireComponent(typeof(FeedbackManager))]
[RequireComponent(typeof(UnitDataBase))]
public class UnitManager : MonoBehaviour
{
    public struct UnitInformation
    {
        private GameObject _unit;
        private Transform _transform;
        private UnitController _unitController;
        private UnitStatus _unitStatus;
        private UnitFeedback _unitFeedback;

        private int _instanceID;

        public GameObject unit { get => _unit; }
        public Transform transform { get => _transform; }
        public UnitController unitController { get => _unitController; }
        public UnitStatus unitStatus { get => _unitStatus; }
        public UnitFeedback unitFeedback { get => _unitFeedback; }
        public float instanceID { get => _instanceID; }

        public UnitInformation(GameObject go, Transform tr, UnitController controller, UnitStatus status, UnitFeedback feedback)
        { _unit = go; _transform = tr; _unitController = controller; _unitStatus = status; _unitFeedback = feedback; _instanceID = go.GetInstanceID(); }

    }
    [Header("Unit parent transform")]
    [SerializeField] private Transform _unitparent;

    [Header("RequireComponent")]
    [SerializeField] private TargetFinder _targetFinder;
    [SerializeField] private UnitDataBase _unitDataBase;
    [SerializeField] private UnitBaseObjectPool _unitBaseObjPool;
    [SerializeField] private UnitModelingObjectPool _unitModelingObjPool;

    List<UnitInformation> _controllerList = new List<UnitInformation>();

    private bool _isBattleMode;
    public bool isBattleMode { get => _isBattleMode; }

    private Dictionary<int, UnitInformation> battleUnitDic = new Dictionary<int, UnitInformation>();
    private Dictionary<int, bool> battleUnitDeathDic = new Dictionary<int, bool>();


    private IntVector2 myTilePos;
    private IntVector2 targetTilePos;

    private GameObject newGameObject;

    static UnitManager minstance;
    public static UnitManager Instance
    {
        get
        {
            if (minstance == null)
            {
                minstance = FindObjectOfType<UnitManager>();
            }
            return minstance;
        }
    }

    void Awake()
    {
        _targetFinder = GetComponent<TargetFinder>();
        _unitDataBase = GetComponent<UnitDataBase>();

        if (GetComponentInChildren<UnitBaseObjectPool>() == null)
        {
            newGameObject = new GameObject("@UnitObjectPool");
            newGameObject.GetComponent<Transform>().parent = transform;
            newGameObject.AddComponent<UnitBaseObjectPool>();
        }

        _unitBaseObjPool = GetComponentInChildren<UnitBaseObjectPool>();

        if (GetComponentInChildren<UnitModelingObjectPool>() == null)
        {
            newGameObject = new GameObject("@UnitModelingObjectPool");
            newGameObject.GetComponent<Transform>().parent = transform;
            newGameObject.AddComponent<UnitModelingObjectPool>();
        }

        _unitModelingObjPool = GetComponentInChildren<UnitModelingObjectPool>();

        newGameObject = new GameObject("InGameUnit");
        _unitparent = newGameObject.transform;

        newGameObject = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isBattleMode)
            {
                BattleStart();
            }
        }
    }




    public void AddUnitInformation(UnitController controller)
    {
        _controllerList.Add(new UnitInformation(controller.gameObject, controller.transform, controller, controller.GetComponent<UnitStatus>(), controller.GetComponent<UnitFeedback>()));
    }

    public GameObject CallUnitModelingObject(int unitID, Transform parent)
    {
        return _unitModelingObjPool.CallUnitModelingObject(unitID, parent);
    }

    public void ReturnUnitModelingObject(UnitModelingObject modelingObject)
    {
        _unitModelingObjPool.EnqueueObject(modelingObject);
    }

    #region Unit Pooling
    public void CallUnit(int unitID, Vector3 pos, eTag tag)
    {
        switch (tag)
        {
            case eTag.Ally:
                _unitBaseObjPool.CallUnitBaseObject(unitID, pos, 0.0f, tag, _unitparent);
                break;

            case eTag.Enemy:
                _unitBaseObjPool.CallUnitBaseObject(unitID, pos, 180.0f, tag, _unitparent);
                break;

            default:
                break;
        }
    }

    public void ReturnUnit(UnitBaseObject baseObject, UnitController unitController)
    {
        for(int i = 0; i < _controllerList.Count; i++)
        {
            if(_controllerList[i].unitController.Equals(unitController))
            {
                _controllerList.RemoveAt(i);
                break;
            }
        }

        _unitBaseObjPool.EnqueueObject(baseObject);
    }
    #endregion

    #region Astar Nav
    public Vector3 NextPos(int myInstanceID, int targetInstanceID)
    {
        myTilePos = TileManager.Instance.GetUnitTilePosition(myInstanceID);
        targetTilePos = TileManager.Instance.GetUnitTilePosition(targetInstanceID);

        List<IntVector2> myPoints = AstarCalculater.Instance.FindAstar(myTilePos.x, myTilePos.z, targetTilePos.x, targetTilePos.z);

        if(myPoints.Count <= 2)
        {
            return new Vector3(myPoints[0].x, 0, myPoints[0].z);
        }
        else
        {
            return new Vector3(myPoints[1].x, 0, myPoints[1].z);
        }

    }
    #endregion

    #region BattleMode
    public void BattleStart()
    {
        battleUnitDic.Clear();
        battleUnitDeathDic.Clear();

        for (int i = 0; i < _controllerList.Count; i++)
        {
            if (_controllerList[i].unitController.CheckOnField())
            {
                battleUnitDic.Add(_controllerList[i].unit.GetInstanceID(), _controllerList[i]);
                battleUnitDeathDic.Add(_controllerList[i].unit.GetInstanceID(), false);

                TileManager.Instance.SetUnitTilePosition
                    ((int)battleUnitDic[_controllerList[i].unit.GetInstanceID()].transform.position.x, (int)battleUnitDic[_controllerList[i].unit.GetInstanceID()].transform.position.z,
                    _controllerList[i].unit.GetInstanceID(), Define.TileType.InUnit);
            }
        }

        foreach (KeyValuePair<int, UnitInformation> unitsPair in battleUnitDic)
        {
            unitsPair.Value.unitController.ActiveBattle(true);
        }

        _isBattleMode = true;
    }

    public InstanceIDContainer TargetFinder(int instanceID, SkillData skillData)
    {
        return _targetFinder.TargetUnitFinder(instanceID, battleUnitDic, skillData.targetRange, skillData.targetGroup, skillData.targetSortingBase);
    }

    public void SetUnitDeath(int instancdID)
    {
        battleUnitDeathDic[instancdID] = true;
        TileManager.Instance.SetTileExitUnit(instancdID);
    }
    #endregion

    public Vector3 GetUnitPosition(int instanceID) { return battleUnitDic[instanceID].transform.position; }
    public UnitFeedback GetUnitFeedback(int instanceID) { return battleUnitDic[instanceID].unitFeedback; }
    public UnitStatus GetUnitStatus(int instanceID) { return battleUnitDic[instanceID].unitStatus; }
    public UnitController GetUnitController(int instanceID) { return battleUnitDic[instanceID].unitController; }

    public bool GetBattleUnitIsDeath(int instancdID) { return battleUnitDeathDic[instancdID]; }
    public UnitData GetUnitData(int _id) { return _unitDataBase.GetUnitData(_id); }
}