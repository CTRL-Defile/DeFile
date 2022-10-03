using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoBattles
{   
    /// <summary>
    /// This class will hold information about our synergies as they unfold during gameplay
    /// We use lists of 'UniquePawn' to keep track of UNIQUE pawns of the same synergies
    /// This way we arent counting two of the same pawn twice toward the same synergy
    /// </summary>

    [System.Serializable]
    public class SynergyObject
    {
        //holds Synergy Scriptable Object data
        public Synergy synergy;

        //holds the reference to the UI object that represents
        //and displays our synergy info to the player
        public Transform widgetTransform;

        //holds the reference to the script on the widget for
        //manipulating the bubbles
        public SynergyWidget widgetScript;

        //total number of UNIQUE active pawns of this synergy in play by the player
        public int uniqueActivePawns;

        //total number of UNIQUE total pawns of this synergy owned by the player
        public int uniqueTotalPawns;

        //list of every UNIQUE active pawn that is contributing towards the synergy and their total count (i.e. 2 'Maces' would show up once in this list and only count once towards the warrior synergy)
        public List<UniquePawn> activePawns = new List<UniquePawn>();

        //list of every UNIQUE pawn currently owned by the player that could possibly contribute to the synergy
        public List<UniquePawn> totalPawns = new List<UniquePawn>();

        //references
        private BenchManager benchManagerScript;

        //contructor
        public SynergyObject(Synergy syn, Transform WidgetTransform, SynergyWidget WidgetScript)
        {
            synergy = syn;
            widgetTransform = WidgetTransform;
            widgetScript = WidgetScript;

            uniqueActivePawns = 0;
            uniqueTotalPawns = 0;

            benchManagerScript = BenchManager.Instance;
            if (!benchManagerScript)
            {
                Debug.LogError("No BenchManager singleton instance found in scene. Please add one before entering playmode.");
            }
        }

        //called anytime we acquire a pawn, regardless of if its in play or not
        public void PawnAcquired(string pawnName)
        {
            //we dont have anything in our TotalPawnsList
            if (totalPawns.Count < 1)
            {
                //simply add the pawn entry to the empty list
                totalPawns.Add(new UniquePawn(pawnName));
                
                //let our synergy widget know to add an outline
                //to the next available bubble
                widgetScript.AddOutline();

                //since totalPawns.Count was < 1 we know this is the first pawn we have
                //acquired of this synergy type, so tell the widget to come out from hiding
                widgetScript.ShowWidget();
            }
            else
            {
                bool duplicateFound = false;

                //search the list to see if we already have a UniquePawn entry
                //for the provided pawn name
                foreach (UniquePawn entry in totalPawns)
                {
                    if (entry.name == pawnName)
                    {
                        //increment our uniquePawn count so we known how many of this particular unique pawn we have
                        entry.count++;

                        //set this to true so we dont create a duplicate entry
                        duplicateFound = true;
                    }
                }

                if (!duplicateFound)
                {
                    //if we have entered into this codeblock that means
                    //we have acquired a UNIQUE pawn for the first time
                    //run the important parts

                    //create a new entry for this unique pawn
                    totalPawns.Add(new UniquePawn(pawnName));

                    //let our synergy widget know to add an outline
                    //to the next available bubble
                    widgetScript.AddOutline();
                }
            }

            //set our uniqueTotalPawns count equal to totalPawns.Count for quick reference later
            uniqueTotalPawns = totalPawns.Count;
        }

        //called whenever we completely lose a pawn (basically sold or upgraded)
        public void PawnLost(string pawnName)
        {
            //these are only required in the event we need to remove a uniquepawn entry entirely
            bool removalRequired = false;

            //this will hold a reference to the uniquepawn item we want removed from our list 
            UniquePawn removalReference = new UniquePawn("");

            //search our list to find our unique pawn entry for this particular pawn
            foreach (UniquePawn entry in totalPawns)
            {
                if (entry.name == pawnName)
                {
                    //decrement our count for this unique pawn
                    entry.count--;

                    //check if this was the last of this unique pawn
                    if (entry.count < 1)
                    {
                        //trigger removal

                        //set our reference for removal
                        removalReference = entry;

                        removalRequired = true;
                    }

                    //we dont need to continue this foreach loop if we found a like entry
                    break;
                }
            }

            //if we triggered removal, do it
            if (removalRequired)
            {
                totalPawns.Remove(removalReference);

                //remove an outline from our synergy widget bubbles
                widgetScript.RemoveOutline();

                //check if this was the last pawn in totalPawns
                if (totalPawns.Count == 0)
                {
                    //hide the widget since we currently do not own any pawns of this synergy type
                    widgetScript.HideWidget();
                }
            }

            //set our uniqueTotalPawns count equal to totalPawns.Count for quick reference later
            uniqueTotalPawns = totalPawns.Count;
        }        

        //called when we put a pawn in play
        public void InPlay(string pawnName)
        {
            //we dont have anything in our activePawns list
            if (activePawns.Count < 1)
            {
                //simply add the pawn entry to the empty list
                activePawns.Add(new UniquePawn(pawnName));

                //tell our widget to add a center fill
                widgetScript.AddCenterFill();
            }
            else
            {
                bool duplicateFound = false;

                //search the list to see if we already have a UniquePawn entry
                //for the provided pawn name
                foreach (UniquePawn entry in activePawns)
                {
                    if (entry.name == pawnName)
                    {
                        //increment our uniquePawn count so we known how many of this particular unique pawn we have
                        entry.count++;

                        //set this to true so we dont create a duplicate entry
                        duplicateFound = true;
                    }
                }

                if (!duplicateFound)
                {
                    //if we have entered into this codeblock that means
                    //we have put a UNIQUE pawn in play for the first time
                    //run the important parts

                    //create a new entry for this unique pawn
                    activePawns.Add(new UniquePawn(pawnName));

                    //tell our widget to add a center fill
                    widgetScript.AddCenterFill();
                }
            }

            //set our uniqueActivePawns count equal to activePawns.Count for quick reference later
            uniqueActivePawns = activePawns.Count;
        }

        //called when we remove a pawn from play (from the chessboard)
        public void OutOfPlay(string pawnName)
        {
            //these are only required in the event we need to remove a uniquepawn entry entirely
            bool removalRequired = false;

            //this will hold a reference to the uniquepawn item we want removed from our list 
            UniquePawn removalReference = new UniquePawn("");

            //search our list to find our unique pawn entry for this particular pawn
            foreach (UniquePawn entry in activePawns)
            {
                if (entry.name == pawnName)
                {
                    //decrement our count for this unique pawn
                    entry.count--;

                    //check if this was the last of this unique pawn
                    if (entry.count < 1)
                    {
                        //trigger removal

                        //set our reference for removal
                        removalReference = entry;

                        removalRequired = true;
                    }

                    //we dont need to continue this foreach loop if we found a like entry
                    break;
                }
            }

            //if we triggered removal, do it
            if (removalRequired)
            {
                activePawns.Remove(removalReference);

                //remove a center fill from our synergy widget
                widgetScript.RemoveCenterFill();
            }

            //set our uniqueActivePawns count equal to activePawns.Count for quick reference later
            uniqueActivePawns = activePawns.Count;
        }

        //called when we sell a pawn
        public void PawnSold(string pawnName, bool isInPlay)
        {
            //remove from both lists if isInPlay was passed as true

            if (isInPlay)
                OutOfPlay(pawnName);

            PawnLost(pawnName);
        }

        //called after we upgrade a pawn to re-adjust the totals from combining
        public void AdjustmentFromUpgrade(string pawnName)
        {
            //set this reference if we havent already
            if (!benchManagerScript)
            {
                benchManagerScript = BenchManager.Instance;

                if (!benchManagerScript)
                {
                    Debug.LogError("No BenchManager singleton instance found in the scene. PLease add one before entering playmode.");
                }
            }

            //search our list to find our unique pawn entry for this particular pawn
            foreach (UniquePawn entry in totalPawns)
            {
                if (entry.name == pawnName)
                {
                    //decrement our count for this unique pawn
                    //in this instance we are decrementing by the total number of pawns needed for an
                    //upgrade, the new upgraded pawn spawned will contribute on their own to the synergy count
                    entry.count -= (benchManagerScript.PawnsNeededForCombo);                    

                    //we dont need to continue this foreach loop if we found a like entry
                    break;
                }
            }

            //set our uniqueTotalPawns count equal to totalPawns.Count for quick reference later
            uniqueTotalPawns = totalPawns.Count;
        }
    }

    /// <summary>
    /// Class that is used by SynergyObject for keeping track of Unique Pawns with the same synergy
    /// Used so that we dont have 2 'Mace' pawns contributing to the same synergy twice
    /// We only want unique pawns of the same synergy to contribute towards the synergy count
    /// </summary>

    [System.Serializable]
    public class UniquePawn
    {
        //name of the unique pawn
        public string name = "";

        //total number of this particular unique pawn
        public int count = 0;

        //constructor
        public UniquePawn(string Name)
        {
            name = Name;

            //this defaults to 1 because we will never create a new UniquePawn unless we have atleast 1 of that unique pawn
            count = 1;
        }       
    }

    /// <summary>
    /// This class is used when we go to actually apply our synergies buffs and debuffs
    /// </summary>

    [System.Serializable]
    public class AffectedSynergy
    {
       // public delegate void Buffs(List<GameObject> pawns);

        public Synergy.Buff applyBuffs;

        public Synergy synergy;

        public int uniquePawns;        

        public int uniquePawnsPerBuff;

        public int buffCounter;

        public List<GameObject> affectedPawns = new List<GameObject>();

        public List<PawnStats> affectedPawnStats = new List<PawnStats>();

        public AffectedSynergy(Synergy synergy, PawnData data)
        {
            this.synergy = synergy;

            this.affectedPawns.Add(data.pawn);

            this.affectedPawnStats.Add(data.pawnStats);

            uniquePawnsPerBuff = (int)(synergy.totalSynergySize / synergy.totalBuffSize);

            uniquePawns = 1;

            buffCounter = 0;

            //Use the modulo operator to see if this unique pawn caused us to land on
            //a new buff and also make sure our current synergy actually contains that many buffs
            if (uniquePawns % uniquePawnsPerBuff == 0 && buffCounter <= synergy.totalBuffSize)
            {
                AddBuff(buffCounter);
            }                      
        }
        
        //Takes in a string and compares it to all other pawns
        //names in affectedPawnStats to see if we have seen the pawn
        //in questions before
        public bool isUnique(string pawnName)
        {
            bool duplicateFound = false;

            foreach (PawnStats pawn in affectedPawnStats)
            {
                if (pawnName == pawn.name)
                {
                    duplicateFound = true;

                    break;
                }
            }

            return !duplicateFound;
        }

        //Called when we want to add a new pawn for this synergy to our
        //affectedPawns list. First checks if it is a new UNIQUE pawn from
        //this synergy (one we havent seen thus far) so that we know to increase
        //our uniquePawn counter for the appropriate number of buffs
        //(This way 3 'Mace' pawns dont trigger the synergy buffs on their own, 3
        //Maces would only count once toward their respective synergy)
        public void AddNewPawn(PawnData data)
        {
            if (isUnique(data.pawnStats.name))
            {
                uniquePawns++;

                //Use the modulo operator to see if this unique pawn caused us to land on
                //a new buff and also make sure our current synergy actually contains that many buffs
                if (uniquePawns % uniquePawnsPerBuff == 0 && buffCounter <= synergy.totalBuffSize)
                {
                    AddBuff(buffCounter);
                }
            }

            affectedPawns.Add(data.pawn);
        }

        //Adds the appropriate buff to the 'applyBuffs' delegate based on the
        //iteration (buffCounter) passed in and then increments the buffCounter
        public void AddBuff(int iteration)
        {
            if (iteration == 0)
            {
                applyBuffs += synergy.buff1;              

                buffCounter++;
            }
            else if (iteration == 1)
            {
               applyBuffs += synergy.buff2;

                buffCounter++;
            }
            else if (iteration == 2)
            {
                applyBuffs += synergy.buff3;

                buffCounter++;
            }
            else
            {
                Debug.LogWarning("AddBuff() was passed an iteration higher than 2, we currently only support 3 total buffs per synergy");
            }
        }
    }

    //simple container for keeping our pawn gameobject with the correct pawn stats
    public class PawnData
    {
        public GameObject pawn;
        public PawnStats pawnStats;
    }

    public class SynergyManager : Singleton<SynergyManager>
    {
        [SerializeField]
        protected List<SynergyObject> synergies = new List<SynergyObject>();

        //** TESTING **//       

        
        //** TESTING **//

        #region Variables

        //references
        private SynergyDatabase _synergyDatabaseScript;
        private SynergyBuffsAndDebuffs _buffScript;
        private UserInterfaceManager _uiManager;
        #endregion

        #region Properties

        //references
        protected SynergyDatabase SynergyDatabaseScript { get => _synergyDatabaseScript; set => _synergyDatabaseScript = value; }
        protected UserInterfaceManager UiManager { get => _uiManager; set => _uiManager = value; }
        protected SynergyBuffsAndDebuffs BuffScript { get => _buffScript; set => _buffScript = value; }
        #endregion

        #region Methods
        protected virtual void Awake()
        {
            SynergyDatabaseScript = SynergyDatabase.Instance;
            if (!SynergyDatabaseScript)
            {
                Debug.LogError("No SynergyDatabase singleton instance found in the scene. Please add one before entering playmode!");
            }

            BuffScript = SynergyBuffsAndDebuffs.Instance;
            if (!BuffScript)
            {
                Debug.LogError("No SynergyBuffsAndDebuffs singleton instance found in the scene. Please add one before entering playmode!");
            }

            UiManager = UserInterfaceManager.Instance;
            if (!UiManager)
            {
                Debug.LogError("No UserInterfaceManager singleton instance found in the scene. Please add one before entering playmode!");
            }

            //fills the 'synergies' list with SynergyObject's
            InitializeSynergies();
        }

        //we call this during initialization (awake) to fill our SynergyObject list with all synergies provided in the database
        protected virtual void InitializeSynergies()
        {
            foreach (Synergy syn in SynergyDatabaseScript.Synergies)
            {
                //create the synergy widget for the UI and set its parent to the hidden position
                GameObject widget = Instantiate(UiManager.SynergyWidgetPrefab, UiManager.SynergyPanel.hiddenPosition);

                //name the widget for easy use in the editor
                widget.name = syn.name + " Synergy Widget";

                //grab the reference to our widget script
                SynergyWidget widgetScript = widget.GetComponent<SynergyWidget>();
                //make sure we actually grabbed it
                if (!widgetScript)
                {
                    Debug.LogError("No SynergyWidget script found on the Synergy Widget prefab. Please add one to the prefab in the editor before entering playmode!");
                }
                else
                {                   
                    //setup our widget script on the newly created gameobject
                    widgetScript.Setup(syn);

                    //create our synergyobject and add it to our list
                    synergies.Add(new SynergyObject(syn, widget.transform, widgetScript));
                }               
            }
        }

        public virtual void PawnAcquired(PawnStats pawn)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach(int iteration in affectedSynergies)
            {
                synergies[iteration].PawnAcquired(pawn.name);
            }
        }

        public virtual void PawnLost(PawnStats pawn)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach (int iteration in affectedSynergies)
            {
                synergies[iteration].PawnLost(pawn.name);
            }
        }

        public virtual void PawnInPlay(PawnStats pawn)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach (int iteration in affectedSynergies)
            {
                synergies[iteration].InPlay(pawn.name);
            }
        }

        public virtual void PawnOutOfPlay(PawnStats pawn)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach (int iteration in affectedSynergies)
            {
                synergies[iteration].OutOfPlay(pawn.name);
            }
        }

        public virtual void PawnSold(PawnStats pawn, bool isInPlay)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach (int iteration in affectedSynergies)
            {
                synergies[iteration].PawnSold(pawn.name, isInPlay);
            }
        }

        public virtual void AdjustmentFromUpgrade(PawnStats pawn)
        {
            //create a list of all synergies we will be affecting
            //from the passed pawn
            List<string> pawnSynergies = SynergyStringsToList(pawn);

            //get the affected iterations in our synergies list of SynergyObjects
            List<int> affectedSynergies = SynergyIterationsAffected(pawnSynergies);

            //loop through our list of affected iterations and perform the desired
            //synergyObject task on that synergy by passing the name of the pawn
            foreach (int iteration in affectedSynergies)
            {
                synergies[iteration].AdjustmentFromUpgrade(pawn.name);
            }
        }        

        /// <summary>
        /// This will be called by both the players team and the enemies team directly before
        /// a combat round begins, it will take in both armies (once as the players army being the 
        /// 'friendly' army and once as the computer being the 'friendly' army) -> determine what synergies
        /// are active for the 'friendly' side -> apply those buffs to their respective sides
        /// (good buffs to friendly side, negative buffs to enemy side...in this case, friendly doesnt necessarily mean player
        /// and hostile doesnt necessilary mean computer)
        /// </summary>
        public virtual void ApplySynergyEffects(List<GameObject> friendlyArmy, List<GameObject> hostileArmy)
        {
            //clear the old list
            //affectedSynergies.Clear();
            //create a list of affected synergies
            List<AffectedSynergy> affectedSynergies = new List<AffectedSynergy>();

            //create list of pawn data, we are about to fill it
            List<PawnData> pawnData = new List<PawnData>();

            //first grab each pawnstats reference for the friendly army and
            //store it with its respective pawn gameobject for later use
            foreach(GameObject pawn in friendlyArmy)
            {
                PawnData data = new PawnData();
                data.pawn = pawn;
                data.pawnStats = pawn.GetComponent<Pawn>().Stats;

                pawnData.Add(data);
            }

            //first sort the friendly army and fill our affectedSynergies list
            foreach (PawnData data in pawnData)
            {
                if (data.pawn == null)
                    continue;

                //grab a list of strings representing this pawns synergies
                List<string> pawnSynergies = SynergyStringsToList(data.pawnStats);

                //loop through the list of strings and either
                //->add to an already affect synergy in our list
                //->or add a new affected synergy to our list
                foreach (string synName in pawnSynergies)
                {                    
                    bool duplicateFound = false;
                    AffectedSynergy reference = null;

                    //check each currently affect synergy to see if we are 
                    //already tracking this synergy
                    foreach (AffectedSynergy syn in affectedSynergies)
                    {
                        //compare names
                        if (synName == syn.synergy.name)
                        {                            
                            duplicateFound = true;

                            reference = syn;

                            break;
                        }
                    }

                    //if we found a duplicate simply add their data
                    //to the already existing AffectedSynergy
                    if (duplicateFound)
                    {
                        reference.AddNewPawn(data);
                    }
                    else
                    {
                        //if we didnt find a duplicate AffectedSynergy that means we need to 
                        //create one and add it to the list

                        Synergy newSynergy = null;

                        foreach (Synergy syn in SynergyDatabaseScript.Synergies)
                        {
                            if (syn.name == synName)
                            {
                                newSynergy = syn;
                            }
                        }

                        if (newSynergy != null)
                            affectedSynergies.Add(new AffectedSynergy(newSynergy, data));
                    }
                    
                }
            }

            //apply the buffs we racked up in our affectSynergies List
            foreach(AffectedSynergy affSyn in affectedSynergies)
            {
                //check if we even have a single buff to give
                if (affSyn.buffCounter > 0)
                {
                    //check if the buffs from this synergy are meant for
                    //the friendly army or the hostile army (debuffs)
                    if (affSyn.synergy.enemyDebuff)
                    {
                        affSyn.applyBuffs(hostileArmy);
                    }
                    else
                    {                   
                        affSyn.applyBuffs(affSyn.affectedPawns);
                    }                    
                }
                else
                {
                    //if we dont, continue the loop
                    continue;
                }
            }           
        }

        //returns a list of synergies for any given PawnStats (origins/classes)
        protected virtual List<string> SynergyStringsToList(PawnStats pawn)
        {
            //grab the synergies we will be working with and put them in a list
            List<string> synergiesList = new List<string>();

            foreach (PawnStats.Origin origin in pawn.origins)
            {
                synergiesList.Add(origin.ToString());
            }

            foreach (PawnStats.Class _class in pawn.classes)
            {
                synergiesList.Add(_class.ToString());
            }

            return synergiesList;
        }

        //returns a list of affected SynergyObject's (by iteration) in the synergies List
        //based on the list of Synergy strings passed to it
        protected virtual List<int> SynergyIterationsAffected(List<string> synergyList)
        {
            List<int> iterationsAffected = new List<int>();

            //iterate through our synergy list and store the iterations affected
            foreach (string synergy in synergyList)
            {
                //loop through our list of all synergies
                for (int i = 0; i < synergies.Count; i++)
                {
                    if (synergy == synergies[i].synergy.name)
                    {
                        //catalog the affected iteration
                        iterationsAffected.Add(i);

                        //go to the next i iteration in our for loop
                        continue;
                    }
                }
            }

            return iterationsAffected;
        }


        #endregion
    }

}
