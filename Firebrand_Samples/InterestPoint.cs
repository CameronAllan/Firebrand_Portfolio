using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum zeitgeist
{
    Content,
    Neutral,
    Patriotic,
    Nationalist,
    Communal,
    Socialist,
    Insecure,
    Fearful,
    Envious,
    Cheated,
    Discriminated,
    Repressed,
    Suspicious,
    Xenophobic,
};

public enum zeitgeistVar
{
    Contentment,
    Nationalism,
    Equality,
    Security,
    Materialism,
    Liberty,
    Identity
}


public class InterestPoint : MonoBehaviour, IPointerClickHandler
{

    public string interestPointName;
    public Sprite interestPointImage;
    public string interestPointDesc;

    public Vector2 voronoiPoint;

    public int interestPointID;
    public bool hasConflictingAgents;

    public int populationLevel;
    public int enfranchisementLevel;



    public int publicOrderLevel;

    //Demographic Tags
    public bool hasMajorityPop;
    public bool hasMinorityPop;

    public bool hasUpperClassPop;
    public bool hasWorkingClassPop;
    

    public List<InterestPointEffectBase> interestPointEffects;


    public int seats;
    public Dictionary<int, int> influenceStandings;
    public Dictionary<int, int> partySeats;

    public InterestPointUI infoPanel;

    public InterestPointHUD hud;

    //Recruitment Variables
    public int recruitLevelFollowers;
    public int recruitLevelMilitants;
    public int recruitLevelRadicals;
    public int recruitLevelActivists;

    //Effect Tags
    public List<string> locationTags;

    [SerializeField]
    //string[] startingDemographics;
    //public List<string> demographicTags;

    //Agents at this location
    public int localAgentsCount;
    public List<Agent> localAgents;

    //Zeitgeist Vars
    public zeitgeist currentZeitgeist;

    public float contentmentLevel;
    public float nationalismLevel;
    public float equalityLevel;
    public float securityLevel;
    public float materialismLevel;
    public float libertyLevel;
    public float identityLevel;

    float zeitgeistMax = 100f;
    float zeitgeistMin = 0f;
    public zeitgeistVar leadingZeitgeist;
    //Dictionary<zeitgeistVar, float> zeitgeistDictionary;

    public InterestPoint[] neighbours;

    bool isInitialized = false;

    [Header("Debug Variables")]
    public string[] factionRankedNames;
    public int[] factionRankedSeats;
    public int[] factionVoteShare;

    public void Initialize()
    {
        interestPointEffects = new List<InterestPointEffectBase>();
        localAgents = new List<Agent>();
        influenceStandings = new Dictionary<int, int>();

        //hud = GetComponent<InterestPointHUD>();
        

        CheckListeners();

        populationLevel = Mathf.Clamp(populationLevel, 1, 5);
        enfranchisementLevel = Mathf.Clamp(enfranchisementLevel, 1, 5);
        publicOrderLevel = Mathf.Clamp(publicOrderLevel, 1, 5);

        hud.Initialize(this);

        isInitialized = true;
    }

    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on: " + interestPointName);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (infoPanel.gameObject.activeSelf == false)
            {
                infoPanel.gameObject.SetActive(true);
            }

