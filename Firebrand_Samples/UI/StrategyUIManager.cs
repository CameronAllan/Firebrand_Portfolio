using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StrategyUIManager : MonoBehaviour
{
    [Header("Civ-Style Button Vars")]
    public Button endTurnButton;
    public Image endTurnButtonImage;
    public Sprite hourglass;
    public Sprite policyIcon;
    public Sprite idleAgentIcon;


    public AgentAbilityUI agentAbilityUI;
    public InterestPointUI interestPointUI;
    public InfluenceUI influenceUI;
    public PolicyUI policyUI;
    public NotificationUI notificationUI;

    [SerializeField]
    int humanFactionID;

    //HumanFaction Combat Menu
    public GameObject combatChoiceUI;
    public Button acceptCombatButton;
    public Button declineCombatButton;

    //HumanFaction Follower UI
    public Text followersCount;
    public Text militantsCount;
    public Text radicalsCount;
    public Text activistsCount;


    // Start is called before the first frame update
    void Start()
    {
        combatChoiceUI.SetActive(false);
        //influenceUI.gameObject.SetActive(false);


    }

    public void Initialize()
    {
        influenceUI.Initialize();
        policyUI.Initialize();

        humanFactionID = StrategyLayerManager.instance.humanFactions[0].factionNumber;

        policyUI.UpdateFactionPolicies(StrategyLayerManager.instance.currentFaction);
    }

    public void ToggleInfluenceMenu()
    {
        Debug.Log("Toggling Influence Menu");

        if(influenceUI.gameObject.activeSelf == false)
        {
            influenceUI.UpdateInfluenceStandings();
            influenceUI.gameObject.SetActive(true);
        } else
        {
            influenceUI.gameObject.SetActive(false);
        }
    }

    public void TogglePolicyMenu()
    {
        if(policyUI.gameObject.activeSelf == false)
        {
            policyUI.gameObject.SetActive(true);
        } else
        {
            policyUI.gameObject.SetActive(false);
        }
    }

    public void UpdateFollowers(Faction faction, bool isGain, int followers, int militants, int radicals, int activists)
    {

        //TODO : +/- Animations that show changes in follower count
        Debug.Log("Updating UI for Faction: " + faction.factionNumber);

        followersCount.text = faction.numberOfFollowers.ToString();
        militantsCount.text = faction.numberOfMilitants.ToString();
        radicalsCount.text = faction.numberOfRadicals.ToString();
        activistsCount.text = faction.numberOfActivists.ToString();

    }

    public void PreparePlayerTurn()
    {
        UpdatePromptButton();

        notificationUI.PopulateNotifications();
        policyUI.UpdateFactionPolicies(StrategyLayerManager.instance.currentFaction);
    }

    public void UpdatePromptButton()
    {
        if(StrategyLayerManager.instance.currentFactionNumber == humanFactionID)
        {
            endTurnButton.onClick.RemoveAllListeners();
            //Cinemachine TO-DOs - focus camera on a given idle Agent
            if (StrategyLayerManager.instance.currentFaction.upgradePoints > 0)
            {
                endTurnButtonImage.sprite = policyIcon;
                endTurnButton.onClick.AddListener(policyUI.PromptPolicyChoice);
            }
            else
            {
                endTurnButtonImage.sprite = hourglass;
                HumanFaction hf = StrategyLayerManager.instance.currentFaction.GetComponent<HumanFaction>();
                endTurnButton.onClick.AddListener(hf.OnTurnOverClicked);
            }
        }
    }

    #region Listeners

    private void OnEnable()
    {
        StrategyLayerManager.instance.AgentPlaced += OnAgentPlaced;
        StrategyLayerManager.instance.AbilityResolved += OnAbilityResolved;
        StrategyLayerManager.instance.FactionTurnStarted += OnTurnStarted;
    }

    private void OnDisable()
    {
        StrategyLayerManager.instance.AgentPlaced -= OnAgentPlaced;
        StrategyLayerManager.instance.AbilityResolved -= OnAbilityResolved;
        StrategyLayerManager.instance.FactionTurnStarted -= OnTurnStarted;
        StrategyLayerManager.instance.FactionTurnEnded -= OnTurnEnded;
    }

    void OnAgentPlaced(object sender, AgentPlacedEventArgs e)
    {
        Debug.Log("StrategyManager Recieved OnAgentPlaced");
        if (e.agent.controllingFaction.factionNumber != humanFactionID)
        {
            string title = e.agent.controllingFaction.factionName + " agent spotted in " + e.point.interestPointName;
            string desc = "Sources spotted " + e.agent.controllingFaction.factionName + " agent " + e.agent.agentName + " in " + e.point.interestPointName + " recently, but could not determine what they were up to or why they were there.";

            notificationUI.AddNotification(title, desc, 1);
        }
    }

    void OnAbilityResolved(object sender, ResolveAbilityEventArgs e)
    {
        Debug.Log("StrategyManager Recieved OnAbilityResolved");
        string title = e.parentAgent.controllingFaction.factionName + " agent held a Rally in " + e.parentAgent.currentInterestPoint.interestPointName;
        string desc = "Sources say " + e.parentAgent.controllingFaction.factionName + " agent " + e.parentAgent.agentName + " held a Rally in " + e.parentAgent.currentInterestPoint.interestPointName + " recently. Their popularity among voters has increased.";

        notificationUI.AddNotification(title, desc, 0);
    }

    void OnTurnStarted(object sender, EventArgs e)
    {
        Debug.Log("StrategyManager Recieved OnTurnStarted");
        if(StrategyLayerManager.instance.currentFactionNumber == humanFactionID)
        {
            Debug.Log("StrategyManager Found PlayerFaction");
            PreparePlayerTurn();

        }
        influenceUI.UpdateInterestPointStandings();
    }

    void OnTurnEnded(object sender, EventArgs e)
    {
        Debug.Log("StrategyManager Recieved OnTurnEnded");
    }

    #endregion
}
