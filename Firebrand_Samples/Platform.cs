using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    
    public void Adopt()
    {
        StrategyLayerManager.instance.FactionTurnStarted += FactionTurnStartedListener;
        StrategyLayerManager.instance.FactionTurnEnded += FactionTurnEndedListener;
        //Combat Result Listener goes here


    }

    public void Abandon()
    {

    }

    //Strategy Manager Turn & Combat Listeners
    private void FactionTurnStartedListener(object sender, EventArgs e)
    {

    }

    private void FactionTurnEndedListener(object sender, EventArgs e)
    {

    }

    //TODO: Combat Resolution Listener

    //Agent Ability Listeners
    private void UseAgentAbilityListener(object sender, UseAbilityEventArgs e)
    {

    }

    private void ResolveAgentAbilityListener(object sender, ResolveAbilityEventArgs e)
    {

    }

    //Agent Create/Place Listeners
    private void AgentCreatedListener(object sender, AgentCreatedEventArgs e)
    {

    }

    private void AgentPlacedListener(object sender, AgentPlacedEventArgs e)
    {

    }

    


}
