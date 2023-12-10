using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseClass;

public class ActionButton : MonoBehaviour
{
    public BaseAttack AbilityToPerform;
    public RestoreObject ItemToUse;
    public int ChosenFusionInt;//Change this to a enum value instead of an int value
    public EnergyType1 Abilitytype;
    public EnergyLevel AbilityLV;
    public void AbilityCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input3(AbilityToPerform, Abilitytype, AbilityLV);
    }
    public void FusionCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input6(ChosenFusionInt);
        Debug.Log("Current Fusion Chosen" + ChosenFusionInt);
    }
    public void ItemCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input7(ItemToUse);
    }
}