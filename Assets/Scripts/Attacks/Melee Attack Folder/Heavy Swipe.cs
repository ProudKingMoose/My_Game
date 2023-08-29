using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavySwipe : BaseAttack
{
    HeavySwipe()
    {
        attackName = "HeavySwipe";
        attackDescription = "This is a stronger version of the normal Swipe, it does more damage but it can only be used with heavy type weapons";
        attackDamage = 20;
        energyCost = 0;
    }
}
