using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Restore Object", menuName = "Inventory/Items/Restores")]
public class RestoreObject : ItemObject
{
    public int healthRestore;
    public int energyRestore;
}
