using UnityEngine;

/// <summary>
/// This script stores the data pertaining to the pawns
/// 'home base' or essentially the tile that they call home
///  whether it be the bench or on the actual chess board
///  used to send the pawn back to their specific tile after dragging
///  or after a round of combat has finished
/// </summary>

namespace AutoBattles
{
    public class HomeBase : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private ChessBoardTile _tileScript;

        //references
        private Movement _movementScript;
        #endregion

        #region Properties
        //this is the tile script that our pawn calls home base
        protected ChessBoardTile TileScript { get => _tileScript; set => _tileScript = value; }

        //references
        protected Movement MovementScript { get => _movementScript; set => _movementScript = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            MovementScript = GetComponent<Movement>();
            if (!MovementScript)
            {
                Debug.LogError("No movement script found on " + gameObject.name + " pawn. Please add one to this specific pawns prefab");
            }
        }

        public virtual void SetHomeBase(ChessBoardTile tileScript)
        {
            TileScript = tileScript;
        }

        //This will only be called at the end of combat
        public virtual void SendToHomeBase()
        {
            TileScript.ChangePawnOutOfCombat(gameObject);
        }
        #endregion
    }
}

