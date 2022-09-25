using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolicyUI : MonoBehaviour
{
    public GameObject policyUIPrefab;

    public GameObject policiesHolder;
    public GameObject patronsHolder;

    [Header("Faction Policy UI")]
    public GameObject factionPolicyUI;
    public GameObject policyHolderParent;

    public float policyEntryPadding;

    //List<FactionUpgrade> displayedUpgrades;
    //List<FactionUpgrade> newUpgrades;

    [Header("Policy Choice UI")]
    public GameObject policyChoiceUI;
    public GameObject policyDescUI;

    public Text policyTitle;
    public Text policyEffect;
    public Text policyDesc;

    public Button adoptButton;
    public GameObject[] policySlots;

    [SerializeField]
    Button[] policySlotButtons;

    [SerializeField]
    Image[] policySlotSprites;

    [SerializeField]
    GameObject[] selectedPolicyIcons;

    //public FactionUpgrade[] currentUpgradeChoices;



    public void Initialize()
    {
        policiesHolder = FirebrandManager.firebrand.policyHolder;
        patronsHolder = FirebrandManager.firebrand.patronHolder;
        //displayedUpgrades = new List<FactionUpgrade>();

        policyChoiceUI.SetActive(false);

        policySlotButtons = new Button[policySlots.Length];
        policySlotSprites = new Image[policySlots.Length];
        selectedPolicyIcons = new GameObject[policySlots.Length];

        for(int x = 0; x < policySlots.Length; x++)
        {
            Button policyButton = policySlots[x].GetComponent<Button>();
            Image policySprite = policySlots[x].GetComponent<Image>();
            GameObject policySelector = policySlots[x].transform.GetChild(0).gameObject;

            policySlotButtons[x] = policyButton;
            policySlotSprites[x] = policySprite;
            selectedPolicyIcons[x] = policySelector;
        }
        
    }

    public void UpdateFactionPolicies(Faction f)
    {
        /*
        Debug.Log("Updating Policies for Faction: " + f.factionName);
        newUpgrades = new List<FactionUpgrade>();
        foreach(FactionUpgrade u in f.upgrades)
        {
            if (!displayedUpgrades.Contains(u))
            {
                newUpgrades.Add(u);
            }
        }

        float offset = policyUIPrefab.GetComponent<RectTransform>().rect.height;
        offset += policyEntryPadding;

        for (int entryIndex = 0; entryIndex < newUpgrades.Count; entryIndex++)
        {
            GameObject entry = Instantiate(policyUIPrefab, policyHolderParent.transform, false);
            RectTransform rect = entry.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y - (offset * entryIndex));

            PolicyEntryUI policyData = entry.GetComponent<PolicyEntryUI>();
            if(policyData != null)
            {
                policyData.DisplayFactionUpgrade(newUpgrades[entryIndex]);
                displayedUpgrades.Add(newUpgrades[entryIndex]);
            }
        }
        */
    }

    public void ToggleFactionPolicyUI()
    {
        if (!factionPolicyUI.activeSelf)
        {
            factionPolicyUI.gameObject.SetActive(true);
        } else
        {
            factionPolicyUI.gameObject.SetActive(false);
        }
    }

    public void DismissPolicyChoiceUI()
    {
        if (policyChoiceUI.activeSelf)
        {
            policyChoiceUI.SetActive(false);
        }
    }

    public void PromptPolicyChoice()
    {
        /*
        currentUpgradeChoices = FirebrandManager.firebrand.upgradeSelection;

        
        for(int x = 0; x < policySlots.Length; x++)
        {

            policySlots[x].SetActive(true);
            selectedPolicyIcons[x].SetActive(false);
            policySlotSprites[x].sprite = currentUpgradeChoices[x].upgradeImage;
            policyDescUI.SetActive(false);
            
        }
        
        policyChoiceUI.SetActive(true);
        */
    }

    public void SetActivePolicyChoice(int policyIndex)
    {
        /*
        foreach(GameObject g in selectedPolicyIcons)
        {
            g.SetActive(false);
        }

        selectedPolicyIcons[policyIndex].SetActive(true);

        if (!policyDescUI.activeSelf)
        {
            policyDescUI.SetActive(true);
        }

        policyTitle.text = currentUpgradeChoices[policyIndex].upgradeName;
        policyEffect.text = currentUpgradeChoices[policyIndex].upgradeEffectDesc;
        policyDesc.text = currentUpgradeChoices[policyIndex].upgradeFluffDesc;

        adoptButton.onClick.RemoveAllListeners();
        adoptButton.onClick.AddListener(() => { currentUpgradeChoices[policyIndex].Adopt(StrategyLayerManager.instance.currentFaction);  });
        adoptButton.onClick.AddListener(DismissPolicyChoiceUI);
        */
    }
}
