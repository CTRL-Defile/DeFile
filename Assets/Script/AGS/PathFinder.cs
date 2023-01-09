using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro.Examples;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking.Types;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PathFinder : MonoBehaviour
{	       
    [SerializeField]
    GameObject m_HostObj;

	[SerializeField] 
    BATTLE_PHASE m_Basic_phase;

	[SerializeField]
	private HYJ_Battle_Tile m_CurrentTile = null;
	[SerializeField]
	private HYJ_Battle_Tile m_DestTile = null;
	public HYJ_Battle_Tile DestTile { get { return m_DestTile; } }

	//[SerializeField]
	private NODE m_StartNode = null;
	//[SerializeField]
	private NODE m_DestNode = null;	

	[SerializeField]
	private NODE PreNode = null;

	//[SerializeField]
	SerialList<NODE> m_OpenNodes = new SerialList<NODE>();
	//[SerializeField]
	SerialList<NODE> m_CloseNodes = new SerialList<NODE>();

	[SerializeField]
	NODE m_NearNode = null;

	bool m_IsArrived = true;	 
	public bool Arrived { get { return m_IsArrived;} set { m_IsArrived = value;} }

	//[SerializeField]
	SerialLinkedList<int> m_FinalPath= new SerialLinkedList<int>();
	public SerialLinkedList<int> FinalPath { get { return m_FinalPath; } }

	// Start is called before the first frame update
	void Start()
    {
		m_HostObj = gameObject;
		m_Basic_phase = 0;		
	}

	private void LateUpdate()
	{
		m_Basic_phase = (BATTLE_PHASE)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE);		
	}

	public void MoveOnPath()
	{
		if (gameObject.GetComponent<Character>().State == Character.STATE.SKILL ||
			gameObject.GetComponent<Character>().State == Character.STATE.SKILL_IDLE)
			return;

		if( m_FinalPath.m_List.Count == 0 ||
			null == gameObject.GetComponent<Character>().Target)
		{
			gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
			return;
		}

		int DestIdx = m_FinalPath.m_List.First();
		m_DestTile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_IN_GRAPH, DestIdx);		
		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		if (false == BattleGraph[DestIdx].Marking)
		{
			BattleGraph[m_DestTile.GraphIndex].Marking = true;
			BattleGraph[m_DestTile.GraphIndex].Unit = gameObject;			
		}
		else if(true == BattleGraph[DestIdx].Marking && BattleGraph[m_DestTile.GraphIndex].Unit != gameObject)
		{
			m_IsArrived = true;
			gameObject.GetComponent<Character>().State = Character.STATE.SKILL;
			return;
		}

		gameObject.transform.LookAt(m_DestTile.transform);
		Vector3 Dir = m_DestTile.Tile_Position - gameObject.transform.position;
		Dir.Normalize();		

		float MoveLength = Vector3.Magnitude(gameObject.transform.position - m_DestTile.Tile_Position);
		if(0.1f >= MoveLength)
		{
			m_IsArrived = true;
			gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(m_DestTile.gameObject);			
			BattleGraph[m_FinalPath.m_List.First()].Marking = false;
			BattleGraph[m_FinalPath.m_List.First()].Unit = null;
			BattleGraph[m_FinalPath.m_List.First()].Tile.HYJ_Basic_onUnit = null;
			m_StartNode = BattleGraph[m_FinalPath.m_List.First()];
			m_FinalPath.RemoveFirst();			
			return;
		}

		m_IsArrived = false;
		m_DestNode = BattleGraph[DestIdx];		
		gameObject.GetComponent<Character>().State = Character.STATE.RUN;
		transform.position = Vector3.MoveTowards(transform.position, m_DestTile.Tile_Position, gameObject.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime);

	}

	public bool StartPathFinding(int StartIdx, int EndIdx)
	{
		// 타일에서 타일로 이동중이면 리턴 -> 이미 정해진 경로에서 타일 이동중에 다시 길을 찾으면 타일이동이 아니라 일반적인 이동할 것 예상
		if (false == m_IsArrived)
			return false;
		
		// 잘못된 인덱스면 리턴
		if(StartIdx < 0 || EndIdx < 0) 
			return false;		

		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		// 상태가 공격중이면 리턴
		if (gameObject.GetComponent<Character>().State == Character.STATE.SKILL ||
			gameObject.GetComponent<Character>().State == Character.STATE.SKILL_IDLE)
		{
			BattleGraph[StartIdx].Marking = true;
			BattleGraph[StartIdx].Unit = gameObject;
			gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(BattleGraph[StartIdx].Tile.gameObject);
			m_FinalPath.ClearList();
			return false;
		}

		// 이미 도착지 이웃이 시작지점이면 리턴
		foreach (var Neighbor in BattleGraph[EndIdx].m_Neighbors.m_List)
		{
			if (BattleGraph[StartIdx].MyIndex == Neighbor.MyIndex)
			{
				BattleGraph[StartIdx].Marking = true;
				BattleGraph[StartIdx].Unit = gameObject;
				gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(BattleGraph[StartIdx].Tile.gameObject);
				gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
				m_FinalPath.ClearList();				
				return false;
			}				
		}

		// 그래프가 없거나 도착지점 이웃이 없다면 길찾기 실패
		if (BattleGraph.Count == 0 || BattleGraph[EndIdx].m_Neighbors.m_List.Count == 0)
			return false;

		// 길 찾기 전 초기화
		m_FinalPath.ClearList();
		m_OpenNodes.ClearList();
		m_CloseNodes.ClearList();
		
		NODE StartNode = BattleGraph[StartIdx];
		NODE DestNode = BattleGraph[EndIdx];		

		// 인자로 들어온 EndIdx 는 Target이 서있는 위치의 그래프 Idx이다. 따라서 Neighbor중 현재 위치에서 가장 가깝고 위에 객체가 없는 Neighbor를 찾아줘야함
		// + 이미 도착지로 정해진 위치에 마킹해서 도착지점이 겹치지 않게 함
		// + 나중에 Path Node 개수 저장해서 더 빨리 도착하는 객체가 도착지점 선점하고 도착지점 갱신하는 작업 할 예정
		float MinDist = float.MaxValue;
		m_NearNode = null;

		//타겟하고 가까워졌을때를 체크하기위해서 거리계산
		float TargetDist = Vector3.Magnitude(gameObject.transform.position - gameObject.GetComponent<Character>().Target.transform.position);

		foreach (var Neighbor in BattleGraph[EndIdx].m_Neighbors.m_List)
		{
			if (null != Neighbor.Tile.HYJ_Basic_onUnit)
				continue;

			// TODO : 매직넘버로 5.0f 해놨는데 나중에 타일간 거리 계산 기반으로 값 바꿀 예정
			if(TargetDist < 5.0f &&
				true == Neighbor.Marking &&
				Neighbor.Unit == gameObject.GetComponent<Character>().Target)
			{
				gameObject.GetComponent<Character>().State = Character.STATE.SKILL_IDLE;
				m_NearNode = null;
				return false;
			}
			else if (true == Neighbor.Marking)
				continue;

			float Dist = Vector3.Magnitude(Neighbor.Position - StartNode.Position);
			if (MinDist > Dist)
			{
				MinDist = Dist;
				m_NearNode = Neighbor;
			}
		}

		// 타겟의 Neighbor중 갈 수 있는 노드가 없으면 길찾기 안하고 IDLE상태로 전환
		if(m_NearNode == null)
		{
			gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
			return false;
		}

		// A* 알고리즘으로 길 찾고 FinalPath 생성
		if (true == FindingPath(StartIdx, m_NearNode.MyIndex))
		{
			MakePath(StartIdx, m_NearNode.MyIndex);			
			m_StartNode = StartNode;
		}

		return true;		
	}

	void MakePath(int StartIdx, int EndIdx)
	{
		int ParentIndex = EndIdx;

		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		while (true)
		{
			if (ParentIndex == StartIdx)
				break;

			////////////////////////////////
			m_FinalPath.AddFirst(ParentIndex);
			ParentIndex = BattleGraph[ParentIndex].ParentIndex;
		}

	}

	bool FindingPath(int startIdx, int endIdx)
	{
		if (m_OpenNodes.m_List.Count != 0)
		{
			m_OpenNodes.m_List.RemoveAt(0);
		}

		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		m_CloseNodes.m_List.Add(BattleGraph[startIdx]);

		//객체가 위에 있으면 클로즈 리스트에 추가
		int size = BattleGraph.Count;
		for(int i = 0; i < size; i++)
		{
			if (null != BattleGraph[i].Tile.HYJ_Basic_onUnit)
				m_CloseNodes.m_List.Add(BattleGraph[i]);

			if (true == BattleGraph[i].Marking)
				m_CloseNodes.m_List.Add(BattleGraph[i]);
		}

		foreach (var Neighbor in BattleGraph[startIdx].m_Neighbors.m_List)
		{
			//이웃이 이미 도착지라면 길찾기 종료.
			if (endIdx == Neighbor.MyIndex)
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
		if (m_OpenNodes.m_List.Count == 0)
			return false;

		// 오픈을 토탈비용 Fcost 기준으로 오름차순 정렬		
		m_OpenNodes.m_List = m_OpenNodes.m_List.OrderBy(x => x.Fcost).ToList();

		// 재귀함수를 사용해서 DFS 구현
		return FindingPath(m_OpenNodes.m_List[0].MyIndex, endIdx);		
	}

	bool CheckExistInClose(NODE node)
	{
		var FindNode = m_CloseNodes.m_List.Find(x => x == node);

		if (FindNode == null)
			return false;
		else
			return true;
	}

	void InsertNodeInOpen(NODE node, int startIdx, int EndIdx)
	{
		var Findnode = m_OpenNodes.m_List.Find(x => x == node);
		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);
		// 오픈에 존재하지 않을 때
		if (Findnode == null)
		{
			// 지금까지 지나온 노드 간의 거리 추적
			node.Gcost = BattleGraph[startIdx].Gcost + Vector3.Magnitude(BattleGraph[startIdx].Position - node.Position);

			// 목적지와의 거리 ( 휴리스틱 추정 값)
			float Hcost = Vector3.Magnitude(BattleGraph[EndIdx].Position - node.Position);

			// 토탈 비용
			node.Fcost = node.Gcost + Hcost;

			// 오픈에 넣기 전 부모 인덱스 저장.
			node.ParentIndex = startIdx;

			m_OpenNodes.m_List.Add(node);
		}
		else // 오픈에 이미 존재하는데 새로 구한 비용이 이전 비용보다 더 작으면 갱신.
		{
			float Gcost = BattleGraph[startIdx].Gcost + Vector3.Magnitude(BattleGraph[startIdx].Position - node.Position);
			float Hcost = Vector3.Magnitude(BattleGraph[EndIdx].Position - node.Position);
			float Fcost = Gcost + Hcost;

			if (Findnode.Fcost > Fcost)
			{
				Findnode.Fcost = Fcost;
				Findnode.ParentIndex = startIdx;
			}
		}
	}


	public void InitCloseNodes()
	{		
		m_CloseNodes.ClearList();
	}

	public void InitMarking()
	{
		if (null == m_DestNode)
		{
			m_StartNode.Marking = false;
			return;
		}
		if(null == m_StartNode)
		{
			m_DestNode.Marking = false;
			return;
		}

		m_DestNode.Marking = false;
		m_StartNode.Marking = false;
	}

}

