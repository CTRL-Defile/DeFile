using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UIElements;

public class PathFinder : MonoBehaviour
{	       
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
	LinkedList<int> m_Path = new LinkedList<int>();

	bool m_IsArrived = false;	 
	public bool Arrived { get { return m_IsArrived;} set { m_IsArrived = value;} }

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
			
		}
	}

	public void MoveOnPath(GameObject Obj, GameObject Target)
	{
		if (Obj == null ||
			Target == null)
			return;

		if (Obj.GetComponent<Character>().State == Character.STATE.SKILL)
			return;

		if (0 == m_Path.Count)
			return;

		//현재 이동중 다음 목적지 노드
		int Index = m_Path.First();
		HYJ_Battle_Tile Tile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_IN_GRAPH, Index);
		Vector3 TilePos = Tile.Tile_Position;
		Vector3 ObjPos = Obj.transform.position;

		//도착 했을때 다음노드 선점관련 셋업 하기위한 변수
		int nextIndex = m_Path.First.Next.Value;
		HYJ_Battle_Tile nextTile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_IN_GRAPH, nextIndex);

		Vector3 Dir = TilePos - ObjPos;
		float Length = Dir.magnitude;
		Dir.Normalize();

		//Vector3 LookDir = nextTile.Tile_Position - TilePos;		
		gameObject.transform.LookAt(Tile.transform);
		
		//if(null != Tile.HYJ_Basic_onUnit)
		//	StartPathFinding(gameObject, Target);

		if (GetComponent<Character>().PreTarget != GetComponent<Character>().Target)
		{
			StartPathFinding(gameObject, Target);
			return;
		}

		if (Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime >= Length)
		{
			float Dist = Vector3.Magnitude(Obj.transform.position - Target.transform.position);
			if (Dist <= 2.5f)
			{
				Arrived = true;
				if (Character.STATE.RUN == gameObject.GetComponent<Character>().State)
					gameObject.GetComponent<Character>().State = Character.STATE.IDLE;

				nextTile.HYJ_Basic_onUnit = null;
				Tile.HYJ_Basic_onUnit = gameObject;
				return;
			}

			Obj.GetComponent<Character>().LSY_Character_Set_OnTile(Tile.gameObject);
			Tile.HYJ_Basic_onUnit = gameObject;

			if (null != nextTile.HYJ_Basic_onUnit)
			{				
				StartPathFinding(gameObject, Target);
				return;
			}

			nextTile.HYJ_Basic_onUnit = gameObject;
			Target = Obj.GetComponent<Character>().Target;

			//StartPathFinding(Obj, Target);

			m_Path.RemoveFirst();
				
			return;
		}

		//ObjPos.x = Mathf.Lerp(ObjPos.x, TilePos.x, Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime);
		//ObjPos.z = Mathf.Lerp(ObjPos.z, TilePos.z, Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime);
		//ObjPos.y = 0.0f;
		
		// 이동가능하다면 객체 Run 상태로
		Obj.GetComponent<Character>().State = Character.STATE.RUN;

		ObjPos += Dir * Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime;
		Obj.transform.position = ObjPos;
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
		if (StartPosUnit == null ||
			EndPosUnit == null)
			return false;

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
		// 인접 노드 중에 가장 가까운 노드부터 Select 하는 로직 추가해야함.
		foreach (var Neighbor in m_Graph[EndIndex].m_Neighbors)
		{
			if (null == Neighbor.Tile.HYJ_Basic_onUnit)
			{
				if( true == FindingPath(StartIndex, Neighbor.MyIndex))
				{					
					MakePath(StartIndex, Neighbor.MyIndex);
					Arrived = false;
					return true;
				}
			}
		}

		return false;		
	}

	bool FindingPath(int startIdx, int endIdx)
	{
		if(m_OpenNodes.Count != 0)
		{
			m_OpenNodes.RemoveAt(0);
		}

		m_CloseNodes.Add(m_Graph[startIdx]);

		foreach(var Neighbor in m_Graph[startIdx].m_Neighbors)
		{
			//이웃이 이미 도착지라면 길찾기 종료.
			if(endIdx == Neighbor.MyIndex)
			{
				Neighbor.ParentIndex = startIdx;
				return true;
			}

			// 이미 클로즈에 들어있으면 오픈에 넣을 필요 없음
			if (CheckExistInClose(Neighbor))
				continue;

			// 오픈에 넣는다.
			InsertNodeInOpen(Neighbor, startIdx, endIdx);
		
		}

		// 오픈이 비어있으면 더이상 갈 길 없음
		if (m_OpenNodes.Count == 0)
			return false;

		// 오픈을 토탈비용 Fcost 기준으로 오름차순 정렬		
		m_OpenNodes = m_OpenNodes.OrderBy(x => x.Fcost).ToList();

		return FindingPath(m_OpenNodes[0].MyIndex, endIdx);
	}

	void MakePath(int StartIdx, int EndIdx)
	{
		int ParentIndex = EndIdx;		

		while(true)
		{
			if (ParentIndex == StartIdx)
				break;

			////////////////////////////////
			m_Path.AddFirst(ParentIndex);
			ParentIndex = m_Graph[ParentIndex].ParentIndex;
		}
	}

	bool CheckExistInClose(NODE node)
	{
		var FindNode = m_CloseNodes.Find(x => x == node);

		if (FindNode == null)
			return false;
		else
			return true;			
	}

	void InsertNodeInOpen(NODE node, int startIdx, int EndIdx)
	{
		var Findnode = m_OpenNodes.Find(x => x == node);

		// 오픈에 존재하지 않을 때
		if(Findnode == null)
		{
			// 지금까지 지나온 노드 간의 거리 추적
			node.Gcost = m_Graph[startIdx].Gcost + Vector3.Magnitude(m_Graph[startIdx].Position - node.Position);

			// 목적지와의 거리 ( 휴리스틱 추정 값)
			float Hcost = Vector3.Magnitude(m_Graph[EndIdx].Position - node.Position);

			// 토탈 비용
			node.Fcost = node.Gcost + Hcost;

			// 오픈에 넣기 전 부모 인덱스 저장.
			node.ParentIndex = startIdx;

			m_OpenNodes.Add(node);
		}
		else // 오픈에 이미 존재하는데 새로 구한 비용이 이전 비용보다 더 작으면 갱신.
		{
			float Gcost = m_Graph[startIdx].Gcost + Vector3.Magnitude(m_Graph[startIdx].Position - node.Position);
			float Hcost = Vector3.Magnitude(m_Graph[EndIdx].Position - node.Position);
			float Fcost = Gcost + Hcost;

			if(Findnode.Fcost > Fcost)
			{
				Findnode.Fcost = Fcost;
				Findnode.ParentIndex = startIdx;
			}
		}
	}

}

public class NODE
{
	private int m_MyIndex = 0;
	public int MyIndex { get { return m_MyIndex; } set { m_MyIndex = value; } }
	private int m_ParentIndex = 0;
	public int ParentIndex { get { return m_ParentIndex; } set { m_ParentIndex = value; } }
	private HYJ_Battle_Tile m_tile = null;
	public HYJ_Battle_Tile Tile { get { return m_tile; } set { m_tile = value; } }
	private float m_Fcost = 0.0f;
	public float Fcost { get { return m_Fcost; } set { m_Fcost = value; } }
	private float m_Gcost = 0.0f;
	public float Gcost { get { return m_Gcost; } set { m_Gcost = value; } }

	private Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 Position { get { return m_Position; } set { m_Position = value; } }	

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