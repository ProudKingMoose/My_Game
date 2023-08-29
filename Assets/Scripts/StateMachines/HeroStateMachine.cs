using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroStatemachine : MonoBehaviour
{
    private CombatStateMachine CSM;
    public BaseHero hero;

    public enum States
    {
        PROCESSING,
        ADDTOLIST,
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
    public GameObject EnemyTargeted;

    private bool alive = true;
    private bool actionStarted = false;

    void Start()
    {
        coldownLimit = 4;
        currentColdown = 0;

        Selector.SetActive(false);
        CSM = GameObject.Find("CombatManager").GetComponent<CombatStateMachine>();
        currentstate = States.PROCESSING;
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentstate)
        {
            case (States.PROCESSING):
                ProcessingProgress();
            break;
            case (States.ADDTOLIST):
                CSM.HerosReadyToAttack.Add (this.gameObject);
                currentstate = States.WAITING;
            break;

            case (States.WAITING):

            break;

            case (States.ATTACKING):
                StartCoroutine(HeroMeleeAttack());
            break;

            case (States.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    this.gameObject.tag = "DeadHero";

                    CSM.HerosReadyToAttack.Remove(this.gameObject);

                    CSM.Heroes.Remove(this.gameObject);

                    Selector.SetActive(false);

                    CSM.AttackPanel.SetActive(false);

                    for (int i = 0; i < CSM.HandlerList.Count; i++)
                    {
                        if (CSM.HandlerList[i].AttackersGameObject == this.gameObject)
                            CSM.HandlerList.Remove(CSM.HandlerList[i]);
                    }

                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);

                    alive = false;
                }
            break;
        }
    }

    void ProcessingProgress()
    {
        currentColdown += Time.deltaTime;
        if (currentColdown >= coldownLimit)
        {
            currentstate = States.ADDTOLIST;
        }
    }

    private IEnumerator HeroMeleeAttack()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        Vector3 enemyPos = new Vector3(EnemyTargeted.transform.position.x + 1.5f, EnemyTargeted.transform.position.y, EnemyTargeted.transform.position.z);
        while (MoveToEnemy(enemyPos)) { yield return null; }

        yield return new WaitForSeconds(0.5f);

        Vector3 startPos = startPosition;
        while (MoveToStartPos(startPos)) { yield return null; }

        CSM.HandlerList.RemoveAt(0);

        CSM.battleState = CombatStateMachine.Action.WAIT;

        actionStarted = false;
        currentColdown = 0;
        currentstate = States.PROCESSING;
    }

    private bool MoveToEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }
    private bool MoveToStartPos(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    public void TakeDamage(float damageAmount)
    {
        hero.currentHP -= damageAmount;
        if (hero.currentHP < 0)
            currentstate = States.DEAD;
    }
}
