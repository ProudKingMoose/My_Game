using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipement Object", menuName = "Inventory/Items/Equipement")]
public class EquipementObject : ItemObject
{
    public float APDiff;
    public float DPDiff;
    public float EPDiff;
    public float HPPoolDiff;
    public float EPoolDiff;
    public enum GearType
    {
        WEAPON,
        ARMOUR,
        CORE,
        NONE,
    }
    public GearType gearType;

    public BaseAttack CoreElement;
}
