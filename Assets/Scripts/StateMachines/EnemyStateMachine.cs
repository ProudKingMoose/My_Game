using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
        DEAD,
    }

    public States currentstate;

    private float coldownLimit;
    private float currentColdown;
    public GameObject Selector;

    private float animationsSpeed = 10;

    private Vector3 startPosition;
    public GameObject HeroTargeted;

    private bool actionStarted = false;


    private float chosenAttackDamage;
    
    void Start()
    {
        coldownLimit = 4;
        currentColdown = 0;

        Selector.SetActive(false);
        currentstate = States.PROCESSING;
        CSM = GameObject.Find("CombatManager").GetComponent<CombatStateMachine>();
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentstate)
        {
            case (States.PROCESSING):
                Processing();
                break;

            case (States.CHOOSEACTION):
                ChooseAction();
                currentstate = States.WAITING;
                break;

            case (States.WAITING):

                break;

            case (States.ATTACKING):
                StartCoroutine(TimeForAction());
                break;

            case (States.DEAD):

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
        Attack.AttackTarget = CSM.Heroes[Random.Range(0, CSM.Heroes.Count)];
        Attack.choosenAttack = enemy.aviableAttacks[Random.Range(0, enemy.aviableAttacks.Count)];
        chosenAttackDamage = Attack.choosenAttack.attackDamage;
        Debug.Log(this.gameObject + "Has Chosen The Attack" + Attack.choosenAttack.attackName   );
        CSM.TakeActions(Attack);
    }

    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        Vector3 heroPos = new Vector3(HeroTargeted.transform.position.x - 1.5f, HeroTargeted.transform.position.y, HeroTargeted.transform.position.z);
        while (MoveToCharacters(heroPos)){yield return null;}

        yield return new WaitForSeconds(0.5f);

        DoDamage();

        Vector3 startPos = startPosition;
        while (MoveToStartPos(startPos)) { yield return null; }

        CSM.HandlerList.RemoveAt(0);

        CSM.battleState = CombatStateMachine.Action.WAIT;

        actionStarted = false;
        currentColdown = 0;
        currentstate = States.PROCESSING;
    }

    private bool MoveToCharacters(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    private bool MoveToStartPos(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    void DoDamage()
    {
        float calculatedDamage = enemy.currentAttackPower + chosenAttackDamage;
        HeroTargeted.GetComponent<HeroStatemachine>().TakeDamage(calculatedDamage);
    }
}
