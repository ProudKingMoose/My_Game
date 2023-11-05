using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour
{
    public string attackName;

    public string attackDescription;

    public float attackDamage; //(BaseDamage + Strength LV) * (1 * EnergyTypeVariable)
    public float energyCost;

    public BaseClass.EnergyType1 type;
    public BaseClass.EnergyLevel LV;
}
