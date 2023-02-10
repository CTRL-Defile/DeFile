//
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static HYJ_Battle_Tile;

public enum BATTLE_PHASE { PHASE_UPDATE = -1, PHASE_INIT, PHASE_PREPARE, PHASE_COMBAT, PHASE_COMBAT_OVER, PHASE_END };

public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [SerializeField]
	BATTLE_PHASE Basic_phase = BATTLE_PHASE.PHASE_INIT;

    double Phase_timer = 10.0;
    double Time_Acc = 0;
    bool StatusBar_isInitialized = false;
    bool CharacterPool_isInitialized = false;

    GameObject prefab;
    GameObjectPool<HYJ_Battle_Tile> m_TilePool;
    [SerializeField]
    SerialList<GameObjectPool<Character>> m_CharacterPools = new SerialList<GameObjectPool<Character>>();    
    
    GameObjectPool<Character> m_CharacterPool;
    int Max_tile = 80, Max_character = 40, Pool_cnt = 0;

    [SerializeField]
    bool _StopVar = false, _StartVar = false;

	[SerializeField]
    private Volume GameVolume;

	//////////  Getter & Setter //////////
	object HYJ_Basic_GetPhase(params object[] _args)
    {
        return Basic_phase;
    }

    //////////  Method          //////////
    object HYJ_ActiveOn(params object[] _args)
    {
        bool aa = (bool)_args[0];

        //
        this.gameObject.SetActive(aa);

        //
        return null;
    }
    public void HYJ_SetActive(bool _isActive)
    {
        Debug.Log(_isActive);
        this.gameObject.SetActive(_isActive);
        //
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.MAP___ACTIVE__ACTIVE_ON, !_isActive);
    }
    object LSY_Set_ShopUI(params object[] _args)
    {
        bool _isActive = (bool)_args[0];
        Battle_Shop.gameObject.SetActive(_isActive);
        
        return null;
    }

    void Battle_Timer()
    {
        if (Phase_timer - Time_Acc > 0.0)
            Battle_Timer_TMP.text = (((int)(Phase_timer - Time_Acc))).ToString();
	}

    //////////  Default Method  //////////
    // Start is called before the first frame update
    void Start()
    {
        // Battle_Map
        Battle_Map = transform.GetChild(0);
        element = Battle_Map.GetChild(0).gameObject;
        std_element = Battle_Map.GetChild(2).gameObject;
        trash_element = Battle_Map.GetChild(3).gameObject;
        Field_parent = Battle_Map.GetChild(4);
        Stand_parent = Battle_Map.GetChild(5);
        Trash_parent = Battle_Map.GetChild(6);

        // Battle_UI
        Battle_UI = transform.GetChild(1);
        Battle_Canvas = Battle_UI.GetChild(0);

        Battle_Shop = Battle_Canvas.GetChild(0);
        Shop_panel = Battle_Shop.GetChild(0);
        for (int i = 0; i < Shop_Panel_cnt; i++)
            Shop_UnitList.Add(Shop_panel.GetChild(0).GetChild(i).gameObject);
        Shop_Exp_Cur = Shop_panel.GetChild(2).GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>();
        Shop_Coin_Text = Shop_panel.GetChild(3).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

        Battle_Synergy = Battle_Canvas.GetChild(1);
        Synergy_Panel = Battle_Synergy.GetChild(0);
        Synergy_Red = Battle_Synergy.GetChild(1);
        Synergy_Green = Battle_Synergy.GetChild(2);
        Synergy_Gray = Battle_Synergy.GetChild(3);

        for (int i=0; i<Synergy_Red.childCount-1; i++)
        {
            Red_list.Add(Synergy_Red.GetChild(i));
            Green_list.Add(Synergy_Green.GetChild(i));
            Gray_list.Add(Synergy_Gray.GetChild(i));
        }

        Battle_Text = Battle_Canvas.GetChild(2);
        Battle_Ally_OnTile = Battle_Text.GetChild(0).GetComponent<TextMeshProUGUI>();
        Battle_Timer_TMP = Battle_Text.GetChild(1).GetComponent<TextMeshProUGUI>();

        End_Btn = Battle_Canvas.GetChild(3).gameObject;

        // Battle_Units
        Battle_Units = transform.GetChild(2);
        Unit_pool = Battle_Units.GetChild(0).transform;
        Unit_parent = Battle_Units.GetChild(1).transform;
        Enemy_parent = Battle_Units.GetChild(2).transform;
        Unit_Sacrificed = Battle_Units.GetChild(3).transform;



        //
        Basic_phase = BATTLE_PHASE.PHASE_INIT;		

		//
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___BASIC__GET_PHASE,   HYJ_Basic_GetPhase);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__ACTIVE_ON,  HYJ_ActiveOn);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___ACTIVE__SHOP_UI, LSY_Set_ShopUI);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE__SYNERGY_UPDATE, LSY_Battle_Synergy_Update);

        Battle_UI_Font();
    }

    // Update is called once per frame
    void Update()
    {
        //Shop_Coin.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_GOLD).ToString();        

        switch (Basic_phase)
        {
            case BATTLE_PHASE.PHASE_UPDATE:
                {
      //              // Init에서 잘안됐다 그러면 어드레서블 컴플리트 체크해서 로딩 끝났을때 켜져있으면 돌게 중복코드..
      //              if (this.gameObject.activeSelf == true)
      //              {
						//// battle active
						//this.gameObject.SetActive(false);
						//Basic_phase = BATTLE_PHASE.PHASE_UPDATE; // 여기는 -1 업데이트내용 다 여기서 실행해야하는 부분 //받았다고 체크가 되야함
      //              }

                    int DB_phase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_PHASE);
                    if (DB_phase == -1)
                    {
                        Basic_phase = BATTLE_PHASE.PHASE_INIT;
						// battle active
						this.gameObject.SetActive(false);
						//Basic_phase = BATTLE_PHASE.PHASE_UPDATE; // 여기는 -1 업데이트내용 다 여기서 실행해야하는 부분 //받았다고 체크가 되야함                 
                    }
                
                }
                break;
            //
            case BATTLE_PHASE.PHASE_INIT:
                {

                    int DB_phase = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_PHASE);
                    if (DB_phase != -1)
                    {
                        Basic_phase = BATTLE_PHASE.PHASE_UPDATE;
                        break;
                    }

                    if (CharacterPool_isInitialized == false)
                    {
                        Debug.Log("DB_phase : " + DB_phase);
                        LSY_Pool_Init();
                        CharacterPool_isInitialized = true;
                        HYJ_Field_Init();
                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DRAG___INIT);
                        // 필드 생성 후 그래프 셋업
					    SetupGraph();
                    }
                    else //(this.gameObject.activeSelf == true && CharacterPool_isInitialized == true)
                    {
                        Basic_phase = BATTLE_PHASE.PHASE_PREPARE;
                        End_Btn.SetActive(false);

                        if (UL_isInitialized)
                            LSY_Shop_Reload(1);
                    }
                }
                break;
			// 전투 준비
			case BATTLE_PHASE.PHASE_PREPARE:
                {
                    Phase_timer = 20.0;
					Time_Acc += Time.deltaTime;
                    Battle_Timer();
					//시간 체크 후 전투 상태로 Phase 전환
					if (Phase_timer - Time_Acc <= 0.0 && _StopVar == false )
                    {
						Basic_phase = BATTLE_PHASE.PHASE_COMBAT;
                        Time_Acc = 0.0;
					}
                    if (_StartVar == true)
                        Basic_phase = BATTLE_PHASE.PHASE_COMBAT;

                    if (!Enemy_isInitialized)
                        LSY_Enemy_Init();

                    if (!UL_isInitialized)
                    {
                        LSY_UnitList_Init();
                        LSY_Shop_Reload(1);
                    }

                    if (!StatusBar_isInitialized)
                    {
						foreach (var OBJ in Enemy_Unit)
						{
							OBJ.GetComponent<Character>().STATUS_HPBAR.SetHPColor(UI_StatusBar.STATUS_HP_COLOR.RED);
						}

						StatusBar_isInitialized = true;
					}

                    foreach (var OBJ in Stand_Unit)
                    {
                        OBJ.GetComponent<Character>().STATUS_HPBAR.SetHPColor(UI_StatusBar.STATUS_HP_COLOR.GREEN);
                    }

                }
                break;
			// 전투 상태
			case BATTLE_PHASE.PHASE_COMBAT:
                {                    
                    InitGraphNodes();
					Find_Target();

                    //여기 길찾기. 그러면 컨테이너돌면서 순서 정해줄수 있음. 정렬 -> 기준이 그래프상에서 우상단 좌하단에 가깝냐 정렬을 한번하고 순회하면서 StartPathFinding
                    UnitSorting();
                    FindingPath();

					Phase_timer = 50.0;
					Time_Acc += Time.deltaTime;
					Battle_Timer();
					//시간 체크 후 전투 상태로 Phase 전환
					if (Phase_timer - Time_Acc <= 0.0)
					{
						Basic_phase = BATTLE_PHASE.PHASE_PREPARE;
						Time_Acc = 0.0;
					}
                    if (Enemy_Unit.Count == 0 || Field_Unit.Count == 0)
                    {
                        Basic_phase = BATTLE_PHASE.PHASE_COMBAT_OVER;
                        Time_Acc = 0.0;
                    }
                }
                break;
            // 전투 끝난 상태
            case BATTLE_PHASE.PHASE_COMBAT_OVER:
                {
                    End_Btn.SetActive(true);
                    if (Enemy_Unit.Count == 0)
                        End_Btn.transform.GetChild(0).gameObject.SetActive(true);
                    //End_Btn.transform.GetComponentInChildren<TextMeshProUGUI>().text = "You Win";
                    else
                        End_Btn.transform.GetChild(1).gameObject.SetActive(true);
                    //End_Btn.transform.GetComponentInChildren<TextMeshProUGUI>().text = "You Lose";

                    DepthOfField dof;
                    if (GameVolume.profile.TryGet<DepthOfField>(out dof))
                    {
                        // dof.active = true;
                        dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, 0.95f, 3.0f * Time.deltaTime);
                    }
                }
                break;
        }
    }

	private void LateUpdate()
	{
		Shop_Coin_Text.text = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_GOLD).ToString();
	}


	private void FindingPath()
    {
		foreach (var Unit in Field_Unit)
		{
			if (null != Unit.GetComponent<Character>().Target)
				Unit.GetComponent<PathFinder>().StartPathFinding(Unit.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex,
																					Unit.GetComponent<Character>().Target.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex);
		}
		foreach (var Unit in Enemy_Unit)
		{
			if (null != Unit.GetComponent<Character>().Target)
				Unit.GetComponent<PathFinder>().StartPathFinding(Unit.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex,
																					Unit.GetComponent<Character>().Target.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex);
		}
	}
	private void UnitSorting()
    {
        //TODO : 일단 우측, 좌측 기준으로 잡았음

        //      // 1. Field 유닛들은 그래프 좌하단 기준으로 우선순위 정렬
        Field_Unit = Field_Unit.OrderBy(x => x.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex).ToList();

        //// 2. Enemy 유닛들은 그래프 우상단 기준으로 우선순위 정렬
        Enemy_Unit = Enemy_Unit.OrderBy(x => x.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex).ToList();

  //      // 3. 맵에서 가운데 라인에 가까울수록인걸 체크해야하는데..

  //      List<GameObject> TmpList = new List<GameObject>();

  //      // 3.1 Line Count의 절반 만약 8줄이면 반 값 == 4
  //      // 012(3 4)567 => -1 하고 반 값 사용
  //      string LineCnt = (Field_tiles.Count).ToString();
  //      int FieldHalfIdx = (int)Math.Round(float.Parse(LineCnt) / 2.0f);
        
  //      for(int i = Field_tiles[FieldHalfIdx].Tiles[0].GraphIndex; i < m_Graph.Count; i++)
  //      {
            
  //          if(null != m_Graph[i].Tile.HYJ_Basic_onUnit)
  //          {
  //              GameObject FindObj = m_Graph[i].Tile.HYJ_Basic_onUnit;

  //              FindObj = Field_Unit.Find(x => FindObj);

  //              if(FindObj == m_Graph[i].Tile.HYJ_Basic_onUnit)
		//		    TmpList.Add(m_Graph[i].Tile.HYJ_Basic_onUnit);
		//	}				
  //      }

  //      Field_Unit = TmpList.ToList();
  //      TmpList.Clear();

		//for (int i = Field_tiles[FieldHalfIdx-1].Tiles[Field_tiles[FieldHalfIdx - 1].Tiles.Count-1].GraphIndex; i > -1; i--)
		//{
		//	if (null != m_Graph[i].Tile.HYJ_Basic_onUnit)
		//	{
		//		GameObject FindObj = m_Graph[i].Tile.HYJ_Basic_onUnit;

		//		FindObj = Enemy_Unit.Find(x => FindObj);

		//		if (FindObj == m_Graph[i].Tile.HYJ_Basic_onUnit)
		//			TmpList.Add(m_Graph[i].Tile.HYJ_Basic_onUnit);
		//	}
		//}

		//Enemy_Unit = TmpList.ToList();
		//TmpList.Clear();

	}

	public void LSY_Battle_End()
    {
        DepthOfField dof;
        if (GameVolume.profile.TryGet<DepthOfField>(out dof))
        {
            dof.focusDistance.value = 10.0f;
            //dof.active = false;
        }

        End_Btn.SetActive(false);
        End_Btn.transform.GetChild(0).gameObject.SetActive(false);
        End_Btn.transform.GetChild(1).gameObject.SetActive(false);
        HYJ_SetActive(false);
        Basic_phase = BATTLE_PHASE.PHASE_INIT;
        StatusBar_isInitialized = false;

        Field_Unit.Clear();
        Enemy_Unit.Clear();        

        int UnitParent_num = Unit_parent.childCount;
        int EnemyParent_num = Enemy_parent.childCount;

        foreach(var node in m_Graph)
        {
            node.Tile.GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;
        }
        
        for (int i = 0; i < UnitParent_num; i++)
        {
            Unit_parent.GetChild(i).gameObject.SetActive(true);
            Unit_parent.GetChild(i).GetComponent<Character>().Dead = false;
			Unit_parent.GetChild(i).GetComponent<PathFinder>().InitPathFinder();            
			Transform tmp = Unit_parent.GetChild(i);
            if (!Stand_Unit.Contains(tmp.gameObject))
				Field_Unit.Add(tmp.gameObject);

		}
        for (int i = 0; i < EnemyParent_num; i++)
        {
            Enemy_parent.GetChild(i).gameObject.SetActive(true);
            Enemy_parent.GetChild(i).GetComponent<Character>().Dead = false;
			Enemy_parent.GetChild(i).GetComponent<PathFinder>().InitPathFinder();			
			Enemy_Unit.Add(Enemy_parent.GetChild(i).gameObject);
        }
        LSY_Unit_Init(Field_Unit);
        LSY_Unit_Init(Enemy_Unit);

        string str = "Enemy";
        LSY_Unit_to_Pool(str, Enemy_Unit);
        Enemy_isInitialized = false;
    }

    public void LSY_Unit_to_Pool(string _type, List<GameObject> unit_list)
    {
        int cnt = unit_list.Count;
        object[] objs = new object[2];
        objs[0] = _type;
        for (int i = 0; i < cnt; i++)
        {
            objs[1] = unit_list[0];
            Unit_to_Trash(objs);
        }
    }


    void LSY_Pool_Init()
    {
        m_TilePool = new GameObjectPool<HYJ_Battle_Tile>(Max_tile, (int n) =>
        {
            var obj = Instantiate(Battle_Map.GetChild(0).gameObject);
            obj.SetActive(false);
            obj.transform.SetParent(Field_parent);
            obj.transform.localScale = Vector3.one;
            var tile = obj.GetComponent<HYJ_Battle_Tile>();
            tile.tile_type = HYJ_Battle_Tile.Tile_Type.Field;
            return tile;
        });

        prefab = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_UNIT_PREFAB);
        int cnt = prefab.transform.childCount;
        Pool_cnt = Max_character / cnt;
        for (int i = 0; i < cnt; i++)
        {
            m_CharacterPool = new GameObjectPool<Character>(Pool_cnt, (int n) =>
            {
                //Debug.Log(i);
                var obj = Instantiate(prefab.transform.GetChild(i).gameObject);
                obj.SetActive(false);
                obj.transform.SetParent(Unit_pool);
                obj.transform.localScale = Vector3.one;
                var character = obj.GetComponent<Character>();
                character.m_UnitType = Character.Unit_Type.Ally;
                character.STATUS_HPBAR.SetHPColor(UI_StatusBar.STATUS_HP_COLOR.GREEN);
                character.HYJ_Status_saveData = new CTRL_Character_Data(i.ToString());

                string obj_name;
                obj_name = character.Character_Status_name_eng;
                //Debug.Log(obj_name);
                obj.name = obj_name + "_#" + n;
                
                return character;
            });
            m_CharacterPools.m_List.Add(m_CharacterPool);
        }



    }

    void LSY_Unit_Init(List<GameObject> unit_list)
    {
        int cnt = unit_list.Count;
        for (int i = 0; i < cnt; i++)
        {
            unit_list[i].SetActive(true);
            unit_list[i].gameObject.transform.rotation = Quaternion.identity;
            Character Char_tmp = unit_list[i].GetComponent<Character>();
            unit_list[i].gameObject.transform.position = Char_tmp.LSY_Character_OriPos;
            float max_hp = Char_tmp.Stat_MaxHP;
            Char_tmp.Stat_HP = max_hp;
            Char_tmp.Stat_MP = 0.0f;
            Char_tmp.CharacterInit();
			unit_list[i].GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_PHASE);
		}
    }

}

