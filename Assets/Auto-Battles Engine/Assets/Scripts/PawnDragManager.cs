using UnityEngine;

/// <summary>
/// This script is responsible for all activies regarding dragging  
/// and dropping pawns either from chess board tiles to other chess 
/// board tiles or from the bench to chessboard tiles and vice versa
/// also updates the ArmyManager script with the current army size
/// 
/// This script is also responsible for opening the stat window
/// if we click on any pawn but do not drag
/// </summary>

namespace AutoBattles
{
    public class PawnDragManager : Singleton<PawnDragManager>
    {
        

        #region Variables       
        [SerializeField]
        private GameObject _pawnClickedOn;
        [SerializeField]
        private GameObject _pawnBeingDragged;
        [SerializeField]
        private Transform _pawnTransform;
        [SerializeField]
        private ChessBoardTile _tileClickedOn;
        [SerializeField]
        private ChessBoardTile _tileDraggedFrom;
        [SerializeField]
        private ChessBoardTile _tileHoveredOver;
        [SerializeField]
        private bool _hoveredOnTrash;

        [Header("Mouse Position for Drag")]
        [SerializeField]
        private Vector3 _lastMousePosition = Vector3.zero;
        [SerializeField]
        private bool _isDraggingMouse;
        [SerializeField]
        private bool _firstTouchTaken;

        [Header("Variables")]
        [SerializeField]
        [Tooltip("This is how far away from the camera the dragged pawn will be displayed. If not set in the inspector at runtime it will default to 25f")]
        private float _zCameraOffset;

        //references
        private Camera _myCamera;
        private ArmyManager _armyManangerScript;
        private GameManager _gameManager;
        private GoldManager _goldManagerScript;
        private UserInterfaceManager _userInterface;
        private BenchManager _benchManagerScript;
        private StatsPanel _statsPanelScript;
        private SynergyManager _synergyManagerScript;
        #endregion

        #region Properties
        //this holds the reference to the pawn we clicked on
        protected GameObject PawnClickedOn { get => _pawnClickedOn; set => _pawnClickedOn = value; }

        //this holds the reference to the pawn currently being dragged
        protected GameObject PawnBeingDragged { get => _pawnBeingDragged; set => _pawnBeingDragged = value; }

        //reference for the pawn being dragged's transform
        protected Transform PawnTransform { get => _pawnTransform; set => _pawnTransform = value; }

        //this is the tile we originally clicked on
        protected ChessBoardTile TileClickedOn { get => _tileClickedOn; set => _tileClickedOn = value; }

        //this is the tile script we dragged from originally
        protected ChessBoardTile TileDraggedFrom { get => _tileDraggedFrom; set => _tileDraggedFrom = value; }

        //this is the tile we are currently hovered over
        protected ChessBoardTile TileHoveredOver { get => _tileHoveredOver; set => _tileHoveredOver = value; }

        //this will be true if we are currently hovering over the trash can button
        public bool HoveredOnTrash { get => _hoveredOnTrash; set => _hoveredOnTrash = value; }

        //for determining if we have moved the mouse for dragging a pawn
        protected Vector3 LastMousePosition { get => _lastMousePosition; set => _lastMousePosition = value; }

        //is true the frame after we dragged the mouse
        protected bool IsDraggingMouse { get => _isDraggingMouse; set => _isDraggingMouse = value; }

        //is true the first frame after we have touched the screen,
        //used to store the intial touch position to know wether or not we ahve dragged
        protected bool FirstTouchTaken { get => _firstTouchTaken; set => _firstTouchTaken = value; }

        //this is how far away from the camera the dragged pawn will be displayed
        //if not set in the inspector at runtime it will default to 25f
        protected float ZCameraOffset { get => _zCameraOffset; set => _zCameraOffset = value; }

