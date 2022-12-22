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

	private NODE m_StartNode = null;
	private NODE m_DestNode = null;
	
	List<NODE> m_CloseNodes = new List<NODE>();

	bool m_IsArrived = true;	 
	public bool Arrived { get { return m_IsArrived;} set { m_IsArrived = value;} }

	Dictionary<int , int> m_PathDic = new Dictionary<int , int>();

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

	public void MoveOnPath(GameObject Obj)
	{
		if (Obj.GetComponent<Character>().State == Character.STATE.SKILL)
			return;

		if(m_PathDic.Count == 0)
			Obj.GetComponent<Character>().State = Character.STATE.IDLE;

		if (m_CurrentTile == null ||
			m_DestTile == null)
			return;

		if (m_CurrentTile == m_DestTile)
			return;

		if (null == Obj.GetComponent<Character>().Target)
			return;

		// Target과의 거리 계산
		float TargetLength = Vector3.Magnitude(Obj.GetComponent<Character>().Target.transform.position - Obj.transform.position);
		// 타일간 거리
		float TileDist = Vector3.Magnitude(m_DestTile.Tile_Position - m_CurrentTile.Tile_Position);

		// 배틀페이즈 그래프
		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		Obj.transform.LookAt(m_DestTile.transform);
		Vector3 Dir = m_DestTile.Tile_Position - m_CurrentTile.Tile_Position;
		Dir.Normalize();		

		float MoveLength = Vector3.Magnitude(Obj.transform.position - m_DestTile.Tile_Position);
		if(0.1f >= MoveLength)
		{
			m_IsArrived = true;
			Obj.GetComponent<Character>().LSY_Character_Set_OnTile(m_DestTile.gameObject);
			//m_StartNode.Marking = false;			
			return;
		}

		Obj.GetComponent<Character>().State = Character.STATE.RUN;
		//Obj.transform.position += Dir * Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime;
		Obj.transform.position = Vector3.Lerp(Obj.transform.position, m_DestTile.Tile_Position, Obj.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime);
	}

	public bool StartPathFinding(GameObject Obj, int StartIdx, int EndIdx)
	{
		if (false == m_IsArrived)
			return false;					

		// 1. 그래프에서 유닛의 현재 노드 구하기
		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		if (Obj.GetComponent<Character>().State == Character.STATE.SKILL)
		{
			BattleGraph[StartIdx].Marking = true;
			return false;
		}

		// 이미 도착지 이웃이 시작지점이면 리턴
		foreach (var Neighbor in BattleGraph[EndIdx].m_Neighbors)
		{
			if (BattleGraph[StartIdx].MyIndex == Neighbor.MyIndex)
			{				
				m_StartNode = BattleGraph[StartIdx];
				m_StartNode.Marking = true;
				m_CurrentTile = BattleGraph[StartIdx].Tile;
				Obj.GetComponent<Character>().LSY_Character_Set_OnTile(m_CurrentTile.gameObject);
				m_PathDic.Clear();
				return false;
			}				
		}

		if (BattleGraph.Count == 0)
			return false;

		m_PathDic.Clear(); 

		NODE StartNode = BattleGraph[StartIdx];
		NODE DestNode = BattleGraph[EndIdx];

		// 타겟 - 시작위치 == 가려는 방향
		Vector3 MoveDir = DestNode.Tile.Tile_Position - StartNode.Tile.Tile_Position;
		
		Vector3 NeighborDir = Vector3.zero;

		// 2. 가려는 방향과 가장 가까운 Neighbor 노드 찾기
		foreach (var Neighbor in BattleGraph[StartIdx].m_Neighbors)
		{
			// Neighbor 노드 - 시작위치 == 현재 위치에서 이웃으로 이동하려는 방향
			NeighborDir = Neighbor.Tile.Tile_Position - StartNode.Tile.Tile_Position;

			// 두 벡터간 이루는 각도			
			int Angle = (int)(Vector3.Angle(MoveDir, NeighborDir));

			// 비교해서 딕셔너리에 담음 key 값 == 두 벡터간 이루는 각도(소수점 버린 정수 값), value == 목적지 Neighbor 노드 Idx
			// 만약 이루는 각도가 45도로 같은 방향이어서 가까운노드가 2개로 잡히는 경우? 어차피 오름차순 정렬할거고 앞에서부터 뽑아서 쓸꺼라 문제될 부분 없음

			if(!m_PathDic.ContainsKey(Angle))
				m_PathDic.Add(Angle, Neighbor.MyIndex);
		}

		// 3. 키 값(각도) 기준으로 오름차순 정렬
		m_PathDic = m_PathDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x=> x.Value);

		// 4. 하나씩 꺼내서 이동 가능한 노드인지 Check		
		while(true)
		{
			if (m_PathDic.Count == 0)
				break;

			var Pair = m_PathDic.First();

			if (true == BattleGraph[Pair.Value].Marking)
			{
				m_PathDic.Remove(Pair.Key);
			}
			//else if (CheckExistInClose(BattleGraph[Pair.Value]))
			//{
			//	m_PathDic.Remove(Pair.Key);
			//}
			//else if (null != BattleGraph[Pair.Value].Tile.HYJ_Basic_onUnit)
			//{
			//	m_PathDic.Remove(Pair.Key);
			//}
			else
			{			
				m_CloseNodes.Add(BattleGraph[Pair.Value]);
				m_CurrentTile = StartNode.Tile;
				m_DestTile = BattleGraph[Pair.Value].Tile;
				m_StartNode = StartNode;
				m_DestNode = BattleGraph[Pair.Value];
				m_StartNode.Marking = false;
				m_DestNode.Marking = true;
				m_IsArrived = false;			
				break;
			}
				
		}

		return true;		
	}

	bool CheckExistInClose(NODE node)
	{
		var FindNode = m_CloseNodes.Find(x => x == node);

		if (FindNode == null)
			return false;
		else
			return true;
	}

	public void InitCloseNodes()
	{		
		m_CloseNodes.Clear();
	}

	public void InitMarking()
	{
		m_DestNode.Marking = false;
		m_StartNode.Marking = false;
	}

}

public class NODE
{
	public NODE(int Idx, Vector3 Pos, HYJ_Battle_Tile tile)
	{
		m_MyIndex = Idx;
		m_Position = Pos;
		m_tile = tile;
		m_Neighbors = new List<NODE>();
		m_Marking = false;
	}

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

	bool m_Marking = false;
	public bool Marking { get { return m_Marking; } set { m_Marking = value; } }

	public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
	private Vector3 m_Position = new Vector3(0.0f, 0.0f, 0.0f);

	// 인접 노드들
	public List<NODE> m_Neighbors;

}