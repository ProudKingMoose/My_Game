using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearEquipButton : MonoBehaviour
{
    public GameObject theButton;
    public EquipementObject item;
    public EquipementObject.GearType gearType;

    public void OpenGearMenu()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().GearItemsEquipMenu(gearType, theButton);
    }
    public void EquipReplaceItem()
    {
        GameObject.Find("Canvas").GetComponent<PauseMenu>().EquipReplaceGear(item, theButton);
    }
}