[System.Serializable]
public class NODE
{
	public NODE(int Idx, Vector3 Pos, HYJ_Battle_Tile tile)
	{
		m_MyIndex = Idx;
		m_Position = Pos;
		m_tile = tile;		
		m_Marking = false;
		m_Neighbors = new SerialList<NODE>();
	}

	[SerializeField]
	private int m_MyIndex = 0;
	public int MyIndex { get { return m_MyIndex; } set { m_MyIndex = value; } }

	[SerializeField]
	private int m_ParentIndex = 0;
	public int ParentIndex { get { return m_ParentIndex; } set { m_ParentIndex = value; } }

	[SerializeField]
	private HYJ_Battle_Tile m_tile = null;
	public HYJ_Battle_Tile Tile { get { return m_tile; } set { m_tile = value; } }

	[SerializeField]
	private float m_Fcost = 0.0f;
	public float Fcost { get { return m_Fcost; } set { m_Fcost = value; } }

	[SerializeField]
	private float m_Gcost = 0.0f;
	public float Gcost { get { return m_Gcost; } set { m_Gcost = value; } }

	[SerializeField]
	bool m_Marking = false;
	public bool Marking { get { return m_Marking; } set { m_Marking = value; } }

	[SerializeField]
	GameObject m_Unit = null;
	public GameObject Unit { get { return m_Unit; } set { m_Unit = value; } }
	