// 전장에 대한 정보(주로 타일)
#region FIELD

[Serializable]
public class HYJ_Battle_Manager_Line
{
    [SerializeField] List<HYJ_Battle_Tile> tiles;

    //////////  Getter & Setter //////////    
    public List<HYJ_Battle_Tile> Tiles { get { return tiles; } }
	public HYJ_Battle_Tile HYJ_Data_Tile(int _count) { return tiles[_count]; }

    public int HYJ_Data_GetCount() { return tiles.Count; }

    // HYJ_Character형에서 -> GameObject 형으로 수정. Battle_Tiles의 주석 참조.
    public GameObject HYJ_Data_GetUnitOnTile(int _count) { return tiles[_count].HYJ_Basic_onUnit; }

    // Battle_Manager_Line의 구성 요소(tiles)에 onUnit 개수 반환
    public int LSY_Count_GetUnitOnTile()
    {
        int cnt = 0;
        for (int idx=0; idx<this.HYJ_Data_GetCount(); idx++)
        {
            if (tiles[idx].HYJ_Basic_onUnit != null)
                cnt++;
        }
        return cnt;
    }

    public HYJ_Battle_Tile TileGraph(int _Idx)
    {
        foreach(var Tile in tiles)
        {
            if (Tile.GraphIndex == _Idx)
                return Tile;                     
		}

		return null;
	}

