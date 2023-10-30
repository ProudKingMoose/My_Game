using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public Vector3 lastHeroPosition, nextHeroPosition;

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
