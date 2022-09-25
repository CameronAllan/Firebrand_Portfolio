using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanFaction : Faction
{
    //public GameObject factionAgentHolder;
    public AgentAbilityUI abilityUI;

    //public Canvas factionUI;

    //Combat Start Menu
    /*
    public GameObject combatChoiceUI;
    public Text combatUIText;
    public Button fightButton;
    public Button avoidButton;

    Agent menuDisplayAgent;
    */
    // Use this for initialization
    void Start()
    {

    }

    private void Update()
    {
        /*
        if (menuDisplayAgent != currentAgent)
        {
            abilityUI.UpdateAgentAbilityMenu(currentAgent);
            menuDisplayAgent = currentAgent;
        }
        */
    }

    public override void Play(StrategyLayerManager strategyBoard)
    {
        base.OnTurnStart();

        //combatChoiceUI.SetActive(false);
    }

    public override void SetCurrentAgent(Agent a)
    {
        base.SetCurrentAgent(a);
        abilityUI.UpdateAgentAbilityMenu(a);
    }

    public void OnTurnOverClicked()
    {
        Debug.Log("Turn Over Clicked");
        if (StrategyLayerManager.instance.CheckFactionTurnComplete(this) == true)
        {
            Debug.Log("Ending Turn");
            base.FinishTurn();
        }
    }

    public void BeginCombatDecisionRound()
    {
        //The combat and tactics systems have been removed for this sample
        /*
        fightButton.onClick.AddListener(CommitAgentToCombat);
        avoidButton.onClick.AddListener(DeclineAgentCombat);
        combatUIText.text = "Player " + factionNumber;
        */
    }

    public override void PromptCombatChoice(InterestPoint i)
    {
        //combatChoiceUI.SetActive(true);
        //currentAgent = a;

    }

    public override void FinishCombatChoice()
    {
        //combatChoiceUI.SetActive(false);
    }


}