	//////////  Method          //////////
	public void HYJ_Tile_Add(Transform _trans)
    {
        tiles.Add(_trans.GetComponent<HYJ_Battle_Tile>());
    }

    //////////  Default Method  //////////
    public HYJ_Battle_Manager_Line()
    {
        tiles = new List<HYJ_Battle_Tile>();
    }
}

partial class HYJ_Battle_Manager
{
    [Header("==================================================")]
    [Header("FIELD")]

    [SerializeField] Transform Battle_Map;
    //[SerializeField]
    GameObject element, std_element, trash_element;
    Transform Field_parent, Stand_parent, Trash_parent;
    [SerializeField] int Field_x;
    [SerializeField] int Field_y;
    [SerializeField] int Stand_x;

    [SerializeField] List<HYJ_Battle_Manager_Line> Field_tiles;
    [SerializeField] HYJ_Battle_Manager_Line Stand_tiles;

	List<NODE> m_Graph = new List<NODE>();

	//////////  Getter & Setter //////////
	object GetGraph(params object[] _args)
    {
        return m_Graph;
    }

	object TileInGraph(params object[] _args)
	{
		foreach (var line in Field_tiles)
		{
			var tile = line.TileGraph((int)_args[0]);

			if (null != tile)
				return tile;
		}
        return null;
	}
    // Field_tiles
    object Get_Field_tiles(params object[] _args)
    {
        return Field_tiles;
    }
    object HYJ_Field_GetFieldX(params object[] _args)
    {
        return Field_x;
    }
    object HYJ_Field_GetFieldY(params object[] _args)
    {
        return Field_y;
    }
    // Stand_tiles
    object HYJ_Field_GetStandtiles(params object[] _args)
    {
        return Stand_tiles;
    }
    object HYJ_Field_GetStandX(params object[] _args)
    {
        return Stand_x;
    }

    object LSY_Field_GetStandX(params object[] _args)
    {
        // Bridge에 추가,,
        return Stand_x;
    }
    object LSY_Count_Ally_OnTile(params object[] _args)
    {
        int cnt = 0;
        for (int i=(Field_y+1)/2; i<Field_y; i++)   // Available 한 Lines
        {
            cnt += Field_tiles[i].LSY_Count_GetUnitOnTile();
        }

        return cnt;
    }


    // Field_tiles
    object HYJ_Field_GetTile(params object[] _args)
    {
        int x = (int)_args[0];
        int y = (int)_args[1];

        return Field_tiles[y].HYJ_Data_Tile(x);
    }    

    // 캐릭터가 위치한 타일을 찾아낸다.
    object HYJ_Field_GetTileFromCharacter(params object[] _args)
    {
        Vector2 pos = (Vector2)HYJ_Field_GetXYFromCharacter(_args);

        return Field_tiles[(int)pos.y].HYJ_Data_Tile((int)pos.x);
    }

    // 캐릭터를 찾아낸다. xy값으로
    object HYJ_Field_GetCharacter(params object[] _args)
    {
        GameObject res = null;

        //
        HYJ_Battle_Tile element = (HYJ_Battle_Tile)HYJ_Field_GetTile(_args);
        res = element.HYJ_Basic_onUnit;

        return res;
    }

    // y의 값을 보내준다.
    object HYJ_Field_GetTilesCount(params object[] _args)
    {
        return Field_tiles.Count;
    }

    // 해당 y의 x값을 보내준다.
    object HYJ_Field_GetTilesGetCount(params object[] _args)
    {
        int count = (int)_args[0];

        return Field_tiles[count].HYJ_Data_GetCount();
    }

    // 해당 캐릭터의 위치를 찾는다.
    object HYJ_Field_GetXYFromCharacter(params object[] _args)
    {
		GameObject target = (GameObject)_args[0];

        //
        Vector2 res = new Vector2(-1, -1);

        for(int forY = 0; forY < Field_tiles.Count; forY++)
        {
            for(int forX = 0; forX < Field_tiles[forY].HYJ_Data_GetCount(); forX++)
            {
                GameObject element = Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX);

                if ((element != null) && Field_tiles[forY].HYJ_Data_GetUnitOnTile(forX).Equals(target))
                {
                    res.x = forX;
                    res.y = forY;

                    break;
                }
            }
        }

