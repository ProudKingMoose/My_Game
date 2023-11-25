using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseClass;

[System.Serializable]
public class HeroStatStorage
{
    public string theName;
    public int level;
    public int XP;
    public int XPToNextLevel;

    public EnergyType1 Type1;
    public EnergyLevel Type1Level;
    public EnergyType1 Type2;
    public EnergyLevel Type2Level;

    public float baseHP;
    public float currentHP;
    public float baseEnergy;
    public float currentEnergy;

    public float baseDefence;
    public float baseAttackPower;
    public float baseEDefence;
    public float baseEAttackPower;

    public List<BaseAttack> EnergyAttacks = new List<BaseAttack>();

    public bool inParty = false;
}
