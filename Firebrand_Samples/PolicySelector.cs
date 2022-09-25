using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PolicySelector : MonoBehaviour
{
    /*
    private System.Random _rnd;

    public void Start()
    {
        _rnd = new System.Random();
    }

    public List<FactionUpgrade> randomUpgrades(Faction faction)
    {
		
		List<FactionUpgrade> allUpgrades = new List<FactionUpgrade>();

        foreach (FactionUpgradeTrack factionUpgradeTrack in faction.upgradeDictionary.Keys)
        {
            foreach (FactionUpgrade upgrade in factionUpgradeTrack.allUpgrades)
            {
                allUpgrades.Add(upgrade);
            }
        }
		if (allUpgrades.Count <= 3)
		{
			return allUpgrades;
		}

		List<FactionUpgrade> returnUpgrades = new List<FactionUpgrade>();

		while (returnUpgrades.Count < 3)
		{
			int randint = _rnd.Next(0, allUpgrades.Count);
			FactionUpgrade upgrade = allUpgrades[randint];
            if (!returnUpgrades.Contains(upgrade))
			{
				returnUpgrades.Add(upgrade);
			}
		}

		return returnUpgrades;


    }
    */
}