        return res;
    }

    //////////  Method          //////////
    void HYJ_Field_CharacterFixed()
    {
        Debug.Log("HYJ called");
        // 대기열 픽스
        for (int i = 0; i < Stand_x; i++)
        {
            // 플레이어에게 저장된 유닛데이터를 가져온다.
            object data = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__GET_WAIT_UNIT_DATA,
                //
                i);

            // 빈칸인가?
            if(data != null)
            {
                CTRL_Character_Data element = (CTRL_Character_Data)data;

                // 의미없는 데이터인가?
                if(element.Data_ID != null)
                {
                    bool isCreate = true;

                    GameObject unit = Stand_tiles.HYJ_Data_Tile(i).HYJ_Basic_onUnit;

                    if(unit != null)
                    {
                        // 유닛이 지금 가져온 데이터와 다르다면 날려준다.
                        if(unit.GetComponent<Character>().HYJ_Status_saveData == element)
                        {
                            isCreate = false;
                        }
                        else
                        {
                            HYJ_Field_CharacterFixed_UnitDestroy(i);
                        }
                    }

                    if (isCreate)
                    {
                        int idx = int.Parse(element.Data_ID);
                        // DB에서 데이터를 불러온다.
                        GameObject unitData
                            = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                                HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                                idx);
                                //int.Parse(element.Data_ID));

                        Vector3 pos = Stand_tiles.HYJ_Data_Tile(i).transform.position;

                        //GameObject tmp = Instantiate(unitData, pos, Quaternion.identity, Unit_parent);
                        //GameObject tmp = m_CharacterPools.m_List[idx].pop().gameObject;
                        GameObject tmp = m_CharacterPools.m_List[idx].objects.PopStack().gameObject;
                        tmp.transform.localPosition = pos;
                        tmp.transform.rotation= Quaternion.identity;
                        tmp.GetComponent<Character>().HYJ_Status_saveData = element;
                    }
                }
            }
            else
            {
                HYJ_Field_CharacterFixed_UnitDestroy(i);
            }
        }
    }

    void HYJ_Field_CharacterFixed_UnitDestroy(int _count)
    {
        GameObject unit = Stand_tiles.HYJ_Data_Tile(_count).HYJ_Basic_onUnit;
        if (unit != null)
        {
            Stand_tiles.HYJ_Data_Tile(_count).HYJ_Basic_onUnit = null;
            Destroy(unit);
        }
    }

	object InitGraphNodes(params object[] _args)
	{
        int size = m_Graph.Count;
        for(int i = 0; i < size; i++)
        {
            m_Graph[i].Fcost = 0;
			m_Graph[i].Gcost = 0;
            m_Graph[i].ParentIndex = 0;

			if (null == m_Graph[i].Tile.HYJ_Basic_onUnit)
            {
				m_Graph[i].Unit = null;
				m_Graph[i].Marking = false;
			}
		}

		return null;
    }

	private void SetupGraph()
	{
		List<HYJ_Battle_Manager_Line> TileLines = (List<HYJ_Battle_Manager_Line>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES);
		int Index = 0;
		int MinimX = TileLines[0].HYJ_Data_GetCount();
		int MaximX = TileLines[1].HYJ_Data_GetCount();
		double TruncateVal = 0.0f;

		if (TileLines.Count > 0)
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

		for (int i = 0; i < TileLines.Count; ++i)
		{
			int TileXSize = TileLines[i].HYJ_Data_GetCount();

			// 행별 열개수 차에 의한 Index 보정 값
			TruncateVal = Math.Truncate(i / 2.0f);

			for (int j = 0; j < TileXSize; ++j)
			{
				if (i % 2 == 0 || i >= 3)
					Index = i * MinimX + j + (int)TruncateVal;
				else
					Index = i * MinimX + j;

				if (m_Graph.Count <= Index)
					continue;

				//Debug.Log("그래프 생성 중 Index : " + Index);

				// 좌 상단 인접 타일
				// 맨윗줄과 맨왼쪽줄에 홀수타일은 좌상단 타일이 없다
				if (i != 0 && (j != 0 || (j == 0 && i % 2 == 0))) // 맨윗줄 아닐때 && ( 맨왼쪽 아닐때 || (  j == 0 인데 짝수 줄이면 좌상단 있어서 예외처리 )
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index - (MaximX)]);
					//Debug.Log("m_Graph[" + Index + "]의 좌상단 타일 Index" + (Index - MaximX));
				}

				// 우 상단 인접 타일
				if (i != 0 && ((j != TileXSize - 1) || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index - (MinimX)]);
					//Debug.Log("m_Graph[" + Index + "]의 우상단 타일 Index" + (Index - MinimX));
				}

				// 우 인접 타일
				if (j < TileXSize - 1)
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index + 1]);
					//Debug.Log("m_Graph[" + Index + "]의 우 타일 Index" + (Index + 1));
				}

				// 우 하단 인접 타일			
				// 맨 아랫줄과 맨오른쪽 홀수줄은 우 하단 타일 없음	
				if (i != TileLines.Count - 1 && (j != TileXSize - 1 || (j == TileXSize - 1 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index + MaximX]);
					//Debug.Log("m_Graph[" + Index + "]의 우하단 타일 Index" + (Index + MaximX));
				}

				// 좌 하단 인접 타일
				if (i != TileLines.Count - 1 && (j != 0 || (j == 0 && i % 2 == 0)))
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index + MinimX]);
					//Debug.Log("m_Graph[" + Index + "]의 좌하단 타일 Index" + (Index + MinimX));
				}

				// 좌 인접 타일
				if (j > 0)
				{
					m_Graph[Index].m_Neighbors.m_List.Add(m_Graph[Index - 1]);
					//Debug.Log("m_Graph[" + Index + "]의 좌 타일 Index" + (Index - 1));
				}
			}
		}
	}

	//////////  Default Method  //////////
	void HYJ_Field_Init()
    {
        //// Battle_Map, Field_parent, Stand_parent 변수명임

        Vector3 pos0 = element.transform.localPosition; // 0,0,0
        Vector3 pos1 = Battle_Map.GetChild(1).localPosition;    // 1,0,-2
        int tile_cnt = -1;
        //
        Field_tiles = new List<HYJ_Battle_Manager_Line>();
        for (int forY = 0; forY < Field_y; forY++)  // Y 가 "행"
        {
            HYJ_Battle_Manager_Line line = new HYJ_Battle_Manager_Line();

            //
            int countX = Field_x;
            if((forY % 2) == 1)
            {
                countX += 1;
            }

            for (int forX = 0; forX < countX; forX++)  // X 가 "열"
            {
                //GameObject obj = Instantiate(element, Field_parent);
                //GameObject obj = m_TilePool.pop().gameObject;
                GameObject obj = m_TilePool.objects.PopStack().gameObject;
                obj.SetActive(true);

                ++tile_cnt;
                obj.name = "(" + forY + "," + forX + ")_" + tile_cnt.ToString();

                obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forY);
                obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forX);

                if ((Field_y + 1) / 2 <= forY) obj.GetComponent<HYJ_Battle_Tile>().tile_Available = HYJ_Battle_Tile.Tile_Available.Available;

                if (countX == Field_x)
                {
                    obj.transform.localPosition = new Vector3(pos0.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                else
                {
                    obj.transform.localPosition = new Vector3(-pos1.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * forY));
                }
                obj.transform.localScale = element.transform.localScale;
                

                //
                line.HYJ_Tile_Add(obj.transform);
            }

            //
            Field_tiles.Add(line);
        }

        // 대기열 9좌석 +@, 레벨업이나 유물로 늘어날 수 있음.
        Stand_tiles = new HYJ_Battle_Manager_Line();

        // left는 Stand_tiles의 좌측 시작점 위치
        int num = Field_tiles[Field_y - 1].HYJ_Data_GetCount();
        Vector3 left;
        if (num % 2 == 0)
        {
            // 마지막 행의 tile 수가 짝수일 때,
            left = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2 - 1).gameObject.transform.localPosition;
            Vector3 right = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2).gameObject.transform.localPosition;
            Vector3 mid = new Vector3((left.x + right.x) / 2.0f, (left.y + right.y) / 2.0f, (left.z + right.z) / 2.0f);

            left = new Vector3(mid.x - 4 * 2.0f, 0, mid.z);
        }
        else
        {
            // 마지막 행의 tile 수가 홀수일 때,
            left = Field_tiles[Field_y - 1].HYJ_Data_Tile(num / 2).gameObject.transform.localPosition;
            left = new Vector3(left.x - 4 * 2.0f, 0, left.z);
        }

        // Stand_x 개 만큼 Stand_tile 생성
        for (int forX = 0; forX < Stand_x; forX++)
        {
            GameObject std_obj = Instantiate(std_element, Stand_parent);
            std_obj.SetActive(true);
            std_obj.name = "stand_" + forX;
            std_obj.GetComponent<HYJ_Battle_Tile>().tile_type = HYJ_Battle_Tile.Tile_Type.Stand;
            std_obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(0);    // Stand는 행이 한 개 뿐이다.
            std_obj.GetComponent<HYJ_Battle_Tile>().Tile_Idx.Add(forX);
            std_obj.GetComponent<HYJ_Battle_Tile>().tile_Available = HYJ_Battle_Tile.Tile_Available.Available;
            std_obj.transform.localPosition = new Vector3(left.x + (pos1.x * 2.0f * forX), 0, pos0.z + (pos1.z * Field_y));
            std_obj.transform.localScale = element.transform.localScale;
            Stand_tiles.HYJ_Tile_Add(std_obj.transform);
        }

        GameObject trash_obj = Instantiate(trash_element, Trash_parent);
        trash_obj.GetComponent<HYJ_Battle_Tile>().TileType = HYJ_Battle_Tile.Tile_Type.Trash;
        trash_obj.SetActive(false);
        trash_obj.name = "trash_0";





		//
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GRAPH_NODE_INIT,          InitGraphNodes              );
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_GRAPH,                    GetGraph                    );
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD_GET_TILES,                    Get_Field_tiles                );
		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_X,                HYJ_Field_GetFieldX             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_FIELD_Y,                HYJ_Field_GetFieldY             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_TILES,            HYJ_Field_GetStandtiles         );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_STAND_X,                HYJ_Field_GetStandX             );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE,                   HYJ_Field_GetTile               );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_FROM_CHARACTER,    HYJ_Field_GetTileFromCharacter  );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_CHARACTER,              HYJ_Field_GetCharacter          );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_COUNT,            HYJ_Field_GetTilesCount         );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILES_GET_COUNT,        HYJ_Field_GetTilesGetCount      );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_XY_FROM_CHARACTER,      HYJ_Field_GetXYFromCharacter    );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__COUNT_ALLY_ONTILE,          LSY_Count_Ally_OnTile           );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__STAND_TO_FIELD,              Stand_to_Field                  );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__FIELD_TO_STAND,              Field_to_Stand                  );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_TRASH,                     Unit_to_Trash                   );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__TO_SACRIFICED,                Unit_to_Sacrificed              );
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT__SACRIFICED_TO_POOL,           Unit_Sacrificed_to_Pool         );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___UNIT_DIE,                           Unit_Die                        );

        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set( HYJ_ScriptBridge_EVENT_TYPE.BATTLE___COUNT__FIELD_UNIT,                 Count_Field_Unit                );

		HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Set(HYJ_ScriptBridge_EVENT_TYPE.BATTLE___FIELD__GET_TILE_IN_GRAPH, TileInGraph);

	}
}

