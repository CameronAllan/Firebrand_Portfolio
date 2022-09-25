using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFaction : Faction
{

    StrategyLayerManager board;

    [Header("AI Behaviour Vars")]
    public int comfortableLead = 10;
    public int winnableDeficit = 20;
    public int targetPointsNumber = 5;

    List<InterestPoint> targetInterestPoints;


    public override void Play(StrategyLayerManager strategyBoard)
    {
        board = strategyBoard;

        base.OnTurnStart();

        foreach(Agent a in factionAgents)
        {
            if (!a.onMission)
            {
                //Need to select a mission, and a location to use it
                AgentAbility ability = ChooseAgentAbility(a);
                InterestPoint targetPoint = ChooseLocation(board.interestPoints, ability);

                PlaceAgent(targetPoint, a);

                ability.UseAbility();
            }
        }

        FinishTurn();
    }

    public override void PromptCombatChoice(InterestPoint i)
    {
        //TODO: Actually do some COGITATIN' HERE

        StrategyLayerManager.instance.HandleCombatResponse(i, this, false);

    }

    public override void FinishCombatChoice()
    {
        throw new NotImplementedException();
    }

    #region Decision Functions

    //Currently, this base AI class plays it conventional, and plays to its base.
    AgentAbility ChooseAgentAbility(Agent a)
    {
        //List<AgentAbility> abilityChoices = new List<AgentAbility>();
        AgentAbility ability = null;

        foreach(AgentAbility aa in a.abilities)
        {
            
            if (aa.hasCost)
            {
                if(aa.followersCost < numberOfFollowers && aa.militantsCost < numberOfMilitants && aa.radicalsCost < numberOfRadicals && aa.activistsCost < numberOfActivists)
                {
                    //For now, disregard info and influence costs
                    if(aa.influenceCost > 0 || aa.infoCost > 0)
                    {
                        continue;
                    } else
                    {
                        if(aa.influenceBonus > ability.influenceBonus || ability == null)
                        {
                            ability = aa;
                        }
                        //abilityChoices.Add(aa);
                    }
                }
            } else
            {
                //abilityChoices.Add(aa);
                if (aa.influenceBonus > ability.influenceBonus || ability == null)
                {
                    ability = aa;
                }
            }
        }

        if(ability == null)
        {
            ability = a.abilities.FirstOrDefault();
        }

        return ability;
    }

    //By default, AI factions will look to protect their bases
    InterestPoint ChooseLocation(List<InterestPoint> points, AgentAbility a)
    {
        List<InterestPoint> possibleLocations = new List<InterestPoint>();

        foreach(InterestPoint i in points)
        {
            Dictionary<int, int> pointStandings = new Dictionary<int, int>(i.influenceStandings);
            

            int maxInfluence = pointStandings.Values.Max();
            int maxFactionID = pointStandings.FirstOrDefault(x => x.Value == maxInfluence).Key;

            int factionInfluence = pointStandings[factionNumber];
            pointStandings.Remove(maxFactionID);

            int secondInfluence = pointStandings.Values.Max();
            int secondFactionID = pointStandings.FirstOrDefault(x => x.Value == secondInfluence).Key;

            //Want to protect close leads, and win close contests
            if (maxFactionID == factionNumber)
            {
                if(maxInfluence - secondInfluence <= comfortableLead)
                {
                    possibleLocations.Add(i);
                }
            } else
            {
                if(maxInfluence - factionInfluence <= winnableDeficit)
                {
                    possibleLocations.Add(i);
                }
            }
        }

        InterestPoint point = null;
        if(possibleLocations.Count > 1)
        {
            int pick = UnityEngine.Random.Range(0, possibleLocations.Count - 1);
            point = possibleLocations.ElementAt(pick);
        } else if (possibleLocations.Count == 1)
        {
            point = possibleLocations.FirstOrDefault();
        } else {
            int pick = UnityEngine.Random.Range(0, points.Count - 1);
            point = points.ElementAt(pick);
        }

        return point;
    }


    #endregion
}



