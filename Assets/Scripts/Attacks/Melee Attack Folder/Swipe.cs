using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : BaseAttack
{
    Swipe()
    {
        attackName = "Swipe";
        attackDescription = "This is a basic swipe attack with any equiped weapon";
        attackDamage = 10f;
        energyCost = 0;
    }

}