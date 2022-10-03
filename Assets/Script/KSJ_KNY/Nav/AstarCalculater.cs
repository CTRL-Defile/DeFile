using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarCalculater : MonoBehaviour
{
    static AstarCalculater s_instance;
    public static AstarCalculater Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<AstarCalculater>();
            }
            return s_instance;
        }
    }
    
    struct PQNode : IComparable<PQNode>   // priorityQueue에 들어갈 노드
    {
        public int F;
        public int G;
        public int Z;
        public int X;

        public int CompareTo(PQNode other)
        {
            if (F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }

    // 상좌하우 예약하기 위한 배열
    // U L D R UL DL DR UR
    int[] deltaZ = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
    int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
    int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 }; // U L D R UL DL DR UR로 가는 비용

    bool[,] closed = new bool[15, 15];
    int[,] open = new int[15, 15];
    IntVector2[,] parent = new IntVector2[15, 15];

    PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();

    int nextX;
    int nextZ;

    int g;
    int h;

    int destX;
    int destZ;

    IntVector2 pos;

    List<IntVector2> _points = new List<IntVector2>();


    private void Awake()
    {
        Instance.Init();
    }
    private void Init(){}


    public List<IntVector2> FindAstar(int posX, int posZ, int destX, int destZ)
    {
        for (int x = 0; x < TileManager.Instance.boardSize; x++)
        {
            for (int z = 0; z < TileManager.Instance.boardSize; z++)
            {
                closed[x, z] = false;
                open[x, z] = int.MaxValue;
                parent[x, z].x = x;
                parent[x, z].z = z;
            }
        }

        pq.Clear();
        
        // 시작점 발견 (예약 진행)
        open[posX, posZ] = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX));
        pq.Push(new PQNode() { F = 10 * (Math.Abs(destZ - posZ) + Math.Abs(destX - posX)), G = 0, Z = posZ, X = posX });
        parent[posX, posZ] = new IntVector2(posX, posZ);

        while (pq.Count > 0)
        {
            // 제일 좋은 후보를 찾는다.
            PQNode node = pq.Pop();

            // 방문한다.
            closed[node.X, node.Z] = true;

            // 목적지 도착했으면 바로종료
            if (node.Z == destZ && node.X == destX)
                break;

            // 상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)한다.
            for (int i = 0; i < deltaZ.Length; i++)
            {
                nextX = node.X + deltaX[i];
                nextZ = node.Z + deltaZ[i];

                // 유효범위를 벗어났으면 스킵
                if (nextX < 0 || nextX >= TileManager.Instance.boardSize || nextZ < 0 || nextZ >= TileManager.Instance.boardSize)
                    continue;

                // 이미 방문한 곳이라면 스킵
                if (closed[nextX, nextZ])
                    continue;

                // 벽으로 막혀서 갈 수 없으면 Close 선언 후 스킵
                if (TileManager.Instance.Tiles[nextX, nextZ].Equals(Define.TileType.Wall))
                {
                    closed[nextX, nextZ] = true;
                    continue;
                }

                // 자리에 유닛이 있고, 그 유닛에 타겟이 위치하고 있지 않다면 Close 선언 후 스킵
                if (TileManager.Instance.Tiles[nextX, nextZ].Equals(Define.TileType.InUnit) && (nextZ != destZ || nextX != destX))
                {
                    closed[nextX, nextZ] = true;
                    continue;
                }


                // 비용계산
                g = node.G + cost[i];
                h = 10 * (Math.Abs(destZ - nextZ) + Math.Abs(destX - nextX));

                // 그런데 다른 경로에서 더 빠른길 이미 찾았으면 스킵한다.
                if (open[nextX, nextZ] < g + h)
                    continue;

                // 예약 진행
                open[nextX, nextZ] = g + h;
                pq.Push(new PQNode() { F = g + h, G = g, Z = nextZ, X = nextX });
                parent[nextX, nextZ] = new IntVector2(node.X, node.Z);
            }
        }

        return CalcPathFromParent(parent, destZ, destX);
    }
    List<IntVector2> CalcPathFromParent(IntVector2[,] parent, int destZ, int destX)
    {
        _points.Clear();        

        this.destZ = destZ;
        this.destX = destX;

        while (parent[this.destX, this.destZ].z != this.destZ || parent[this.destX, this.destZ].x != this.destX)
        {
            _points.Add(new IntVector2(this.destX, this.destZ));
            pos = parent[this.destX, this.destZ];

            this.destZ = pos.z;
            this.destX = pos.x;
        }
        _points.Add(new IntVector2(this.destX, this.destZ));
        _points.Reverse();

        //return _unit.SetNextPath(_points);
        return _points;
    }
}
