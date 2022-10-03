using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script will set up the chess board scripts and store them for later 
/// reference by pawns for calculations of movement, targeting, etc.
/// 
/// Altering the size of the chessboard size (overall scale and/or scale of 
///  individual tiles) will result in unknown and undesirable behaviour
///  
/// Chess board can start anywhere in the game world so long as no changes
///  to its size are made
///  
/// Chess board tiles are 5 units away from each other on the x (left and right)
/// and x (up and down) axis starting at 0,0 in the bottom left corner and ending
///  at 35, 35 in the top right corner
/// </summary>

namespace AutoBattles
{
    public class ChessBoardManager : Singleton<ChessBoardManager>
    {
       
        #region Variables
        [Header("Chess Board")]
        [SerializeField]
        private Transform _chessBoardTransform;
        private Vector3 _chessBoardStartPos;

        [Header("Chess Board Tile Prefabs")]
        [SerializeField]
        private GameObject _playerChessBoardTilePrefab;
        [SerializeField]
        private GameObject _enemyChessBoardTilePrefab;

        [Header("Active Chess Board Tile Scripts")]
        [SerializeField]
        private List<ChessBoardTile> _chessBoardTiles = new List<ChessBoardTile>();

        private ArmyManager _armyManagerScript;
        #endregion

        #region Properties
        //We will use this reference to map our chessboard tiles
        //to fit over the board regardless of world start position
        protected Transform ChessBoardTransform { get => _chessBoardTransform; set => _chessBoardTransform = value; }

        //This reference will ensure we have the correct starting
        //position for later calculations
        protected Vector3 ChessBoardStartPos { get => _chessBoardStartPos; set => _chessBoardStartPos = value; }

        //this will be where we store all our chess board tile scripts
        //so pawns can do calculations like movement and targeting
        //and so we can move the pawns around the board
        public List<ChessBoardTile> ChessBoardTiles { get => _chessBoardTiles; protected set => _chessBoardTiles = value; }

        //this is the tile prefab reference for creating the player side 
        //of the chessboard
        protected GameObject PlayerChessBoardTilePrefab { get => _playerChessBoardTilePrefab; set => _playerChessBoardTilePrefab = value; }

        //this is the tile prefab reference for creating the enemy side 
        //of the chessboard
        protected GameObject EnemyChessBoardTilePrefab { get => _enemyChessBoardTilePrefab; set => _enemyChessBoardTilePrefab = value; }

        //REFERENCES
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        #endregion

        #region Methods

        //INITIALIZATION
        protected virtual void Awake()
        {
            //Check our references 
            if (!ChessBoardTransform)
            {
                Debug.LogError("Please drag your chess board transform into the ChessBoardManager script located on the Game Manager gameobject.");
                return;
            }

            if (!PlayerChessBoardTilePrefab)
            {
                Debug.LogError("No player chess board tile prefab set in the ChessBoardManager script located on the Game Manager gameobject.");
                return;
            }

            if (!EnemyChessBoardTilePrefab)
            {
                Debug.LogError("No enemy chess board tile prefab set in the ChessBoardManager script located on the Game Manager gameobject.");
                return;
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager script found, please add one to the game manager gameobject.");
            }

            //Create the Chess Board Tiles
            CreateChessBoardTiles();
        }

        //Creating the chess board at the start of runtime
        protected virtual void CreateChessBoardTiles()
        {
            int counter = 0;

            for (int z = 0; z < 8; z++)
            {
                for (int x = 0; x < 8; x++)
                {
                    //if z is less than 4, we are still making the bottom half of the chess board
                    //these tiles need to be player chess board tiles
                    if (z < 4)
                    {
                        //create tile and set chess board as parent and store as a temporary game object
                        GameObject tile = Instantiate(PlayerChessBoardTilePrefab, ChessBoardTransform);

                        //name it for use in the inspector
                        tile.name = "Player Tile " + counter.ToString() + " Position: (" + x.ToString() + ", " + z.ToString() + ")";

                        //set tile position
                        tile.transform.localPosition = new Vector3(x * 5, 0, z * 5);

                        //Grab the tile script from the gameobject we just created
                        ChessBoardTile tileScript = tile.GetComponent<ChessBoardTile>();

                        //setup the tilescript
                        tileScript.Setup(counter, new Vector2(x, z), ChessBoardTile.TileCategory.Player);

                        //Add the tile script on our newly created gameobject for later reference
                        ChessBoardTiles.Add(tileScript);
                    }
                    //once z has passed iteration 4, we are making the top half of the chess board
                    //these tiles will need to be enemy chess board tiles
                    else
                    {
                        //create tile and set chess board as parent and store as a temporary game object
                        GameObject tile = Instantiate(EnemyChessBoardTilePrefab, ChessBoardTransform);

                        //name it for use in the inspector
                        tile.name = "Enemy Tile " + counter.ToString() + " Position: (" + x.ToString() + ", " + z.ToString() + ")";

                        //set tile position
                        tile.transform.localPosition = new Vector3(x * 5, 0, z * 5);

                        //Grab the tile script from the gameobject we just created
                        ChessBoardTile tileScript = tile.GetComponent<ChessBoardTile>();

                        //setup the tilescript
                        tileScript.Setup(counter, new Vector2(x, z), ChessBoardTile.TileCategory.Enemy);

                        //Add the tile script on our newly created gameobject for later reference
                        ChessBoardTiles.Add(tileScript);
                    }

                    //increment our counter
                    counter++;
                }
            }
        }

        #endregion
    }
}

