using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnHandler
{
    public string Attacker;
    public string Type;
    public GameObject AttackersGameObject;
    public Animator AttackersAnimator;

    public GameObject AttackTarget;
    public BaseAttack choosenAttack;

    public int fusion;
}
