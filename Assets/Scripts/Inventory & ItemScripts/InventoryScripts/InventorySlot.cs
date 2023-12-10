using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemObject Item;
    public int Amount;
    public InventorySlot(ItemObject item, int amount)
    {
        Item = item;
        Amount = amount;
    }
    public void AmountIncrease(int amount)
    {
        Amount += amount;
    }
    public void AmountDecrease(int amount)
    {
        Amount -= amount;
    }
}