#endregion

// 전투 상점 및 UI 정보
# region SHOP & UI
public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [Header("==================================================")]
    [Header("SHOP")]

    [SerializeField]
    List<GameObject> Shop_UnitList;
    //[SerializeField]
    Transform Battle_UI, Battle_Canvas, Battle_Shop, Battle_Synergy, Battle_Text, Shop_panel;
    //[SerializeField]
    TextMeshProUGUI Shop_Coin_Text, Battle_Ally_OnTile, Battle_Timer_TMP;
    //[SerializeField]
    UnityEngine.UI.Image Shop_Exp_Cur;
    //[SerializeField]
    GameObject End_Btn;


    [SerializeField]
    List<int> Prob_list = new List<int>();
    [SerializeField]
    List<int> Cost_list;

    List<List<Dictionary<string, object>>> Unit_DB;

    [SerializeField]
    List<string> UnitName_list = new List<string>();
    [SerializeField]
    List<int> UnitID_list = new List<int>();
    [SerializeField]
    List<int> UnitIdx_list = new List<int>();
    int Shop_Panel_cnt = 5;
    int Player_Lv = 1, cur_EXP = 0;
    List<int> Max_EXP_List = new List<int>() { 0, 2, 6, 10, 20, 36, 56, 80, 108, 140, 170, 190, 210 };
    float Max_EXP;
    bool UL_isInitialized = false;

    // Method
    public void LSY_UnitList_Init()
    {
        // Level
        cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);
        Max_EXP = Max_EXP_List[Player_Lv];
        Shop_Exp_Cur.fillAmount = cur_EXP / Max_EXP;
        Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);

        Show_Ally_OnTile();

        // Text, Image, Cost
        Unit_DB = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);

        for (int i = 0; i < Unit_DB[0].Count; i++)
        {
            UnitName_list.Add(Unit_DB[0][i]["NAME"].ToString());
            UnitID_list.Add((int)Unit_DB[0][i]["ID"]);
        }

        LSY_Shop_Reload(1);
        Debug.Log("UnitList_Init end..");

        UL_isInitialized = true;
    }

    bool Enemy_isInitialized = false;
    public void LSY_Enemy_Init()
    {
        int enemy_num = 6;
        List<Vector3> pos = new List<Vector3>();
        for (int i = 0; i < enemy_num; i++)
        {
            pos.Add(new Vector3(i*2, 0, 0));
        }


        for (int i=0; i<enemy_num; i++)
        {
            System.Random r = new System.Random();
            int n = r.Next(3);

            GameObject unitData
             = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                 HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                 n);

            GameObject tmp;
            if (false)
                //GameObject tmp = Instantiate(unitData, pos[i], Quaternion.identity, Enemy_parent);
                tmp = Instantiate(unitData, pos[i], Quaternion.identity, Enemy_parent);
            else if (m_CharacterPools.m_List[n].get_Count() > 0)
            {
                Debug.Log("[BM]Pools.mList.getCount : " + m_CharacterPools.m_List[n].get_Count());
                //tmp = m_CharacterPools.m_List[n].pop().gameObject;
                tmp = m_CharacterPools.m_List[n].objects.PopStack().gameObject;
                tmp.SetActive(true);
                tmp.transform.localPosition = pos[i];
                tmp.transform.rotation = Quaternion.identity;
                tmp.transform.SetParent(Enemy_parent);
            }
            else
            {
                // -- enemy init 할 때 pooling에 다시 넣어주기, pool에 모자랄 경우 예외 처리.

                continue;


                //tmp = m_CharacterPools[6].pop().gameObject;
                //tmp.SetActive(true);
                //tmp.transform.localPosition = pos[i];
                //tmp.transform.rotation = Quaternion.identity;
                //tmp.transform.SetParent(Enemy_parent);
            }

            tmp.transform.Rotate(0f, 180f, 0f);
            tmp.tag = "Enemy";
            tmp.GetComponent<Character>().m_UnitType = Character.Unit_Type.Enemy;

            Enemy_Unit.Add(tmp);

        }

        Enemy_isInitialized = true;
    }

    public void LSY_Shop_Reload(int n)
    {
        for(int i=0; i<Shop_Panel_cnt; i++)
        {
            Shop_UnitList[i].SetActive(true);
        }

        Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);

        if (n != 1) HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, 2);

        // Before Reload, Clear Idx list
        UnitIdx_list.Clear();
        // 유닛 초기화 성공할 때 까지 리롤
        while (!LSY_Calc_Proba()) { ; }
        
        for (int i = 0; i < Shop_UnitList.Count; i++)  // 0~5
        {
            int idx = UnitIdx_list[i];
            Shop_UnitList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = UnitName_list[idx].ToString();
        }
    }

    public bool LSY_Calc_Proba()
    {
        Cost_list = new List<int>();

        // 확률 적용 방법에 따라, 전체 파이가 100이 아닐 수 있음. 그때 수정하면 됨.
        int tot_num = 100;

        // 레벨에 따른 유닛 등장 확률
        switch (Player_Lv)
        {
            case 1:
                List<int> Prob_1 = new List<int>() { 100, 0, 0, 0, 0, 0 }; Prob_list = Prob_1; break;
            case 2:
                List<int> Prob_2 = new List<int>() { 90, 10, 0, 0, 0, 0 }; Prob_list = Prob_2; break;
            case 3:
                List<int> Prob_3 = new List<int>() { 75, 25, 0, 0, 0, 0 }; Prob_list = Prob_3; break;
            case 4:
                List<int> Prob_4 = new List<int>() { 60, 30, 10, 0, 0, 0 }; Prob_list = Prob_4; break;
            case 5:
                List<int> Prob_5 = new List<int>() { 45, 38, 15, 2, 0, 0 }; Prob_list = Prob_5; break;
            case 6:
                List<int> Prob_6 = new List<int>() { 35, 40, 20, 5, 0, 0 }; Prob_list = Prob_6; break;
            case 7:
                List<int> Prob_7 = new List<int>() { 34, 30, 25, 10, 1, 0 }; Prob_list = Prob_7; break;
            case 8:
                List<int> Prob_8 = new List<int>() { 18, 20, 40, 20, 2, 0 }; Prob_list = Prob_8; break;
            case 9:
                List<int> Prob_9 = new List<int>() { 16, 15, 30, 30, 8, 1 }; Prob_list = Prob_9; break;
            case 10:
                List<int> Prob_10 = new List<int>() { 11, 10, 20, 40, 15, 4 }; Prob_list = Prob_10; break;
            case 11:
                List<int> Prob_11 = new List<int>() { 0, 7, 15, 45, 25, 8 }; Prob_list = Prob_11; break;
            case 12:
                List<int> Prob_12 = new List<int>() { 0, 0, 10, 50, 30, 10 }; Prob_list = Prob_12; break;
        }


        // 등장 확률 누적 리스트
        for(int i = 0; i < Prob_list.Count - 1; i++)
        {
            if (Prob_list[i] != tot_num)
                Prob_list[i + 1] += Prob_list[i];
        }

        // 유닛 코스트 배열 초기화
        for(int i = 0; i < Shop_Panel_cnt; i++)
        {
            System.Random r = new System.Random();
            int n = r.Next(1, tot_num + 1); // min 이상, max 미만
            for(int j = 0; j < Prob_list.Count; j++)
            {
                if (n <= Prob_list[j])
                {
                    Cost_list.Add(j + 1);
                    break;
                }
            }
        }

        // 코스트 별 유닛 랜덤 설정
        for(int i = 0; i < Shop_Panel_cnt; i++)
        {
            List<int> Unit_Candi = new List<int>();
            for(int j = 0; j < Unit_DB[0].Count - 1; j++)
            {
                int unit_cost = (int)Unit_DB[0][j]["COST"];
                int unit_idx = (int)Unit_DB[0][j]["Index"];
                int unit_cnt = m_CharacterPools.m_List[unit_idx].get_Count();

                if (unit_cost == Cost_list[i])
                {
                    if (unit_cnt == 0)
                    {
                        Debug.Log("[BM] Allocate " + unit_idx + " one more.");

                        var obj = Instantiate(prefab.transform.GetChild(unit_idx).gameObject);
                        obj.SetActive(false);
                        obj.transform.SetParent(Unit_pool);
                        obj.transform.localScale = Vector3.one;
                        var character = obj.GetComponent<Character>();
                        character.m_UnitType = Character.Unit_Type.Ally;
                        character.STATUS_HPBAR.SetHPColor(UI_StatusBar.STATUS_HP_COLOR.GREEN);
                        character.HYJ_Status_saveData = new CTRL_Character_Data(unit_idx.ToString());
                        string obj_name;
                        obj_name = character.Character_Status_name_eng;
                        obj.name = obj_name + "_#" + m_CharacterPools.m_List[unit_idx].get_tot_count();

                        m_CharacterPools.m_List[unit_idx].allocate_onemore(character);
                    }

                    for (int k = 0; k < unit_cnt; k++)
                        Unit_Candi.Add(unit_idx);
                }
            }

            if (Unit_Candi.Count > 0)
            {
                System.Random r = new System.Random();
                int n = r.Next(Unit_Candi.Count);
                //Debug.Log(Unit_Candi.Count + "<-unitcandi.count || n->" + n);
                UnitIdx_list.Add(Unit_Candi[n]);
                Unit_Candi.RemoveAt(n);
            }
            else
            {
                Debug.Log("None of Unit COST " + Cost_list[i] + " is Available...");
                //UnitIdx_list.Add(6);
                return false;
            }
        }

        return true;
    }

    public void LSY_Buy_Unit()
    {
        if (true)
        {
            // Detect clicked btn -> getName -> Calc pos -> Instant
            var Btn_idx = EventSystem.current.currentSelectedGameObject;
            string Btn_name = Btn_idx.name.ToString();
            Btn_name = Btn_name.Substring(Btn_name.Length - 1);
            Debug.Log("Btn " + Btn_name + " is clicked");

            int unit_idx = UnitIdx_list[int.Parse(Btn_name)];
            //GameObject std_tmp = null;

            int cnt = Stand_tiles.LSY_Count_GetUnitOnTile(), pos_num = -1;
            if (cnt < Stand_x)
            {
                for (int idx = 0; idx < Stand_x; idx++)
                {
                    if (Stand_tiles.HYJ_Data_GetUnitOnTile(idx) == null)
                    {
                        //Debug.Log(Stand_tiles.HYJ_Data_Tile(idx).gameObject);
                        //std_tmp = Stand_tiles.HYJ_Data_Tile(idx).gameObject;
                        pos_num = idx;
                        break;
                    }
                }
                if (pos_num == -1)
                    pos_num = cnt;

                //Debug.Log(pos_num + " " + cnt);

                GameObject _onTile = Stand_tiles.HYJ_Data_Tile(pos_num).gameObject;
                //Vector3 pos = Stand_tiles.HYJ_Data_Tile(pos_num).transform.position;

                GameObject unitData
                    = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                        HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                        unit_idx);
                if (unitData)
                {
                    GameObject tmp;
                    if (false)
                        tmp = Instantiate(unitData, _onTile.transform.position, Quaternion.identity, Unit_parent);
                    else
                    {
                        if (m_CharacterPools.m_List[unit_idx].get_Count() == 0)
                        {
                            Debug.Log("[BM] Allocate " + unit_idx + " one more.");

                            var obj = Instantiate(prefab.transform.GetChild(unit_idx).gameObject);
                            obj.SetActive(false);
                            obj.transform.SetParent(Unit_pool);
                            obj.transform.localScale = Vector3.one;
                            var character = obj.GetComponent<Character>();
                            character.m_UnitType = Character.Unit_Type.Ally;
                            character.STATUS_HPBAR.SetHPColor(UI_StatusBar.STATUS_HP_COLOR.GREEN);
                            character.HYJ_Status_saveData = new CTRL_Character_Data(unit_idx.ToString());
                            string obj_name;
                            obj_name = character.Character_Status_name_eng;
                            obj.name = obj_name + "_#" + m_CharacterPools.m_List[unit_idx].get_tot_count();

                            m_CharacterPools.m_List[unit_idx].allocate_onemore(character);
                        }

                        tmp = m_CharacterPools.m_List[unit_idx].objects.PopStack().gameObject;
                        tmp.SetActive(true);
                        tmp.transform.localPosition = _onTile.transform.position;
                        tmp.transform.rotation = Quaternion.identity;
                        tmp.transform.SetParent(Unit_parent);
                        //tmp.tag = "Ally";
                    }
                    // Stand_Unit에 추가, 생성될 때 On_Tile..... 
                    tmp.transform.localPosition = _onTile.transform.position;
                    tmp.GetComponent<Character>().UnitType = Character.Unit_Type.Ally;
                    tmp.GetComponent<Character>().LSY_Character_OriPos = _onTile.transform.position;   // 처음 구매할 때 char_ori_Pos 초기화 필요함
                    tmp.GetComponent<Character>().LSY_Character_Set_OnTile(_onTile);    // 처음 구매할 때 onTile 설정 필요.
                    Stand_Unit.Add(tmp);
                    //unitData.GetComponent<Character>().LSY_Character_Set_OnTile(Stand_tiles.HYJ_Data_Tile(pos_num).gameObject);
                    //Debug.Log(Stand_tiles.HYJ_Data_Tile(pos_num).gameObject);

                    int cost = unitData.GetComponent<Character>().Stat_Cost;
                    HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, cost);
                    Debug.Log(tmp.name + " is spawned");

                    // 구매한 카드 사라지게.
                    Btn_idx.SetActive(false);

                    // Status_saveData를 꼭 구매할 때 저장해야 하는가?
                    tmp.GetComponent<Character>().HYJ_Status_saveData = new CTRL_Character_Data(unit_idx+"");
                }
                else // 유닛이 없을 경우!
                    Debug.Log("Unit " + unit_idx + " is NULL");
            }
            else
                Debug.Log("Stand Tile is full..");

        }
        else
        {
            var Btn_idx = EventSystem.current.currentSelectedGameObject;
            string Btn_name = Btn_idx.name.ToString();
            Btn_name = Btn_name.Substring(Btn_name.Length - 1);
            Debug.Log("Btn " + Btn_name + " is clicked");

            int unit_idx = UnitIdx_list[int.Parse(Btn_name)];

            GameObject unitData
                = (GameObject)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATA_FROM_ID,
                    unit_idx);

            if (unitData != null)
            {
                object success = HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(
                    HYJ_ScriptBridge_EVENT_TYPE.PLAYER___UNIT__INSERT,
                    //
                    unitData.name, -1);

                if (success != null)
                {
                    if ((bool)success)
                    {
                        int cost = unitData.GetComponent<Character>().Stat_Cost;

                        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, cost);

                        // 구매한 카드 사라지게.
                        Btn_idx.SetActive(false);

                        HYJ_Field_CharacterFixed();
                    }
                    else
                    {

                    }
                }
            }
        }
    }
    public void LSY_Buy_EXP()
    {
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GOLD_MINUS, 4);
        HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_INCREASE, 4);
        cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);

        if (cur_EXP >= Max_EXP)
        {
            Debug.Log("cur : " + cur_EXP + ", Max_Exp : " + Max_EXP + ", LV : " + Player_Lv);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__LEVEL_INCREASE, 1);
            HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__EXP_DECREASE, Max_EXP_List[Player_Lv]);
            Player_Lv = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_LEVEL);
            cur_EXP = (int)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.PLAYER___BASIC__GET_EXP);
            Max_EXP = Max_EXP_List[Player_Lv];
            Show_Ally_OnTile();
        }

        Shop_Exp_Cur.fillAmount = cur_EXP / Max_EXP;

    }

    //Font JSong_Bold, JSong_Light, Chosun_Bold, Chosun_Light;
    public const string FONT_JSONG = "Assets/TextMesh Pro/Fonts/JSONGMYEONG SDF.asset";
    public const string FONT_CHOSUN = "Assets/TextMesh Pro/Fonts/CHOSUNCENTENNIAL_TTF SDF.asset";
    public void Battle_UI_Font()
    {
        int _cnt = Shop_UnitList.Count;
        for (int i=0; i<_cnt; i++)
        {
            Shop_UnitList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().font =
                AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_JSONG);
        }
        Shop_Coin_Text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_JSONG);
        Battle_Ally_OnTile.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_JSONG);
        Battle_Timer_TMP.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FONT_JSONG);

    }



    //

}
#endregion

