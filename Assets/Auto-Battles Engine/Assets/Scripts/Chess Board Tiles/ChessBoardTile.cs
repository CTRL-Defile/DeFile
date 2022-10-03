using UnityEngine;
using System.Collections;

namespace AutoBattles
{
    public class ChessBoardTile : MonoBehaviour
    {
        #region Variables
        [Header("Pawn Info")]
        [SerializeField]
        private GameObject _activePawn;

        [Header("Tile Info")]
        [SerializeField]
        private int _id;
        [SerializeField]
        private Vector2 _gridPosition;
        [SerializeField]
        private TileCategory _tileType;
        [SerializeField]
        private float _defaultPawnYRotation;

        [Header("Visual References")]
        [SerializeField]
        private GameObject _hoverHighlight;

        //References
        private PawnDragManager _pawnDragScript;
        private GameManager _gameManager;
        private ArmyManager _armyManagerScript;
        private UserInterfaceManager _uiManager;

        public enum TileCategory
        {
            None,
            Player,
            Enemy,
            Bench
        }
        #endregion

        #region Properties
        //this holds the info of the active pawn this tile currently owns
        public GameObject ActivePawn { get => _activePawn; protected set => _activePawn = value; }

        //this is set when the tile is created
        public int Id { get => _id; protected set => _id = value; }

        //this is the tiles position on the total chess board grid
        //used for movement calculating
        public Vector2 GridPosition { get => _gridPosition; protected set => _gridPosition = value; }

        //this is the type of tile
        public TileCategory TileType { get => _tileType; protected set => _tileType = value; }

        //This will be set at the time of Setup based on the TileCategory set at that time
        protected float DefaultPawnYRotation { get => _defaultPawnYRotation; set => _defaultPawnYRotation = value; }

        protected GameObject HoverHighlight { get => _hoverHighlight; set => _hoverHighlight = value; }

        //references
        protected PawnDragManager PawnDragScript { get => _pawnDragScript; set => _pawnDragScript = value; }
        protected GameManager GameManager { get => _gameManager; set => _gameManager = value; }
        protected ArmyManager ArmyManagerScript { get => _armyManagerScript; set => _armyManagerScript = value; }
        protected UserInterfaceManager UIManager { get => _uiManager; set => _uiManager = value; }

        #endregion

