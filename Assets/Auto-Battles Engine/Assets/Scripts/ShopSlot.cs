using UnityEngine;
using UnityEngine.UI;

namespace AutoBattles
{
    public class ShopSlot : MonoBehaviour
    {
        #region Variables
        [Header("Pawn Info")]
        [SerializeField]
        private PawnStats _pawn;
        private int _goldCost;

        [Header("UI References")]
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Text _name;
        [SerializeField]
        private Text _originClass;
        [SerializeField]
        private Text _goldCostText;

        //References
        private BenchManager _benchManagerScript;
        private GoldManager _goldManagerScript;
        private Button myButton;
        #endregion

        #region Properties
        //Pawn Info

        //This will hold all the data for the pawn that this particular shop slot is offering
        public PawnStats Pawn { get => _pawn; set => _pawn = value; }

        //how much gold this particular pawn will cost in the shop
        //determined during setup by the quality
        protected int GoldCost { get => _goldCost; set => _goldCost = value; }

        //UI References
        protected Image Icon { get => _icon; set => _icon = value; }
        protected Text Name { get => _name; set => _name = value; }
        protected Text OriginClass { get => _originClass; set => _originClass = value; }
        protected Text GoldCostText { get => _goldCostText; set => _goldCostText = value; }

        //references
        protected BenchManager BenchManagerScript { get => _benchManagerScript; set => _benchManagerScript = value; }
        protected GoldManager GoldManagerScript { get => _goldManagerScript; set => _goldManagerScript = value; }

        #endregion

        #region Methods
        protected virtual void Awake()
        {
            if (!Icon)
            {
                Debug.LogError("No Icon image reference set in the ShopSlot script on the" + gameObject.name + "prefab. Please set the reference in the inspector on the prefab.");
            }

            if (!Name)
            {
                Debug.LogError("No Name text reference set in the ShopSlot script on the" + gameObject.name + "prefab. Please set the reference in the inspector on the prefab.");
            }

            if (!OriginClass)
            {
                Debug.LogError("No OriginClass text reference set in the ShopSlot script on the" + gameObject.name + "prefab. Please set the reference in the inspector on the prefab.");
            }

            if (!GoldCostText)
            {
                Debug.LogError("No Gold Cost text reference set in the ShopSlot script on the" + gameObject.name + "prefab. Please set the reference in the inspector on the prefab.");
            }

            BenchManagerScript = BenchManager.Instance;
            if (!BenchManagerScript)
            {
                Debug.LogError("No BenchManager singleton instance found in the scene. Please add a BenchManager script to the GameManager gameobject before entering playmode.");
            }

            GoldManagerScript = GoldManager.Instance;
            if (!BenchManagerScript)
            {
                Debug.LogError("No GoldManager singleton instance found in the scene. Please add a GoldManager script to the GameManager gameobject before entering playmode.");
            }

            //Bind our OnPurchase function to our button
            myButton = GetComponent<Button>();
            myButton.onClick.AddListener(() => { OnPurchase(); });
        }

        public virtual void Setup(PawnStats pawn)
        {
            Pawn = pawn;

            //set our icon
            Icon.sprite = Pawn.icon;

            //set our name
            Name.text = Pawn.name;

            //set the color of the name text relative to its quality
            Name.color = QualityColor(pawn.pawnQuality);

            //set cost
            GoldCost = QualityCost(pawn.pawnQuality);

            //set gold cost text
            GoldCostText.text = GoldCost.ToString();

            //set our class/origin
            //we may have more than one class/origin so we will
            //cycle through and add them all

            string OriginAndClassString = "";

            //add all origins to string
            foreach (PawnStats.Origin origin in Pawn.origins)
            {
                OriginAndClassString += origin.ToString() + " ";
            }

            //add all classes to string
            foreach (PawnStats.Class _class in Pawn.classes)
            {
                OriginAndClassString += _class.ToString() + " ";
            }

            OriginClass.text = OriginAndClassString;
        }

        public virtual void Clear()
        {
            //Reset Everything

            Pawn = null;

            Icon.sprite = null;

            Name.text = "";

            OriginClass.text = "";

            GoldCost = 0;

            GoldCostText.text = "";
        }

        public virtual void OnPurchase()
        {
            //if we don't have a pawn to sell, just return
            if (Pawn == null)
                return;

            //ask the gold manager script if we have enough gold for this purchase
            if (BenchManagerScript.BenchHasSpace())
            {
                if (GoldManagerScript.SpendGold(GoldCost))
                {
                    //Attempt to add our current pawn (pawnstats) to the bench
                    if (BenchManagerScript.AddNewPawnToBench(Pawn, GoldCost))
                    {
                        Clear();
                    }
                    else
                    {
                        //TODO Warn the player that their bench is full
                        print("Player bench is full!");
                    }
                }
                else
                {
                    //TODO add UI element for this
                    print("You do not have enough gold!");
                }
                
            }
            //otherwise, let the player know they dont have enough gold
            else
            {
                //TODO Warn the player that their bench is full
                print("Player bench is full!");
            }
        }

        protected virtual Color QualityColor(PawnStats.PawnQuality quality)
        {
            //set our default color in case the switch statement doesnt hit a result
            Color color = Color.white;

            switch (quality)
            {
                case PawnStats.PawnQuality.Common:
                    {
                        //do nothing because we want white,
                        //if you want common to have a different color,
                        //set it here
                        //color = new Color(0, 0, 0);
                        break;
                    }
                case PawnStats.PawnQuality.Uncommon:
                    {
                        //light green
                        color = new Color(77 / 255F, 255 / 255F, 0);
                        break;
                    }
                case PawnStats.PawnQuality.Rare:
                    {
                        //blue
                        color = new Color(0, 81 / 255F, 255 / 255F);
                        break;
                    }
                case PawnStats.PawnQuality.Epic:
                    {
                        //purple
                        color = new Color(191 / 255F, 0, 255 / 255F);
                        break;
                    }
                case PawnStats.PawnQuality.Legendary:
                    {
                        //orange
                        color = new Color(255 / 255F, 149 / 255F, 0);
                        break;
                    }
                default:
                    {
                        Debug.LogError("We were passed a quality that has not been defined in the QualityColor method on the ShopSlot script. Please assign a case for the " + quality.ToString() + " quality.");
                        break;
                    }
            }

            return color;
        }

        protected virtual int QualityCost(PawnStats.PawnQuality quality)
        {
            //set our default cost to 1 in cast the quality passed into the
            //function doesnt catch any cases on the switch statement
            int cost = 1;

            switch (quality)
            {
                case PawnStats.PawnQuality.Common:
                    {
                        //do nothing because we want 1 cost common pawns
                        //if you want to change the base cost of common pawns
                        //do so here
                        //cost = 300;
                        break;
                    }
                case PawnStats.PawnQuality.Uncommon:
                    {
                        cost = 2;
                        break;
                    }
                case PawnStats.PawnQuality.Rare:
                    {
                        cost = 3;
                        break;
                    }
                case PawnStats.PawnQuality.Epic:
                    {
                        cost = 4;
                        break;
                    }
                case PawnStats.PawnQuality.Legendary:
                    {
                        cost = 5;
                        break;
                    }
                default:
                    {
                        Debug.LogError("We were passed a quality that has not been defined in the QualityCost method on the ShopSlot script. Please assign a case for the " + quality.ToString() + " quality.");
                        break;
                    }
            }

            return cost;
        }
        #endregion
    }
}

