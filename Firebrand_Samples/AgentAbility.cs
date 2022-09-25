using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAbility : MonoBehaviour
{

    public Agent parentAgent;

    [Header("Ability Core Vars")]
    public Sprite abilityIcon;
    public string abilityName;
    public string abilityDesc;

    public int abilityDuration;

    [Header("Ability Anim Vars")]
    public int[] agentEffectIds;


    public enum AbilityPoses
    {
        Leaning,
        Cheering
    }

    public AbilityPoses pose;

    [Header("Ability Costs")]
    public bool hasCost;

    public int infoCost;
    public int influenceCost;

    public int followersCost;
    public int militantsCost;
    public int radicalsCost;
    public int activistsCost;

    public List<zeitgeist> zeitgeistRequiered;

    [Header("Ability Rewards")]
    public bool returnsFollowers;
    public bool returnsInfo;
    public bool returnsInfluence;
    public bool returnsZeitgeist;

    [Header("Followers Reward Vars")]
    public int followersBonus;
    public int militantsBonus;
    public int radicalsBonus;
    public int activistsBonus;

    [Header("Info Reward Vars")]
    public int infoBonus;

    [Header("Influence Reward Vars")]
    public bool hasInfluenceBonus;
    public int influenceBonus;

    public bool hasInfluenceMalus;
    public int influenceMalus;
    public int influenceMalusTargets;
    public Faction[] influenceMalusFactions;


    [Header("Zeitgeist Reward Vars")]
    public float zeitgeistBonus;
    public float zeitgeistMalus;

    public zeitgeistVar zeitgeistToInfluence;

    public zeitgeistVar[] availableZeitgeists;

    zeitgeist currentIncreaseZeitgeist;
 

    public event EventHandler<UseAbilityEventArgs> AbilityUsed;
    public event EventHandler<ResolveAbilityEventArgs> AbilityResolved;

    public bool abilityActive;

    public void FindParent()
    {
        parentAgent = GetComponentInParent<Agent>();
        abilityActive = false;
    }

    private void Start()
    {
        FindParent();
    }

    //Human Player Ability Preview
    public virtual void PreviewAbility()
    {

    }

    //Ability effect overrides
    public virtual void UseAbility()
    {
        abilityActive = true;
        parentAgent.UseAgentAbility(this);
        Debug.Log("Using Ability: " + abilityName);

        StrategyLayerManager.instance.OnAbilityUsed(this, parentAgent);
    }

    //UseAbility overload
    public virtual void UseAbility(zeitgeist increase)
    {
        currentIncreaseZeitgeist = increase;
    

        abilityActive = true;
        parentAgent.UseAgentAbility(this);
        Debug.Log("Using Ability: " + abilityName);
    }

    public virtual void ResolveAbility()
    {
        abilityActive = false;

        if (returnsFollowers)
        {
            if (parentAgent.currentInterestPoint != null)
            {
                 parentAgent.controllingFaction.numberOfFollowers += (parentAgent.currentInterestPoint.recruitLevelFollowers + followersBonus);
                 parentAgent.controllingFaction.numberOfActivists += (parentAgent.currentInterestPoint.recruitLevelActivists + activistsBonus);
                 parentAgent.controllingFaction.numberOfRadicals += (parentAgent.currentInterestPoint.recruitLevelRadicals + radicalsBonus);
                 parentAgent.controllingFaction.numberOfMilitants += (parentAgent.currentInterestPoint.recruitLevelMilitants + militantsBonus);

            }
        }

        if (returnsInfo)
        {
            if (parentAgent.currentInterestPoint != null)
            {
                if (parentAgent.controllingFaction.pointInfoDictionary.ContainsKey(parentAgent.currentInterestPoint.interestPointName))
                {
                    parentAgent.controllingFaction.pointInfoDictionary[parentAgent.currentInterestPoint.interestPointName] = parentAgent.controllingFaction.pointInfoDictionary[parentAgent.currentInterestPoint.interestPointName] + infoBonus;

                    if (parentAgent.controllingFaction.pointInfoDictionary[parentAgent.currentInterestPoint.interestPointName] >= FirebrandManager.firebrand.interestPointInfoMax)
                    {
                        parentAgent.controllingFaction.pointInfoDictionary[parentAgent.currentInterestPoint.interestPointName] = FirebrandManager.firebrand.interestPointInfoMax;
                    }
                }
                else
                {
                    parentAgent.controllingFaction.pointInfoDictionary.Add(parentAgent.currentInterestPoint.interestPointName, infoBonus);
                }
            }
        }

        if (returnsInfluence)
        {
            //parentAgent.controllingFaction.influencePoints += influenceBonus;

            if (hasInfluenceBonus)
            {
                parentAgent.currentInterestPoint.InfluenceChange(parentAgent.controllingFaction, influenceBonus, true);

                StrategyLayerManager.instance.OnInfluenceChange();
            }

            if (hasInfluenceMalus)
            {
                foreach(Faction f in influenceMalusFactions)
                {
                    parentAgent.currentInterestPoint.InfluenceChange(f, influenceMalus, false);

                    StrategyLayerManager.instance.OnInfluenceChange();
                }
            }

        }
        else if (returnsZeitgeist)
        {
            parentAgent.currentInterestPoint.EncourageZeitgeist(currentIncreaseZeitgeist);
        }



        StrategyLayerManager.instance.OnAbilityResolved(this, parentAgent);

    }

    //Strategy Layer Listener Overrides
    public virtual void OnRoundStart()
    {

    }

    public virtual void OnRoundEnd()
    {

    }

    public virtual void OnTurnStart()
    {

    }

    public virtual void OnTurnEnd()
    {

    }

}

public class UseAbilityEventArgs : EventArgs
{
    public AgentAbility ability;
    public Agent parentAgent;

    public UseAbilityEventArgs(AgentAbility ability, Agent parentAgent)
    {
        this.ability = ability;
        this.parentAgent = parentAgent;
    }
}

public class ResolveAbilityEventArgs : EventArgs
{
    public AgentAbility ability;
    public Agent parentAgent;

    public ResolveAbilityEventArgs(AgentAbility ability, Agent parentAgent)
    {
        this.ability = ability;
        this.parentAgent = parentAgent;
    }
}

