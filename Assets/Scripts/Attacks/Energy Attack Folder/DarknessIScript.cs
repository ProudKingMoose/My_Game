using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessIScript : BaseAttack
{
    DarknessIScript()
    {
        attackName = "Darkness I";
        attackDescription = "A Charge of energy that Creates Shadows around the opponent making your attack good for tagets with light based elements";
        attackDamage = 30;
        energyCost = 30;

        type = BaseClass.EnergyType1.Darkness;
        LV = BaseClass.EnergyLevel.I;
    }
}
