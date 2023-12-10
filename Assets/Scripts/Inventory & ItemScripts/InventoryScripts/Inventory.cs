using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventorySlot> ItemContainer = new List<InventorySlot>();
    public void AddItems(ItemObject item, int amount)
    {
        bool alreadyInInventory = false;
        foreach (InventorySlot slot in ItemContainer)
        {
            if (slot.Item == item)
            {
                slot.AmountIncrease(amount);
                alreadyInInventory = true;
                break;
            }
        }
        if (!alreadyInInventory)
        {
            ItemContainer.Add(new InventorySlot(item, amount));
        }
    }
    public void RemoveItems(ItemObject item, int amount)
    {
        foreach (InventorySlot slot in ItemContainer)
        {
            if (slot.Item == item)
            {
                slot.AmountDecrease(amount);
                if (slot.Amount <= 0)
                    ItemContainer.Remove(slot);
                break;
            }
        }
    }
}