using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Faction : MonoBehaviour
{
    public bool isHumanFaction;
    public bool isAuthority;

    public GameObject upgradeHolder;
    public GameObject platformHolder;

    public int factionNumber;
    public string factionName;

    public Sprite factionLogo;
    public Color factionColor1;
    public Color factionColor2;

    public bool finishedPlacing;
    public bool finishedCombatChoice;

    public GameObject[] agentHolders;

    public List<Agent> factionAgents;
    public List<Agent> agentsInCombat;
    public Agent currentAgent;


    public int idleAgents;

    public int influencePoints;
    public int upgradePoints;
    public int heatPoints;

    public int numberOfFollowers;
    public int numberOfMilitants;
    public int numberOfRadicals;
    public int numberOfActivists;

    public List<int> interactableZeitgeistIndexes;

    public Dictionary<string, int> pointInfoDictionary;


    public abstract void Play(StrategyLayerManager strategyBoard);


    public void Initialize()
    {
        upgradePoints = 0;

        factionAgents = new List<Agent>();
        agentsInCombat = new List<Agent>();
        pointInfoDictionary = new Dictionary<string, int>();

        foreach (InterestPoint i in StrategyLayerManager.instance.interestPoints)
        {
            if (!pointInfoDictionary.ContainsKey(i.interestPointName))
            {
                pointInfoDictionary.Add(i.interestPointName, 0);
            }

        }

    }

    public virtual void OnTurnStart()
    {
        StrategyLayerManager.instance.CallTurnStarted();
        finishedPlacing = false;
        factionAgents.Clear();
        factionAgents = StrategyLayerManager.instance.agents.FindAll(a => a.factionNumber == factionNumber).ToList();

        
    }

    public virtual void OnTurnEnd()
    {

    }


    public void FinishTurn()
    {
        StrategyLayerManager.instance.CheckForCombats();
    }

    public virtual void SetCurrentAgent(Agent a)
    {
        currentAgent = a;
    }


    //AI Utils
    public virtual void PlaceAgent(InterestPoint i, Agent a)
    {
        Collider2D collider = i.gameObject.GetComponent<Collider2D>();
        if (collider != null)
        {
            Vector3 targetDestination = collider.bounds.center;
            a.transform.position = new Vector3(targetDestination.x, targetDestination.y, a.transform.position.z);
            a.GetInterestPoint();
        }
    }

    //Combat Accept/Decline Functions

    public abstract void PromptCombatChoice(InterestPoint i);

    public abstract void FinishCombatChoice();

    public void CommitAgentToCombat()
    {
        currentAgent.lookingForCombat = true;
        currentAgent.combatResponseSent = true;
        //StrategyLayerManager.instance.HandleCombatResponse();
    }

    public void DeclineAgentCombat()
    {
        currentAgent.lookingForCombat = false;
        currentAgent.combatResponseSent = true;
        //StrategyLayerManager.instance.HandleCombatResponse();
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


    private void CheckListeners()
    {
        //Check that all Turn listeners are present
        StrategyLayerManager.instance.FactionTurnStarted -= TurnStartRecieved;
        StrategyLayerManager.instance.FactionTurnStarted += TurnStartRecieved;

        StrategyLayerManager.instance.FactionTurnEnded -= TurnEndRecieved;
        StrategyLayerManager.instance.FactionTurnEnded += TurnEndRecieved;
    }
}
