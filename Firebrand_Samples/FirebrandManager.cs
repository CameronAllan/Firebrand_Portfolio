using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FirebrandManager : MonoBehaviour
{

    public static FirebrandManager firebrand;

    //InfluenceUI influenceMenu;

    //holder gameobjects
    public GameObject policyHolder;
    public GameObject patronHolder;

    //constants
    public int interestPointInfoMax;
    public int interestPointInfluenceMax = 100;

    public float influenceThreshold;
    public float totalInfluence;

    public int influenceBase;
    public int influencePerFaction;

    //Menu Vars
    public GameObject influenceUI;
    bool uiActive;

    [Header("Gameplay Variables")]
    public int turnLimit;
    public int seats = 0;
    public int winningSeatsNumber = 0;

    public float winningVotePercentage;

    public Dictionary<int, int> interestPointVotes;
    public Dictionary<int, int> factionVoteTotals;

    [Header("Map Generation Variables")]

    [SerializeField]
    float ethnicMinorityPercentage;

    [SerializeField]
    float ethnicOverlapPercentage;

    [SerializeField]
    float upperClassPercentage;

    [SerializeField]
    float econOverlapPercentage;


    [Header("Zeitgeist Variables")]
    public zeitgeistVar[] zeitgeists;
    public Sprite[] zeitgeistIcons;

    [Header("Debug Variables")]
    public string[] factionRankedNames;
    public int[] factionRankedSeats;

    

    public Dictionary<zeitgeistVar, int> zeitgeistLookup;

    private void Awake()
    {
        if (firebrand == null)
        {
            firebrand = this;
        }
        else if (firebrand != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);

        zeitgeistLookup = new Dictionary<zeitgeistVar, int>();
        for(int x = 0; x < zeitgeists.Length; x++)
        {
            zeitgeistLookup.Add(zeitgeists[x], x);
        }
    }


    public void GenerateGameStart(List<InterestPoint> points, List<Faction> factions)
    {
        if(ethnicOverlapPercentage > (1 - ethnicMinorityPercentage))
        {
            ethnicOverlapPercentage = (1 - ethnicMinorityPercentage);
        }

        if (econOverlapPercentage > (1 - upperClassPercentage))
        {
            econOverlapPercentage = (1 - upperClassPercentage);
        }


        List<Faction> establishmentFactions = new List<Faction>();

        foreach(Faction f in factions)
        {
            if (f.isAuthority)
            {
                establishmentFactions.Add(f);
            }
        }


        foreach (InterestPoint i in points)
        {
            float ethnicRand = UnityEngine.Random.Range(0f, 1f);
            float econRand = UnityEngine.Random.Range(0f, 1f);

            int populationRand = UnityEngine.Random.Range(2, 5);
            int publicOrderRand = UnityEngine.Random.Range(2, 5);
            int enfranchisementRand = UnityEngine.Random.Range(2, 5);


            i.populationLevel = populationRand;
            i.publicOrderLevel = publicOrderRand;
            i.enfranchisementLevel = enfranchisementRand;
            i.seats = 0;

            //Randomly set Ethnic Bools
            if(ethnicRand <= ethnicMinorityPercentage)
            {
                i.hasMinorityPop = true;
                if(ethnicRand <= (ethnicMinorityPercentage + ethnicOverlapPercentage))
                {
                    i.hasMajorityPop = true;
                } else
                {
                    i.hasMajorityPop = false;
                }
            } else
            {
                i.hasMinorityPop = false;
                i.hasMajorityPop = true;
            }

            //Randomly set Econ Bools
            if (econRand <= upperClassPercentage)
            {
                i.hasUpperClassPop = true;
                if (econRand <= (upperClassPercentage + econOverlapPercentage))
                {
                    i.hasWorkingClassPop = true;
                }
                else
                {
                    i.hasWorkingClassPop = true;
                }
            }
            else
            {
                i.hasUpperClassPop = false;
                i.hasWorkingClassPop = true;
            }

            //Randomly allocate starting Influence
            int remainingVoteShare = interestPointInfluenceMax;

            foreach(Faction f in establishmentFactions)
            {
                int voteShare = UnityEngine.Random.Range(0, remainingVoteShare);
                remainingVoteShare = remainingVoteShare - voteShare;
                i.influenceStandings.Add(f.factionNumber, voteShare);
                //Debug.Log("Faction " + f.factionNumber + " has " + voteShare + " votes at " + i.interestPointName);
            }

            UpdateInterestPointSeats(i);
            UpdateInterestPointStandings(i);
        }

        CalculateStandings(points);
    }

    #region Politics Sim Functions

    public void ToggleInfluenceMenu()
    {
        if (uiActive == false)
        {
            influenceUI.SetActive(true);
            uiActive = true;
        }
        else
        {
            influenceUI.SetActive(false);
            uiActive = false;
        }
    }

    public void UpdateInterestPointSeats(InterestPoint point)
    {
        int oldSeats = point.seats;
        point.seats = Mathf.CeilToInt((point.populationLevel * point.enfranchisementLevel) / 2.5f);

        int seatChange;
        if (oldSeats <= 0)
        {
            seatChange = point.seats;
        } else
        {
            seatChange = point.seats - oldSeats;
        }

        seats += seatChange;
    }

    public void UpdateInterestPointStandings(InterestPoint point)
    {
        point.partySeats = new Dictionary<int, int>();

        //DEBUG LISTS
        List<string> factionNames = new List<string>();
        List<int> factionSeats = new List<int>();
        List<int> factionVoteShare = new List<int>();

        foreach (KeyValuePair<int, int> influenceEntry in point.influenceStandings)
        {
            Faction f = StrategyLayerManager.instance.GetFaction(influenceEntry.Key);
            int influencePercent = influenceEntry.Value;

            float influenceConv = influencePercent / 100f;

            int entrySeats = Mathf.FloorToInt(point.seats * influenceConv);
            //int entrySeats = Mathf.RoundToInt(point.seats * influenceConv);

            point.partySeats.Add(f.factionNumber, entrySeats);

            factionVoteShare.Add(influencePercent);
        }

        //Debug.Log(point.interestPointName + " has " + point.partySeats.Count + " competing parties");
        point.partySeats.OrderBy(x => x.Value);

        //OUTPUT DEBUG ARRAY TO INTERESTPOINT
        foreach(KeyValuePair<int, int> k in point.partySeats)
        {
            factionNames.Add(StrategyLayerManager.instance.GetFaction(k.Key).factionName);
            factionSeats.Add(k.Value);
        }

        point.factionRankedNames = factionNames.ToArray();
        point.factionRankedSeats = factionSeats.ToArray();
        point.factionVoteShare = factionVoteShare.ToArray();
        
        int leaderSeats = point.partySeats.First().Value;
        int remainingSeats = point.partySeats.Skip(1).Sum(y => y.Value);
        int leftoverSeats = point.seats - (leaderSeats + remainingSeats);
        /*
        Debug.Log("Leader at " + point.interestPointName + " has " + leaderSeats + " seats");
        Debug.Log("Opposition seats at " + point.interestPointName + ": " + remainingSeats);
        Debug.Log("Seats Leftover at " + point.interestPointName + ": " + leftoverSeats);
        */
        
        if (leftoverSeats > 0)
        {
            int partyIndex = 0;
            while(leftoverSeats > 0)
            {
                if(partyIndex >= point.partySeats.Count)
                {
                    partyIndex = 0;
                }
                KeyValuePair<int, int> runnerUp = point.partySeats.Skip(partyIndex).First();
                int runnerUpID = runnerUp.Key;
                int runnerUpSeats = runnerUp.Value;
                runnerUpSeats++;

                point.partySeats[runnerUpID] = runnerUpSeats;

                leftoverSeats--;
                partyIndex++;
            }
        }
        
    }

    public void RecalculateSeats(List<InterestPoint> pointList)
    {
        
        foreach (InterestPoint i in pointList)
        {
            UpdateInterestPointSeats(i);
            UpdateInterestPointStandings(i);
        }

        winningSeatsNumber = Mathf.CeilToInt(seats * winningVotePercentage);
    }

    public void CalculateStandings(List<InterestPoint> pointList)
    {
        winningSeatsNumber = Mathf.CeilToInt(seats * winningVotePercentage);
        factionVoteTotals = new Dictionary<int, int>();

        foreach(InterestPoint i in pointList)
        {
            Dictionary<int, int> pointStandings = i.partySeats;

            foreach(KeyValuePair<int, int> k in pointStandings)
            {
                if (factionVoteTotals.ContainsKey(k.Key))
                {
                    factionVoteTotals[k.Key] += k.Value;
                } else
                {
                    factionVoteTotals.Add(k.Key, k.Value);
                }
            }
        }

        //FOR DEBUG - Until UI is in, throw results in Editor in like, an array by standings
        List<string> factionNames = new List<string>();
        List<int> factionSeats = new List<int>();

        factionVoteTotals.OrderBy(x => x.Value);
        foreach(KeyValuePair<int, int> entry in factionVoteTotals)
        {
            factionNames.Add(StrategyLayerManager.instance.GetFaction(entry.Key).factionName);
            factionSeats.Add(entry.Value);
        }

        factionRankedNames = factionNames.ToArray();
        factionRankedSeats = factionSeats.ToArray();

    }

    private void InterestPointRefresh(List<InterestPoint> points)
    {
        Debug.Log("WHOA FUCK, SOMETHING CHANGED (Influence)");
        RecalculateSeats(points);
        CalculateStandings(points);
    }

    #endregion

    #region Listeners

    private void OnEnable()
    {
        StrategyLayerManager.instance.FactionTurnStarted += TurnStartRecieved;
        StrategyLayerManager.instance.FactionTurnEnded += TurnEndRecieved;
        StrategyLayerManager.instance.InfluenceUpdated += InfluenceUpdatedRecieved;
    }

    private void OnDisable()
    {
        StrategyLayerManager.instance.FactionTurnStarted -= TurnStartRecieved;
        StrategyLayerManager.instance.FactionTurnEnded -= TurnEndRecieved;
        StrategyLayerManager.instance.InfluenceUpdated -= InfluenceUpdatedRecieved;
    }

    private void RoundStartRecieved(object sender, EventArgs e)
    {

    }

    private void TurnStartRecieved(object sender, EventArgs e)
    {
        Debug.Log("FirebrandManager Recieved OnTurnStarted for Faction " + StrategyLayerManager.instance.currentFactionNumber);
    }

    private void TurnEndRecieved(object sender, EventArgs e)
    {
        //influenceMenu.UpdateInfluenceStandings();
        Debug.Log("FirebrandManager Recieved OnTurnEnded");

    }

    private void RoundEndRecieved(object sender, EventArgs e)
    {
        EndGameListener();
    }

    private void InfluenceUpdatedRecieved(object sender, EventArgs e)
    {
        InterestPointRefresh(StrategyLayerManager.instance.interestPoints);
    }

    void EndGameListener()
    {
        foreach (Faction f in StrategyLayerManager.instance.factions)
        {
            if (f.influencePoints >= influenceThreshold)
            {
                //END THE GAME!
                Debug.Log("Game Over, Faction " + f.factionName + " wins!");
            }
        }
    }

    #endregion
}

