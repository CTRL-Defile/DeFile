using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AutoBattles
{
    public class SynergyWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        #region Variables
        [Header("Variables")]
        [SerializeField]
        private Synergy _synergy;
        [SerializeField]
        [Tooltip("Must be '0' at runtime.")]
        private int _currentOutlineIteration = 0;
        [SerializeField]
        [Tooltip("Must be '0' at runtime.")]
        private int _currentCenterIteration = 0;
        [SerializeField]
        [Tooltip("The list that will hold the references to our synergy bubbles outline image")]
        private List<Image> outlines = new List<Image>();
        [SerializeField]
        [Tooltip("The list that will hold the references to our synergy bubbles center image")]
        private List<Image> centers = new List<Image>();

        //we will swap these colors in our out based on 
        //synergy active/inactive information
        private Color colorOff;
        private Color colorOn;

        [Header("References")]
        [SerializeField]
        private Image _synergyIcon;
        [SerializeField]
        private Transform _gridParent;
        [SerializeField]
        private GridLayoutGroup _grid;

        //references
        private UserInterfaceManager _uiManager;
        #endregion

        #region Properties
        //this is the synergy this widget is tracking
        protected Synergy Synergy { get => _synergy; set => _synergy = value; }

        //tracks which synergy bubble we are currently at (incrementing will use this iteration, decrementing will use this -1)
        protected int CurrentOutlineIteration { get => _currentOutlineIteration; set => _currentOutlineIteration = value; }

        protected int CurrentCenterIteration { get => _currentCenterIteration; set => _currentCenterIteration = value; }

        //list of all outline images from our bubbles
        protected List<Image> Outlines { get => outlines; set => outlines = value; }

        //list of all center images from our bubbles
        protected List<Image> Centers { get => centers; set => centers = value; }

        //reference to the synergy icon image
        protected Image SynergyIcon { get => _synergyIcon; set => _synergyIcon = value; }

        //reference to the transform of the grid for bubbles
        protected Transform GridParent { get => _gridParent; set => _gridParent = value; }

        //holds the reference to the actual grid component
        protected GridLayoutGroup Grid { get => _grid; set => _grid = value; }

        //references
        protected UserInterfaceManager UiManager { get => _uiManager; set => _uiManager = value; }


        #endregion

        #region Methods
        protected virtual void Awake()
        {
            UiManager = UserInterfaceManager.Instance;
            if (!UiManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. PLease add one before entering playmode!");
            }

            //this must be zero at runtime
            CurrentOutlineIteration = 0;
            CurrentCenterIteration = 0;
        }

        public virtual void Setup (Synergy synergy)
        {
            //set our grid cell size based on the size of the total synergy size
            //and also define our sizes for the outline and center children
            float[] sizes = SetGridSize(synergy.totalSynergySize);

            //create our synergy bubbles to display on the widget
            for (int i = 0; i < synergy.totalSynergySize; i++)
            {
                //create one bubble and set its parent to the gridparent
                GameObject bubble = Instantiate(UiManager.SynergyBubblePrefab, GridParent);

                //grab our references for the outline and center images
                Image outline = bubble.transform.GetChild(0).GetComponent<Image>();
                if (!outline)
                {
                    Debug.LogError("No image found on child 0 of the Synergy Bubble prefab. Please add one or re-arrange your children so the first one has an image for the outline.");
                }

                RectTransform outlineCenter = bubble.transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();

                Image center = bubble.transform.GetChild(1).GetComponent<Image>();
                if (!center)
                {
                    Debug.LogError("No image found on child 1 of the Synergy Bubble prefab. Please add one or re-arrange your children so the second one has an image for the center.");
                }

                //change size of outline and center transforms we got from earlier
                outline.rectTransform.sizeDelta = new Vector2(sizes[0], sizes[0]);
                outlineCenter.sizeDelta = new Vector2(sizes[1], sizes[1]);
                center.rectTransform.sizeDelta = new Vector2(sizes[1], sizes[1]);                
                
                //set our synergy
                Synergy = synergy;

                //change the color of our bubbles based on the synergy
                colorOn = synergy.color;
                colorOn.a = 1f;

                colorOff = synergy.color;
                colorOff.a = 0f;

                outline.color = colorOff;
                center.color = colorOff;

                //add to our lists
                Outlines.Add(outline);
                Centers.Add(center);
            }

            //set our icon
            SynergyIcon.sprite = synergy.icon;
        }


        //takes in an int (the total size of the synergy) and changes the grid cell size to fit nicely
        //within the synergy widget and also returns an int[] containing the size for the outline and center children
        // return new float[] { outline size, center size };
        protected virtual float[] SetGridSize(int totalSynergySize)
        {
            if (totalSynergySize == 1)
            {                
                Grid.cellSize = new Vector2(15, 15);
                return new float[] { 15, 12.5f };
            }
            else if (totalSynergySize < 5)
            {
                Grid.cellSize = new Vector3(10, 10);
                return new float[] { 10, 7.5f };
            }
            else
            {
                Grid.cellSize = new Vector3(7.5f, 7.5f);
                return new float[] { 7.5f, 5 };
            }
        }

        //increment the number of outlined bubbles
        //basically when we have acquired a new unique pawn for this
        //synergy we will always call this
        public virtual void AddOutline()
        {
            //make sure we are not requesting an outline on a bubble we have not created
            //this would happen when you have more pawns for a synergy than are required
            //for the max buff of that synergy (which is fine)
            if (CurrentOutlineIteration >= outlines.Count)
            {
                return;
            }

            //turn on our current iterations color
            outlines[CurrentOutlineIteration].color = colorOn;

            //increment our iteration
            CurrentOutlineIteration++;
        }

        //decrement the number of outline bubbles by 1
        //called anytime we completely lose a unique pawn for this synergy
        public virtual void RemoveOutline()
        {
            if (CurrentOutlineIteration == 0)
            {
                //this should never happen, but if it does throw an error and return
                Debug.LogError("You called for an outline decrement when the current iteration was already 0 (meaning you had no outlines).");
                return;
            }

            //turn off our current iteration - 1 color
            outlines[CurrentOutlineIteration - 1].color = colorOff;

            //decrement our iteration
            CurrentOutlineIteration--;
        }

        //increment the number of filled center bubbles
        //called anytime we have put a new unique pawn for this synergy
        //type into play
        public virtual void AddCenterFill()
        {
            //make sure we are not requesting an outline on a bubble we have not created
            //this would happen when you have more pawns for a synergy than are required
            //for the max buff of that synergy (which is fine)
            if (CurrentCenterIteration >= centers.Count)
            {
                return;
            }

            centers[CurrentCenterIteration].color = colorOn;

            CurrentCenterIteration++;
        }

        //decrement the number of filled bubbles
        //called anytime we take a unique pawn of this synergy
        //type out of play
        public virtual void RemoveCenterFill()
        {
            if (CurrentCenterIteration == 0)
            {
                //this should never happen, but if it does throw an error and return
                Debug.LogError("You called for a center decrement when the current iteration was already 0 (meaning you had no centers filled).");
                return;
            }

            centers[CurrentCenterIteration - 1].color = colorOff;

            CurrentCenterIteration--;
        }

        //brings the synergy widget into the players view
        public virtual void ShowWidget()
        {
            transform.parent = UiManager.SynergyPanel.menuTransform;
        }

        //hides the synergy widget from the players view
        public virtual void HideWidget()
        {
            transform.parent = UiManager.SynergyPanel.hiddenPosition.transform;
        }

        #endregion

        #region Mouse Functions
        public virtual void OnPointerEnter(PointerEventData pointerEventData)
        {
            //print("mouse entered synergy widget");
            UiManager.OpenDesktopTooltip(Synergy);
        }

        public virtual void OnPointerExit(PointerEventData pointerEventData)
        {
            //print("mouse exited synergy widget");
            UiManager.CloseDesktopTooltip();
        }

        #endregion
    }
}

