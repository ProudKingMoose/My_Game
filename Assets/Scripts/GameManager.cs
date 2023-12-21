using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static BaseClass;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public RegionData currentRegion;

    public Inventory inventory;

    public ItemObject item1;
    public ItemObject item2;

    public GameObject heroCharacter;

    public string sceneToLoad;
    public string lastScene;

    public bool isWalking = false;
    public bool encounterPosible = false;
    public bool attacked;

    public int amountOfEnemies;
    public List<GameObject> enemiesToBattle = new List<GameObject>();
    public List<GameObject> heroesToBattle = new List<GameObject>();

    public Vector3 lastHeroPosition, nextHeroPosition;


    public List<HeroStatStorage> StatStorage = new List<HeroStatStorage>();
    public List<GameObject> HeroesUnlocked = new List<GameObject>();

    public enum Gamestates
    {
        OverWorld,
        SafePlace,
        CombatState,
        Idle
    }

    public Gamestates state;

    public void LoadSceneAfterBattle()
    {
        SceneManager.LoadScene(lastScene);
    }

    private void OnApplicationQuit()
    {
        if (inventory.ItemContainer != null)
           inventory.ItemContainer.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            inventory.AddItems(item1, 1);
        if (Input.GetKeyDown(KeyCode.I))
            inventory.AddItems(item2, 2);


        switch (state)
        {
            case Gamestates.OverWorld:
                if (isWalking)
                {
                    Encounter();
                }
                if (attacked)
                {
                    state = Gamestates.CombatState;
                    UnityEngine.Cursor.visible = true;
                    UnityEngine.Cursor.lockState = CursorLockMode.None;
                }
                break;

            case Gamestates.SafePlace:
                
                break;

            case Gamestates.CombatState:
                BattleStart();
                state = Gamestates.Idle;
                break;

            case Gamestates.Idle:

                break;
        }
        if (StatStorage.Count == 0)
            FirstStatStor();
    }
    private bool PartyCheck()
    {
        foreach (HeroStatStorage hero in StatStorage)
        {
            if (hero.inParty)
                return true;
        }
        return false;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("Player"))
        {
            GameObject Player = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;
            Player.name = "Player";
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

    }

    public void FirstStatStor()
    {
        foreach (GameObject Hero in HeroesUnlocked)
        {
            BaseHero HSM = Hero.GetComponent<HeroStatemachine>().hero;
            GameManager.instance.SavePlayerStats(HSM.theName, HSM.Level.currentXP,
                HSM.Level.currentLV, HSM.Level.XPToNextLevel,HSM.Type1, HSM.Type2, HSM.Type1Level, HSM.Type2Level,
                HSM.baseHP, HSM.currentHP, HSM.baseEnergy, HSM.currentEnergy,
                HSM.baseDefence, HSM.baseAttackPower, HSM.baseEDefence, HSM.baseAttackPower,
                HSM.EnergyAttacks);
        }
    }

    public void SavePlayerStats(string name, int XP, int LV, int XPNextLV, EnergyType1 Element1, EnergyType1 Element2, EnergyLevel Element1LV, EnergyLevel Element2LV, float baseHP, float curHP, float baseE, float curE, float baseDef, float baseAP, float baseED, float baseEAP, List<BaseAttack> energyAttacks)
    {
        bool alreadyInList = false;
        foreach (HeroStatStorage heroes in StatStorage)
        {
            if (heroes.theName == name)
            {
                alreadyInList = true;
                heroes.XP = XP;
                heroes.level = LV;
                heroes.XPToNextLevel = XPNextLV;

                heroes.Type1 = Element1;
                heroes.Type2 = Element2;
                heroes.Type1Level = Element1LV;
                heroes.Type2Level = Element2LV;

                heroes.baseHP = baseHP;
                heroes.currentHP = curHP;
                heroes.baseEnergy = baseE;
                heroes.currentEnergy = curE;

                heroes.baseDefence = baseDef;
                heroes.baseAttackPower = baseAP;
                heroes.baseEDefence = baseED;
                heroes.baseAttackPower = baseEAP;

                heroes.EnergyAttacks = energyAttacks;
            }
        }
        if (!alreadyInList)
        {
            HeroStatStorage curHeroStats = new HeroStatStorage();
            curHeroStats.theName = name;
            curHeroStats.XP = XP;
            curHeroStats.level = LV;
            curHeroStats.XPToNextLevel = XPNextLV;

            curHeroStats.Type1 = Element1;
            curHeroStats.Type2 = Element2;
            curHeroStats.Type1Level = Element1LV;
            curHeroStats.Type2Level = Element2LV;

            curHeroStats.baseHP = baseHP;
            curHeroStats.currentHP = curHP;
            curHeroStats.baseEnergy = baseE;
            curHeroStats.currentEnergy = curE;

            curHeroStats.baseDefence = baseDef;
            curHeroStats.baseAttackPower = baseAP;
            curHeroStats.baseEDefence = baseED;
            curHeroStats.baseAttackPower = baseEAP;

            curHeroStats.EnergyAttacks = energyAttacks;

            StatStorage.Add(curHeroStats);
        }
    }

    public HeroStatStorage GetPlayerStats(string name)
    {
        foreach (HeroStatStorage heroes in StatStorage)
        {
            if (heroes.theName == name)
                return heroes;
        }
        return null;
    }

    void Encounter()
    {
        if (isWalking && encounterPosible && PartyCheck())
            if (Random.Range(0, 5000) < 5)
            {
                Debug.Log("ATTACKED");
                attacked = true;
            }
    }

    void BattleStart()
    {
        amountOfEnemies = Random.Range(1, currentRegion.maxAmountOfEnemies);

        for (int i = 0; i < amountOfEnemies; i++)
        {
            enemiesToBattle.Add(currentRegion.enemies[Random.Range(0, currentRegion.enemies.Count)]);
        }

        lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
        nextHeroPosition = lastHeroPosition;
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentRegion.BattleScene);
        HeroReset();
    }

    void HeroReset()
    {
        isWalking = false;
        attacked = false;
        encounterPosible = false;
    }
}
