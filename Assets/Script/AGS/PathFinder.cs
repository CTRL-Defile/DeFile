using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    PriorityQueue<int> PQ = new PriorityQueue<int>();

    [SerializeField]
    Transform HostObjTrans;

    // Start is called before the first frame update
    void Start()
    {
        HostObjTrans = GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update()
    {

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
        for (int y = 0; y < IdxY; ++y)
        {
            for (int x = 0; x < IdxX; ++x)
            {
                open[y, x] = Int32.MaxValue;
            }
        }

        // 시작점 발견 ( 예약 진행 )
        //open[]
    }

}
