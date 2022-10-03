using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// The trash panel is a UI element (unlike other interactable things for pawns (i.e. chess tiles) which are 
/// in-game objects) that will only appear when the player is dragging a pawn they own.
/// If the player drags the cursor over the trash panel the PawnDragManager will be notified
/// and if the player lets up on the mouse button the current pawn they are dragging will be sold and destroyed
/// </summary>

namespace AutoBattles
{
    public class TrashManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Variables
        //set this in the inspector or in the awake function
        public Color hoveredColor;
        private Color normalColor;

        //references
        private Image myImage;
        private PawnDragManager _pawnDragScript;
        #endregion

        #region Properties
        protected PawnDragManager PawnDragScript { get => _pawnDragScript; set => _pawnDragScript = value; }
        #endregion

        protected virtual void Awake()
        {
            myImage = GetComponent<Image>();

            PawnDragScript = PawnDragManager.Instance;
            if (!PawnDragScript)
            {
                Debug.LogError("No PawnDragManager singleton instance found in the scene. PLease add a PawnDragManager script to the Game Manager gameobject before entering playmode.");
            }

            //set our normal color so we dont have to keep looking it up
            normalColor = Color.white;
        }

        #region Pointer Handlers
        public virtual void OnPointerEnter(PointerEventData data)
        {
            //let the pawn drag manager script know we entered the trash panel
            PawnDragScript.HoveredOnTrash = true;

            myImage.color = hoveredColor;
        }
       
        public virtual void OnPointerExit(PointerEventData data)
        {
            StartCoroutine(DelayedExitTrashPanel());
        }
        #endregion

        protected virtual IEnumerator DelayedExitTrashPanel()
        {
            yield return new WaitForEndOfFrame();
            {
                //let the pawn drag manager script know we exited the trash panel
                PawnDragScript.HoveredOnTrash = false;

                myImage.color = normalColor;
            }
        }
    }

    
}
