using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass
{
    public string theName;

    public enum EnergyType1
    {
        None,
        Heat,
        Chill,
        Zapp,
        Light,
        Darkness,
    }

    public EnergyType1 Type1;

    public enum EnergyType2
    {
        None,
        Heat,
        Chill,
        Zapp,
        Light,
        Darkness,
    }

    public EnergyType2 Type2;

    public float baseHP;
    public float currentHP;

    public float baseEnergy;
    public float currentEnergy;

    public float baseDefence;
    public float currentDefence;

    public float baseAttackPower;
    public float currentAttackPower;

    public List<BaseAttack> aviableAttacks = new List<BaseAttack>();
}
