using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static BaseClass;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public RegionData currentRegion;

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

    void Update()
    {
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
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
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
            HeroStatemachine HSM = Hero.GetComponent<HeroStatemachine>();
            GameManager.instance.SavePlayerStats(HSM.hero.theName, HSM.hero.Level.currentXP,
                HSM.hero.Level.currentLV, HSM.hero.Type1, HSM.hero.Type2, HSM.hero.Type1Level, HSM.hero.Type2Level,
                HSM.hero.baseHP, HSM.hero.currentHP, HSM.hero.baseEnergy, HSM.hero.currentEnergy,
                HSM.hero.baseDefence, HSM.hero.baseAttackPower, HSM.hero.baseEDefence, HSM.hero.baseAttackPower,
                HSM.hero.EnergyAttacks);
        }
    }

    public void SavePlayerStats(string name, int XP, int LV, EnergyType1 Element1, EnergyType1 Element2, EnergyLevel Element1LV, EnergyLevel Element2LV, float baseHP, float curHP, float baseE, float curE, float baseDef, float baseAP, float baseED, float baseEAP, List<BaseAttack> energyAttacks)
    {
        bool alreadyInList = false;
        foreach (HeroStatStorage heroes in StatStorage)
        {
            if (heroes.theName == name)
            {
                alreadyInList = true;
                heroes.XP = XP;
                heroes.level = LV;

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
        if (isWalking && encounterPosible)
            if (Random.Range(0, 5000) < 5)
            {
                Debug.Log("ATTACKED");
                attacked = true;
            }
    }

    void BattleStart()
    {
        amountOfEnemies = Random.Range(1, currentRegion.maxAmountOfEnemies+1);

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
