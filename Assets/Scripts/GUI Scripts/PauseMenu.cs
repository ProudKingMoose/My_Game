using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static EquipementObject;
using static UnityEditor.Progress;

public class PauseMenu : MonoBehaviour//FIX UI PROBLEMS WHERE THE HERO IN TEAM PANEL DOES NOT SHOW HEROES CORRECTLY AND DOES NOT WORK ATT AL.
{
    public static bool IsPaused = false;

    //PAUSEMENU OBJECTS
    [Header("PauseMenu")]
    public GameObject pauseMenuUI;

    public Transform PauseUIHeroDisplayer;

    //PARTYMENU OBJECTS
    [Header("PartyMenu")]
    public GameObject partyMenuUI;
    public GameObject HeroTeamButton;

    public Transform InPartySpacer;
    public Transform PartyUIHeroDisplayer;
    public Transform AvialableHeroesSpacer;

    private List<GameObject> HeroInTeamPanels = new List<GameObject>();
    private List<GameObject> HeroStatusPanelsDisplayed = new List<GameObject>();

    //ITEMMENU OBJECTS
    [Header("ItemMenu")]
    public GameObject itemMenuUI;
    public GameObject ItemBox;

    public Transform ItemBoxSpacer;

    private List<GameObject> ItemBoxes = new List<GameObject>();

    //STATMENU OBJECTS
    [Header("StatMenu")]
    public GameObject statMenuUI;
    public GameObject HeroStatusPanel;
    public GameObject HeroNamePanelStat;

    public Transform UnlockedHeroesStatSpacer;
    public Transform HeroStatsSpacer;

    //EQUIPEMENTMENU OBJECTS
    [Header("EquipementMenu")]
    public GameObject EquipementUI;
    public GameObject GearEquipButton;
    public GameObject HeroGear;
    public GameObject AviableGear;
    public GameObject ChangingGearHeroPanel;

    public Transform AviableGearSpacer;
    public Transform HeroChangingGearSpacer;

    private List<GameObject> GearShown = new List<GameObject>();

    private string heroChangeGearName;


    private void Awake()
    {
        AviableGear.SetActive(false);
        HeroGear.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (IsPaused)
                ExitUI();
            else
                Pause();
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        if (HeroInTeamPanels.Count == 0)
            TeamPanelUpdate();
    }

    void TeamPanelUpdate()
    {
        foreach (HeroStatStorage heroes in GameManager.instance.StatStorage)
        {
            if (heroes.inParty)
            {
                InTeamPanels(heroes);
            }
        }
        if (HeroStatusPanelsDisplayed.Count > 0)
        {
            foreach (GameObject panel in HeroStatusPanelsDisplayed)
            {
                Destroy(panel);
            }
            HeroStatusPanelsDisplayed.Clear();
        }
    }

    public void PartyUI()
    {
        pauseMenuUI.SetActive(false);
        partyMenuUI.SetActive(true);
        RemoveTeamPanels();
        if (InPartySpacer.transform.childCount == 0 && AvialableHeroesSpacer.transform.childCount == 0)
            CreateTeamButtons();
    }

    public void StatUI()
    {
        pauseMenuUI.SetActive(false);
        statMenuUI.SetActive(true);
        RemoveTeamPanels();
        if (UnlockedHeroesStatSpacer.transform.childCount == 0)
            CreateTeamButtons();
    }

    public void ItemUI()
    {
        pauseMenuUI.SetActive(false);
        itemMenuUI.SetActive(true);
        RemoveTeamPanels();
        if (ItemBoxSpacer.transform.childCount == 0)
            CreateItemBoxes();
        else
            UpdateItemBoxes();
    }

    public void GearUI()
    {
        pauseMenuUI.SetActive(false);
        EquipementUI.SetActive(true);
        RemoveTeamPanels();
        if (HeroChangingGearSpacer.transform.childCount == 0)
            CreateTeamButtons();
    }

