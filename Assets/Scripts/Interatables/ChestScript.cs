using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{
    public ItemObject containedItem;
    private bool opened = false;
    public void Interact()
    {
        if (!opened)
            GameManager.instance.inventory.AddItems(containedItem, 1);
        opened = true;
    }
}
