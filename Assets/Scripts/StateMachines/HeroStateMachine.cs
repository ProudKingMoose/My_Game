using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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

    private HeroPanelStats Stats;
    private Transform HeroPanelSpacer;
    public GameObject HeroPanel;

    public Transform AttackNameSpacer;
    public GameObject AttackNamePanel;
    private List<GameObject> attackNames = new List<GameObject>();
    private String attackName;

    void Start()
    {
        coldownLimit = 4;
        currentColdown = 0;

        AttackNameSpacer = GameObject.Find("AttackNameSpacer").GetComponent<Transform>();

        HeroPanelSpacer = GameObject.Find("Battle UI").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        CreateHeroPanel();

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

                    CSM.ActionPanel.SetActive(false);

                    for (int i = 0; i < CSM.HandlerList.Count; i++)
                    {
                        if (CSM.HandlerList[i].AttackersGameObject == this.gameObject)
                            CSM.HandlerList.Remove(CSM.HandlerList[i]);
                    }

                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);

                    CSM.battleState = CombatStateMachine.Action.ALIVECONTROL;
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

        attackName = CSM.HandlerList[0].choosenAttack.name;

        actionStarted = true;

        ANamePanel();

        Vector3 enemyPos = new Vector3(EnemyTargeted.transform.position.x + 1.5f, EnemyTargeted.transform.position.y, EnemyTargeted.transform.position.z);
        while (MoveToEnemy(enemyPos)) { yield return null; }

        yield return new WaitForSeconds(0.5f);

        DoDamage();

        RemoveAttackText();

        Vector3 startPos = startPosition;
        while (MoveToStartPos(startPos)) { yield return null; }

        CSM.HandlerList.RemoveAt(0);

        if(CSM.battleState != CombatStateMachine.Action.WIN && CSM.battleState != CombatStateMachine.Action.LOSE)
        {
            CSM.battleState = CombatStateMachine.Action.WAIT;

            currentColdown = 0;
            currentstate = States.PROCESSING;
        }
        else
        {
            currentstate = States.WAITING;
        }

        actionStarted = false;
    }

    private bool MoveToEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }
    private bool MoveToStartPos(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
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

    public void TakeDamage(float damageAmount)
    {
        hero.currentHP -= damageAmount;
        if (hero.currentHP < 0)
        {
            hero.currentHP = 0;
            currentstate = States.DEAD;
        }
        HeroPanelUpdate();
    }
    void DoDamage()
    {
        float calculatedDamage = hero.currentAttackPower + CSM.HandlerList[0].choosenAttack.attackDamage;
        EnemyTargeted.GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage);
        hero.currentEnergy -= CSM.HandlerList[0].choosenAttack.energyCost;
        HeroPanelUpdate();
    }

    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel) as GameObject;
        Stats = HeroPanel.GetComponent<HeroPanelStats>();
        Stats.HeroName.text = hero.theName;
        Stats.HeroHP.text = "Health: " + hero.currentHP + "/" + hero.baseHP;
        Debug.Log(Stats.HeroHP.text);
        Stats.HeroEP.text = "Energy: " + hero.currentEnergy + "/" + hero.baseEnergy;

        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }
    void HeroPanelUpdate()
    {
        Stats.HeroHP.text = "Health: " + hero.currentHP + "/" + hero.baseHP;
        Stats.HeroEP.text = "Energy: " + hero.currentEnergy + "/" + hero.baseEnergy;
    }
}
