using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStateMachine : MonoBehaviour
{
    private CombatStateMachine CSM;
    public BaseEnemy enemy;

    public enum States
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ATTACKING,
        CHECKTURN,
        DEAD,
    }

    public States currentstate;

    private float coldownLimit;
    private float currentColdown;
    public GameObject Selector;

    private float animationsSpeed = 10;

    private Vector3 startPosition;
    public GameObject HeroTargeted;

    public Transform AttackNameSpacer;
    public GameObject AttackNamePanel;
    private List<GameObject> attackNames = new List<GameObject>();
    private String attackName;

    private bool actionStarted = false;

    private float chosenAttackDamage;

    private bool alive = true;
    
    void Start()
    {
        coldownLimit = 4;
        currentColdown = 0;

        Selector.SetActive(false);
        currentstate = States.CHECKTURN;
        AttackNameSpacer = GameObject.Find("AttackNameSpacer").GetComponent<Transform>();
        CSM = GameObject.Find("CombatManager").GetComponent<CombatStateMachine>();
        startPosition = transform.position;
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {

        switch (currentstate)
        {
            case (States.CHECKTURN):
                if (CSM.turn == CombatStateMachine.Turn.ENEMYTURN)
                    currentstate = States.PROCESSING;
                break;

            case (States.PROCESSING):
                Processing();
                break;

            case (States.CHOOSEACTION):
                ChooseAction();
                currentstate = States.WAITING;
                break;

            case (States.WAITING):
                if (CSM.HandlerList.Count == 0)
                {
                    CSM.turn = CombatStateMachine.Turn.HEROTURN;
                    currentstate = States.CHECKTURN;
                }
                break;

            case (States.ATTACKING):
                StartCoroutine(TimeForAction());
                break;

            case (States.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadEnemy";

                    CSM.Enemies.Remove(this.gameObject);

                    Selector.SetActive(false);

                    CSM.ActionPanel.SetActive(false);

                    for (int i = 0; i < CSM.HandlerList.Count; i++)
                    {
                        if (CSM.HandlerList[i].AttackersGameObject == this.gameObject)
                            CSM.HandlerList.Remove(CSM.HandlerList[i]);
                    }

                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);

                    alive = false;

                    CSM.battleState = CombatStateMachine.Action.ALIVECONTROL;
                }
                break;
        }
    }

    void Processing()
    {
        currentColdown += Time.deltaTime;
        if (currentColdown >= coldownLimit)
        {
            currentstate = States.CHOOSEACTION;
        }
    }

    void ChooseAction()
    {

        TurnHandler Attack = new TurnHandler();
        Attack.Attacker = enemy.theName;
        Attack.Type = "Enemy";
        Attack.AttackersGameObject = this.gameObject;
        Attack.AttackTarget = CSM.Heroes[UnityEngine.Random.Range(0, CSM.Heroes.Count)];
        Attack.choosenAttack = enemy.aviableAttacks[UnityEngine.Random.Range(0, enemy.aviableAttacks.Count)];
        chosenAttackDamage = Attack.choosenAttack.attackDamage;
        attackName = Attack.choosenAttack.name;
        Debug.Log(this.gameObject + "Has Chosen The Attack" + Attack.choosenAttack.attackName);
        CSM.TakeActions(Attack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        ANamePanel();

        Vector3 heroPos = new Vector3(HeroTargeted.transform.position.x - 1.5f, HeroTargeted.transform.position.y, HeroTargeted.transform.position.z);
        while (MoveToCharacters(heroPos)){yield return null;}

        yield return new WaitForSeconds(0.5f);

        DoDamage();

        RemoveAttackText();

        Vector3 startPos = startPosition;
        while (MoveToStartPos(startPos)) { yield return null; }

        CSM.HandlerList.RemoveAt(0);

        CSM.battleState = CombatStateMachine.Action.WAIT;

        actionStarted = false;
        currentColdown = 0;
        currentstate = States.WAITING;
    }

    void ANamePanel()
    {
        GameObject ANamePanel = Instantiate(AttackNamePanel) as GameObject;
        Text AttackNamePanelText = ANamePanel.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        AttackNamePanelText.text = attackName;
        ANamePanel.transform.SetParent(AttackNameSpacer, false);
        attackNames.Add(ANamePanel);
    }

    void RemoveAttackText()
    {
        foreach (GameObject name in attackNames)
        {
            Destroy(name);
        }
    }

    private bool MoveToCharacters(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    private bool MoveToStartPos(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    public void TakeDamage(float damageAmount)
    {
        enemy.currentHP -= damageAmount;
        if (enemy.currentHP < 0)
        {
            enemy.currentHP = 0;
            currentstate = States.DEAD;
        }
    }
    void DoDamage()
    {
        float calculatedDamage = enemy.currentAttackPower + chosenAttackDamage;
        HeroTargeted.GetComponent<HeroStatemachine>().TakeDamage(calculatedDamage);
    }
}
