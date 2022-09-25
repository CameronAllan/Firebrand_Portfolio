using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentAbilityUI : MonoBehaviour
{

    //UnitAppearance agentIcon;
    public Text agentName;

    [SerializeField]
    GameObject iconHolder;

    [SerializeField]
    //UnitAppearance iconAppearance;

    public List<GameObject> abilityIconSlots;
    public GameObject iconBkg;
    public GameObject textBkg;

    [Header ("Ability Menu Properties")]
    public GameObject useAbilityUI;
    public Text useAbilityName;
    public Button useAbilityButton;
    public Button cancelAbilityButton;

    public GameObject increaseZeitgeistUI;
    public Image currentIncZeitgeistImage;
    public zeitgeistVar currentIncZeitgeist;
    public int[] increaseInts;
    int increaseIndex;

    public GameObject decreaseZeitgeistUI;
    public Image currentDecZeitgeistImage;
    public zeitgeistVar currentDecZeitgeist;
    public int[] decreaseInts;
    int decreaseIndex;


    /*
    public GameObject followerEffects;
    public GameObject infoEffects;
    public GameObject zeitgeistEffects;
    */

    // Use this for initialization
    void Start()
    {
        foreach (Transform t in iconHolder.transform)
        {
            abilityIconSlots.Add(t.gameObject);
        }

        foreach (GameObject g in abilityIconSlots)
        {
            g.SetActive(false);
        }

        iconBkg.SetActive(false);
        textBkg.SetActive(false);

        cancelAbilityButton.onClick.AddListener(DismissAbilityPreview);
        useAbilityUI.SetActive(false);
    }


    public void UpdateAgentAbilityMenu(Agent a)
    {
        foreach (GameObject g in abilityIconSlots)
        {
            g.SetActive(false);
        }

        for (int i = 0; i < a.abilities.Count; i++)
        {
            AgentAbility ability = a.abilities[i];
            GameObject icon = abilityIconSlots[i];
            Button btn = icon.GetComponent<Button>();
            Image img = icon.GetComponent<Image>();

            btn.onClick.AddListener(delegate { PreviewAbility(ability); });
            img.sprite = ability.abilityIcon;
            //iconAppearance.CloneAppearance(a.appearance);
            icon.SetActive(true);
        }

        //Copy Agent Appearance to agentIcon
        agentName.text = a.agentName;
        Debug.Log("Now Showing: " + a.agentName);

        iconBkg.SetActive(true);
        textBkg.SetActive(true);
    }

    public void DismissAgentAbilityMenu()
    {
        foreach (GameObject g in abilityIconSlots)
        {
            g.SetActive(false);
        }

        iconBkg.SetActive(false);
        textBkg.SetActive(false);
    }

    public void PreviewAbility(AgentAbility a)
    {
        useAbilityButton.onClick.RemoveAllListeners();
        useAbilityButton.onClick.AddListener(delegate { ConfirmAbility(a); });
        if (!useAbilityButton.gameObject.activeSelf)
        {
            useAbilityButton.gameObject.SetActive(true);
        }

        if(a.returnsZeitgeist) 
        {
            increaseZeitgeistUI.SetActive(true);
            decreaseZeitgeistUI.SetActive(true);
        } else if (a.returnsZeitgeist)
        {
            increaseZeitgeistUI.SetActive(true);
            decreaseZeitgeistUI.SetActive(false);
        }  else
        {
            increaseZeitgeistUI.SetActive(false);
            decreaseZeitgeistUI.SetActive(false);
        }

        if (a.hasCost)
        {
            Faction f = a.parentAgent.controllingFaction;
            if(f.pointInfoDictionary[a.parentAgent.currentInterestPoint.interestPointName] < a.infoCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }

            if (f.influencePoints < a.influenceCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }

            if (f.numberOfFollowers < a.followersCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }

            if (f.numberOfMilitants < a.militantsCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }

            if (f.numberOfRadicals < a.radicalsCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }

            if (f.numberOfActivists < a.activistsCost)
            {
                useAbilityButton.gameObject.SetActive(false);
            }
            if (a.zeitgeistRequiered.Contains(a.parentAgent.currentInterestPoint.currentZeitgeist) )//use neutral to turn off zeitgesit req
            {

                useAbilityButton.gameObject.SetActive(false);
            }
        }

        increaseIndex = 0;
        decreaseIndex = 0;

        increaseInts = a.parentAgent.controllingFaction.interactableZeitgeistIndexes.ToArray();
        decreaseInts = new int[] { 0, 1, 2, 3, 4, 5, 6 };

        useAbilityName.text = a.abilityName;

        useAbilityUI.SetActive(true);
    }

    public void ConfirmAbility(AgentAbility a)
    {
        a.UseAbility();
        useAbilityUI.SetActive(false);
    }

    public void DismissAbilityPreview()
    {

        useAbilityUI.SetActive(false);
    }

    public void UpdateIncZeitgeistImages()
    {
        currentIncZeitgeistImage.sprite = FirebrandManager.firebrand.zeitgeistIcons[increaseIndex];
        currentIncZeitgeist = FirebrandManager.firebrand.zeitgeists[increaseIndex];
    }

    public void UpdateDecZeitgeistImages()
    {
        currentDecZeitgeistImage.sprite = FirebrandManager.firebrand.zeitgeistIcons[decreaseIndex];
        currentDecZeitgeist = FirebrandManager.firebrand.zeitgeists[decreaseIndex];
    }

    public void IncreaseZeitgeistScrollUp()
    {
        if(increaseIndex >= (increaseInts.Length-1))
        {
            increaseIndex = 0;
        } else
        {
            increaseIndex++;
        }

        UpdateIncZeitgeistImages();
    }

    public void IncreaseZeitgeistScrollDown()
    {
        if(increaseIndex <= 0)
        {
            increaseIndex = (increaseInts.Length - 1);
        } else
        {
            increaseIndex--;
        }

        UpdateIncZeitgeistImages();
    }

    public void DecreaseZeitgeistScrollUp()
    {
        if (decreaseIndex >= (decreaseInts.Length - 1))
        {
            decreaseIndex = 0;
        }
        else
        {
            decreaseIndex++;
        }

        UpdateDecZeitgeistImages();
    }

    public void DecreaseZeitgeistScrollDown()
    {
        if (decreaseIndex <= 0)
        {
            decreaseIndex = (decreaseInts.Length - 1);
        } else
        {
            decreaseIndex--;
        }

        UpdateDecZeitgeistImages();
    }
}

