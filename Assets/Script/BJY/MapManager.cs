using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static bool[] chessBoard = new bool[64];
    private static bool[] playerChessBoard = new bool[32];
    private static bool[] enemyChessBoard = new bool[32];
    private static bool[] waitingBoard = new bool[9];
    private static int waitingBoardCount = 0;
    private const float startWaitingBoardPos = -0.5f, endWaitingBoardPos = 7.5f;
    private const float startChessBoardPosX = 0.0f, startChessBoardPosZ = 7.0f, startPlayerChessBoardPosZ = 3.0f, endChessBoardPosX = 7.0f, endChessBoardPosZ = 0.0f;
   
    public static void InitBoard(){
        for(int i=0;i<64;i++){
            chessBoard[i] = false;
        }

        for(int i=0;i<4;i++){
            enemyChessBoard[i] = false;
            playerChessBoard[i] = false;
        }

        for(int i=0;i<9;i++){
            waitingBoard[i]=false;
        }
    }

    public static bool ClickPutWaitingBoard(ref Vector3 position){
        if(!IsFullWaitingBoard()){
            for(int i=0;i<9;i++){
                if(!waitingBoard[i]){
                    waitingBoard[i] = true;
                    AddWaitingBoardCount();
                    position.x = startWaitingBoardPos + i;
                    return true;
                }
            }
            return false;
        }
        else
            return false;
    }

    public static bool DragPutWaitingBoard(Vector3 position){
        int n = (int) Math.Abs(startWaitingBoardPos - position.x);
        if(waitingBoard[n])
            return false;
        waitingBoard[n] = true;
        AddWaitingBoardCount();
        return true;
    }

    public static void TakeOutWaitingBoard(Vector3 position){
        int n = (int) Math.Abs(startWaitingBoardPos - position.x);
        waitingBoard[n] = false;
        SubWaitingBoardCount();
    }
    public static bool ChangePosWaitingBoard(Vector3 initalPos, Vector3 currPos){
        int a = (int) Math.Abs(startWaitingBoardPos - initalPos.x);
        int b = (int) Math.Abs(startWaitingBoardPos - currPos.x);

        if(!waitingBoard[b]){
            waitingBoard[a] = false;
            waitingBoard[b] = true;
            return true;
        }
        return false;
    }

    public static void AddWaitingBoardCount(){
        waitingBoardCount++;
    }

    public static void SubWaitingBoardCount(){
        waitingBoardCount--;
    }

    public static bool IsFullWaitingBoard(){
        return (waitingBoardCount==9);
    }

    public static bool IsInChessBoard(Vector3 position){
        if((position.x<=endChessBoardPosX) && (position.x>=startChessBoardPosX))
            if((position.z<=startPlayerChessBoardPosZ) && (position.z>=endChessBoardPosZ))
                return true;
        return false;
    }

    public static bool IsInWaitingBoard(Vector3 position){
        if((position.z>=-2.5f) && (position.z<=-1.5f) && ((position.x<=endWaitingBoardPos) && (position.x>=startWaitingBoardPos)))
            return true;
        return false;
    }

    public static Vector3 ClearWaitingPosition(Vector3 position){
        if(position.x < startWaitingBoardPos)
            position.x += 0.5f;
        else if(position.x > endWaitingBoardPos)
            position.x -= 0.5f;
        else
            position.x -= 0.5f;

        return position;
    }

    public static bool PutChessBoard(Vector3 position){
        int n = ChessBoardPosition(position);

        if(chessBoard[n])
            return false;
        chessBoard[n] = true;
        return true;
    }

    public static void TakeOutChessBoard(Vector3 position){
        int n = ChessBoardPosition(position);
        chessBoard[n] = false;
    }


    public static bool ChangePosChessBoard(Vector3 initalPos, Vector3 currPos){
        int a = ChessBoardPosition(initalPos);
        int b = ChessBoardPosition(currPos);

        if(!chessBoard[b]){
            chessBoard[a] = false;
            chessBoard[b] = true;
            return true;
        }
        return false;
    }


    public static int ChessBoardPosition(Vector3 position){
        int x = (int) Math.Abs(startChessBoardPosX - position.x);
        int z = (int) Math.Abs(startPlayerChessBoardPosZ - position.z);
        int n = x+(z*8);

        return n;
    }


}