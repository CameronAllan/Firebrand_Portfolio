using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InterestPointEntry : MonoBehaviour
{
    public int referencePointID;
    public Text pointName;
    public Image pointZeitgeist;

    public GameObject[] standingsNumbers;
    public Text[] standings;
    public Text[] standingPercents;

    public SeatIcon[] SeatIcons;
    public Color emptyColour1;
    public Color emptyColour2;

    public void DisplayInterestPoint(InterestPoint point)
    {

        foreach (SeatIcon s in SeatIcons)
        {
            s.gameObject.SetActive(false);
        }

        foreach (GameObject g in standingsNumbers)
        {
            g.SetActive(false);
        }

        foreach (Text t in standings)
        {
            t.gameObject.SetActive(false);
        }

        pointName.text = point.interestPointName;

        pointZeitgeist.sprite = FirebrandManager.firebrand.zeitgeistIcons[FirebrandManager.firebrand.zeitgeistLookup[point.leadingZeitgeist]];

        //get point standings and populate seats graph

        var pointSeats = point.partySeats.OrderByDescending(key => key.Value);
        var pointStandings = point.influenceStandings.OrderByDescending(key => key.Value);

        //populate poll standings
        for (int x = 0; x < pointStandings.Count() && x < standings.Count(); x++)
        {
            standings[x].text = StrategyLayerManager.instance.GetFaction(pointStandings.ElementAt(x).Key).factionName;
            standingPercents[x].text = pointStandings.ElementAt(x).Value.ToString();

            standings[x].gameObject.SetActive(true);
            standingsNumbers[x].SetActive(true);
        }

        int seatIconIndex = 0;
        int seats = point.seats;

        if (seats < SeatIcons.Count())
        {
            for (int y = 0; y < pointSeats.Count(); y++)
            {
                Faction f = StrategyLayerManager.instance.GetFaction(pointSeats.ElementAt(y).Key);
                int factionSeats = pointSeats.ElementAt(y).Value;

                //Debug.Log(f.factionName + " displays " + factionSeats + " at " + point.interestPointName);
                if (factionSeats > 0)
                {
                    
                    for(int x = 0; x < factionSeats; x++)
                    {
                        SeatIcons[seatIconIndex].UpdateColours(f.factionColor1, f.factionColor2, true);
                        seatIconIndex++;
                    }
                }
            }

            /*
            int unclaimedSeats = seats - (seatIconIndex - 1);

            if(unclaimedSeats > 0)
            {
                for(int z = unclaimedSeats; unclaimedSeats <= 0; z--)
                {
                    SeatIcons[seatIconIndex].UpdateColours(emptyColour1, emptyColour2, true);
                    seatIconIndex++;
                }
            }
            */
        }
    }
}
