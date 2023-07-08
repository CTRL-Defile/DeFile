using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
//using TMPro.EditorUtilities;
using TMPro.Examples;
using Unity.Mathematics;
//using UnityEditor.Experimental.GraphView;
//using UnityEditor.PackageManager.Requests;
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

	[SerializeField]
	bool m_IsArrived = true;
	public bool Arrived { get { return m_IsArrived; } set { m_IsArrived = value; } }

	[SerializeField]
	SerialLinkedList<int> m_FinalPath = new SerialLinkedList<int>();
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
		{
			m_IsArrived = true;
			return;
		}

		if (m_FinalPath.m_List.Count == 0 ||
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
		else if (true == BattleGraph[DestIdx].Marking && BattleGraph[m_DestTile.GraphIndex].Unit != gameObject)
		{
			m_IsArrived = true;
			gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
			return;
		}

		gameObject.transform.LookAt(m_DestTile.transform);
		Vector3 Dir = m_DestTile.Tile_Position - gameObject.transform.position;
		Dir.Normalize();

		float MoveLength = Vector3.Magnitude(gameObject.transform.position - m_DestTile.Tile_Position);
		if (0.1f >= MoveLength)
		{
			m_IsArrived = true;
			gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(m_DestTile.gameObject);
			BattleGraph[m_FinalPath.m_List.First()].Marking = false;
			BattleGraph[m_FinalPath.m_List.First()].Unit = null;
			//BattleGraph[m_FinalPath.m_List.First()].Tile.HYJ_Basic_onUnit = null;
			m_StartNode = BattleGraph[m_FinalPath.m_List.First()];
			m_FinalPath.RemoveFirst();

			// �̵� ������ �� ������ ��Ÿ� ������ Check Range
			CheckRange();

			return;
		}

		m_IsArrived = false;
		m_DestNode = BattleGraph[DestIdx];
		gameObject.GetComponent<Character>().State = Character.STATE.RUN;
		transform.position = Vector3.MoveTowards(transform.position, m_DestTile.Tile_Position, gameObject.GetComponent<Character>().Stat_MoveSpeed * Time.deltaTime);

	}

	public bool StartPathFinding(int StartIdx, int EndIdx)
	{
		// Ÿ�Ͽ��� Ÿ�Ϸ� �̵����̸� ���� -> �̹� ������ ��ο��� Ÿ�� �̵��߿� �ٽ� ���� ã���� Ÿ���̵��� �ƴ϶� �Ϲ����� �̵��� �� ����
		if (false == m_IsArrived)
			return false;

		// �߸��� �ε����� ����
		if (StartIdx < 0 || EndIdx < 0)
			return false;

		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		// ���°� �������̸� ����
		if (gameObject.GetComponent<Character>().State == Character.STATE.SKILL ||
			gameObject.GetComponent<Character>().State == Character.STATE.SKILL_IDLE)
		{
			BattleGraph[StartIdx].Marking = true;
			BattleGraph[StartIdx].Unit = gameObject;
			gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(BattleGraph[StartIdx].Tile.gameObject);
			//m_FinalPath.ClearList();
			return false;
		}

		// �̹� ������ �̿��� ���������̸� ����
		foreach (var Neighbor in BattleGraph[EndIdx].m_Neighbors.m_List)
		{
			if (BattleGraph[StartIdx].MyIndex == Neighbor.MyIndex)
			{
				BattleGraph[StartIdx].Marking = true;
				BattleGraph[StartIdx].Unit = gameObject;
				gameObject.GetComponent<Character>().LSY_Character_Set_OnTile(BattleGraph[StartIdx].Tile.gameObject);
				gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
				//m_FinalPath.ClearList();				
				return false;
			}
		}

		// �׷����� ���ų� �������� �̿��� ���ٸ� ��ã�� ����
		if (BattleGraph.Count == 0 || BattleGraph[EndIdx].m_Neighbors.m_List.Count == 0)
			return false;

		// �� ã�� �� �ʱ�ȭ
		m_FinalPath.ClearList();
		m_OpenNodes.ClearList();
		m_CloseNodes.ClearList();

		NODE StartNode = BattleGraph[StartIdx];
		NODE DestNode = BattleGraph[EndIdx];

		// ���ڷ� ���� EndIdx �� Target�� ���ִ� ��ġ�� �׷��� Idx�̴�. ���� Neighbor�� ���� ��ġ���� ���� ������ ���� ��ü�� ���� Neighbor�� ã�������
		// + �̹� �������� ������ ��ġ�� ��ŷ�ؼ� ���������� ��ġ�� �ʰ� ��
		// + ���߿� Path Node ���� �����ؼ� �� ���� �����ϴ� ��ü�� �������� �����ϰ� �������� �����ϴ� �۾� �� ����
		float MinDist = 10000.0f;
		m_NearNode = null;

		//Ÿ���ϰ� ������������� üũ�ϱ����ؼ� �Ÿ����
		float TargetDist = Vector3.Magnitude(gameObject.transform.position - gameObject.GetComponent<Character>().Target.transform.position);

		foreach (var Neighbor in BattleGraph[EndIdx].m_Neighbors.m_List)
		{
			//// TODO : �����ѹ��� 5.0f �س��µ� ���߿� Ÿ�ϰ� �Ÿ� ��� ������� �� �ٲ� ����
			if (TargetDist < 5.0f &&
				true == Neighbor.Marking &&
				Neighbor.Unit == gameObject.GetComponent<Character>().Target)
			{
				gameObject.GetComponent<Character>().State = Character.STATE.SKILL_IDLE;
				m_NearNode = null;
				return false;
			}
			else if (true == Neighbor.Marking)
				continue;

			if (null != Neighbor.Tile.HYJ_Basic_onUnit)
				continue;

			float Dist = Vector3.Magnitude(Neighbor.Position - StartNode.Position);
			if (MinDist > Dist)
			{
				MinDist = Dist;
				m_NearNode = Neighbor;
			}
		}

		// Ÿ���� Neighbor�� �� �� �ִ� ��尡 ������ ��ã�� ���ϰ� IDLE���·� ��ȯ		
		if (m_NearNode == null)
		{
			gameObject.GetComponent<Character>().State = Character.STATE.IDLE;
			return false;
		}

		// A* �˰������� �� ã�� FinalPath ����
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

		BattleGraph[m_FinalPath.m_List.First()].Marking = true;
		BattleGraph[m_FinalPath.m_List.First()].Unit = gameObject;
	}

	bool FindingPath(int startIdx, int endIdx)
	{
		if (m_OpenNodes.m_List.Count != 0)
		{
			m_OpenNodes.m_List.RemoveAt(0);
		}

		List<NODE> BattleGraph = (List<NODE>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH);

		m_CloseNodes.m_List.Add(BattleGraph[startIdx]);

		//��ü�� ���� ������ Ŭ���� ����Ʈ�� �߰�
		int size = BattleGraph.Count;
		for (int i = 0; i < size; i++)
		{
			if (null != BattleGraph[i].Tile.HYJ_Basic_onUnit)
				m_CloseNodes.m_List.Add(BattleGraph[i]);

			if (true == BattleGraph[i].Marking)
				m_CloseNodes.m_List.Add(BattleGraph[i]);
		}

		foreach (var Neighbor in BattleGraph[startIdx].m_Neighbors.m_List)
		{
			//�̿��� �̹� ��������� ��ã�� ����.
			if (endIdx == Neighbor.MyIndex)
			{
				Neighbor.ParentIndex = startIdx;
				return true;
			}

			// �̹� Ŭ��� ��������� ���¿� ���� �ʿ� ����
			if (CheckExistInClose(Neighbor))
				continue;

			// ���¿� �ִ´�.
			InsertNodeInOpen(Neighbor, startIdx, endIdx);
		}

		// ������ ��������� ���̻� �� �� ����
		if (m_OpenNodes.m_List.Count == 0)
			return false;

		// ������ ��Ż��� Fcost �������� �������� ����		
		m_OpenNodes.m_List = m_OpenNodes.m_List.OrderBy(x => x.Fcost).ToList();

		// ����Լ��� ����ؼ� DFS ����
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
		// ���¿� �������� ���� ��
		if (Findnode == null)
		{
			// ���ݱ��� ������ ��� ���� �Ÿ� ����
			node.Gcost = BattleGraph[startIdx].Gcost + Vector3.Magnitude(BattleGraph[startIdx].Position - node.Position);

			// ���������� �Ÿ� ( �޸���ƽ ���� ��)
			float Hcost = Vector3.Magnitude(BattleGraph[EndIdx].Position - node.Position);

			// ��Ż ���
			node.Fcost = node.Gcost + Hcost;

			// ���¿� �ֱ� �� �θ� �ε��� ����.
			node.ParentIndex = startIdx;

			m_OpenNodes.m_List.Add(node);
		}
		else // ���¿� �̹� �����ϴµ� ���� ���� ����� ���� ��뺸�� �� ������ ����.
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
		if (null == m_DestNode && null == m_StartNode)
			return;

		if (null == m_DestNode)
		{
			m_StartNode.Marking = false;
			return;
		}
		if (null == m_StartNode)
		{
			m_DestNode.Marking = false;
			return;
		}

		m_DestNode.Marking = false;
		m_StartNode.Marking = false;
	}
	public void InitPathFinder()
	{
		m_CloseNodes.ClearList();
		m_OpenNodes.ClearList();
		m_FinalPath.ClearList();
		m_CurrentTile = null;
		m_DestTile = null;
		m_DestNode = null;
		m_NearNode = null;
		m_IsArrived = true;
		m_StartNode = null;
	}

	public void CheckRange()
	{
		// ��Ÿ� ���� ���� ����üũ�ؼ� ���� ������ �⺻ �������� �Ϲݰ������� ���� �� Spell������ ���� ��Ÿ� ������ �̵�.

		Character ThisUnit = GetComponent<Character>();
		// 1. ����üũ
		if (ThisUnit.Stat_MP < ThisUnit.Stat_MaxMP)
		{
			int range = (int)ThisUnit.Spell_0.HYJ_Data_range;

			if (range == 1)
				--range;

			if (m_FinalPath.m_List.Count == 0)
				return;

			// spell_0 ����
			if (GetComponent<Character>().Target != null &&
				m_FinalPath.m_List.Count <= range)
			{
				if(m_StartNode != null)
				{
					m_StartNode.Marking = true;
					m_StartNode.Unit = gameObject;
				}
				ThisUnit.State = Character.STATE.SKILL_IDLE;
				ThisUnit.InRange = true;
			}

		}
		else if (ThisUnit.Stat_MP >= ThisUnit.Stat_MaxMP)
		{
			//spell_1 ����
		}
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
    public SerialList(int capacity)
    {
        m_List = new List<T>(capacity);
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