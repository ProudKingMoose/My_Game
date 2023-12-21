using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour//FIX UI PROBLEMS WHERE THE HERO IN TEAM PANEL DOES NOT SHOW HEROES CORRECTLY AND DOES NOT WORK ATT AL.
{
    public static bool IsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject partyMenuUI;
    public GameObject statMenuUI;
    public GameObject itemMenuUI;

    public GameObject HeroStatusPanel;
    public GameObject HeroTeamButton;
    public GameObject HeroNamePanelStat;
    public GameObject ItemBox;

    public Transform InPartySpacer;
    public Transform HeroStatsSpacer;
    public Transform ItemBoxSpacer;

    public Transform AvialableHeroesSpacer;
    public Transform UnlockedHeroesStatSpacer;

    public Transform PartyUIHeroDisplayer;
    public Transform PauseUIHeroDisplayer;

    private List<GameObject> HeroStatusPanelsDisplayed = new List<GameObject>();
    private List<GameObject> HeroInTeamPanels = new List<GameObject>();
    private List<GameObject> ItemBoxes = new List<GameObject>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
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
        else if (pauseMenuUI.activeSelf)
            Resume();

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
        }
    }

}
