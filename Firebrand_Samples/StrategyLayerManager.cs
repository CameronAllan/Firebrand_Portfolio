using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StrategyLayerManager : MonoBehaviour
{
    public static StrategyLayerManager instance = null;

    [SerializeField]
    StrategyUIManager strategyUIManager;

    //[SerializeField]
    //EncounterBuilder encounterBuilder;

    //[SerializeField]
    //TacticsSceneManager tacticsScenes;

    [SerializeField]
    FirebrandManager firebrandManager;

    [SerializeField]
    MapGenerator mapGenerator;

    

    public GameObject strategyMapBkg;

    /// <summary>
    /// LevelLoading event is invoked before Initialize method is run.
    /// </summary>
    public event EventHandler StrategyLevelLoading;
    /// <summary>
    /// LevelLoadingDone event is invoked after Initialize method has finished running.
    /// </summary>
    public event EventHandler StrategyLevelLoadingDone;
    /// <summary>
    /// GameStarted event is invoked at the beggining of StartGame method.
    /// </summary>
    public event EventHandler GameStarted;
    /// <summary>
    /// GameEnded event is invoked when there is a single player left in the game.
    /// </summary>
    public event EventHandler GameEnded;
    /// <summary>
    /// Turn ended event is invoked at the end of each turn.
    /// </summary>
    public event EventHandler FactionTurnStarted;
    /// <summary>
    /// Turn ended event is invoked at the end of each turn.
    /// </summary>
    public event EventHandler FactionTurnEnded;
    /// <summary>
    /// Influence Updated is called whenever a player gains influence.
    /// </summary>
    public event EventHandler InfluenceUpdated;
    /// <summary>
    /// Zeitgeist Updated is called whenever a player increases or decreases Zeitgeists.
    /// </summary>
    public event EventHandler<ZeitgeistUpdatedEventArgs> ZeitgeistUpdated;
    /// <summary>
    /// Policy Adopted is called when a player adopts a policy.
    /// </summary>
    //public event EventHandler<UpgradeAdoptedEventArgs> PolicyAdopted;
    /// <summary>
    /// AgentAdded event is invoked each time AddAgent method is called.
    /// </summary>
    public event EventHandler<AgentCreatedEventArgs> AgentAdded;
    /// <summary>
    /// AgentPlaced event is invoked each time PlaceAgent method is called.
    /// </summary>
    public event EventHandler<AgentPlacedEventArgs> AgentPlaced;
    /// <summary>
    /// AbilityUsed event is invoked when an agent uses an Ability.
    /// </summary>
    public event EventHandler<UseAbilityEventArgs> AbilityUsed;
    /// <summary>
    ///  AbilityUsed event is invoked when an agent completes an Ability.
    /// </summary>
    public event EventHandler<ResolveAbilityEventArgs> AbilityResolved;

    private StrategyManagerState _strategyManagerState;
    public StrategyManagerState StrategyManagerState
    {
        private get
        {
            return _strategyManagerState;
        }
        set
        {
            if(_strategyManagerState != null)
            {
                if (_strategyManagerState != null)
                {
                    _strategyManagerState.OnStateExit();
                }
                _strategyManagerState = value;
                _strategyManagerState.OnStateEnter();
            }
        }
    }

    public bool randomMap = false;

    public int NumberOfFactions { get; private set; }
    public Faction currentFaction
    {
        get { return factions.Find(f => f.factionNumber.Equals(currentFactionNumber)); }
    }
    public int currentFactionNumber { get; private set; }

    public Transform strategyCamera;

    public Transform boardParent;
    public Transform boardDecorations;
    public Transform factionsParent;
    public Transform agentsParent;

    public List<Faction> factions { get; private set; }
    public List<Faction> aiFactions { get; private set; }
    public List<HumanFaction> humanFactions { get; private set; }
    public List<Faction> conflictingFactions { get; private set; }

    public int numberOfFactions;
    public int numberOfHumanFactions;

    public List<InterestPoint> interestPoints { get; private set; }
    public List<InterestPoint> contestedInterestPoints { get; private set; }

    //public List<Encounter> conflicts { get; private set; }

    public int numberOfInterestPoints;
    public int numberOfConflicts;

    public List<Agent> agents { get; private set; }
    public List<Agent> agentsInCombat { get; private set; }

    public int numberOfAgents;
    public int numberOfAgentsinConflict;

    public int turnNumber { get; private set; }

    //private Encounter currentEncounter;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    #region Turn System
    // Use this for initialization
    void Start()
    {
        //encounterBuilder = GetComponent<EncounterBuilder>();

        if (StrategyLevelLoading != null)
            StrategyLevelLoading.Invoke(this, new EventArgs());

        if(randomMap == true)
        {
            mapGenerator.GenerateMap();
        }


        InitStrategyFlow();
        Debug.Log("Initialized");

        if (StrategyLevelLoadingDone != null)
            StrategyLevelLoadingDone.Invoke(this, new EventArgs());

        StartGame();
        Debug.Log("Game Started");
    }

    public void InitStrategyFlow()
    {
        StrategyCameraController stratCam = strategyCamera.GetComponent<StrategyCameraController>();
        if(stratCam != null)
        {
            stratCam.panLimit = new Vector2(mapGenerator.m_mapWidth, mapGenerator.m_mapHeight);
        }

        interestPoints = new List<InterestPoint>();
        contestedInterestPoints = new List<InterestPoint>();
        //conflicts = new List<Encounter>();
        for (int i = 0; i < boardParent.childCount; i++)
        {
            var interestPoint = boardParent.GetChild(i).GetComponent<InterestPoint>();
            if (interestPoint != null)
            {
                interestPoints.Add(interestPoint);
                interestPoint.interestPointID = i;
                interestPoint.infoPanel = strategyUIManager.interestPointUI;
                interestPoint.Initialize();
            }
            else
            {
                Debug.LogError("Invalid object in Board Parent game object");
            }
        }
        numberOfInterestPoints = interestPoints.Count();

        humanFactions = new List<HumanFaction>();
        aiFactions = new List<Faction>();
        factions = new List<Faction>();
        for (int i = 0; i < factionsParent.childCount; i++)
        {
            var player = factionsParent.GetChild(i).GetComponent<Faction>();
            if (player != null)
            {
                factions.Add(player);
                player.factionNumber = i;
                player.isHumanFaction = false;
                aiFactions.Add(player);
                player.Initialize();

            }
            else
            {
                Debug.LogError("Invalid object in Factions Parent game object");
            }

            var humanPlayer = factionsParent.GetChild(i).GetComponent<HumanFaction>();
            if (humanPlayer != null)
            {
                humanFactions.Add(humanPlayer);
                humanPlayer.isHumanFaction = true;
                humanPlayer.abilityUI = strategyUIManager.agentAbilityUI;
            }
        }

        numberOfFactions = factions.Count;
        numberOfHumanFactions = humanFactions.Count;
        currentFactionNumber = factions.Min(p => p.factionNumber);
        Debug.Log(currentFactionNumber);

        agents = new List<Agent>();
        agentsInCombat = new List<Agent>();
        for (int a = 0; a < agentsParent.childCount; a++)
        {
            var agent = agentsParent.GetChild(a).GetComponent<Agent>();
            if (agent != null)
            {
                agent.agentID = a;
                agent.parentBoard = strategyMapBkg;
                agents.Add(agent);
                agent.controllingFaction = factions.Find(f => f.factionNumber == agent.factionNumber);
                agent.Initialize();

                if (agent.factionNumber != currentFactionNumber)
                {
                    agent.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("Invalid object in Agent Parent");
            }
        }
        numberOfAgents = agents.Count;



        firebrandManager.GenerateGameStart(interestPoints, aiFactions);
        strategyUIManager.Initialize();
    }

    public void StartGame()
    {
        if (GameStarted != null)
        {
            GameStarted.Invoke(this, new EventArgs());
        }

        //Get all the current Faction's agents, figure out how much they can see around them.
        currentFaction.Play(this);
        HumanFaction humanFaction = null;
        humanFaction = currentFaction.GetComponent<HumanFaction>();
        if (humanFaction != null)
        {
            strategyUIManager.UpdatePromptButton();
            //strategyUIManager.endTurnButton.onClick.AddListener(humanFaction.OnTurnOverClicked);
        }
    }

    
    public void CheckForCombats()
    {
        contestedInterestPoints.Clear();
        foreach(Agent a in currentFaction.factionAgents)
        {
            InterestPoint point = a.currentInterestPoint;

            {
                List<Agent> potentialEnemies = point.localAgents;
                bool canSeeEnemies = potentialEnemies.Select(x => x.isVisible && x.factionNumber != currentFactionNumber).Contains(true);
                if (canSeeEnemies)
                {
                    contestedInterestPoints.Add(a.currentInterestPoint);
                }
            }
        }

        if(contestedInterestPoints.Count > 0)
        {
            SendCombatChoice(contestedInterestPoints.First(), currentFaction);
        } else
        {
            EndTurn();
        }
    }

    public void SendCombatChoice(InterestPoint i, Faction f)
    {
        if (humanFactions.Contains(f))
        {
            strategyUIManager.acceptCombatButton.onClick.RemoveAllListeners();
            strategyUIManager.declineCombatButton.onClick.RemoveAllListeners();

            strategyUIManager.acceptCombatButton.onClick.AddListener(() => { HandleCombatResponse(i, f, true); });
            strategyUIManager.declineCombatButton.onClick.AddListener(() => { HandleCombatResponse(i, f, false); });
            strategyUIManager.combatChoiceUI.SetActive(true);
        } else
        {
            f.PromptCombatChoice(i);
        }

    }

    public void HandleCombatResponse(InterestPoint i, Faction f, bool combatAccepted)
    {

        Debug.Log("Faction " + f.factionNumber + " starting combat at InterestPoint " + i.interestPointName);
        strategyUIManager.combatChoiceUI.SetActive(false);
        contestedInterestPoints.Remove(i);
        if(combatAccepted == true)
        {
            /*
            tacticsScenes.LoadTacticalScene(tacticsScenes.tacticsScenesIndexes[0]);
            currentEncounter = encounterBuilder.SetupEncounter(i);

            strategyUIManager.transitionUI.TriggerOverlay();
            Debug.Log("CHALLENGE ACCEPTED.");
            //StartCombatEncounter();
            */
        } else
        {
            Debug.Log("PERHAPS ANOTHER TIME");
            if(contestedInterestPoints.Count > 0)
            {
                SendCombatChoice(contestedInterestPoints.First(), currentFaction);
            } else
            {
                EndTurn();
            }
        }
    }

    public void TacticalLevelLoaded()
    {
        /*
        encounterBuilder.BuildEncounter(currentEncounter);
        StartCoroutine(SceneTransitionListener());
        //StartCombatEncounter();
        */
    }

    IEnumerator SceneTransitionListener()
    {
        /*
        while (strategyUIManager.transitionUI.isPlaying)
        {
            /*
            if(!strategyUIManager.transitionUI.isPlaying && strategyUIManager.transitionUI.overlaying)
            {
                //HideStrategyLayer();
                //strategyUIManager.transitionUI.DismissOverlay();
                StartCombatEncounter();
            }
            
            yield return 0;
        }
        */
        
        //HideStrategyLayer();
        StartCombatEncounter();
        yield break;

    }

    public void TacticalLevelUnloaded()
    {

    }

    public void StartCombatEncounter()
    {
        /*
        CellGrid loadedLevelCellGrid = GameObject.FindGameObjectWithTag("CellGrid").GetComponent<CellGrid>();
        if(loadedLevelCellGrid != null)
        {
            HideStrategyLayer();
            loadedLevelCellGrid.InitTacticsFlow();
        } else
        {
            Debug.Log("Couldn't find loaded CellGrid!");
        }

        Debug.Log("FIGHT INITIATED.");
        */
    }
    /*
    public void EndCombatHandler(List<UnitBase> defeatedUnits)
    {
        Debug.Log("Ending Combat");
        foreach(UnitBase u in defeatedUnits)
        {
            Agent agent = agents.Find(a => a.factionNumber == u.PlayerNumber && a.agentID == u.unitID);
            if(agent != null)
            {
                agent.InterruptAgentAbility();
            }
        }

        ShowStrategyLayer();
        //TODO: Swap this to some kind of "Player X wins!" etc.
        strategyUIManager.transitionUI.DismissOverlay();
        tacticsScenes.UnloadTacticalScene();


        Debug.Log("FIGHT CONCLUDED");
        if (contestedInterestPoints.Count > 0)
        {
            SendCombatChoice(contestedInterestPoints.First(), currentFaction);
        }
        else
        {
            EndTurn();
        }
    }
    */
    public void EndTurn()
    {
        //Call turn end event
        agents.FindAll(a => a.factionNumber.Equals(currentFactionNumber)).ForEach(a => { a.OnTurnEnd(); a.gameObject.SetActive(false); a.isVisible = false; });

        if (FactionTurnEnded != null)
        {
            FactionTurnEnded.Invoke(this, new EventArgs());
        }

        //When all agents are placed, etc.
        currentFactionNumber = (currentFactionNumber + 1) % numberOfFactions;

        agents.FindAll(a => a.factionNumber.Equals(currentFactionNumber)).ForEach(a => { a.gameObject.SetActive(true); a.OnTurnStart(); a.isVisible = true; a.RefreshAbilityEffects(); });

        HumanFaction humanFaction = null;
        humanFaction = currentFaction.GetComponent<HumanFaction>();
        strategyUIManager.endTurnButton.onClick.RemoveAllListeners();
        if (humanFaction != null)
        {
            strategyUIManager.endTurnButton.onClick.AddListener(humanFaction.OnTurnOverClicked);
        }

        strategyUIManager.agentAbilityUI.DismissAgentAbilityMenu();
        /*
        foreach (InterestPoint i in interestPoints)
        {
            CalcFactionVisibility(i);
        }
        */
        foreach (Agent a in agents)
        {
            if(a.factionNumber != currentFactionNumber)
            {
                CheckAgentVisibility(a);
            }
        }

        Debug.Log("Starting Turn For Faction: " + currentFactionNumber);

        currentFaction.Play(this);
        //Call turn start event
        /*
        if (FactionTurnStarted != null)
        {
            FactionTurnStarted.Invoke(this, new EventArgs());
        }
        */
    }

    #endregion

    #region Tactics Scene Management

    public void HideStrategyLayer()
    {
        strategyCamera.gameObject.SetActive(false);
        boardParent.gameObject.SetActive(false);
        boardDecorations.gameObject.SetActive(false);
        agentsParent.gameObject.SetActive(false);
        factionsParent.gameObject.SetActive(false);
    }

    public void ShowStrategyLayer()
    {
        strategyCamera.gameObject.SetActive(true);
        boardParent.gameObject.SetActive(true);
        boardDecorations.gameObject.SetActive(true);
        agentsParent.gameObject.SetActive(true);
        factionsParent.gameObject.SetActive(true);
    }


    #endregion

    #region Gameplay Helpers

    public Faction GetFaction(int factionID)
    {
        return factions.Find(f => f.factionNumber.Equals(factionID));
    }

    public InterestPoint GetInterestPoint(int interestPointID)
    {
        return interestPoints.Find(i => i.interestPointID.Equals(interestPointID));
    }

    public void CallTurnStarted()
    {
        if (FactionTurnStarted != null)
            FactionTurnStarted.Invoke(this, new EventArgs());

    }
        

    public void AddAgent(Transform agent)
    {
        Agent agentToAdd = agent.GetComponent<Agent>();

        if (agentToAdd != null)
        {

        }



        if (AgentAdded != null)
            AgentAdded.Invoke(this, new AgentCreatedEventArgs(agent));
    }

    public void UpdatePlayerPromptButton()
    {
        strategyUIManager.UpdatePromptButton();
    }

    public void PlaceAgent(Agent agent, InterestPoint point, AgentAbility ability)
    {


        if (AgentPlaced != null)
            AgentPlaced.Invoke(this, new AgentPlacedEventArgs(agent, point, ability));
    }

    public void OnAbilityUsed(AgentAbility ability, Agent agent)
    {
        if (currentFaction.isHumanFaction && ability.hasCost)
        {
            strategyUIManager.UpdateFollowers(currentFaction, false, ability.followersBonus, ability.militantsBonus, ability.radicalsBonus, ability.activistsBonus);
        }

        if (AbilityUsed != null)
            AbilityUsed.Invoke(this, new UseAbilityEventArgs(ability, agent));
    }

    public void OnAbilityResolved(AgentAbility ability, Agent agent)
    {
        if (currentFaction.isHumanFaction)
        {
            strategyUIManager.UpdateFollowers(currentFaction, true, ability.followersBonus, ability.militantsBonus, ability.radicalsBonus, ability.activistsBonus);
        }


        if (AbilityResolved != null)
            AbilityResolved.Invoke(this, new ResolveAbilityEventArgs(ability, agent));
    }

    /*
    public void OnPolicyAdopted(Faction faction, FactionUpgrade upgrade, FactionUpgradeTrack upgradeTrack)
    {
        
        if (PolicyAdopted != null)
            PolicyAdopted.Invoke(this, new UpgradeAdoptedEventArgs(faction, upgrade, upgradeTrack));

        if (humanFactions.Contains(currentFaction))
        {
            strategyUIManager.UpdatePromptButton();
        }
    }
    */

    public void OnZeitgeistUpdated(zeitgeistVar z, InterestPoint i)
    {
        if (ZeitgeistUpdated != null)
            ZeitgeistUpdated.Invoke(this, new ZeitgeistUpdatedEventArgs(z, i));
        
    }

    public void OnInfluenceChange()
    {
        Debug.Log("SOMEONE'S TRYNA CHANGE INFLUENCE!");
        if(InfluenceUpdated != null)
        {
            InfluenceUpdated.Invoke(this, new EventArgs());
        }
    }

    public bool CheckFactionTurnComplete(Faction f)
    {
        int factionAgentsIdle = agents.Select(a => a.factionNumber == f.factionNumber && !a.onMission).ToList().Count;
        if(factionAgentsIdle > 0)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private void CheckAgentVisibility(Agent a)
    {
        if(a.currentInterestPoint != null)
        {
            if (a.sneakiness <= currentFaction.pointInfoDictionary[a.currentInterestPoint.interestPointName])
            {
                a.gameObject.SetActive(true);
                a.isVisible = true;
            }
        }
    }

    /*
    private void CalcFactionVisibility(InterestPoint point)
    {
        if(point != null)
        {
            Debug.Log("Calculating visibility for Faction " + currentFactionNumber + " at " + point.interestPointName);
            foreach (Agent a in point.localAgents)
            {
                if (a.factionNumber != currentFactionNumber && a.sneakiness <= currentFaction.pointInfoDictionary[point.interestPointName])
                {
                    Debug.Log("Current faction can see Agent " + a.agentName + " at " + point.interestPointName);
                    a.gameObject.SetActive(true);
                    a.isVisible = true;
                }
            }
        } 
    }
    */
    #endregion
}