        #region Methods
        //Initialization
        protected virtual void Awake()
        {
            if (!HoverHighlight)
            {
                Debug.LogError("No hoverHighlight reference set on the ChessBoardTile script on Chess Board Tile prefab. Please drag 'Highlight' child on" +
                    "Chess Board Tile prefab into the ChessBoardTile script in the inspector.");
            }

            PawnDragScript = PawnDragManager.Instance;
            if (!PawnDragScript)
            {
                Debug.LogError("No PawnDragManager singleton instance found in the scene. Please add a PawnDragManager script to the Game Manager gameobject before entering playmode!");
            }

            ArmyManagerScript = ArmyManager.Instance;
            if (!ArmyManagerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. Please add a ArmyManager script to the Game Manager gameobject before entering playmode!");
            }

            GameManager = AutoBattles.GameManager.Instance;
            if (!GameManager)
            {
                Debug.LogError("No GameManager singleton instance found in the scene. Please add an AutoBattles.GameManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            UIManager = UserInterfaceManager.Instance;
            if (!UIManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add an AutoBattles.GameManager script to the game manager gameobject " +
                    "before entering playmode!");
            }
        }

        #region Used for setting up the tile at runtime
        public virtual void Setup(int id, Vector2 gridPosition, TileCategory tileType)
        {
            Id = id;

            GridPosition = gridPosition;

            TileType = tileType;

            SetDefaultYRotation(TileType);
        }

        private void SetDefaultYRotation(TileCategory tileCategory)
        {
            if (tileCategory == TileCategory.Bench)
            {
                DefaultPawnYRotation = 180f;
            }
            else if (tileCategory == TileCategory.Enemy)
            {
                DefaultPawnYRotation = 180f;
            }
            else if (tileCategory == TileCategory.Player)
            {
                DefaultPawnYRotation = 0f;
            }
        }
        #endregion

        //returns true if we currenly have an active pawn
        public virtual bool HasActivePawn()
        {
            if (ActivePawn != null)
                return true;
            else
                return false;
        }

        public virtual void ClearActivePawn()
        {
            ActivePawn = null;
        }

        //This is only called for 1 of 2 reasons,
        //1). the pawn has upgraded and we need to replace the old one
        //2). we bought a pawn from the shop and this tile currently doesnt have an active pawn
        public virtual void CreatePlayerPawn(PawnStats pawnStats, int goldCost)
        {
            //delete old (this will only happen when a pawn is upgrading)
            if (ActivePawn != null)
            {
                //TODO (maybe) replace this with an object pooling function
                //if we go that route
                Destroy(ActivePawn);
            }

            //Instantiate the actual pawn gameobject
            ActivePawn = Instantiate(pawnStats.pawn);

            //store this reference for multiple uses
            Status status = ActivePawn.GetComponent<Status>();

            //set to player pawn
            status.IsPlayer = true;

            //set what we paid for this particular pawn
            status.GoldWorth = goldCost;

            //add this newly created pawn to the players total roster
            ArmyManagerScript.AddPawnToTotalPlayerRoster(ActivePawn);

            //still call ChangePawn() because we want to be sure to set
            //this newly created pawn up the same way as all other existing
            //pawns would be when entering this tile
            ChangePawnOutOfCombat(ActivePawn);
        }


        //this function will be called when swapping pawns around the chessboard or bench
        //out of combat
        public virtual void ChangePawnOutOfCombat(GameObject newPawn)
        {
            //if we are changing this slot to no pawn
            //clear the active pawn and exit the function
            if (newPawn == null)
            {
                ClearActivePawn();
                return;
            }

            //this will set the pawns home base for later use when
            //dragging or after a round of combat has ended
            newPawn.GetComponent<HomeBase>().SetHomeBase(this);

            //let the pawn know which tile it is currently residing on
            //this is seperate from the homebase
            newPawn.GetComponent<Movement>().SetCurrentTileOutOfCombat(this);

            //change our active pawn
            ActivePawn = newPawn;

            //update new pawns position
            ActivePawn.transform.position = transform.position;

            //update new pawns rotation
            ActivePawn.transform.rotation = Quaternion.Euler(0, DefaultPawnYRotation, 0);
        }

        public virtual void ChangePawnInCombat(GameObject newPawn)
        {
            //if we are changing this slot to no pawn
            //clear the active pawn and exit the function
            if (newPawn == null)
            {
                ClearActivePawn();
                return;
            }

            //let the pawn know which tile it is currently residing on
            //this is seperate from the homebase, if the pawn had an old tile
            //this function call will also let that tile know the pawn moved
            newPawn.GetComponent<Movement>().SetCurrentTileInCombat(this);

            //change our active pawn
            ActivePawn = newPawn;
        }
        #endregion

        #region Mouse Functions

        protected virtual void OnMouseEnter()
        {
            //on mouse enter this specific tile, enable the tile highlight
            HoverHighlight.SetActive(true);

            //tell the PawnDragManager script that we are hovered over this specific tile
            PawnDragScript.SetHoveredTile(this);
        }

        protected virtual void OnMouseExit()
        {
            //when mouse exits this specific tile, turn off the tile hightlight
            HoverHighlight.SetActive(false);

            //tell the PawnDragManager script that we are no longer hovered over this tile
            PawnDragScript.SetHoveredTile(null);
        }

        protected virtual void OnMouseDown()
        {
            //let the pawn drag manager script know we just clicked on
            //this tile/pawn
            PawnDragScript.ClickPawn(ActivePawn, this);

            /*
            //we dont want to be able to pick up enemy pawns
            if (TileType == TileCategory.Enemy)
                return;

            //if we are in combat and we are a player chessboard tile, do nothing
            if (GameManager.InCombat && TileType == TileCategory.Player)
                return;

            //if this tile currently has a pawn
            if (ActivePawn != null)
            {
                //tell the pawn drag script to begin dragging this tiles pawn
                PawnDragScript.DragPawn(ActivePawn, this);
            }*/
        }       

        

        #endregion
    }
}

