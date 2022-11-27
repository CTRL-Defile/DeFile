using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


public class PathFinder : MonoBehaviour
{	
    PriorityQueue<int> m_PQ = new PriorityQueue<int>();
    
    [SerializeField]
    GameObject m_HostObj;

	[SerializeField] 
    BATTLE_PHASE m_Basic_phase;

	[SerializeField]
    private int m_CurIdxX = 0;
	[SerializeField]
	private int m_CurIdxY = 0;

	[SerializeField]
	List<HYJ_Battle_Manager_Line> TileLines;
	
	List<NODE> m_Graph = new List<NODE>();

	List<NODE> m_OpenNodes = new List<NODE>();
	List<NODE> m_CloseNodes = new List<NODE>();
	List<int> m_Path = new List<int>();

	// 오픈리스트에 있는 정보들 중에서 가장 좋은 후보를 빠르게 뽑아오기위한 컨테이너 PriorityQueue 적용 예정
		

	// Start is called before the first frame update
	void Start()
    {
		m_HostObj = gameObject;
		m_Basic_phase = 0;
		SetupGraph();
	}

    // Update is called once per frame
    void Update()
    {

	}

	private void LateUpdate()
	{
		m_Basic_phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);

		if (m_Basic_phase == BATTLE_PHASE.PHASE_COMBAT)
		{				
			AStar();			
		}
	}

	void AStar()
    {				
		// Score
		// F = G + H
		// F = 최종 점수 ( 작을 수록 좋고 경로에 따라 다름 )
		// G = 시작점에서 해당 좌표까지 이동하는데 드는 비용 ( 작을 수록 좋고 경로에 따라 다름 )
		// H = 목적지에서 얼마나 가까운지 ( 작을 수록 좋고 고정 값 )

		// (y, x) 이미 방문했는지 여부 ( 방문 == closed )
		// 배열로하면 메모리 많이 사용. But 연산속도 Up
		int IdxX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
		int IdxY = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y);
        bool[,] closed = new bool[IdxX, IdxY];

        //(y, x) 가는 길을 한번이라도 발견했는지
        // 발견 X == MaxValue
        // 발견 O == ( F = G + H )
        int[,] open = new int[IdxX, IdxY];
        for (int y = 0; y < IdxY; y++)
        {
            for (int x = 0; x < IdxX; x++)
            {
                open[x, y] = Int32.MaxValue;
            }
        }

        // 시작점 발견 ( 예약 진행 )
        //open[m_CurIdxX, m_CurIdxY] = Math.Abs( , );        
	}

	private void SetupGraph()
	{
		TileLines = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
		int Index = 0;
		int MinimX = TileLines[0].HYJ_Data_GetCount();
		int MaximX = TileLines[1].HYJ_Data_GetCount();
		double TruncateVal = 0.0f;		

		if(TileLines.Count >0)
		{
			for (int i = 0; i < TileLines.Count; ++i)
			{
				int TileXSize = TileLines[i].HYJ_Data_GetCount();
				for (int j = 0; j < TileXSize; ++j)
				{					
					NODE node = new NODE(Index, TileLines[i].HYJ_Data_Tile(j).gameObject.transform.localPosition, TileLines[i].HYJ_Data_Tile(j));
					m_Graph.Add(node);
					TileLines[i].HYJ_Data_Tile(j).GraphIndex = Index;
					Index++;
				}
			}
		}

		for(int i = 0; i < TileLines.Count; ++i)
		{
			int TileXSize = TileLines[i].HYJ_Data_GetCount();

			// 행별 열개수 차에 의한 Index 보정 값
			TruncateVal = Math.Truncate(i / 2.0f);

			for (int j = 0; j < TileXSize; ++j)
			{
				if (i % 2 == 0 || i >= 3)
					Index = i * MinimX + j + (int)TruncateVal;				
				else
					Index = i * MinimX + j;

				if (m_Graph.Count <= Index)
					continue;

				//Debug.Log("그래프 생성 중 Index : " + Index);
				
				// 좌 상단 인접 타일
				// 맨윗줄과 맨왼쪽줄에 홀수타일은 좌상단 타일이 없다
				if (i != 0 && (j != 0 || (j == 0 && i % 2 == 0))) // 맨윗줄 아닐때 && ( 맨왼쪽 아닐때 || (  j == 0 인데 짝수 줄이면 좌상단 있어서 예외처리 )
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - (MaximX)]);
					//Debug.Log("m_Graph[" + Index + "]의 좌상단 타일 Index" + (Index - MaximX));
				}

				// 우 상단 인접 타일
				if (i != 0 && ((j != TileXSize - 1) || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - (MinimX)]);
					//Debug.Log("m_Graph[" + Index + "]의 우상단 타일 Index" + (Index - MinimX));
				}

				// 우 인접 타일
				if (j < TileXSize - 1 )
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + 1]);
					//Debug.Log("m_Graph[" + Index + "]의 우 타일 Index" + (Index + 1));
				}

				// 우 하단 인접 타일			
				// 맨 아랫줄과 맨오른쪽 홀수줄은 우 하단 타일 없음	
				if (i != TileLines.Count - 1 && (j != TileXSize - 1 || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + MaximX]);
					//Debug.Log("m_Graph[" + Index + "]의 우하단 타일 Index" + (Index + MaximX));
				}

				// 좌 하단 인접 타일
				if (i != TileLines.Count - 1 && (j != 0 || (j == 0 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + MinimX]);
					//Debug.Log("m_Graph[" + Index + "]의 좌하단 타일 Index" + (Index + MinimX));
				}

				// 좌 인접 타일
				if (j > 0)
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - 1]);
					//Debug.Log("m_Graph[" + Index + "]의 좌 타일 Index" + (Index - 1));
				}
			}			
		}
	}

	public bool StartPathFinding(GameObject StartPosUnit, GameObject EndPosUnit)
	{
		if (m_Basic_phase != BATTLE_PHASE.PHASE_COMBAT)
			return false;

		m_OpenNodes.Clear();
		m_CloseNodes.Clear();
		m_Path.Clear();

		var Tiles = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
		HYJ_Battle_Tile StartTile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER, StartPosUnit);
		int StartIndex = StartTile.GraphIndex;
		HYJ_Battle_Tile EndTile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER, EndPosUnit);
		int EndIndex = EndTile.GraphIndex;

		// 잘못된 인덱스일 때 길찾기 실패.
		if (0 > StartIndex || 0 > EndIndex)
			return false;

		// 시작점 도착점 같으면 길찾기 필요없음
		if (StartIndex == EndIndex)
			return false;

		// 도착점의 Neighbor가 없으면 못감
		if ( 0 == m_Graph[EndIndex].m_Neighbors.Count)
			return false;

		// 도착점의 인접노드들 탐색해서 위에 유닛이없으면 이동 가능
		foreach (var Neighbor in m_Graph[EndIndex].m_Neighbors)
		{
			if (null == Neighbor.Tile.HYJ_Basic_onUnit)
			{
				if( true == FindingPath(StartIndex, EndIndex))
				{
					MakePath(StartIndex, EndIndex);

					return true;
				}
			}
		}

		return false;
		
	}

	bool FindingPath(int startIdx, int endIdx)
	{


		return false;
	}

	void MakePath(int startIdx, int goalIdx)
	{
		
	}

}

[System.Serializable]
public class NODE
{
	int m_MyIndex = 0;
	int m_ParentIndex = 0;
	HYJ_Battle_Tile m_tile = null;
	public HYJ_Battle_Tile Tile { get { return m_tile; } set { m_tile = value; } }
	float m_Fcost = 0.0f;
	public float Fcost { get { return m_Fcost; } set { m_Fcost = value; } }
	float m_Gcost = 0.0f;
	public float Gcost { get { return m_Gcost; } set { m_Gcost = value; } }

	Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);

	public NODE(int Idx, Vector3 Pos, HYJ_Battle_Tile tile)
	{
		m_MyIndex = Idx;
		m_Position = Pos;
		m_tile = tile;
		m_Neighbors = new List<NODE>();
	}

	// 인접 노드들
	public List<NODE> m_Neighbors;
}