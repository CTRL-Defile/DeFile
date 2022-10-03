using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private static TileManager s_instance = null;
    public static TileManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<TileManager>();
            }
            return s_instance;
        }
    }

	UnitController _unit;

    Dictionary<int, IntVector2> unitPositionDic = new Dictionary<int, IntVector2>();

    private int _boardSize = 8;
   
    public int boardSize { get => _boardSize;}

    public Define.TileType[,] Tiles { get; private set; }

    public IntVector2 tempPos;

    private void Awake()
    {
        if (Tiles == null)
        {
            Tiles = new Define.TileType[boardSize, boardSize];
        }
    }

    public void SetUnitTilePosition(int posX, int posZ, int unitInstanceID, Define.TileType tileType)
	{
        if (!unitPositionDic.ContainsKey(unitInstanceID))
        {
            unitPositionDic.Add(unitInstanceID, new IntVector2(posX, posZ));
            Tiles[unitPositionDic[unitInstanceID].x, unitPositionDic[unitInstanceID].z] = tileType;
        }
        else
        {
            Tiles[unitPositionDic[unitInstanceID].x, unitPositionDic[unitInstanceID].z] = Define.TileType.Empty;
            
            if(!tileType.Equals(Define.TileType.Empty))
            {
                tempPos.x = posX;
                tempPos.z = posZ;

                unitPositionDic[unitInstanceID] = tempPos;
                Tiles[unitPositionDic[unitInstanceID].x, unitPositionDic[unitInstanceID].z] = tileType;
            }
        }
    }

    public void SetTileExitUnit(int unitInstanceID)
    {
        if (unitPositionDic.ContainsKey(unitInstanceID))
        {
            Tiles[unitPositionDic[unitInstanceID].x, unitPositionDic[unitInstanceID].z] = Define.TileType.Empty;
            unitPositionDic.Remove(unitInstanceID);
        }
    }

    public IntVector2 GetUnitTilePosition(int unitInstanceID) { return unitPositionDic[unitInstanceID]; }
}
