using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AutoBattles;

public class CustomGameManager : GameManager
{
    public override void BeginRound()
    {
        base.BeginRound();

        print("working :)");
    }
}