// 유닛 정보
#region UNIT
partial class HYJ_Battle_Manager
{
    [Header("==================================================")]
    [Header("UNIT")]

    [SerializeField] Transform Battle_Units;
    Transform Unit_pool, Unit_parent, Enemy_parent, Unit_Sacrificed;
    [SerializeField]
    List<GameObject> Stand_Unit;
    [SerializeField]
    List<GameObject> Field_Unit;
    [SerializeField]
    List<GameObject> Enemy_Unit;
    [SerializeField]
    List<GameObject> Sacrificed_Unit;

    /// Method
    object Stand_to_Field(params object[] _args)
    {
        GameObject obj = (GameObject)_args[0];

        Debug.Log("Move " + obj.name + "stand to field");
        Stand_Unit.Remove(obj);
        Field_Unit.Add(obj);

        Show_Ally_OnTile();

        return null;
    }
    object Field_to_Stand(params object[] _args)
    {
        GameObject obj = (GameObject)_args[0];

        Debug.Log("Move " + obj.name + "field to stand");
        Stand_Unit.Add(obj);
        Field_Unit.Remove(obj);

        Show_Ally_OnTile();

        return null;
    }
    object Unit_to_Trash(params object[] _args)
    {
        string tile_type = _args[0].ToString();
        GameObject obj = (GameObject)_args[1];
        //int id = int.Parse(obj.GetComponent<Character>().HYJ_Status_saveData.Data_ID);

