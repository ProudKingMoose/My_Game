using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static BaseClass;

public class CombatStateMachine : MonoBehaviour
{

    public enum Action
    {
        WAIT,
        INPUTACTION,
        PERFORMACTION,
        ALIVECONTROL,
        WIN,
        LOSE
    }

    public Action battleState;

    public enum Turn
    {
        HEROTURN = 0,
        ENEMYTURN = 1
    }
    public Turn turn;

    public List<TurnHandler> HandlerList = new List<TurnHandler>();
    public List<GameObject> Heroes = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();
    public List<GameObject> DeadEnemies = new List<GameObject>();
    public List<GameObject> DeadHeroes = new List<GameObject>();

    [SerializeField]
    public LayerMask interactableLayer;
    private int heroLayer = 7;
    private int enemyLayer = 6;
    private GameObject hoveredObject;
    private GameObject selectedObject;
    private bool selectEnemy;
    private bool selectHero;

    private GameObject FSPanel;

    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1,
        INPUT2,
        DONE
    }

    public HeroGUI HeroInput;

    public List <GameObject> HerosReadyToAttack = new List<GameObject>();
    private TurnHandler ChoisefromHero;

    public GameObject ActionPanel;
    public GameObject EnergyPanel;
    public GameObject FusionPanel;
    public GameObject ItemPanel;

    public Transform ActionSpacer;
    public Transform EnergySpacer;
    public Transform FusionSpacer;
    public Transform ItemSpacer;

    public GameObject ActionButton;
    public GameObject Ebutton;
    public GameObject FuseButton;
    public GameObject ItemButton;
    private List<GameObject> aButtons = new List<GameObject>();

    //spawnPoints
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> HeroPositions = new List<Transform>();

    private void Awake()
    {
        int pos = 0;
        for (int i = 0; i < GameManager.instance.amountOfEnemies; i++)
        {
            GameObject NewEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quaternion.identity) as GameObject;
            NewEnemy.name = NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName+"_" + i;
            NewEnemy.GetComponent<EnemyStateMachine>().enemy.theName = NewEnemy.name;
            Enemies.Add(NewEnemy);
        }
        foreach (GameObject hero in GameManager.instance.heroesToBattle)
        {
            GameObject NewHero = Instantiate(hero, HeroPositions[pos].position, Quaternion.identity) as GameObject;
            NewHero.name = NewHero.GetComponent<HeroStatemachine>().hero.theName;
            NewHero.GetComponent<HeroStatemachine>().hero.theName = NewHero.name;
            Heroes.Add(NewHero);
            pos++;
        }
    }
    void Start()
    {
        turn = Turn.HEROTURN;

        selectEnemy = false;
        selectHero = false;
        battleState = Action.WAIT;

        HeroInput = HeroGUI.ACTIVATE;
        ActionPanel.SetActive (false);
        EnergyPanel.SetActive (false);
        FusionPanel.SetActive (false);
        ItemPanel.SetActive (false);
    }

    // Update is called once per frame
    void Update()
    {

        switch (battleState)
        {
            case (Action.WAIT):
                if (HandlerList.Count > 0)
                    battleState = Action.INPUTACTION;
            break;

            case (Action.INPUTACTION):
                GameObject attacker = GameObject.Find(HandlerList[0].Attacker);
                if (HandlerList[0].Type == "Enemy")
                {
                    EnemyStateMachine ESM = attacker.GetComponent<EnemyStateMachine>();
                    for (int i = 0; i<Heroes.Count; i++)
                    {
                        if (HandlerList[0].AttackTarget == Heroes[i])
                        {
                            ESM.HeroTargeted = HandlerList[0].AttackTarget;
                            ESM.currentstate = EnemyStateMachine.States.ATTACKING;
                            break;
                        }
                        else
                        {
                            HandlerList[0].AttackTarget = Heroes[UnityEngine.Random.Range(0, Heroes.Count)];
                            ESM.HeroTargeted = HandlerList[0].AttackTarget;
                            ESM.currentstate = EnemyStateMachine.States.ATTACKING;
                        }
                    }
                }
                if (HandlerList[0].Type == "Hero")
                {
                    HeroStatemachine HSM = attacker.GetComponent<HeroStatemachine>();
                    if (HandlerList[0].AttackTarget != null)
                    {
                        HSM.EnemyTargeted = HandlerList[0].AttackTarget;
                        HSM.currentstate = HeroStatemachine.States.ATTACKING;
                    }
                    else if (HandlerList[0].BuffTarget != null)
                    {
                        HSM.currentstate = HeroStatemachine.States.BUFFING;
                    }
                }
                battleState = Action.PERFORMACTION;
                break;

            case (Action.PERFORMACTION):

            break;
            case (Action.ALIVECONTROL):
                if (turn == Turn.HEROTURN)
                {
                    if (Heroes.Count < 1)
                    {
                        battleState = Action.LOSE;
                    }
                    else if (Enemies.Count < 1)
                    {
                        battleState = Action.WIN;
                    }
                    else if (Enemies.Count > 1)
                        battleState = Action.INPUTACTION;
                    else
                    {
                        ClearAttackPanel();
                        HeroInput = HeroGUI.ACTIVATE;
                    }
                }
                break;

            case (Action.LOSE):

            break;
            case (Action.WIN):
                for (int i = 0; i < Heroes.Count; i++)
                {
                    Heroes[i].GetComponent<HeroStatemachine>().currentstate = HeroStatemachine.States.WAITING;
                }
                SaveStats();
                GameManager.instance.LoadSceneAfterBattle();
                GameManager.instance.state = GameManager.Gamestates.OverWorld;
                GameManager.instance.enemiesToBattle.Clear();

            break;
        }

        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                if (HerosReadyToAttack.Count > 0)
                {
                    HerosReadyToAttack[0].transform.Find("Selector").gameObject.SetActive(true);
                    ChoisefromHero = new TurnHandler();

                    ActionPanel.SetActive(true);
                    CreateCombatButtons();
                    HeroInput = HeroGUI.WAITING;
                }
            break;
            case (HeroGUI.WAITING):
                if (HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.FusionUses > 0)
                    FSPanel.GetComponent<Button>().interactable = false;

                if (selectEnemy)
                    HandleMouseClick("Enemy");
                else if (selectHero)
                    HandleMouseClick("Hero");
            break;
            case (HeroGUI.DONE):
                HeroInputDone();
            break;
        }
    }

    public void TakeActions(TurnHandler action)
    {
        HandlerList.Add(action);
    }

    private void SaveStats()
    {
        foreach (var Hero in Heroes)
        {
            Hero.GetComponent<HeroStatemachine>().hero.Level.AddXP(XPFromBattle());
            Hero.GetComponent<HeroStatemachine>().hero.Level.XPToNextLevel = Hero.GetComponent<HeroStatemachine>().hero.Level.GetXPForLevel(Hero.GetComponent<HeroStatemachine>().hero.Level.currentLV);
            BaseHero HSM = Hero.GetComponent<HeroStatemachine>().hero;
            GameManager.instance.SavePlayerStats(HSM.theName, HSM.Level.currentXP,
                HSM.Level.currentLV, HSM.Level.XPToNextLevel, HSM.Type1, HSM.Type2, HSM.Type1Level, HSM.Type2Level,
                HSM.baseHP, HSM.currentHP, HSM.baseEnergy, HSM.currentEnergy,
                HSM.baseDefence, HSM.baseAttackPower, HSM.baseEDefence, HSM.baseAttackPower,
                HSM.EnergyAttacks);
        }
        foreach (var DeadHero in DeadHeroes)
        {
            BaseHero HSM = DeadHero.GetComponent<HeroStatemachine>().hero;
            GameManager.instance.SavePlayerStats(HSM.theName, HSM.Level.currentXP,
                HSM.Level.currentLV, HSM.Level.XPToNextLevel, HSM.Type1, HSM.Type2, HSM.Type1Level, HSM.Type2Level,
                HSM.baseHP, HSM.currentHP, HSM.baseEnergy, HSM.currentEnergy,
                HSM.baseDefence, HSM.baseAttackPower, HSM.baseEDefence, HSM.baseAttackPower,
                HSM.EnergyAttacks);
        }
    }

    private int XPFromBattle()
    {
        int XP = 0;

        foreach (GameObject Enemy in DeadEnemies)
        {
            XP += Enemy.GetComponent<EnemyStateMachine>().enemy.XPValue;
        }
        return XP;
    }

    //The Button To Do A Melee Attack
    public void Input1()
    {
        ChoisefromHero.Attacker = HerosReadyToAttack[0].name;
        ChoisefromHero.AttackersGameObject = HerosReadyToAttack[0];
        ChoisefromHero.Type = "Hero";
        ChoisefromHero.choosenAttack = HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.aviableAttacks[0];
        selectEnemy = true;
        ActionPanel.SetActive (false);
    }

    //The Script For Selecting A Enemy
    public void Input2(GameObject selectedEnemy)
    {
        ChoisefromHero.AttackTarget = selectedEnemy;
        Debug.Log (ChoisefromHero.AttackTarget);
        HeroInput = HeroGUI.DONE;
    }
    public void Input2Buff(GameObject selectedHero)
    {
        ChoisefromHero.BuffTarget = selectedHero;
        HeroStatemachine HSM = HerosReadyToAttack[0].GetComponent<HeroStatemachine>();
        HeroInput = HeroGUI.DONE;
        Debug.Log(HeroInput);
    }

    public void Input3(BaseAttack choosenAbility, EnergyType1 AbilityType, EnergyLevel AbilityLV)
    {
        ChoisefromHero.Attacker = HerosReadyToAttack[0].name;
        ChoisefromHero.AttackersGameObject = HerosReadyToAttack[0];
        ChoisefromHero.Type = "Hero";

        ChoisefromHero.choosenAttack = choosenAbility;
        ChoisefromHero.AbilityLV = AbilityLV;
        ChoisefromHero.Abilitytype = AbilityType;

        selectEnemy = true;
        EnergyPanel.SetActive(false);
    }

    public void Input4()
    {
        ActionPanel.SetActive(false);
        EnergyPanel.SetActive(true);
    }

    public void Input5()
    {
        ActionPanel.SetActive(false);
        FusionPanel.SetActive(true);
    }

    public void Input6(int ChosenEnhancement)//Takes Input from the buttons that represent what type of fussions that hero can use and switches its statemachine to Fusion
    {
        ChoisefromHero.fusion = ChosenEnhancement;

        HeroStatemachine HSM = HerosReadyToAttack[0].GetComponent<HeroStatemachine>();

        HSM.hero.currentFusionType = (BaseClass.EnergyType1)ChosenEnhancement;

        HSM.currentstate = HeroStatemachine.States.FUSING;
    }

    public void Input7(RestoreObject item)
    {
        ChoisefromHero.Attacker = HerosReadyToAttack[0].name;
        ChoisefromHero.AttackersGameObject = HerosReadyToAttack[0];
        ChoisefromHero.Type = "Hero";

        ChoisefromHero.choosenItem = item;

        selectHero = true;
        ItemPanel.SetActive(false);
    }

    public void Input8()
    {
        ActionPanel.SetActive(false);
        ItemPanel.SetActive(true);
    }

    void HeroInputDone()
    {
        ClearAttackPanel();

        HerosReadyToAttack[0].transform.Find("Selector").gameObject.SetActive(false);
        HandlerList.Add(ChoisefromHero);
        HerosReadyToAttack.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }

    void ClearAttackPanel()
    {
        ActionPanel.SetActive(false);
        EnergyPanel.SetActive(false);
        FusionPanel.SetActive(false);
        ItemPanel.SetActive(false);
        foreach (GameObject aButton in aButtons)
        {
            Destroy(aButton);
        }
    }

    private void HandleMouseClick(string selectType)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (selectType == "Enemy")
        {
            interactableLayer |= (1 << enemyLayer);
            interactableLayer &= ~(1 << heroLayer);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
            {
                GameObject curHoveredObject = hit.collider.gameObject;

                if (curHoveredObject.tag != "DeadEnemy")
                {
                    curHoveredObject.transform.Find("Selector").gameObject.SetActive(true);

                    if (curHoveredObject != hoveredObject)
                    {
                        curHoveredObject.transform.Find("Selector").gameObject.SetActive(false);
                    }
                    hoveredObject = curHoveredObject;
                }
            }
            else if (hoveredObject != null)
                hoveredObject.transform.Find("Selector").gameObject.SetActive(false);


            if (Input.GetMouseButtonDown(0))
            {
                if (hoveredObject.tag != "DeadEnemy")
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
                    {
                        selectEnemy = false;
                        selectedObject = hit.collider.gameObject;
                        selectedObject.transform.Find("Selector").gameObject.SetActive(false);
                        Input2(selectedObject);
                    }
                }
            }
        }

        if (selectType == "Hero")
        {
            interactableLayer &= ~(1 << enemyLayer);
            interactableLayer |= (1 << heroLayer);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
            {
                GameObject curHoveredObject = hit.collider.gameObject;

                if (curHoveredObject.tag != "DeadHero")
                {
                    if (curHoveredObject.transform.Find("Selector").gameObject.activeSelf == false)
                        curHoveredObject.transform.Find("Selector").gameObject.SetActive(true);
                    else
                        curHoveredObject.transform.Find("Selector").gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;

                    if (curHoveredObject != hoveredObject)
                    {
                        curHoveredObject.transform.Find("Selector").gameObject.SetActive(false);
                    }
                    hoveredObject = curHoveredObject;
                }
            }
            else if (hoveredObject != null)
                hoveredObject.transform.Find("Selector").gameObject.SetActive(false);


            if (Input.GetMouseButtonDown(0))
            {
                if (hoveredObject.tag != "DeadHero")
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
                    {
                        selectHero = false;
                        selectedObject = hit.collider.gameObject;
                        selectedObject.transform.Find("Selector").gameObject.SetActive(false);
                        Input2Buff(selectedObject);
                    }
                }
            }
        }
    }

    void CreateCombatButtons()
    {
        GameObject AttackButton = Instantiate(ActionButton) as GameObject;
        Text AttackButtonText = AttackButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        AttackButtonText.text = "Attack";
        AttackButton.GetComponent<Button>().onClick.AddListener(() => Input1());
        AttackButton.transform.SetParent(ActionSpacer, false);
        aButtons.Add(AttackButton);

        GameObject EnergyAttackButton = Instantiate(ActionButton) as GameObject;
        Text EnergyAttackButtonText = EnergyAttackButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        EnergyAttackButtonText.text = "EP Attack";
        EnergyAttackButton.GetComponent<Button>().onClick.AddListener(() => Input4());
        EnergyAttackButton.transform.SetParent(ActionSpacer, false);
        aButtons.Add(EnergyAttackButton);

        GameObject itemButtonS = Instantiate(ActionButton) as GameObject;
        Text ItemButtonText = itemButtonS.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        ItemButtonText.text = "Items";
        itemButtonS.GetComponent<Button>().onClick.AddListener(() => Input8());
        itemButtonS.transform.SetParent(ActionSpacer, false);
        aButtons.Add(itemButtonS);

        GameObject FusionFuseButton = Instantiate(ActionButton) as GameObject;
        Text FusionFuseButtonText = FusionFuseButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        FusionFuseButtonText.text = "Fusion";
        FusionFuseButton.GetComponent<Button>().onClick?.AddListener(() => Input5());
        FusionFuseButton.transform.SetParent(ActionSpacer, false);
        aButtons.Add(FusionFuseButton);
        FSPanel = FusionFuseButton;

        if (HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.EnergyAttacks.Count > 0)
        {
            foreach (BaseAttack energyAbility in HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.EnergyAttacks)
            {
                GameObject EnergyButton = Instantiate(this.Ebutton) as GameObject;
                Text EnergyButtonText = EnergyButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
                EnergyButtonText.text = energyAbility.name;
                ActionButton AB = EnergyButton.GetComponent<ActionButton>();
                AB.AbilityToPerform = energyAbility;
                AB.Abilitytype = energyAbility.type;//there are problems here that need to be fixed 
                AB.AbilityLV = energyAbility.LV;
                EnergyButton.transform.SetParent(EnergySpacer, false);
                aButtons.Add(EnergyButton);
            }
        }
        else
        {
            EnergyAttackButton.GetComponent<Button>().interactable = false;
        }

        if (GameManager.instance.inventory.ItemContainer.Count > 0)
        {
            foreach (InventorySlot item in GameManager.instance.inventory.ItemContainer)
            {
                if (item.Item.type == ItemObject.ItemType.RESTOREITEM)
                {
                    GameObject itemsButton = Instantiate(ItemButton) as GameObject;
                    Text itemsButtonText = itemsButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
                    itemsButtonText.text = item.Item.name + "   : " + item.Amount + "X";
                    ActionButton ItemToUse = itemsButton.GetComponent<ActionButton>();
                    ItemToUse.ItemToUse = (RestoreObject)item.Item;
                    itemsButton.transform.SetParent(ItemSpacer, false);
                    aButtons.Add(itemsButton);
                }
            }
        }
        else
        {
            itemButtonS.GetComponent<Button>().interactable = false;
        }

        if (HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type1 != BaseClass.EnergyType1.None)
        {
            if (HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type1 != BaseClass.EnergyType1.None)
            {
                GameObject FusionButton = Instantiate(FuseButton) as GameObject;
                Text FusionButtonText = FusionButton.transform.Find("Text (Legacy)").gameObject.GetComponent <Text>();
                String FE = HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type1.ToString() + " " + HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type1Level.ToString();
                FusionButtonText.text = FE;
                ActionButton FusionType = FusionButton.GetComponent<ActionButton>();
                FusionType.ChosenFusionInt = ((int)HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type1);
                FusionButton.transform.SetParent(FusionSpacer, false);
                aButtons.Add(FusionButton);
            }
            if (HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type2 != BaseClass.EnergyType1.None)
            {
                GameObject FusionButton = Instantiate(FuseButton) as GameObject;
                Text FusionButtonText = FusionButton.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
                String FE = HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type2.ToString() + " " + HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type2Level.ToString();
                FusionButtonText.text = FE;
                ActionButton FusionType = FusionButton.GetComponent<ActionButton>();
                FusionType.ChosenFusionInt = ((int)HerosReadyToAttack[0].GetComponent<HeroStatemachine>().hero.Type2);
                FusionButton.transform.SetParent(FusionSpacer, false);
                aButtons.Add(FusionButton);
            }
        }
        else
        {
            FusionFuseButton.GetComponent<Button>().interactable = false;
        }
    }
}

