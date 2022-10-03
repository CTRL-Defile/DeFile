using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AutoBattles
{
    public class MyCustomGoldManager : GoldManager
    {
        public override void GainGold(int amount)
        {
            //Do something before the base call

            //Gain an extra gold every time this is called
            CurrentGold += 1;

            base.GainGold(amount); //or remove this entirely and insert your own completely custom code

            //Do something after the base call
             
        }
    }
}