        Character obj_char = obj.GetComponent<Character>();
        int id = obj_char.Character_Status_Index;

        switch (tile_type)
        {
            case "Stand":
                Stand_Unit.Remove(obj);
                obj.transform.SetParent(Unit_pool);
                m_CharacterPools.m_List[id].objects.PushStack(obj.GetComponent<Character>());
                break;

            case "Field":
                Field_Unit.Remove(obj);
                obj.transform.SetParent(Unit_pool);
                m_CharacterPools.m_List[id].objects.PushStack(obj.GetComponent<Character>());
                break;

            case "Enemy":
                Debug.Log(obj + " Enemy remove");
                Enemy_Unit.Remove(obj);
                obj.transform.SetParent(Unit_pool);
                obj.tag = "Ally";
                m_CharacterPools.m_List[id].objects.PushStack(obj.GetComponent<Character>());
                break;
        }

        List<List<Dictionary<string, object>>> Unit_csv = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);
        obj.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_DEFAULT);
        obj_char.HYJ_Status_SettingData(Unit_csv[0][obj_char.Character_Status_Index]);
        obj.SetActive(false);


        Show_Ally_OnTile();

        return null;
    }
    object Unit_to_Sacrificed(params object[] _args)
    {
        string tile_type = _args[0].ToString();
        GameObject obj = (GameObject)_args[1];

        switch (tile_type)
        {
            case "Stand":
                Stand_Unit.Remove(obj);
                Sacrificed_Unit.Add(obj);
                //obj.SetActive(false);
                break;

            case "Field":
                Field_Unit.Remove(obj);
                Sacrificed_Unit.Add(obj);
                //obj.SetActive(false);
                break;
        }

        Character obj_char = obj.GetComponent<Character>();

        // TriggerExit 으로 tile의 onUnit이 갱신되지 않음
        obj_char.LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().HYJ_Basic_onUnit = null;

        List<List<Dictionary<string, object>>> Unit_csv = (List<List<Dictionary<string, object>>>)HYJ_ScriptBridge.HYJ_Static_instance.HYJ_Event_Get(HYJ_ScriptBridge_EVENT_TYPE.DATABASE___UNIT__GET_DATABASE_CSV);
        obj.GetComponent<Shader_Effect>().Set_EffectMode(Shader_Effect.EFFECT_MODE.MODE_DEFAULT);
        obj_char.HYJ_Status_SettingData(Unit_csv[0][obj_char.Character_Status_Index]);

        obj.transform.SetParent(Unit_Sacrificed);
        obj.SetActive(false);

        Show_Ally_OnTile();

        return null;
    }
    object Unit_Sacrificed_to_Pool(params object[] _args)
    {
        int _idx = (int)_args[0], cnt = 0;
        Character.Unit_Star _star = (Character.Unit_Star)_args[1];

        // 2성 -> 1성 2개 부활
        // 3성 -> 2성 2개 + 1성 6개 부활
        switch(_star)
        {
            case Character.Unit_Star.TWO:
                cnt = 2;
                break;
            case Character.Unit_Star.THREE:
                cnt = 8;
                break;
        }

        int _len = Sacrificed_Unit.Count, num = 0;
        for (int i=0; i<_len; i++)
        {
            GameObject obj = Sacrificed_Unit[i - num];
            if (obj.GetComponent<Character>().Character_Status_Index == _idx)
            {
                Sacrificed_Unit.Remove(obj);
                obj.transform.SetParent(Unit_pool);
                m_CharacterPools.m_List[_idx].objects.PushStack(obj.GetComponent<Character>());
                num++;
                if (num == cnt)
                    return null;
            }
        }



        return null;
    }
    object Unit_Die(params object[] _args)
    {

        GameObject obj = (GameObject)_args[0];
        //string obj_tag = obj.tag;

        Character.Unit_Type _Type = obj.GetComponent<Character>().UnitType;

		foreach(var unit in Field_Unit)
        {
            if (unit.GetComponent<Character>().Target == obj)
                unit.GetComponent<Character>().Target = null;
		}

		foreach (var unit in Enemy_Unit)
		{
			if (unit.GetComponent<Character>().Target == obj)
				unit.GetComponent<Character>().Target = null;
		}

		switch (_Type)
        {
            case Character.Unit_Type.Ally:
                int obj_tile_idx = obj.GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex;
                if (obj_tile_idx < 0) // stand
                {
                    Stand_Unit.Remove(obj);
                }
                else // field
                {
                    Field_Unit.Remove(obj);
                }
                break;

            case Character.Unit_Type.Enemy:
                Enemy_Unit.Remove(obj);
                break;

        }
        Show_Ally_OnTile();

        return null;
    }

    object Count_Field_Unit(params object[] _args)
    {
        int num = Field_Unit.Count;
        return num;
    }
    public void Show_Ally_OnTile()
    {
        int lv = Player_Lv + 1;
        string cnt_restrict = "(" + Field_Unit.Count + "/" + lv + ")";
        Battle_Ally_OnTile.text = cnt_restrict;
    }

    public void Find_Target()
    {
        int Num_Ally = Field_Unit.Count;
        int Num_Enemy = Enemy_Unit.Count;
        int MinIdx = 0;

        if (Num_Ally <= 0 || Num_Enemy <= 0)
            return;

		// TODO : 거리가 똑같은게 있다면 현재 객체가 보고있는 방향 기준으로 각도가 작은것으로 타겟 설정
		for (int i = 0; i < Num_Ally; i++)
        {
			if (Field_Unit[i].GetComponent<Character>().Dead ||
				Field_Unit[i].GetComponent<Character>().State == Character.STATE.SKILL ||
				Field_Unit[i].GetComponent<Character>().State == Character.STATE.SKILL_IDLE)
				continue;

			//Debug.Log("i"+i);
			Vector3 A_pos = Field_Unit[i].gameObject.transform.position;
            float minDist = 10000.0f;
			Vector3 Dir = Vector3.zero;
			float minAngle = 10000.0f;

			for (int k = 0; k < Num_Enemy; k++)
            {
                // 죽어있으면 타겟 계산 X
                if (Enemy_Unit[k].GetComponent<Character>().Dead)
					continue;
                //Debug.Log("k" + k);
                Vector3 E_pos = Enemy_Unit[k].gameObject.transform.position;
                float Dist = Vector3.Magnitude(A_pos - E_pos);
                float Angle = 0.0f;

				Dir = Enemy_Unit[k].transform.position - Field_Unit[i].transform.position;				
				Angle = Quaternion.Angle(Field_Unit[i].transform.rotation, Quaternion.LookRotation(Dir));

				if (Dist <= minDist /*&& Angle < minAngle*/)
                {
					foreach (var node in m_Graph[(Enemy_Unit[k].GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex)].m_Neighbors.m_List)
                    {
                        if(false == node.Marking && node.Unit == Field_Unit[i])
                        {							
						}
						else if (false == node.Marking)
                        {
							minDist = Dist;
							minAngle = Angle;
							MinIdx = k;
						}
					}                   					
                }
            }

			// TODO : 타겟한테 갈 길 없으면 / 다음 가까운적 체크해서 길체크 없으면 다음
			// PathFinding 해서 그적한테 갈길이없어야해. -> 다시리타겟팅? 모든 몬스터 기준으로 갈 길 있는얘를 골라내야해 -> Start Path Finding이 Enemy다돌면서 갈길있는애 찾을때까지 A*를 계속 돌려야함.
            // Dictionary < Monstername, GraphDistCount > -> 오름차순order -> Path 돌려서 생성된 monster로 Target 설정. 이게 findTarget 대체해야할듯.
            // 칸 계산 괜찮은 아이디어 있으면 적용하면 좋을듯..

			//if(null != Field_Unit[i].GetComponent<Character>().Target)
			Field_Unit[i].GetComponent<Character>().PreTarget = Field_Unit[i].GetComponent<Character>().Target;
			Field_Unit[i].GetComponent<Character>().Target = Enemy_Unit[MinIdx];            
			//Enemy_Unit[MinIdx].GetComponent<Character>().Target = Field_Unit[i];
		}

		for (int i = 0; i < Num_Enemy; i++)
		{
			if (Enemy_Unit[i].GetComponent<Character>().Dead ||
				Enemy_Unit[i].GetComponent<Character>().State == Character.STATE.SKILL ||
				Enemy_Unit[i].GetComponent<Character>().State == Character.STATE.SKILL_IDLE)
				continue;
			//Debug.Log("i"+i);
			Vector3 A_pos = Enemy_Unit[i].gameObject.transform.position;
			float minDist = 10000f;
			Vector3 Dir = Vector3.zero;
            float minAngle = 10000.0f;

			for (int k = 0; k < Num_Ally; k++)
			{
				// 죽어있으면 타겟 계산 X
				if (Field_Unit[k].GetComponent<Character>().Dead)
					continue;

				//Debug.Log("k" + k);
				Vector3 E_pos = Field_Unit[k].gameObject.transform.position;
				float Dist = Vector3.Magnitude(A_pos - E_pos);
				float Angle = 0.0f;

				Dir = Field_Unit[k].transform.position - Enemy_Unit[i].transform.position;
				Angle = Quaternion.Angle(Enemy_Unit[i].transform.rotation, Quaternion.LookRotation(Dir));

				if (Dist < minDist /*&& Angle < minAngle*/)
				{

					foreach (var node in m_Graph[(Field_Unit[k].GetComponent<Character>().LSY_Character_Get_OnTile().GetComponent<HYJ_Battle_Tile>().GraphIndex)].m_Neighbors.m_List)
					{
						if (false == node.Marking &&
                            node.Unit != null &&
                            node.Unit == Field_Unit[k])
						{
						}
						else if (false == node.Marking)
						{
							minDist = Dist;
							minAngle = Angle;
							MinIdx = k;
						}
					}
				}
			}

			//if (null != Enemy_Unit[i].GetComponent<Character>().Target)
			Enemy_Unit[i].GetComponent<Character>().PreTarget = Enemy_Unit[i].GetComponent<Character>().Target;
			Enemy_Unit[i].GetComponent<Character>().Target = Field_Unit[MinIdx];

		}

	}


}
#endregion


