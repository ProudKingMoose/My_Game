using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatIScript : BaseAttack
{
    public HeatIScript()
    {
        attackName = "Heat I";
        attackDescription = "A Charge of energy that creates heat making your attack good for cold targets";
        attackDamage = 20;
        energyCost = 20;
    }
}