    public void HeroCurrentGear(string name)
    {
        if (heroChangeGearName == null || heroChangeGearName != name)
        {
            ClearGearNamePanels();
            heroChangeGearName = name;
            foreach (HeroStatStorage hero in GameManager.instance.StatStorage)
                if (hero.theName == heroChangeGearName)
                {
                    int Armnum = 0;
                    int Gearnum = 0;
                    foreach (EquipementObject gear in hero.HeroGear)
                    {
                        if (gear.gearType == GearType.WEAPON)
                        {
                            HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                            HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot").GetComponent<GearEquipButton>().item = gear;
                        }
                        else if (gear.gearType == GearType.ARMOUR)
                        {
                            switch (Armnum)
                            {
                                case 0:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (1)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (1)").GetComponent<GearEquipButton>().item = gear;
                                    break;

                                case 1:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (2)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (2)").GetComponent<GearEquipButton>().item = gear;
                                    break;

                                case 2:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (3)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (3)").GetComponent<GearEquipButton>().item = gear;
                                    break;
                            }
                            Armnum++;
                        }
                        else if (gear.gearType == GearType.CORE)
                        {
                            switch (Gearnum)
                            {
                                case 0:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (4)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (4)").GetComponent<GearEquipButton>().item = gear;
                                    break;

                                case 1:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (5)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (5)").GetComponent<GearEquipButton>().item = gear;
                                    break;

                                case 2:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (6)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (6)").GetComponent<GearEquipButton>().item = gear;
                                    break;

                                case 3:
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (7)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = gear.name;
                                    HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (7)").GetComponent<GearEquipButton>().item = gear;
                                    break;
                            }
                            Gearnum++;
                        }
                    }
                }
        }
        HeroGear.SetActive(true);
    }

