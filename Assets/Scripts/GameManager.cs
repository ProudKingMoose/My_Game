using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    [System.Serializable]
    public class locationData
    {
        public string locationName;
        public int maxAmountOfEnemies = 5;
        public string BattleScene;
        public List<GameObject> enemies = new List<GameObject>();
    }

    public List<locationData> locations = new List<locationData>();

    public string sceneToLoad;
    public string lastScene;

    public bool isWalking = false;
    public bool encounterPosible = false;
    public bool attacked;

    public int currentLocation;

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
        amountOfEnemies = Random.Range(1, locations[currentLocation].maxAmountOfEnemies+1);

        for (int i = 0; i < amountOfEnemies; i++)
        {
            enemiesToBattle.Add(locations[currentLocation].enemies[Random.Range(0, locations[currentLocation].enemies.Count)]);
        }

        lastHeroPosition = GameObject.Find("Player").gameObject.transform.position;
        nextHeroPosition = lastHeroPosition;
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(locations[currentLocation].BattleScene);
        HeroReset();
    }

    void HeroReset()
    {
        isWalking = false;
        attacked = false;
        encounterPosible = false;
    }
}
