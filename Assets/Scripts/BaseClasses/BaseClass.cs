using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

    public enum EnergyType1
    {
        None = 0,
        Heat_I = 1,
        Chill_I = 2,
        Zapp_I = 3,
        Light_I = 4,
        Darkness_I = 5,
        Heat_II = 6,
        Chill_II = 7,
        Zapp_II = 8,
        Light_II = 9,
        Darkness_II = 10,
        Heat_III = 11,
        Chill_III = 12,
        Zapp_III = 13,
        Light_III = 14,
        Darkness_III = 15,
    }

    public EnergyType1 Type1;
    public EnergyType1 Type2;

    public float baseHP;
    public float currentHP;

    public float baseEnergy;
    public float currentEnergy;

    public float baseDefence;
    public float currentDefence;

    public float baseAttackPower;
    public float currentAttackPower;

    public float FusionUses;
    public float CurrentFusionTypeInt;
    public EnergyType1 currentFusionType;
    public float FusionPower;

    public List<BaseAttack> aviableAttacks = new List<BaseAttack>();
}
