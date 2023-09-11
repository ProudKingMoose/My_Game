using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseHero: BaseClass
{
    public int strength;
    public int energyPower;
    public int armour;
    public int elementDefence;
    public int elementCombinationPower;

    public List<BaseAttack> EnergyAttacks = new List<BaseAttack>();
}
