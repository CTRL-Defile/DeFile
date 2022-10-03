using System.Collections.Generic;
using UnityEngine;

namespace AutoBattles
{    
    public class Movement : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField]
        private bool usingAnimations = false;
        [SerializeField]
        private string _movementBoolString = "Moving";

        #region Variables
        [Header("Positioning")]
        [SerializeField]
        private ChessBoardTile _currentTile;

        private ChessBoardTile _previousTile;
        private ChessBoardTile _previousTile2;
        private ChessBoardTile _previousTile3;

        [SerializeField]
        private Vector2 _gridPosition;
        [SerializeField]
        private Vector3 _desiredPosition;

        [Header("Variables")]
        private bool _moving;

        //references
        private Targeting _targetingScript;
        private Pawn _pawnScript;
        private ChessBoardManager _boardManagerScript;
        private Animator _anim;
        #endregion

        #region Properties   
        //what string we call to affect the moving parameter of our animator controller
        public string MovementBoolString { get => _movementBoolString; set => _movementBoolString = value; }

        //if we are currently moving or not
        protected bool Moving { get => _moving; set => _moving = value; }

        //this is the tile the pawn currently resides on
        public ChessBoardTile CurrentTile { get => _currentTile; set => _currentTile = value; }

        //store this for movement calculations to avoid walking back and forth between the same two tiles
        protected ChessBoardTile PreviousTile { get => _previousTile; set => _previousTile = value; }
        protected ChessBoardTile PreviousTile2 { get => _previousTile2; set => _previousTile2 = value; }
        protected ChessBoardTile PreviousTile3 { get => _previousTile3; set => _previousTile3 = value; }

        //this is our current position on the 'grid'
        public Vector2 GridPosition { get => _gridPosition; set => _gridPosition = value; }

        //this is the position in world space we currently want
        protected Vector3 DesiredPosition { get => _desiredPosition; set => _desiredPosition = value; }

        //references
        protected Targeting TargetingScript { get => _targetingScript; set => _targetingScript = value; }
        protected Pawn PawnScript { get => _pawnScript; set => _pawnScript = value; }
        protected ChessBoardManager BoardManagerScript { get => _boardManagerScript; set => _boardManagerScript = value; }
        protected Animator Anim { get => _anim; set => _anim = value; }


        #endregion

        #region Methods

        protected virtual void Awake()
        {
            BoardManagerScript = ChessBoardManager.Instance;
            if (!BoardManagerScript)
            {
                Debug.LogError("No ChessBoardManager singleton instance found in the scene. Please add a ChessBoardManager script to the GameManager gameobject before" +
                    "entering playmode!");
            }

            //cache references for later use
            TargetingScript = GetComponent<Targeting>();
            PawnScript = GetComponent<Pawn>();

            //Only set reference to animator if we are using animations
            if (usingAnimations)
            {
                Anim = GetComponent<Animator>();
                if (!Anim)
                {
                    Debug.LogError("AutoAttack script has 'useAnimations' set to true but does not have an Animator component.");
                }

                if (MovementBoolString == "")
                {
                    Debug.LogWarning("No 'MovementBoolString' set in the AutoAttack script for the " + gameObject.name + " pawn. Please set the name of the trigger " +
                        "in the corresponding animator componenet or disable animations on the prefab");
                }
            }
        }
        
        protected virtual void Update()
        {
            if (Moving)
            {
                //store the distance from our desired position
                float distance = Vector3.Distance(transform.position, DesiredPosition);

                //if we are within 0.02f of our desired position, do something
                if (distance < 0.02f)
                {
                    Moving = false;

                    //now that we have moved closer, check our range to make
                    //our next move
                    TargetingScript.SearchForNewTarget();
                }
                //otherwise, move towards our desired position
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, DesiredPosition, PawnScript.MoveSpeed * Time.deltaTime);
                }
            }

            if (usingAnimations)
            {
                Anim.SetBool("Moving", Moving);
            }
        }

        public virtual void SetCurrentTileOutOfCombat(ChessBoardTile tile)
        {
            if (CurrentTile != null)
            {
                if (CurrentTile.ActivePawn == gameObject)
                {
                    CurrentTile.ClearActivePawn();
                }
            }

            PreviousTile3 = PreviousTile2;

            PreviousTile2 = PreviousTile;

            //set previous tile
            PreviousTile = CurrentTile;

            //set a reference to our current tile script
            CurrentTile = tile;

            //set our grid position 
            GridPosition = tile.GridPosition;

            //set our desired position
            DesiredPosition = tile.transform.position;
        }

        public virtual void SetCurrentTileInCombat(ChessBoardTile tile)
        {
            //if we had an old tile, let it know we have moved on
            if (CurrentTile != null)
            {
                CurrentTile.ClearActivePawn();
            }

            PreviousTile3 = PreviousTile2;

            PreviousTile2 = PreviousTile;

            //set previous tile
            PreviousTile = CurrentTile;

            //set a reference to our current tile script
            CurrentTile = tile;

            //set our grid position 
            GridPosition = tile.GridPosition;

            //set our desired position
            DesiredPosition = tile.transform.position;
        }

        //used when calculating movement
        //to compare tiles and their distance to a specific target
        protected struct TileMovementData
        {
            public ChessBoardTile tile;
            public float distance;
        }

        //when called, this will move us 1 tile closer to our target
        public virtual void MoveOneTileToTarget()
        {
            ChessBoardTile tile = GetNearestTileClosestToTarget();

            if (tile != null)
            {
                tile.ChangePawnInCombat(gameObject);

                Moving = true;

                RotatePawn(tile.transform);
            }
            else
            {
                //we retuned a null tile from GetNearestTileClosestToTarget(), so do nothing
            }
        }

        //this function will search all tiles 1 away and return the one that is 
        //currently not used by an active pawn and is closest to our target
        public virtual ChessBoardTile GetNearestTileClosestToTarget()
        {
            //if this gets called but we dont have a target, exit the function
            if (TargetingScript.Target == null)
                return null;

            //first make a list of the tiles current surrounding us
            //we will generate the list from least distance to target first 
            //and most distance to target at the end of the list
            List<TileMovementData> nearbyTiles = new List<TileMovementData>();

            //grab the tiles around us starting from the the bottom right (-1, -1)
            //and ending in the top right (1, 1)
            for (int z = -1; z <= 1; z++)
            {
                int gridPositionZ = (int)GridPosition.y + z;

                //this will check if we are trying to check beyond the bounds of the
                //chess board on the x axis
                if (gridPositionZ < 0 || gridPositionZ >= 8)
                    continue;

                for (int x = -1; x <= 1; x++)
                {
                    int gridPositionX = (int)GridPosition.x + x;

                    //this will check if we are trying to check beyond the bounds of the
                    //chess board on the z axis
                    if (gridPositionX < 0 || gridPositionX >= 8)
                        continue;

                    int id = ConvertGridPositionToTileId(new Vector2(gridPositionX, gridPositionZ));

                    //pull the tile from the chessboardmanager script that we are trying to check
                    ChessBoardTile tile = BoardManagerScript.ChessBoardTiles[id];

                    //////
                    Vector2 dir = TargetingScript.TargetsMovementScript.GridPosition - tile.GridPosition;

                    float distanceSqrMag = Vector2.SqrMagnitude(dir);

                    ////////
                    //get the distance from the target this particular tile is
                    float distance = Vector3.Distance(TargetingScript.Target.transform.position, tile.transform.position);

                    //once we have the tile and the distance we can make a new TileMovementData variable
                    TileMovementData tileData = new TileMovementData();
                    tileData.tile = tile;
                    tileData.distance = distanceSqrMag;

                    //add to our list in ascending order of distance to target
                    //if we dont have anything to check against, just add the data
                    if (nearbyTiles.Count == 0)
                    {
                        nearbyTiles.Add(tileData);
                    }
                    //otherwise, cycle through the list and add in at the position when
                    //we have found a tiledata that has a greater distance than our current tiledata
                    else
                    {
                        bool inserted = false;

                        for (int i = 0; i < nearbyTiles.Count; i++)
                        {
                            //check the distance of our current tile data vs the current iteration
                            //of the nearbyTiles list
                            if (tileData.distance < nearbyTiles[i].distance)
                            {
                                //if we found one that greater than our current distance
                                //add our current tile data into this tiles slot
                                nearbyTiles.Insert(i, tileData);

                                inserted = true;

                                break;
                            }
                        }

                        if (!inserted)
                        {
                            //if we got here that means our current tile has the greatest distance
                            //of all previously checked tiles, put it at the back
                            nearbyTiles.Add(tileData);
                        }
                    }
                }
            }

            //At this point we should have a list of tileMovementData in ascending order of
            //least amount of distance to our current target

            //now we should iterate that list until we find a tile that isnt already occupied with
            //an active pawn
            for (int i = 0; i < nearbyTiles.Count; i++)
            {
                //this will prevent the pawn from traveling to the previous 3 most recent tiles it has already been too
                if (PreviousTile != null && PreviousTile2 != null && PreviousTile3 != null)
                {
                    if (nearbyTiles[i].tile == PreviousTile || nearbyTiles[i].tile == PreviousTile2 || nearbyTiles[i].tile == PreviousTile3)
                    {
                        continue;
                    }
                }

                if (nearbyTiles[i].tile.ActivePawn == null)
                {
                    //we have a winner
                    return nearbyTiles[i].tile;
                }
            }

            //if we have reached this point that means we are currently surrounded
            //by other active pawns and cannot move

            TargetingScript.DelayedSearchForNewTarget(0.5f);

            return null;
        }

        //this will take a grid position Vector2 and convert it into an ID we can use
        //in the chessboardmanager script to find a tile in the chessboardtiles list!
        protected virtual int ConvertGridPositionToTileId(Vector2 position)
        {
            int id = (int)((position.y * 8) + position.x);

            return id;
        }

        public virtual void RotatePawn(Transform target)
        {
            transform.LookAt(target);
        }

        public virtual void ResetPreviousTiles()
        {
            PreviousTile = null;
            PreviousTile2 = null;
            PreviousTile3 = null;
        }
        #endregion
    }
}


