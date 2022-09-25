using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfluenceUI : MonoBehaviour
{
    //InterestPoint Vars
    public GameObject interestPointScrollContent;
    public GameObject interestPointEntryPrefab;

    public float interestPointEntryPadding;

    List<InterestPointEntry> entries;


    //Influence Bar Vars
    public GameObject influenceBarAnchor;

    public GameObject influenceBarPrefab;
    public float barMinWidth;

    public List<GameObject> playerBarList;

    public GameObject authorityBar;
    RectTransform authorityRT;
    Vector2 authBarSize;

    float totalPlayerBarWidth;

    RectTransform anchorRT;
    Vector2 anchorSize;
    /*
    public GameObject[] playerInfluenceNumbers;

    public List<GameObject> influenceNumbers;

    public GameObject authorityInfluenceNumber;
    */

    public void Initialize()
    {
        playerBarList = new List<GameObject>();

        authorityRT = authorityBar.GetComponent<RectTransform>();
        authBarSize = authorityRT.sizeDelta;

        foreach (Faction f in StrategyLayerManager.instance.factions)
        {
            GameObject bar = Instantiate(influenceBarPrefab, influenceBarAnchor.transform, false);
            InfluenceBarUI barUI = bar.GetComponent<InfluenceBarUI>();
            barUI.ownerFaction = f.factionNumber;

            playerBarList.Add(bar);
            /*
            Text playerNameText = playerInfluenceNumbers[f.factionNumber].GetComponent<Text>();
            playerNameText.text = f.factionName;
            */


        }

        entries = new List<InterestPointEntry>();
        List <InterestPoint> points = StrategyLayerManager.instance.interestPoints;
        points.OrderBy(p => p.interestPointName);

        float offset = influenceBarPrefab.GetComponent<RectTransform>().rect.height;
        offset += interestPointEntryPadding;

        //int entryIndex = 0;
        //foreach (InterestPoint i in points)
        for(int entryIndex = 0; entryIndex < points.Count; entryIndex++)
        {
            InterestPoint i = points[entryIndex];

            GameObject entry = Instantiate(interestPointEntryPrefab, interestPointScrollContent.transform, false);
            RectTransform rect = entry.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y-(offset * entryIndex));
            //entry.transform.position = new Vector2(transform.position.x, -(offset * entryIndex));

            InterestPointEntry entryData = entry.GetComponent<InterestPointEntry>();
            entryData.referencePointID = i.interestPointID;
            entryData.DisplayInterestPoint(i);
            entries.Add(entryData);

            //entryIndex++;
        }

        RectTransform contentRect = interestPointScrollContent.GetComponent<RectTransform>();
        contentRect.sizeDelta = new Vector2(0, entries.Count * offset);

        UpdateInfluenceStandings();
        gameObject.SetActive(false);
    }

    public void UpdateInfluenceStandings()
    {
        int barIndex = 0;
        anchorRT = influenceBarAnchor.GetComponent<RectTransform>();
        anchorSize = anchorRT.sizeDelta;
        Debug.Log(anchorSize.ToString());

        foreach (GameObject bar in playerBarList)
        {
            InfluenceBarUI ui = bar.GetComponent<InfluenceBarUI>();
            float influenceTotal = StrategyLayerManager.instance.factions.Find(f => f.factionNumber == ui.ownerFaction).influencePoints;

            RectTransform barRT = bar.GetComponent<RectTransform>();
            Vector2 barSize = barRT.sizeDelta;

            float influenceMax = FirebrandManager.firebrand.totalInfluence;
            float barMultiplier;

            if (influenceTotal / influenceMax < barMinWidth || influenceTotal <= 0)
            {
                Debug.Log("Using Default Width");
                barMultiplier = barMinWidth;
            }
            else
            {
                Debug.Log("Calculating Custom Width");
                barMultiplier = influenceTotal / influenceMax;
            }

            barRT.sizeDelta = new Vector2(anchorSize.x * barMultiplier, barSize.y);

            if (barIndex == 0)
            {
                totalPlayerBarWidth = barRT.sizeDelta.x;
            }
            else
            {
                barRT.anchoredPosition = new Vector2(totalPlayerBarWidth, barRT.anchoredPosition.y);
                totalPlayerBarWidth = totalPlayerBarWidth + barRT.sizeDelta.x;
            }

            barIndex++;

            Debug.Log("Calculated Bar #: " + barIndex + " Influence Total: " + influenceTotal + " Influence Max: "+influenceMax);
        }

        authBarSize = authorityRT.sizeDelta;

        float remainingBarWidth = anchorSize.x - totalPlayerBarWidth;
        authorityRT.sizeDelta = new Vector2(remainingBarWidth, authBarSize.y);



    }

    public void UpdateInterestPointStandings()
    {
        foreach(InterestPointEntry e in entries)
        {
            e.DisplayInterestPoint(StrategyLayerManager.instance.GetInterestPoint(e.referencePointID));
        }
    }

}

