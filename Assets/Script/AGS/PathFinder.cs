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

	// ���¸���Ʈ�� �ִ� ������ �߿��� ���� ���� �ĺ��� ������ �̾ƿ������� �����̳� PriorityQueue ���� ����
		

	// Start is called before the first frame update
	void Start()
    {
		m_HostObj = gameObject;
		m_Basic_phase = 0;
		SetupGraph();
		MoveOnPath(null);
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
			//AStar();						
		}
	}

	void AStar()
    {				
		// Score
		// F = G + H
		// F = ���� ���� ( ���� ���� ���� ��ο� ���� �ٸ� )
		// G = ���������� �ش� ��ǥ���� �̵��ϴµ� ��� ��� ( ���� ���� ���� ��ο� ���� �ٸ� )
		// H = ���������� �󸶳� ������� ( ���� ���� ���� ���� �� )

		// (y, x) �̹� �湮�ߴ��� ���� ( �湮 == closed )
		// �迭���ϸ� �޸� ���� ���. But ����ӵ� Up
		int IdxX = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X);
		int IdxY = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y);
        bool[,] closed = new bool[IdxX, IdxY];

        //(y, x) ���� ���� �ѹ��̶� �߰��ߴ���
        // �߰� X == MaxValue
        // �߰� O == ( F = G + H )
        int[,] open = new int[IdxX, IdxY];
        for (int y = 0; y < IdxY; y++)
        {
            for (int x = 0; x < IdxX; x++)
            {
                open[x, y] = Int32.MaxValue;
            }
        }

        // ������ �߰� ( ���� ���� )
        //open[m_CurIdxX, m_CurIdxY] = Math.Abs( , );        
	}

	void MoveOnPath(GameObject Obj)
	{
		//if (Obj == null)
		//	return;

		//if (0 == m_Path.Count)
		//	return;

		//int Index = m_Path.First();
		int Index = 0;
		var Tile = (HYJ_Battle_Tile)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_IN_GRAPH, Index);

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

			// �ະ ������ ���� ���� Index ���� ��
			TruncateVal = Math.Truncate(i / 2.0f);

			for (int j = 0; j < TileXSize; ++j)
			{
				if (i % 2 == 0 || i >= 3)
					Index = i * MinimX + j + (int)TruncateVal;				
				else
					Index = i * MinimX + j;

				if (m_Graph.Count <= Index)
					continue;

				//Debug.Log("�׷��� ���� �� Index : " + Index);
				
				// �� ��� ���� Ÿ��
				// �����ٰ� �ǿ����ٿ� Ȧ��Ÿ���� �»�� Ÿ���� ����
				if (i != 0 && (j != 0 || (j == 0 && i % 2 == 0))) // ������ �ƴҶ� && ( �ǿ��� �ƴҶ� || (  j == 0 �ε� ¦�� ���̸� �»�� �־ ����ó�� )
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - (MaximX)]);
					//Debug.Log("m_Graph[" + Index + "]�� �»�� Ÿ�� Index" + (Index - MaximX));
				}

				// �� ��� ���� Ÿ��
				if (i != 0 && ((j != TileXSize - 1) || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - (MinimX)]);
					//Debug.Log("m_Graph[" + Index + "]�� ���� Ÿ�� Index" + (Index - MinimX));
				}

				// �� ���� Ÿ��
				if (j < TileXSize - 1 )
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + 1]);
					//Debug.Log("m_Graph[" + Index + "]�� �� Ÿ�� Index" + (Index + 1));
				}

				// �� �ϴ� ���� Ÿ��			
				// �� �Ʒ��ٰ� �ǿ����� Ȧ������ �� �ϴ� Ÿ�� ����	
				if (i != TileLines.Count - 1 && (j != TileXSize - 1 || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + MaximX]);
					//Debug.Log("m_Graph[" + Index + "]�� ���ϴ� Ÿ�� Index" + (Index + MaximX));
				}

				// �� �ϴ� ���� Ÿ��
				if (i != TileLines.Count - 1 && (j != 0 || (j == 0 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index + MinimX]);
					//Debug.Log("m_Graph[" + Index + "]�� ���ϴ� Ÿ�� Index" + (Index + MinimX));
				}

				// �� ���� Ÿ��
				if (j > 0)
				{
					m_Graph[Index].m_Neighbors.Add(m_Graph[Index - 1]);
					//Debug.Log("m_Graph[" + Index + "]�� �� Ÿ�� Index" + (Index - 1));
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

		// �߸��� �ε����� �� ��ã�� ����.
		if (0 > StartIndex || 0 > EndIndex)
			return false;

		// ������ ������ ������ ��ã�� �ʿ����
		if (StartIndex == EndIndex)
			return false;

		// �������� Neighbor�� ������ ����
		if ( 0 == m_Graph[EndIndex].m_Neighbors.Count)
			return false;

		// �������� �������� Ž���ؼ� ���� �����̾����� �̵� ����
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
		if(m_OpenNodes.Count != 0)
		{
			m_OpenNodes.RemoveAt(0);
		}

		m_CloseNodes.Add(m_Graph[startIdx]);

		foreach(var Neighbor in m_Graph[startIdx].m_Neighbors)
		{
			//�̿��� �̹� ��������� ��ã�� ����.
			if(endIdx == Neighbor.MyIndex)
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
		if (m_OpenNodes.Count == 0)
			return false;

		// ������ ��Ż��� Fcost �������� �������� ����
		m_OpenNodes.OrderBy(x => x.Fcost);

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

		// ���¿� �������� ���� ��
		if(Findnode == null)
		{
			// ���ݱ��� ������ ��� ���� �Ÿ� ����
			node.Gcost = m_Graph[startIdx].Gcost + Vector3.Magnitude(m_Graph[startIdx].Position - node.Position);

			// ���������� �Ÿ� ( �޸���ƽ ���� ��)
			float Hcost = Vector3.Magnitude(m_Graph[EndIdx].Position - node.Position);

			// ��Ż ���
			node.Fcost = node.Gcost + Hcost;

			// ���¿� �ֱ� �� �θ� �ε��� ����.
			node.ParentIndex = startIdx;

			m_OpenNodes.Add(node);
		}
		else // ���¿� �̹� �����ϴµ� ���� ���� ����� ���� ��뺸�� �� ������ ����.
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

[System.Serializable]
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

	// ���� ����
	public List<NODE> m_Neighbors;
}