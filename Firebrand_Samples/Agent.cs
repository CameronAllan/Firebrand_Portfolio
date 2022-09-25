using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public class Agent : MonoBehaviour, IPointerClickHandler
{
    public int agentID;
    public string agentName;

    public Sprite agentPhoto;

    public int factionNumber;
    public Faction controllingFaction;

    public InterestPoint currentInterestPoint;

    public bool isVisible;

    public bool lookingForCombat;
    public bool combatResponseSent;

    public bool onMission;
    public AgentAbility currentAbility;
    public int currentActionTurnCount;

    public GameObject abilityHolder;
    public List<AgentAbility> abilities;

    //Attribute Variables
    public int sneakiness;

    //Drag and Drop Variables

    private Vector3 offset;
    public float cameraDist;

    public GameObject parentBoard;

    private Bounds boardBounds;
    private Transform agentTransform;
    private Agent agent;

    public LayerMask boardMask;

    private Vector3 boardDirection;
    private Vector3 raycastDirection;

    public GameObject currentLocation;

    //Anim Variables
    public Animator anim;
    public SortingGroup sortingGroup;

    public void Initialize()
    {
        
        boardBounds = parentBoard.GetComponent<Renderer>().bounds;
        agentTransform = this.transform;
        boardMask = parentBoard.layer;

        boardDirection = transform.position - parentBoard.transform.position;
        boardDirection.Normalize();

        raycastDirection = new Vector3(0, 0, 1);

        anim = GetComponent<Animator>();
        //anim.keepAnimatorControllerStateOnDisable = true;

        onMission = false;
        GetAbilities();
    }


    public void OnTurnStart()
    {
        GetAbilities();
        if (onMission && StrategyLayerManager.instance.currentFactionNumber == factionNumber)
        {
            if(currentActionTurnCount > 1)
            {
                currentActionTurnCount--;
            } else
            {
                currentActionTurnCount = 0;
                onMission = false;
                currentAbility.ResolveAbility();

                ClearAnimBools();
            }
        }
    }

    public void OnTurnEnd()
    {

    }

    public void UseAgentAbility(AgentAbility a)
    {
        if (!onMission)
        {
            onMission = true;
            currentActionTurnCount = a.abilityDuration;
            currentAbility = a;

            SetAnimBools(a);

            StrategyLayerManager.instance.PlaceAgent(this, currentInterestPoint, a);
        }
    }

    public void RefreshAbilityEffects()
    {

        ClearAnimBools();
        if (onMission && currentAbility != null)
        {
            SetAnimBools(currentAbility);
        }
    }

    public void InterruptAgentAbility()
    {
        if (onMission)
        {
            ClearAnimBools();

            onMission = false;
            currentActionTurnCount = 0;
            currentAbility = null;
            currentInterestPoint = null;
            currentLocation = null;
        }
    }

    void SetAnimBools(AgentAbility a)
    {
        anim.SetBool("OnMission", true);

        switch (a.pose)
        {
            case AgentAbility.AbilityPoses.Leaning:
                anim.SetBool("Leaning", true);
                break;
            case AgentAbility.AbilityPoses.Cheering:
                anim.SetBool("Cheering", true);
                break;
            default:
                anim.SetBool("OnMission", false);
                break;
        }
    }

    void ClearAnimBools()
    {
        foreach(AnimatorControllerParameter param in anim.parameters)
        {
            anim.SetBool(param.name, false);
        }
    }

    public void OnPointerClick(PointerEventData click)
    {


    }

    public void GetAbilities()
    {
        abilities.Clear();
        foreach(Transform t in abilityHolder.transform)
        {
            AgentAbility ability = t.GetComponent<AgentAbility>();
            if(ability != null)
            {
                abilities.Add(ability);
            }
        }
    }

    
    //Drag and Drop Functions
    private void OnMouseDown()
    {
        if (!onMission && StrategyLayerManager.instance.currentFaction == controllingFaction)
        {
            cameraDist = gameObject.transform.position.z - Camera.main.transform.position.z;
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDist));
            controllingFaction.SetCurrentAgent(this);
        }
    }

    private void OnMouseDrag()
    {
        if (!onMission)
        {
            cameraDist = gameObject.transform.position.z - Camera.main.transform.position.z;
            Vector3 newPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, cameraDist);
            transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;

            RaycastHit hit;

            Debug.DrawRay(agentTransform.position, raycastDirection, Color.blue);
            Physics.Raycast(agentTransform.position, raycastDirection, out hit, cameraDist);
        }
    }

    private void OnMouseUp()
    {
        if (!onMission)
        {
            GetInterestPoint();
        }
    }

    public GameObject GetInterestPoint()
    {
        Vector2 point = new Vector2(transform.position.x, transform.position.y);

        foreach (InterestPoint i in StrategyLayerManager.instance.interestPoints)
        {
            Collider2D iCollider = i.GetComponent<Collider2D>();
            if (iCollider != null && iCollider.OverlapPoint(point))
            {
                currentLocation = i.gameObject;
                if (currentLocation.GetComponent<InterestPoint>() != null)
                {
                    currentInterestPoint = currentLocation.GetComponent<InterestPoint>();
                }
                continue;
            }
        }

        return currentLocation;
    }


    // Turn Event Listeners
    private void FactionTurnStartRecieved(object sender, EventArgs e)
    {
        Debug.Log("Turn Start Recieved");
        //OnTurnStart();
    }

    private void FactionTurnEndRecieved(object sender, EventArgs e)
    {
        Debug.Log("Turn End Recieved");
        //OnTurnEnd();
    }


    private void CheckListeners()
    {
        //Check that all Turn listeners are present
        StrategyLayerManager.instance.FactionTurnStarted -= FactionTurnStartRecieved;
        StrategyLayerManager.instance.FactionTurnStarted += FactionTurnStartRecieved;

        StrategyLayerManager.instance.FactionTurnEnded -= FactionTurnEndRecieved;
        StrategyLayerManager.instance.FactionTurnEnded += FactionTurnEndRecieved;
    }


}

public class AgentCreatedEventArgs : EventArgs
{
    public Transform agent;

    public AgentCreatedEventArgs(Transform agent)
    {
        this.agent = agent;
    }
}

public class AgentPlacedEventArgs : EventArgs
{
    public Agent agent;
    public InterestPoint point;
    public AgentAbility ability;

    public AgentPlacedEventArgs(Agent agent, InterestPoint point, AgentAbility ability)
    {
        this.agent = agent;
        this.point = point;
        this.ability = ability;
    }
}
