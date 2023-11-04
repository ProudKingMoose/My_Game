using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public BaseAttack AbilityToPerform;
    public int ChosenFusionInt;//Change this to a enum value instead of an int value
    public void AbilityCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input3(AbilityToPerform);
    }
    public void FusionCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input6(ChosenFusionInt);
        Debug.Log("Current Fusion Chosen" + ChosenFusionInt);
    }
}