#region SYNERGY

public partial class HYJ_Battle_Manager : MonoBehaviour
{
    [Header("==================================================")]
    [Header("SYNERGY")]


    [SerializeField]
    Transform Synergy_Red, Synergy_Green, Synergy_Gray, Synergy_Panel;

    List<Transform> Red_list = new List<Transform>();
    List<Transform> Green_list = new List<Transform>();
    List<Transform> Gray_list = new List<Transform>();

    object LSY_Battle_Synergy_Update(params object[] _args)
    {
        Battle_Synergy_Init();

        var synergy_dic = (Dictionary<int, int>)_args[0];
        synergy_dic = synergy_dic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        bool init_red = false, init_green = false, init_gray = false;

        for (int i=0; i<synergy_dic.Count; i++)
        {
            int sy_cnt = synergy_dic.Values.ToList()[i];
            if (sy_cnt >= 4)
            {
                //Transform red = Synergy_Red.GetChild(i);
                Transform red = Red_list[i];
                red.SetParent(Synergy_Panel);
                red.GetChild(0).GetComponent<TextMeshProUGUI>().text = synergy_dic.Keys.ToList()[i].ToString() + " Cost";
                if (sy_cnt >= 6)
                    red.GetChild(1).GetComponent<TextMeshProUGUI>().text = "6";
                else
                    red.GetChild(1).GetComponent<TextMeshProUGUI>().text = "4>6";

                red.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = sy_cnt.ToString();

                init_red = true;
            }
            else if(sy_cnt >= 2)
            {
                if (init_red)
                {
                    int n = Synergy_Red.childCount;
                    Transform red_dot = Synergy_Red.GetChild(n - 1);
                    red_dot.SetParent(Synergy_Panel);
                    init_red = false;
                }

                //Transform green = Synergy_Green.GetChild(i);
                Transform green = Green_list[i];
                green.SetParent(Synergy_Panel);

                green.GetChild(0).GetComponent<TextMeshProUGUI>().text = synergy_dic.Keys.ToList()[i].ToString() + " Cost";
                green.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2>4";
                green.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = sy_cnt.ToString();

                init_green = true;
            }
            else if(sy_cnt == 1)
            {
                if (init_red)
                {
                    int n = Synergy_Red.childCount;
                    Transform red_dot = Synergy_Red.GetChild(n - 1);
                    red_dot.SetParent(Synergy_Panel);
                    init_red = false;
                }
                if (init_green)
                {
                    int n = Synergy_Green.childCount;
                    Transform green_dot = Synergy_Green.GetChild(n - 1);
                    green_dot.SetParent(Synergy_Panel);
                    init_green = false;
                }

                //Transform gray = Synergy_Gray.GetChild(i);
                Transform gray = Gray_list[i];
                gray.SetParent(Synergy_Panel);

                gray.GetChild(0).GetComponent<TextMeshProUGUI>().text = synergy_dic.Keys.ToList()[i].ToString() + " Cost";
                gray.GetChild(1).GetComponent<TextMeshProUGUI>().text = "2>4";
                gray.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = sy_cnt.ToString();

                init_gray = true;
            }
        }

        if (init_red)
        {
            int n = Synergy_Red.childCount;
            Transform red_dot = Synergy_Red.GetChild(n - 1);
            red_dot.SetParent(Synergy_Panel);
            init_red = false;
        }
        if (init_green)
        {
            int n = Synergy_Green.childCount;
            Transform green_dot = Synergy_Green.GetChild(n - 1);
            green_dot.SetParent(Synergy_Panel);
            init_green = false;
        }
        if (init_gray)
        {
            int n = Synergy_Gray.childCount;
            Transform gray_dot = Synergy_Gray.GetChild(n - 1);
            gray_dot.SetParent(Synergy_Panel);
            init_gray = false;
        }

        return null;
    }

    void Battle_Synergy_Init()
    {
        int cnt = Synergy_Panel.childCount;
        if (cnt == 0) return;
        for (int i=0; i<cnt; i++)
        {
            Transform tmp = Synergy_Panel.GetChild(0);
            LSY_Battle_Synergy.Synergy_Level _Level = tmp.GetComponent<LSY_Battle_Synergy>().Get_synergy_lv;

            switch(_Level)
            {
                case LSY_Battle_Synergy.Synergy_Level.Red:
                    tmp.SetParent(Synergy_Red);
                    tmp.localPosition = Vector3.zero;
                    break;
                case LSY_Battle_Synergy.Synergy_Level.Green:
                    tmp.SetParent(Synergy_Green);
                    tmp.localPosition = Vector3.zero;
                    break;
                case LSY_Battle_Synergy.Synergy_Level.Gray:
                    tmp.SetParent(Synergy_Gray);
                    tmp.localPosition = Vector3.zero;
                    break;
            }
        }

        //
    }


}


#endregion