        //references
        protected Camera MyCamera { get => _myCamera; set => _myCamera = value; }
        protected ArmyManager ArmyManangerScript { get => _armyManangerScript; set => _armyManangerScript = value; }
        protected GameManager GameManager { get => _gameManager; set => _gameManager = value; }
        protected GoldManager GoldManagerScript { get => _goldManagerScript; set => _goldManagerScript = value; }
        protected UserInterfaceManager UserInterface { get => _userInterface; set => _userInterface = value; }
        protected BenchManager BenchManagerScript { get => _benchManagerScript; set => _benchManagerScript = value; }
        protected StatsPanel StatsPanelScript { get => _statsPanelScript; set => _statsPanelScript = value; }
        protected SynergyManager SynergyManagerScript { get => _synergyManagerScript; set => _synergyManagerScript = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            MyCamera = Camera.main;

            ArmyManangerScript = ArmyManager.Instance;
            if (!ArmyManangerScript)
            {
                Debug.LogError("No ArmyManager singleton instance found in the scene. Please add an ArmyManager component to the Game Manager gameobject before entering playmode!");
            }

            SynergyManagerScript = SynergyManager.Instance;
            if (!SynergyManagerScript)
            {
                Debug.LogError("No SynergyManager singleton instance found in the scene. Please add an ArmyManager component to the Game Manager gameobject before entering playmode!");
            }

            GameManager = AutoBattles.GameManager.Instance;
            if (!GameManager)
            {
                Debug.LogError("No GameManager singleton instance found in the scene. Please add an GameManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            GoldManagerScript = GoldManager.Instance;
            if (!GoldManagerScript)
            {
                Debug.LogError("No GoldManager singleton instance found in the scene. Please add a GoldManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            UserInterface = UserInterfaceManager.Instance;
            if (!UserInterface)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add a UserInterfaceManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            BenchManagerScript = BenchManager.Instance;
            if (!BenchManagerScript)
            {
                Debug.LogError("No BenchManager singleton instance found in the scene. Please add a BenchManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            StatsPanelScript = StatsPanel.Instance;
            if (!StatsPanelScript)
            {
                Debug.LogError("No StatsPanel singleton instance found in the scene. Please add a BenchManager script to the game manager gameobject " +
                    "before entering playmode!");
            }

            //check if we set any offset, if not default to 25
            if (ZCameraOffset == 0)
            {
                ZCameraOffset = 25f;
            }
        }

        //this will make our pawn we are dragging follow the mouse
        protected virtual void Update()
        {

            //this will cause the code within the if statement to only be compiled in the
            //android and IOS builds
            //for more info visit https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
#if UNITY_IOS || UNITY_ANDROID

            //see if we currently have any touches on the screen
            if (Input.touchCount > 0)
            {
                //grab the first touches info
                Touch touch = Input.GetTouch(0);

                //see if this is the first frame we touched the screen
                if (!FirstTouchTaken)
                {
                    //if it is, save the position
                    LastMousePosition = touch.position;

                    //stop checking for first touch
                    FirstTouchTaken = true;
                }

                //set our vector2 touch.Pos to a new vector3
                Vector3 touchPos = new Vector3(touch.position.x, touch.position.y, 0);

                //grab the difference between the vectors
                Vector3 touchDelta = touchPos - LastMousePosition;

                //get the magnitude of our new vector3
                float mobileMag = touchDelta.sqrMagnitude;

                //compare it to our sensitivity
                if (mobileMag >= 50)
                {
                    //we are dragging
                    IsDraggingMouse = true;
                }
            }
            else
            {
                //if not touches on the screen, reset this to look
                //for new first touch
                FirstTouchTaken = false;

                //reset this aswell
                IsDraggingMouse = false;
            }

#endif

            //this will cause the code within the if statement to only be compiled in the 
            //editor, standalone (windows, mac, linux), and webGL builds
            //for more info visit https://docs.unity3d.com/Manual/PlatformDependentCompilation.html
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL

            #region Mouse Dragging
            //use to determine if the mouse has moved since last frame
            Vector3 mouseDelta = Input.mousePosition - LastMousePosition;

            //grab the sqr magnitude of the mouse delta to see if we moved
            float magnitude = mouseDelta.sqrMagnitude;            

            //check the magnitude against the desired sensitivity
            if (magnitude >= 2)
            {
                //we moved the mouse since last frame               
                IsDraggingMouse = true;
            }
            else
            {
                //we didnt drag the mouse
                IsDraggingMouse = false;
            }

            //update lastMousePosition for next frame
            LastMousePosition = Input.mousePosition;
#endregion

#endif

            //we have pressed the mouse button down on a friendly or enemy pawn 
            //and we are not currently dragging a pawn
            if (PawnClickedOn != null && PawnBeingDragged == null)
            {
                //is bench pawn
                if (TileClickedOn.TileType == ChessBoardTile.TileCategory.Bench)
                {
                    //check if we dragged the mouse last frame
                    if (IsDraggingMouse)
                    {
                        //set our pawn to be dragged
                        DragPawn(PawnClickedOn, TileClickedOn);

                        //clear the clicked properties
                        ClearClickedPawn();

                        //return so we dont open the stat window
                        return;
                    }

                    //if we press up on the mouse before dragging begins
                    //we want to open the stat window for this pawn
                    if (Input.GetMouseButtonUp(0))
                    {
                        OpenStatsWindow();
                    }
                }
                //if this is a player pawn that is on the chess board, 
                //we do everything as we did for a bench pawn except check if we are
                //in combat so that we are not dragging pawns while in combat
                else if (TileClickedOn.TileType == ChessBoardTile.TileCategory.Player)
                {
                    //check if we dragged the mouse last frame
                    if (IsDraggingMouse)
                    {
                        //check for combat
                        if (!GameManager.InCombat)
                        {
                            //set our pawn to be dragged
                            DragPawn(PawnClickedOn, TileClickedOn);

                            //clear the clicked properties
                            ClearClickedPawn();

                            //return so we dont open the stat window
                            return;
                        }                        
                    }

                    //if we press up on the mouse before dragging begins
                    //we want to open the stat window for this pawn
                    if (Input.GetMouseButtonUp(0))
                    {
                        OpenStatsWindow();
                    }
                }
                //is enemy pawn
                else
                {
                    //when we press up, open the stat window for the enemy pawn
                    if (Input.GetMouseButtonUp(0))
                    {
                        OpenStatsWindow();
                    }
                }
            }

            //only runs if we have a currently active pawn being dragged
            if (PawnBeingDragged != null)
            {
                //while we are dragging a pawn, open the trash panel
                UserInterface.TrashPanel.Open();

                SendPawnToMousePosition();                

                //This will track once we have let up the mouse 0 button (or our finger for mobile)
                //and will appropriately send the dragged pawn to the new correct position
                if (Input.GetMouseButtonUp(0))
                {                    
                    //check if we are hovering over the trash panel
                    if (HoveredOnTrash)
                    {
                        //check where we dragged from
                        //this is important because if we dragged from the board
                        //we will need to update our active army roster
                        if (TileDraggedFrom.TileType == ChessBoardTile.TileCategory.Player)
                        {
                            //sell the dragged pawn with a 'true' to let it know we
                            //need to remove the pawn from the active player roster
                            SellDraggedPawn(true);                            
                        }
                        //we dragged from the bench, just delete the pawn and refund the gold
                        else
                        {
                            //sell the dragged pawn with a 'false' to let it know we
                            //dont need to update the active player roster
                            SellDraggedPawn(false);
                        }                        
                    }
                    else
                    {
                        //we are not hovered over a tile
                        if (TileHoveredOver == null)
                        {
                            SendPawnBack();
                        }
                        else
                        {
                            //DO THIS FIRST so we dont need to check multiple times later
                            //if we are currently hovered over the enemies side of the chess board
                            //send the tile right back to where it was dragged from
                            if (TileHoveredOver.TileType == ChessBoardTile.TileCategory.Enemy)
                            {
                                SendPawnBack();
                            }
                            else
                            {
                                //if we were originally dragging from the bench
                                if (TileDraggedFrom.TileType == ChessBoardTile.TileCategory.Bench)
                                {
                                    //and we are now also hovered over another bench slot
                                    if (TileHoveredOver.TileType == ChessBoardTile.TileCategory.Bench)
                                    {
                                        //first check if we are simply hovered over the same tile
                                        //that we just dragged from
                                        if (TileHoveredOver.Id == TileDraggedFrom.Id)
                                        {
                                            SendPawnBack();
                                        }
                                        else
                                        {
                                            SwapPawns();
                                        }
                                    }
                                    // and we are not hovered over the chess board
                                    else if (TileHoveredOver.TileType == ChessBoardTile.TileCategory.Player)
                                    {
                                        // if we are attempting to drag a piece from the bench onto the 
                                        //chessboard during combat, send the pawn back and return
                                        if (GameManager.InCombat)
                                        {
                                            SendPawnBack();
                                            return;
                                        }

                                        //this will check if we need to update the army manager for any 
                                        //roster changes, call this first before we do any swapping of pawns
                                        SyncArmyChangesFromBench();

                                        SwapPawns();
                                    }

                                }
                                //if we were originally dragging from the chess board
                                else if (TileDraggedFrom.TileType == ChessBoardTile.TileCategory.Player)
                                {
                                    //and we are not also hovered over a player chess board piece
                                    if (TileHoveredOver.TileType == ChessBoardTile.TileCategory.Player)
                                    {
                                        //first check if we are simply hovered over the same tile
                                        //that we just dragged from
                                        if (TileHoveredOver.Id == TileDraggedFrom.Id)
                                        {
                                            SendPawnBack();
                                        }
                                        else
                                        {
                                            SwapPawns();
                                        }
                                    }
                                    //and we are not hovered over a bench slot
                                    else if (TileHoveredOver.TileType == ChessBoardTile.TileCategory.Bench)
                                    {
                                        //this will check if we need to update the army manager for any 
                                        //roster changes, call this first before we do any swapping of pawns
                                        SyncArmyChangesFromBoard();

                                        //use this variation because we just took pawns from the board
                                        //and added them to the bench, check if we just made any combinations
                                        SwapPawnsWithComboCheck();
                                    }
                                }
                            }
                        }
                    }

                    //after we have let up on the mouse button
                    //we should no longer be dragging a pawn so close the trash panel
                    UserInterface.TrashPanel.Close();
                }
            }
        }

        //called when a pawn is clicked on but not dragged
        protected virtual void OpenStatsWindow()
        {
            //open stat window
            UserInterface.StatsPanel.Open();

            //Let the stats panel know to update what stats
            //it is displaying
            StatsPanelScript.DisplayNewPawnStats(PawnClickedOn);

            //reset clicked pawn
            ClearClickedPawn();
        }

        //called when we start dragging a pawn or when the X is clicked on the panel itself
        public virtual void CloseStatsWindow()
        {
            UserInterface.StatsPanel.Close();

            StatsPanelScript.StopStatTick();

            ClearClickedPawn();
        }

        //called when we click on a friendly or enemy pawn
        public virtual void ClickPawn(GameObject pawn, ChessBoardTile tileScript)
        {
            PawnClickedOn = pawn;

            TileClickedOn = tileScript;            
        }

        //called after we are done clicking on a pawn
        //reset all related clicking properties
        public virtual void ClearClickedPawn()
        {
            PawnClickedOn = null;

            TileClickedOn = null;            
        }

        //return true or false based on wether we currently have a pawn clicked on
        public virtual bool IsClickedOnPawn()
        {
            if (PawnClickedOn == null)
                return false;
            else
                return true;
        }

        //this is called when we are ready to drag a new pawn
        public virtual void DragPawn(GameObject pawn, ChessBoardTile tileScript)
        {
            PawnTransform = pawn.transform;

            PawnBeingDragged = pawn;

            TileDraggedFrom = tileScript;

            CloseStatsWindow();
        }

        //this will return a bool based on whether we are currently dragging a pawn or not
        public virtual bool IsDraggingPawn()
        {
            if (PawnBeingDragged == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //this is called when we need to set the tile currently hovered over
        public virtual void SetHoveredTile(ChessBoardTile tileScript)
        {
            TileHoveredOver = tileScript;
        }

        //this will reset all variables having to do with dragging a pawn
        //after the drag is completed
        public virtual void Clear()
        {
            PawnBeingDragged = null;

            PawnTransform = null;

            TileDraggedFrom = null;
        }

        //this function will cause our dragged pawn to follow the mouse cursor / finger (on mobile)
        //TODO make sure this works on mobile
        protected virtual void SendPawnToMousePosition()
        {
            Vector3 point = new Vector3();

            Vector2 mousePos = Input.mousePosition;

            point = MyCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 25));

            PawnTransform.position = Vector3.Lerp(PawnTransform.position, point, 20 * Time.deltaTime);
        }

        //this will send the pawn back to the original place it was dragged from
        public virtual void SendPawnBack()
        {
            //send this pawn right back to the tile it was dragged from
            TileDraggedFrom.ChangePawnOutOfCombat(PawnBeingDragged);

            //reset drag variables
            Clear();
        }

        //Called when we want to swap two pawns that are either both currently on the bench or both on the chessboard
        protected virtual void SwapPawns()
        {
            //change tile we dragged from first since we already have a reference to that
            //tiles pawn saved in this script       
            TileDraggedFrom.ChangePawnOutOfCombat(TileHoveredOver.ActivePawn);

            //change tile being hovered over
            TileHoveredOver.ChangePawnOutOfCombat(PawnBeingDragged);

            Clear();
        }

        //same as 'SwapPawns' but also checks if we just made any combinations on the bench
        //Called when we want to swap two pawns when one is on the bench and one is on the chessboard
        protected virtual void SwapPawnsWithComboCheck()
        {
            //change tile we dragged from first since we already have a reference to that
            //tiles pawn saved in this script

            TileDraggedFrom.ChangePawnOutOfCombat(TileHoveredOver.ActivePawn);

            //change tile being hovered over
            TileHoveredOver.ChangePawnOutOfCombat(PawnBeingDragged);

            //check if we just made a combination        
            BenchManagerScript.CheckForCombination(PawnBeingDragged.GetComponent<Pawn>().Stats);

            Clear();
        }


        //called when we are finished dragging FROM the bench TO the players side of
        //the chess board SPECIFICALLY
        protected virtual void SyncArmyChangesFromBench()
        {
            //we began the drag from the bench which means
            //we defintely have a pawn to give to the chess board
            //check if we are replacing an already existing piece
            //on the chess board
            if (TileHoveredOver.HasActivePawn())
            {
                //removes the pawn being hovered over because it is about to be swapped to the bench
                ArmyManangerScript.RemoveActivePawnFromPlayerRoster(TileHoveredOver.ActivePawn);

                //remove this pawns active synergies
                SynergyManagerScript.PawnOutOfPlay(TileHoveredOver.ActivePawn.GetComponent<Pawn>().Stats);
            }

            //adds the pawn we are currently dragging from the bench to the active player roster
            ArmyManangerScript.AddActivePawnToPlayerRoster(PawnBeingDragged);

            //add this pawns active synergies
            SynergyManagerScript.PawnInPlay(PawnBeingDragged.GetComponent<Pawn>().Stats);
        }

        //called when we are finished dragging FROM the player side of chess board
        //TO the bench SPECIFICALLY
        protected virtual void SyncArmyChangesFromBoard()
        {
            //we began the drag from the chess board which means
            //we defintely have a pawn to give to the bench
            //check if we are replacing an already existing piece
            //on the bench
            if (TileHoveredOver.HasActivePawn())
            {
                //add the pawn from the bench we were hovered over to the roster because 
                //it is about to be on the chess board
                ArmyManangerScript.AddActivePawnToPlayerRoster(TileHoveredOver.ActivePawn);

                //add this pawns active synergies 
                SynergyManagerScript.PawnInPlay(TileHoveredOver.ActivePawn.GetComponent<Pawn>().Stats);
            }

            //remove the pawn we were dragging from the chess board as it is now on the bench
            ArmyManangerScript.RemoveActivePawnFromPlayerRoster(PawnBeingDragged);

            //remove this pawns active synergies
            SynergyManagerScript.PawnOutOfPlay(PawnBeingDragged.GetComponent<Pawn>().Stats);
        }

        protected virtual void SellDraggedPawn(bool draggedFromBoard)
        {
            //do this only if we dragged this pawn from the chess board
            if (draggedFromBoard)
            {
                //let the army manager know it just lost a pawn from its active roster
                ArmyManangerScript.RemoveActivePawnFromPlayerRoster(PawnBeingDragged);                
            }

            //remove the synergies
            SynergyManagerScript.PawnSold(PawnBeingDragged.GetComponent<Pawn>().Stats, draggedFromBoard);

            //Remove the pawn from the total player roster
            ArmyManangerScript.RemovePawnFromTotalPlayerRoster(PawnBeingDragged);

            //grab a reference to the status script atteched to this pawn
            Status status = PawnBeingDragged.GetComponent<Status>();

            //give the player the same gold he payed for the pawn originally
            GoldManagerScript.GainGold(status.GoldWorth);

            //destroy the pawn
            status.SelfDestruct();

            //stop dragging the pawn
            Clear();
        }

#endregion
    }
}