    void ClearGearNamePanels()
    {
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (1)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (1)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (2)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (2)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (3)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (3)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (4)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (4)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (5)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (5)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (6)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (6)").GetComponent<GearEquipButton>().item = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (7)").transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        HeroGear.transform.Find("HeroGearSpacer").transform.Find("Slot (7)").GetComponent<GearEquipButton>().item = null;
    }


    public void AddRemoveHero(GameObject heroButton, string HeroName)
    {
        foreach (HeroStatStorage heroes in GameManager.instance.StatStorage)
        {

            if (heroes.theName == HeroName)
            {
                if (heroes.inParty && InPartySpacer.transform.childCount > 1)
                {
                    heroButton.transform.SetParent(AvialableHeroesSpacer, false);
                    heroes.inParty = false;
                    RemoveFromParty(heroes);
                }
                else if (InPartySpacer.transform.childCount < 3)
                {
                    if (!heroes.inParty)
                    {
                        heroButton.transform.SetParent(InPartySpacer, false);
                        heroes.inParty = true;
                        foreach (GameObject heroPref in GameManager.instance.HeroesUnlocked)
                        {
                            if (heroes.theName == heroPref.name)
                                GameManager.instance.heroesToBattle.Add(heroPref);
                        }
                    }
                }
            }
        }
    }

    void RemoveFromParty(HeroStatStorage heroes)
    {
        foreach (GameObject hero in GameManager.instance.heroesToBattle)
        {
            if (heroes.theName == hero.name)
            {
                GameManager.instance.heroesToBattle.Remove(hero);
                return;
            }
        }
    }

    void InTeamPanels(HeroStatStorage Hero)
    {
        GameObject StatPanel = Instantiate(HeroStatusPanel) as GameObject;

        TextMeshProUGUI Name = StatPanel.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Level = StatPanel.transform.Find("Level Variable").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Health = StatPanel.transform.Find("Health Variable").gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Energy = StatPanel.transform.Find("Energy Variable").gameObject.GetComponent<TextMeshProUGUI>();

        Name.text = Hero.theName;
        Level.text = Hero.level.ToString();
        Health.text = Hero.currentHP.ToString() + "/" + Hero.baseHP;
        Energy.text = Hero.currentEnergy.ToString() + "/" + Hero.baseEnergy;

        StatPanel.transform.SetParent(PauseUIHeroDisplayer, false);

        HeroInTeamPanels.Add(StatPanel);
    }
    void RemoveTeamPanels()
    {
        foreach (GameObject panel in HeroInTeamPanels)
        {
            Destroy(panel);
        }
    }
    void RemoveGearShown()
    {
        foreach (GameObject gear in GearShown)
        {
            Destroy(gear);
        }
    }

    public void HoverDisplayHeroes(string HeroName)
    {
        if (HeroStatusPanelsDisplayed.Count > 0)
        {
            foreach(GameObject panel in HeroStatusPanelsDisplayed)
            {
                Destroy(panel);
            }
            HeroStatusPanelsDisplayed.Clear();
        }
        else
        {
            foreach (HeroStatStorage heroes in GameManager.instance.StatStorage)
            {
                if (heroes.theName == HeroName)
                {
                    GameObject StatPanel = Instantiate(HeroStatusPanel) as GameObject;

                    TextMeshProUGUI Name = StatPanel.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI Level = StatPanel.transform.Find("Level Variable").gameObject.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI Health = StatPanel.transform.Find("Health Variable").gameObject.GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI Energy = StatPanel.transform.Find("Energy Variable").gameObject.GetComponent<TextMeshProUGUI>();

                    Name.text = heroes.theName;
                    Level.text = heroes.level.ToString();
                    Health.text = heroes.currentHP.ToString() + "/" + heroes.baseHP;
                    Energy.text = heroes.currentEnergy.ToString() + "/" + heroes.baseEnergy;

                    StatPanel.transform.SetParent(PartyUIHeroDisplayer, false);

                    HeroStatusPanelsDisplayed.Add(StatPanel);
                }
            }
        }
    }

    public void HoverDisplayHeroStats(string HeroName)
    {
        foreach (HeroStatStorage Hero in GameManager.instance.StatStorage)
        {
            if (Hero.theName == HeroName)
            {
                TextMeshProUGUI LV = HeroStatsSpacer.Find("LVContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                LV.text = Hero.level.ToString();
                TextMeshProUGUI XP = HeroStatsSpacer.Find("XPContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                XP.text = Hero.XP.ToString();
                TextMeshProUGUI NextLV = HeroStatsSpacer.Find("NextLVXPContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();

                NextLV.text = Hero.XPToNextLevel.ToString();

                TextMeshProUGUI HP = HeroStatsSpacer.Find("HealthContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                HP.text = Hero.currentHP.ToString() + "/" + Hero.baseHP;
                TextMeshProUGUI En = HeroStatsSpacer.Find("EnergyContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                En.text = Hero.currentEnergy.ToString() + "/" + Hero.baseEnergy;

                TextMeshProUGUI E1 = HeroStatsSpacer.Find("Element1Container").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                E1.text = Hero.Type1.ToString() + " " + Hero.Type1Level;
                TextMeshProUGUI E2 = HeroStatsSpacer.Find("Element2Container").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                E2.text = Hero.Type2.ToString() + " " + Hero.Type2Level;

                TextMeshProUGUI DF = HeroStatsSpacer.Find("DefenceContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                DF.text = Hero.baseDefence.ToString();
                TextMeshProUGUI AP = HeroStatsSpacer.Find("AttackContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                AP.text = Hero.baseAttackPower.ToString();
                TextMeshProUGUI EDF = HeroStatsSpacer.Find("EDefenceContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                EDF.text = Hero.baseEDefence.ToString();
                TextMeshProUGUI EAP = HeroStatsSpacer.Find("EAttackContainer").transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>();
                EAP.text = Hero.baseEAttackPower.ToString();
            }
        }
    }

    void ExitUI()
    {
        if (partyMenuUI.activeSelf)
        {
            partyMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            TeamPanelUpdate();
        }
        else if (statMenuUI.activeSelf)
        {
            statMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            TeamPanelUpdate();
        }
        else if (itemMenuUI.activeSelf)
        {
            itemMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            TeamPanelUpdate();
        }
        else if (EquipementUI.activeSelf)
        {
            EquipementUI.SetActive(false);
            AviableGear.SetActive(false);
            HeroGear.SetActive(false);
            pauseMenuUI.SetActive(true);
            TeamPanelUpdate();
            RemoveTeamPanels();
        }
        else if (pauseMenuUI.activeSelf)
            Resume();

    }

    public void GearItemsEquipMenu(GearType gearToEquip, GameObject GearButtonClicked)
    {
        AviableGear.SetActive(true);
        RemoveGearShown();
        AviableGearSpacer.transform.Find("UnequipButton").GetComponent<GearEquipButton>().theButton = GearButtonClicked;

        switch (gearToEquip)
        {
            case GearType.WEAPON:
                foreach (InventorySlot itemslot in GameManager.instance.inventory.ItemContainer)
                {
                    if (itemslot.Item is EquipementObject)
                    {
                        EquipementObject item = itemslot.Item as EquipementObject;
                        if (item.gearType == GearType.WEAPON)
                        {
                            GameObject GearButton = Instantiate(GearEquipButton) as GameObject;
                            TextMeshProUGUI HeroButtonText = GearButton.transform.Find("SlotItem").gameObject.GetComponent<TextMeshProUGUI>();

                            GearEquipButton GEB = GearButton.GetComponent<GearEquipButton>();
                            GEB.item = item;
                            GEB.theButton = GearButtonClicked;

                            HeroButtonText.text = itemslot.Item.name;
                            GearButton.transform.SetParent(AviableGearSpacer, false);
                            GearShown.Add(GearButton);
                        }
                    }
                }
                break;

            case GearType.ARMOUR:
                foreach (InventorySlot itemslot in GameManager.instance.inventory.ItemContainer)
                {
                    if (itemslot.Item is EquipementObject)
                    {
                        EquipementObject item = itemslot.Item as EquipementObject;
                        if (item.gearType == GearType.ARMOUR)
                        {
                            GameObject GearButton = Instantiate(GearEquipButton) as GameObject;
                            TextMeshProUGUI HeroButtonText = GearButton.transform.Find("SlotItem").gameObject.GetComponent<TextMeshProUGUI>();

                            GearEquipButton GEB = GearButton.GetComponent<GearEquipButton>();
                            GEB.item = item;
                            GEB.theButton = GearButtonClicked;

                            HeroButtonText.text = itemslot.Item.name;
                            GearButton.transform.SetParent(AviableGearSpacer, false);
                            GearShown.Add(GearButton);
                        }
                    }
                }
                break;

            case GearType.CORE:
                foreach (InventorySlot itemslot in GameManager.instance.inventory.ItemContainer)
                {
                    if (itemslot.Item is EquipementObject)
                    {
                        EquipementObject item = itemslot.Item as EquipementObject;
                        if (item.gearType == GearType.CORE)
                        {
                            GameObject GearButton = Instantiate(GearEquipButton) as GameObject;
                            TextMeshProUGUI HeroButtonText = GearButton.transform.Find("SlotItem").gameObject.GetComponent<TextMeshProUGUI>();

                            GearEquipButton GEB = GearButton.GetComponent<GearEquipButton>();
                            GEB.item = item;
                            GEB.theButton = GearButtonClicked;

                            HeroButtonText.text = itemslot.Item.name;
                            GearButton.transform.SetParent(AviableGearSpacer, false);
                            GearShown.Add(GearButton);
                        }
                    }
                }
                break;
        }
    }

    public void EquipReplaceGear(EquipementObject item, GameObject button)
    {
        foreach (HeroStatStorage hero in GameManager.instance.StatStorage)
        {
            if (hero.theName == heroChangeGearName)
            {
                hero.HeroGear.Add(item);
                EquipementObject previousItem = button.GetComponent<GearEquipButton>().item;
                if (previousItem != null)
                    GameManager.instance.inventory.AddItems(previousItem, 1);
                hero.HeroGear.Remove(previousItem);
                GearStatChange(item, hero, button);
            }
        }
        GameManager.instance.inventory.RemoveItems(item, 1);
        button.transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = item.name;
        button.GetComponent<GearEquipButton>().item = item;

        RemoveGearShown();
        AviableGear.SetActive(false);
    }

    public void UnequipItem(GameObject button)
    {
        EquipementObject previousItem = button.GetComponent<GearEquipButton>().item;
        if (previousItem != null)
            GameManager.instance.inventory.AddItems(previousItem, 1);
        foreach (HeroStatStorage hero in GameManager.instance.StatStorage)
        {
            if (hero.theName == heroChangeGearName)
                hero.HeroGear.Remove(previousItem);
            GearStatChange(null, hero, button);
        }

        button.transform.Find("SlotItem").GetComponent<TextMeshProUGUI>().text = null;
        button.GetComponent<GearEquipButton>().item = null;
        RemoveGearShown();
        AviableGear.SetActive(false);
    }

    private void GearStatChange(EquipementObject item, HeroStatStorage hero, GameObject button)
    {
        foreach (HeroStatStorage HERO in GameManager.instance.StatStorage)
        {
            if (HERO == hero)
            {
                if (item != null)
                {
                    HERO.baseAttackPower += item.APDiff;
                    HERO.baseDefence += item.DPDiff;
                    HERO.baseEAttackPower += item.EPDiff;
                    HERO.baseEnergy += item.EPoolDiff;
                    HERO.baseHP += item.HPPoolDiff;
                    if (item.gearType == GearType.CORE)
                        HERO.EnergyAttacks.Add(item.CoreElement);//There are still problems with this part of the code (THE PREFABS STILL CHANGE!!!!!)
                }
                EquipementObject previousItem = button.GetComponent<GearEquipButton>().item;
                if (previousItem != null)
                {
                    HERO.baseAttackPower -= previousItem.APDiff;
                    HERO.baseDefence -= previousItem.DPDiff;
                    HERO.baseEAttackPower -= previousItem.EPDiff;
                    HERO.baseEnergy -= previousItem.EPoolDiff;
                    HERO.baseHP -= previousItem.HPPoolDiff;
                    if (previousItem.gearType == GearType.CORE)
                        HERO.EnergyAttacks.Remove(previousItem.CoreElement);
                }
            }
        }
    }


    void CreateItemBoxes()
    {
        foreach (InventorySlot itemSlot in GameManager.instance.inventory.ItemContainer)
        {
            if (itemMenuUI.activeSelf)
            {
                GameObject itemBox = Instantiate(ItemBox) as GameObject;
                TextMeshProUGUI ItemBoxItemName = itemBox.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI ItemBoxItemAmount = itemBox.transform.Find("Amount").gameObject.GetComponent<TextMeshProUGUI>();

                ItemBoxItemName.text = itemSlot.Item.name;
                ItemBoxItemAmount.text = itemSlot.Amount.ToString() + "X";

                itemBox.transform.SetParent(ItemBoxSpacer);
                ItemBoxes.Add(itemBox);
            }
        }
    }

    void UpdateItemBoxes()
    {
        foreach (GameObject boxes in ItemBoxes)
        {
            Destroy(boxes);
        }
        CreateItemBoxes();
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void CreateTeamButtons()
    {
        foreach (HeroStatStorage Hero in GameManager.instance.StatStorage)
        {
            if (partyMenuUI.activeSelf)
            {
                GameObject HeroButton = Instantiate(HeroTeamButton) as GameObject;
                TextMeshProUGUI HeroButtonText = HeroButton.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshProUGUI>();

                HeroTeamButton HTB = HeroButton.GetComponent<HeroTeamButton>();
                HTB.theButton = HeroButton;
                HTB.HeroName = Hero.theName;

                HeroButtonText.text = Hero.theName;
                if (Hero.inParty)
                    HeroButton.transform.SetParent(InPartySpacer, false);
                if (!Hero.inParty)
                    HeroButton.transform.SetParent(AvialableHeroesSpacer, false);
            }
            else if (statMenuUI.activeSelf)
            {
                GameObject HeroButton = Instantiate(HeroNamePanelStat) as GameObject;
                TextMeshProUGUI HeroButtonText = HeroButton.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshProUGUI>();

                HeroTeamButton HTB = HeroButton.GetComponent<HeroTeamButton>();
                HTB.HeroName = Hero.theName;

                HeroButtonText.text = Hero.theName;
                HeroButton.transform.SetParent(UnlockedHeroesStatSpacer, false);
            }
            else if (EquipementUI.activeSelf)//Complete this code
            {
                if (Hero.inParty)
                {
                    GameObject HeroButton = Instantiate(ChangingGearHeroPanel) as GameObject;
                    TextMeshProUGUI HeroButtonText = HeroButton.transform.Find("TextMeshPro").gameObject.GetComponent<TextMeshProUGUI>();

                    HeroTeamButton HTB = HeroButton.GetComponent<HeroTeamButton>();
                    HTB.theButton = HeroButton;
                    HTB.HeroName = Hero.theName;

                    HeroButtonText.text = Hero.theName;
                    HeroButton.transform.SetParent(HeroChangingGearSpacer, false);
                    HeroInTeamPanels.Add(HeroButton);
                }
            }
        }
    }

}
