using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    Button levelUpBtn;
    private static int playerLevel;
    private static int playerExp;
    private static int[] expToLevelup = {2,6,10,20,36,56,80,108,140,170,190,210};
    private const int reducingMana=1;

    private static int mana;

    void Start(){
        InitPlayerInfo();

        levelUpBtn = this.transform.GetComponent<Button>();
        if(levelUpBtn != null){
            levelUpBtn.onClick.AddListener(LevelUpBtnClick);
        }
    }

    void InitPlayerInfo(){
        playerLevel=1;
        playerExp=0;
        mana=20;
    }

    public static int GetPlayerLevel(){
        return playerLevel;
    }

    public static int GetPlayerExp(){
        return playerExp;
    }

    public static int GetMana(){
        return mana;
    }
    
    void LevelUpBtnClick(){
        if(mana >= reducingMana){
            LevelUp();
            mana -= reducingMana;
        }
        else{
            Debug.Log("Mana 부족!");
        }
        Debug.Log("현재 마나: "+mana+"Exp: "+ playerExp+"level: "+ playerLevel);
    }

    void LevelUp(){
        playerExp ++;
        if(playerExp>=expToLevelup[playerLevel-1]){
            playerExp %= expToLevelup[playerLevel-1];
            playerLevel++;
            Debug.Log("level up!");
        }
    }

    public static int ReduceMana(int reducingMana){
        mana -= reducingMana;
        return mana;
    }

}
