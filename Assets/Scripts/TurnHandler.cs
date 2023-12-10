using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BaseClass;

[System.Serializable]
public class TurnHandler
{
    public string Attacker;
    public string Type;
    public GameObject AttackersGameObject;
    public Animator AttackersAnimator;

    public GameObject AttackTarget;
    public BaseAttack choosenAttack;
    public RestoreObject choosenItem;
    public GameObject BuffTarget;

    public int fusion;
    public EnergyType1 Abilitytype;
    public EnergyLevel AbilityLV;
}
