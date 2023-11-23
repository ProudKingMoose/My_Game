using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject partyMenuUI;

    public GameObject HeroStatusPanel;
    public GameObject HeroTeamButton;

    public Transform InPartySpacer;
    public Transform AvialableHeroesSpacer;
    public Transform PartyUIHeroDisplayer;
    public Transform PauseUIHeroDisplayer;

    private List<GameObject> HeroStatusPanelsDisplayed = new List<GameObject>();
    private List<GameObject> HeroInTeamPanels = new List<GameObject>();

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

    public void AddRemoveHero(GameObject heroButton, string HeroName)
    {
        foreach (HeroStatStorage heroes in GameManager.instance.StatStorage)
        {

            if (heroes.theName == HeroName)
            {
                if (heroes.inParty)
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

    void ExitUI()
    {
        if (partyMenuUI.activeSelf)
        {
            partyMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            TeamPanelUpdate();
        }
        else if (pauseMenuUI.activeSelf)
            Resume();

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
    }
}
