using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InterestPointUI : MonoBehaviour
{
    public Text interestPointName;

    public Image interestPointImage;
    public Text interestPointDescription;

    public Text playerInfluenceLevel;
    public Text leaderInfluenceLevel;
    public Text leaderInfluenceName;
    public Image leaderInfluenceIcon;


    public GameObject intrigueLevelImage;
    public float intrigueBarAnimMin;

    [SerializeField]
    Image intrigueBarColour1;

    [SerializeField]
    Image intrigueBarColour2;

    public Text currentZeitgeist;
    public Image currentZeitgeistIcon;


    public Text populationLevelText;
    public Text publicOrderLevelText;
    public Text enfranchisementLevelText;

    public Text ethnicityText;
    public Text economyText;


    /*
    public Text workerRecruitLevel;
    public Text veteranRecruitLevel;
    public Text criminalRecruitLevel;
    public Text academicRecruitLevel;



    public Text contentmentLevelText;
    public Text nationalismLevelText;
    public Text equalityLevelText;
    public Text securityLevelText;
    public Text materialismLevelText;
    public Text libertyLevelText;
    public Text identityLevelText;
    */


    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void PopulatePointInfo(InterestPoint point)
    {
        interestPointName.text = point.interestPointName;

        //interestPointImage - Later
        interestPointDescription.text = point.interestPointDesc;

        /*
        workerRecruitLevel.text = point.recruitLevelFollowers.ToString();
        veteranRecruitLevel.text = point.recruitLevelMilitants.ToString();
        criminalRecruitLevel.text = point.recruitLevelRadicals.ToString();
        academicRecruitLevel.text = point.recruitLevelActivists.ToString();
        */

        //Influence vars
        int leaderTotal = point.influenceStandings.Values.Max();
        int leaderID = point.influenceStandings.FirstOrDefault(f => f.Value == leaderTotal).Key;
        Faction leader = StrategyLayerManager.instance.GetFaction(leaderID);
        string leaderName = leader.factionName;
        Sprite leaderIcon = leader.factionLogo;

        int playerTotal = 0;

        if (point.influenceStandings.ContainsKey(StrategyLayerManager.instance.currentFaction.factionNumber))
        {
            playerTotal = point.influenceStandings[StrategyLayerManager.instance.currentFaction.factionNumber];
        } 
        
        

        playerInfluenceLevel.text = playerTotal.ToString() + "%";
        leaderInfluenceLevel.text = leaderTotal.ToString() + "%";
        leaderInfluenceName.text = leaderName;
        leaderInfluenceIcon.sprite = leaderIcon;


        //Demographic vars
        switch (point.populationLevel)
        {
            case 1:
                populationLevelText.text = "Sparse";
                break;
            case 2:
                populationLevelText.text = "Low";
                break;
            case 3:
                populationLevelText.text = "Medium";
                break;
            case 4:
                populationLevelText.text = "High";
                break;
            case 5:
                populationLevelText.text = "Dense";
                break;
        }

        switch (point.publicOrderLevel)
        {
            case 1:
                publicOrderLevelText.text = "Anarchy";
                break;
            case 2:
                publicOrderLevelText.text = "Wavering";
                break;
            case 3:
                publicOrderLevelText.text = "Stable";
                break;
            case 4:
                publicOrderLevelText.text = "Secure";
                break;
            case 5:
                publicOrderLevelText.text = "Crackdown";
                break;
        }

        switch (point.enfranchisementLevel)
        {
            case 1:
                enfranchisementLevelText.text = "Suppressed";
                break;
            case 2:
                enfranchisementLevelText.text = "Disenfranchised";
                break;
            case 3:
                enfranchisementLevelText.text = "Represented";
                break;
            case 4:
                enfranchisementLevelText.text = "Engaged";
                break;
            case 5:
                enfranchisementLevelText.text = "Dominant";
                break;
        }

        //Ethnic Majority/Minority display
        if(point.hasMajorityPop && point.hasMinorityPop)
        {
            ethnicityText.text = "Cosmopolitan";
        } else if (point.hasMinorityPop)
        {
            ethnicityText.text = "Ethnic Minority";
        } else if (point.hasMajorityPop)
        {
            ethnicityText.text = "Ethnic Majority";
        } else
        {
            ethnicityText.text = "Scattered";
        }

        //Upper/Lower Class display
        if(point.hasUpperClassPop && point.hasWorkingClassPop)
        {
            economyText.text = "Upper/Working Class";
        } else if (point.hasWorkingClassPop)
        {
            economyText.text = "Working Class";
        } else if (point.hasUpperClassPop)
        {
            economyText.text = "Upper Class";
        } else
        {
            economyText.text = "Scattered";
        }


        currentZeitgeist.text = point.currentZeitgeist.ToString();
        //TODO: Get the Zeitgeist icon, too.

        /*
        contentmentLevelText.text = point.contentmentLevel.ToString();
        nationalismLevelText.text = point.nationalismLevel.ToString();
        equalityLevelText.text = point.equalityLevel.ToString();
        securityLevelText.text = point.securityLevel.ToString();
        materialismLevelText.text = point.materialismLevel.ToString();
        libertyLevelText.text = point.libertyLevel.ToString();
        identityLevelText.text = point.identityLevel.ToString();
        */
        //intrigueDebug.text = StrategyLayerManager.instance.currentFaction.pointInfoDictionary[point.interestPointName].ToString();

        //Intrigue Bar Transform
        RectTransform uiRT = transform.GetComponent<RectTransform>();
        Vector2 uiSize = uiRT.sizeDelta;

        RectTransform intrigueRT = intrigueLevelImage.GetComponent<RectTransform>();
        float barMultiplier;
        float infoMax = FirebrandManager.firebrand.interestPointInfoMax;
        if (StrategyLayerManager.instance.currentFaction.pointInfoDictionary[point.interestPointName] < intrigueBarAnimMin)
        {
            barMultiplier = intrigueBarAnimMin / infoMax;
        }
        else
        {
            barMultiplier = StrategyLayerManager.instance.currentFaction.pointInfoDictionary[point.interestPointName] / infoMax;
        }

        Debug.Log("Intrigue Bar Max: " + uiRT.sizeDelta.y);
        Debug.Log("Bar Multiplier: " + barMultiplier);
        Debug.Log("Current intrigue height: " + uiRT.sizeDelta.y * barMultiplier);
        intrigueRT.sizeDelta = new Vector2(intrigueRT.sizeDelta.x, uiRT.sizeDelta.y * barMultiplier);

        
    }
}