	[SerializeField]
	private Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);
	public Vector3 Position { get { return m_Position; } set { m_Position = value; } }

	public SerialList<NODE> m_Neighbors;

}

[System.Serializable]
public class SerialList<T>
{
	public SerialList()
	{
		m_List = new List<T>();
	}

	public void ClearList()
	{
		m_List.Clear();		
	}

	[SerializeField]
	public List<T> m_List;
}

[System.Serializable]
public class SerialLinkedList<T>
{
	public SerialLinkedList()
	{
		m_List = new LinkedList<T>();
		m_ShowList = new List<T>();
	}

	public void ClearList()
	{
		m_List.Clear();
		m_ShowList.Clear();
	}

	public void RemoveFirst()
	{		
		m_ShowList.Remove(m_List.First());
		m_List.RemoveFirst();		
	}

	public void AddFirst(T val)
	{
		m_List.AddFirst(val);
		m_ShowList.Insert(0, val);
	}
	
	public LinkedList<T> m_List;

	[SerializeField]
	public List<T> m_ShowList;
}

[System.Serializable]
public class DicPath
{
	public DicPath()
	{
		Path = new Dictionary<int, int>();
		Keys = new SerialList<int>();
		Values = new SerialList<int>();
	}

	public void AscendingOrder()
	{
		Path = Path.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
	}	

	public void AddPath(int key, int value)
	{
		Path.Add(key, value);
		Keys.m_List.Add(key);
		Values.m_List.Add(value);
	}

	public void ClearPath()
	{
		Path.Clear();
		Keys.ClearList();
		Values.ClearList();
	}
		
	public Dictionary<int, int> Path;

	[SerializeField]
	SerialList<int> Keys;
	[SerializeField]
	SerialList<int> Values;
}