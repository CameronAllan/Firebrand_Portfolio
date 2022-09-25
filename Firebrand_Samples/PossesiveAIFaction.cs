using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossesiveAIFaction : Faction
{
    // Start is called before the first frame update
    //public GameObject factionAgentHolder;

    public StrategyLayerManager board;
    private System.Random _rnd;
    public List<InterestPoint> targetedPoints;
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
        _rnd = new System.Random();
        targetedPoints = new List<InterestPoint>();
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

        board = strategyBoard;

        foreach (Agent a in factionAgents)
        {
            if (!a.onMission)
            {

                int randint = _rnd.Next(0, 1);
                /*
                if (randint != 0 | targetedPoints.Count == 0)
                {
                    randint = _rnd.Next(0, board.interestPoints.Count);
                    InterestPoint interestPoint = board.interestPoints[randint];


                    randint = _rnd.Next(0, a.abilities.Count);

                    PlaceAgent(interestPoint, a);
                    
                    AgentAbility ability = a.abilities[randint];
                    if (ability.hasCost)
                    {
                        if (this.numberOfRadicals < ability.radicalsCost || this.numberOfMilitants < ability.militantsCost || this.numberOfFollowers < ability.followersCost || this.numberOfActivists < ability.activistsCost) 
                        {
                            ability = a.abilities[0];//switch to default ability
                        }
                        if (this.influencePoints < ability.influenceCost || this.pointInfoDictionary[interestPoint.name] < ability.infoCost)
                        {
                            ability = a.abilities[1];
                        }
                        if (ability.zeitgeistRequiered.Contains(interestPoint.currentZeitgeist) )
                        {
                            ability = a.abilities[0];
                        }
                        }
                    ability.UseAbility();

                    //board.PlaceAgent(a, interestPoint, ability);
                    if (!targetedPoints.Contains(interestPoint))
                    {
                        targetedPoints.Add(interestPoint);
                    }
                }
                else
                {
                    randint = _rnd.Next(0, targetedPoints.Count);
                    InterestPoint interestPoint = targetedPoints[randint];
                    randint = _rnd.Next(0, a.abilities.Count);

                    PlaceAgent(interestPoint, a);

                    AgentAbility ability = a.abilities[0];
                    ability.UseAbility();

                    //AgentAbility ability = a.abilities[randint];
                    //board.PlaceAgent(a, interestPoint, ability);
                }
                */
                
                
                randint = _rnd.Next(0, board.interestPoints.Count);
                InterestPoint interestPoint = board.interestPoints[randint];
                randint = _rnd.Next(0, a.abilities.Count);

                PlaceAgent(interestPoint, a);

                AgentAbility ability = a.abilities[0];
                ability.UseAbility();

                //AgentAbility ability = a.abilities[randint];
                //board.PlaceAgent(a, interestPoint, ability);
            }
        }
        if (upgradePoints > 0)
        {
            PolicySelector selector = new PolicySelector();
            //List<FactionUpgrade> options = selector.randomUpgrades(this);
            int randint = _rnd.Next(0, 2);
            //options[randint].Adopt(this);

        }

        OnTurnOverClicked();
    }

    public override void SetCurrentAgent(Agent a)
    {
        base.SetCurrentAgent(a);

    }

    public void OnTurnOverClicked()
    {
        Debug.Log("Turn Over Clicked");

        //if (StrategyLayerManager.instance.CheckFactionTurnComplete(this) == true)
        if (!factionAgents.Exists(a => a.onMission == false))
        {
            Debug.Log("Ending Turn");
            base.FinishTurn();
        }
    }

    public void BeginCombatDecisionRound()
    {
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

    public override void PlaceAgent(InterestPoint i, Agent a)
    {
        base.PlaceAgent(i, a);
        /*
        Collider2D collider = i.gameObject.GetComponent<Collider2D>();
        if(collider != null)
        {
            Vector3 targetDestination = collider.bounds.center;
            a.transform.position = new Vector3(targetDestination.x, targetDestination.y, a.transform.position.z);
            a.GetInterestPoint();
        }
        */
    }
}
