using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

    public enum EnergyType1
    {
        None = 0,
        Heat = 1,
        Chill = 2,
        Zapp = 3,
        Light = 4,
        Darkness = 5,
    }

    public enum EnergyLevel
    {
        I = 1,
        II = 2,
        III = 3,
    }


    public EnergyType1 Type1;
    public EnergyLevel Type1Level;
    public EnergyType1 Type2;
    public EnergyLevel Type2Level;

    public LevelSystem Level;

    public float baseHP;
    public float currentHP;

    public float baseEnergy;
    public float currentEnergy;

    public float baseDefence;
    public float currentDefence;

    public float baseAttackPower;
    public float currentAttackPower;

    public float baseEDefence;
    public float currentEDefence;

    public float baseEAttackPower;
    public float currentEAttackPower;

    public float FusionUses;
    public int CurrentFusionTypeInt;
    public EnergyType1 currentFusionType;
    public EnergyLevel usedFusionLevel;
    public EnergyType1 EnergyEffectedType;
    public EnergyLevel EnergyLVEffected;
    public float FusionPower;

    public int effectDamageHold;
    public bool beenEffected;

    public Animator animator;

    public List<BaseAttack> aviableAttacks = new List<BaseAttack>();
}