            infoPanel.PopulatePointInfo(this);
        }
        
    }
    

    void EvaluateZeitgeist()
    {
        //contentmentLevel = zeitgeistDictionary[zeitgeistVar.Contentment];
        //contentmentLevel = ZeitgeistBounds(contentmentLevel);
        //zeitgeistDictionary[zeitgeistVar.Contentment] = contentmentLevel;

        //nationalismLevel = zeitgeistDictionary[zeitgeistVar.Nationalism];
        //nationalismLevel = ZeitgeistBounds(nationalismLevel);
        //zeitgeistDictionary[zeitgeistVar.Nationalism] = nationalismLevel;

        //equalityLevel = zeitgeistDictionary[zeitgeistVar.Equality];
        //equalityLevel = ZeitgeistBounds(equalityLevel);
        //zeitgeistDictionary[zeitgeistVar.Equality] = equalityLevel;

        //securityLevel = zeitgeistDictionary[zeitgeistVar.Security];
        //securityLevel = ZeitgeistBounds(securityLevel);
        //zeitgeistDictionary[zeitgeistVar.Security] = securityLevel;

        //materialismLevel = zeitgeistDictionary[zeitgeistVar.Materialism];
        //materialismLevel = ZeitgeistBounds(materialismLevel);
        //zeitgeistDictionary[zeitgeistVar.Materialism] = materialismLevel;

        //libertyLevel = zeitgeistDictionary[zeitgeistVar.Liberty];
        //libertyLevel = ZeitgeistBounds(libertyLevel);
        //zeitgeistDictionary[zeitgeistVar.Liberty] = libertyLevel;

        //identityLevel = zeitgeistDictionary[zeitgeistVar.Identity];
        //identityLevel = ZeitgeistBounds(identityLevel);
        //zeitgeistDictionary[zeitgeistVar.Identity] = identityLevel;


        //leadingZeitgeist = zeitgeistDictionary.FirstOrDefault(x => x.Value == zeitgeistDictionary.Values.Max()).Key;

        //if (zeitgeistDictionary[leadingZeitgeist] >= (zeitgeistMax / 2))
        //{
          //  switch (leadingZeitgeist)
            //{
              //  case zeitgeistVar.Contentment:
                //    currentZeitgeist = zeitgeist.Content;
                  //  break;
                //case zeitgeistVar.Nationalism:
                //    currentZeitgeist = zeitgeist.Nationalist;
                //    break;
                //case zeitgeistVar.Equality:
                //    currentZeitgeist = zeitgeist.Socialist;
                //    break;
                //case zeitgeistVar.Security:
                //    currentZeitgeist = zeitgeist.Fearful;
                //    break;
                //case zeitgeistVar.Materialism:
                //    currentZeitgeist = zeitgeist.Cheated;
                //    break;
                //case zeitgeistVar.Liberty:
                //    currentZeitgeist = zeitgeist.Repressed;
                //    break;
                //case zeitgeistVar.Identity:
                //    currentZeitgeist = zeitgeist.Xenophobic;
                //    break;
            //}
        //}
        //else
        //{
        //    switch (leadingZeitgeist)
        //    {
        //        case zeitgeistVar.Contentment:
        //            currentZeitgeist = zeitgeist.Content;
        //            break;
        //        case zeitgeistVar.Nationalism:
        //            currentZeitgeist = zeitgeist.Patriotic;
        //            break;
        //        case zeitgeistVar.Equality:
        //            currentZeitgeist = zeitgeist.Communal;
        //            break;
        //        case zeitgeistVar.Security:
        //            currentZeitgeist = zeitgeist.Insecure;
        //            break;
        //        case zeitgeistVar.Materialism:
        //            currentZeitgeist = zeitgeist.Envious;
        //            break;
        //        case zeitgeistVar.Liberty:
        //            currentZeitgeist = zeitgeist.Discriminated;
        //            break;
        //        case zeitgeistVar.Identity:
        //            currentZeitgeist = zeitgeist.Suspicious;
        //            break;
        //    }
        //}
    }

    float ZeitgeistBounds(float x)
    {
        if (x > zeitgeistMax)
        {
            x = zeitgeistMax;
            return x;

        }
        else if (x < zeitgeistMin)
        {
            x = zeitgeistMin;
            return x;
        }
        else
        {
            return x;
        }
    }

    public void EncourageZeitgeist(zeitgeist name)
    {

        //for now, just set the zeitgeist
        currentZeitgeist = name;
        
    }

    

    public void InfluenceChange(Faction f, int change, bool isIncrease)
    {
        int factionID = f.factionNumber;
        int influenceChange = change;
        Debug.Log("Changing Influence!");
        if (isIncrease)
        {
            //Check that the individual increase doesn't exceed 100
            if (influenceStandings.ContainsKey(factionID))
            {
                if(influenceStandings[factionID] + change > 100)
                {
                    influenceChange = 100 - influenceStandings[factionID];
                }
                influenceStandings[factionID] += influenceChange;
                Debug.Log("Updated " + f.factionName + " at InterestPoint " + interestPointName + " with " + influenceStandings[f.factionNumber] + " influence!");
            }
            else
            {
                if(influenceChange > 100)
                {
                    influenceChange = 100;
                }
                influenceStandings.Add(factionID, change);
                Debug.Log("Added " + f.factionName + " to InterestPoint " + interestPointName + " with " + influenceStandings[f.factionNumber] + " influence!");
            }

            
            //TODO: Smarter way of redistributing influence - e.g. from parties with closest policies
            int potentialOverflow = influenceStandings.Sum(x => x.Value) + influenceChange;
            if (potentialOverflow > 100)
            {
                int correction = potentialOverflow - 100;
                int playerCorrection = Mathf.CeilToInt(correction / (influenceStandings.Count - 1));
                int leftoverCorrection = 0;
                foreach (Faction faction in StrategyLayerManager.instance.factions)
                {
                    //Check that individual decrease doesn't go lower than 0
                    if (influenceStandings.ContainsKey(faction.factionNumber) && faction.factionNumber != factionID)
                    {
                        if (influenceStandings[faction.factionNumber] - playerCorrection >= 0)
                        {
                            influenceStandings[faction.factionNumber] -= playerCorrection;
                        }
                        else
                        {
                            int leftover = (playerCorrection - influenceStandings[faction.factionNumber]);
                            influenceStandings[faction.factionNumber] -= (playerCorrection - leftover);
                            leftoverCorrection += leftover;
                        }
                    }
                }

                //TODO: Do something with that overflow (take from leader?)
                if(leftoverCorrection > 0)
                {
                    int maxFactionID = influenceStandings.FirstOrDefault(x => x.Value == influenceStandings.Values.Max()).Key;
                    if (influenceStandings[maxFactionID] - leftoverCorrection >= 0)
                    {
                        influenceStandings[maxFactionID] -= leftoverCorrection;
                    }
                    else
                    {
                        influenceStandings[factionID] -= leftoverCorrection;
                    }
                }
            }


        } else
        {
            //TODO: See above, but the opposite
            if (influenceStandings.ContainsKey(factionID))
            {
                if(influenceStandings[factionID] - influenceChange < 0)
                {
                    influenceChange = influenceStandings[factionID];
                }
                influenceStandings[factionID] -= influenceChange;
            }
            else
            {
                if(influenceChange < 0)
                {
                    influenceChange = 0;
                }
                influenceStandings.Add(factionID, influenceChange);
            }

            //TODO: Smarter way of redistributing influence - e.g. from parties with closest policies
            int potentialOverflow = influenceStandings.Sum(x => x.Value);
            int leftoverCorrection = 0;
            if (potentialOverflow > 100)
            {
                int correction = potentialOverflow - 100;
                int playerCorrection = Mathf.FloorToInt(correction / (influenceStandings.Count - 1));
                foreach (Faction faction in StrategyLayerManager.instance.factions)
                {
                    //Check that individual decrease doesn't exceed 100
                    if (influenceStandings.ContainsKey(faction.factionNumber) && faction.factionNumber != factionID)
                    {
                        if (influenceStandings[faction.factionNumber] + playerCorrection <= 100)
                        {
                            influenceStandings[faction.factionNumber] += playerCorrection;
                        }
                        else
                        {
                            int leftover = (playerCorrection + influenceStandings[faction.factionNumber]);
                            influenceStandings[faction.factionNumber] += (playerCorrection - leftover);
                            leftoverCorrection += leftover;
                        }
                    }
                }

                //TODO: Do something with that overflow (give to last place?)
                if(leftoverCorrection > 0)
                {
                    int minFactionID = influenceStandings.FirstOrDefault(x => x.Value == influenceStandings.Values.Min()).Key;
                    if (influenceStandings[minFactionID] + leftoverCorrection <= 100)
                    {
                        influenceStandings[minFactionID] += leftoverCorrection;
                    }
                    else
                    {
                        influenceStandings[factionID] += leftoverCorrection;
                    }
                }
            }
        }
    }

    public void UpdateInterestPointInfoPanel()
    {

    }

    void OnAgentPlaced()
    {
        GetLocalAgents();
    }

    void GetLocalAgents()
    {
        localAgents.Clear();
        localAgents = StrategyLayerManager.instance.agents.FindAll(a => a.currentInterestPoint == this).ToList();
        localAgentsCount = localAgents.Count;
        if (localAgents.Select(i => i.factionNumber).Distinct().Count() > 1)
        {
            hasConflictingAgents = true;
        }
        else
        {
            hasConflictingAgents = false;
        }
    }

    void OnTurnStart()
    {
        GetLocalAgents();

        populationLevel = Mathf.Clamp(populationLevel, 1, 5);
        enfranchisementLevel = Mathf.Clamp(enfranchisementLevel, 1, 5);
        publicOrderLevel = Mathf.Clamp(publicOrderLevel, 1, 5);

        hud.UpdatePlayerInfoLevels();
        hud.PopulateInfoIcons();
    }

    void OnTurnEnd()
    {

    }



    // Turn Event Listeners
    private void TurnStartRecieved(object sender, EventArgs e)
    {
        OnTurnStart();
    }

    private void TurnEndRecieved(object sender, EventArgs e)
    {
        OnTurnEnd();
    }

    private void OnAgentPlaced(object sender, AgentPlacedEventArgs e)
    {
        if(e.point == this)
        {
            OnAgentPlaced();
        }
    }


    private void CheckListeners()
    {
        StrategyLayerManager.instance.FactionTurnStarted -= TurnStartRecieved;
        StrategyLayerManager.instance.FactionTurnStarted += TurnStartRecieved;


        StrategyLayerManager.instance.FactionTurnEnded -= TurnEndRecieved;
        StrategyLayerManager.instance.FactionTurnEnded += TurnEndRecieved;

        StrategyLayerManager.instance.AgentPlaced -= OnAgentPlaced;
        StrategyLayerManager.instance.AgentPlaced += OnAgentPlaced;
    }
}

public class ZeitgeistUpdatedEventArgs : EventArgs
{
    zeitgeistVar zeitgeist;
    InterestPoint point;

    public ZeitgeistUpdatedEventArgs(zeitgeistVar z, InterestPoint i)
    {
        zeitgeist = z;
        point = i;
    }
}
