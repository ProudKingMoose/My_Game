using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillIScript : BaseAttack
{
    public ChillIScript()
    {
        attackName = "Chill I";
        attackDescription = "A Charge of energy that creates freezing temperatures that makes your attack good for warm targets";
        attackDamage = 20;
        energyCost = 10;
    }
}
