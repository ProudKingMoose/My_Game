using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : MonoBehaviour
{
    public BaseAttack AbilityToPerform;

    public void AbilityCast()
    {
        GameObject.Find("CombatManager").GetComponent<CombatStateMachine>().Input3(AbilityToPerform);
    }
}
