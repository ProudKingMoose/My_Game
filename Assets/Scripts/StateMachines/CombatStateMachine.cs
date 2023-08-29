using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStateMachine : MonoBehaviour
{

    public enum Action
    {
        WAIT,
        INPUTACTION,
        PERFORMACTION,
    }

    public Action battleState;

    public List<TurnHandler> HandlerList = new List<TurnHandler>();
    public List<GameObject> Heroes = new List<GameObject>();
    public List<GameObject> Enemies = new List<GameObject>();

    [SerializeField]
    public LayerMask interactableLayer;
    private GameObject hoveredObject;
    private GameObject selectedObject;
    private bool selectEnemy;

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

    public GameObject AttackPanel;

    void Start()
    {
        selectEnemy = false;
        battleState = Action.WAIT;
        Enemies.AddRange(GameObject.FindGameObjectsWithTag ("Enemy"));
        Heroes.AddRange(GameObject.FindGameObjectsWithTag("Hero"));

        HeroInput = HeroGUI.ACTIVATE;
        AttackPanel.SetActive (false);
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
                Debug.Log (attacker);
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
                            HandlerList[0].AttackTarget = Heroes[Random.Range(0, Heroes.Count)];
                            ESM.HeroTargeted = HandlerList[0].AttackTarget;
                            ESM.currentstate = EnemyStateMachine.States.ATTACKING;
                        }
                    }
                }
                if (HandlerList[0].Type == "Hero")
                {
                    HeroStatemachine HSM = attacker.GetComponent<HeroStatemachine>();
                    HSM.EnemyTargeted = HandlerList[0].AttackTarget;
                    HSM.currentstate = HeroStatemachine.States.ATTACKING;
                }
                battleState = Action.PERFORMACTION;
                break;

            case (Action.PERFORMACTION):

            break;
        }

        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                if (HerosReadyToAttack.Count > 0)
                {
                    HerosReadyToAttack[0].transform.Find("Selector").gameObject.SetActive (true);
                    ChoisefromHero = new TurnHandler ();

                    AttackPanel.SetActive(true);
                    HeroInput = HeroGUI.WAITING;
                }
            break;
            case (HeroGUI.WAITING):
                if (selectEnemy)
                    HandleMouseClick();
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

    //The Button To Do A Melee Attack
    public void Input1()
    {
        ChoisefromHero.Attacker = HerosReadyToAttack[0].name;
        ChoisefromHero.AttackersGameObject = HerosReadyToAttack[0];
        ChoisefromHero.Type = "Hero";

        selectEnemy = true;
        AttackPanel.SetActive (false);
    }

    //The Script For Selecting A Enemy
    public void Input2(GameObject selectedEnemy)
    {
        ChoisefromHero.AttackTarget = selectedEnemy;
        Debug.Log (ChoisefromHero.AttackTarget);
        HeroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        HerosReadyToAttack[0].transform.Find("Selector").gameObject.SetActive(false);
        HandlerList.Add(ChoisefromHero);
        HerosReadyToAttack.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }

    private void HandleMouseClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, interactableLayer))
        {
            GameObject curHoveredObject = hit.collider.gameObject;

            curHoveredObject.transform.Find("Selector").gameObject.SetActive(true);

            if (curHoveredObject != hoveredObject)
            {
                curHoveredObject.transform.Find("Selector").gameObject.SetActive(false);
            }
            hoveredObject = curHoveredObject;
        }
        else if (hoveredObject != null)
            hoveredObject.transform.Find("Selector").gameObject.SetActive(false);


        if (Input.GetMouseButtonDown(0))
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

