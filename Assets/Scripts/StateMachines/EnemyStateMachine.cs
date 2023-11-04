using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BaseClass;

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
                    //EffectDamageActivate();
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

                    CSM.battleState = CombatStateMachine.Action.ALIVECONTROL;

                    alive = false;
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

    void FindEnergyCurrentLV()
    {
        if (enemy.currentFusionType == enemy.Type1)
            enemy.usedFusionLevel = enemy.Type1Level;
        else if (enemy.currentFusionType == enemy.Type2)
            enemy.usedFusionLevel = enemy.Type2Level;
    }

    private bool MoveToCharacters(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    private bool MoveToStartPos(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animationsSpeed * Time.deltaTime));
    }

    public void TakeDamage(float damageAmount, EnergyType1 effect, EnergyLevel level)
    {
        enemy.currentHP -= damageAmount;
        if (enemy.currentHP < 0)
        {
            enemy.currentHP = 0;
            currentstate = States.DEAD;
        }
        if (effect != EnergyType1.None)
        {
            enemy.EnergyEffectedType = effect;
            enemy.EnergyLVEffected = level;
            enemy.beenEffected = true;
            Debug.Log(enemy.beenEffected + "effected by" + enemy.EnergyEffectedType + " " + enemy.EnergyLVEffected);
        }
    }

    public void TakeEffectDamage(float heroEPower)
    {
        float DMGMult = EffectLVCalculation();

        float totDMG = (heroEPower - enemy.currentEDefence) * DMGMult;
        if (totDMG < 0)
            totDMG = 0;
        enemy.currentHP -= totDMG;
        enemy.effectDamageHold--;

        if (enemy.currentHP < 0)
        {
            enemy.currentHP = 0;
            currentstate = States.DEAD;
        }

        Debug.Log(totDMG + " Damage was done to " + enemy.theName);
        if (enemy.effectDamageHold == 0)
            enemy.beenEffected = false;
    }
    void DoDamage()
    {
        float calculatedDamage = enemy.currentAttackPower + chosenAttackDamage;
        FindEnergyCurrentLV();
        HeroTargeted.GetComponent<HeroStatemachine>().TakeDamage(calculatedDamage, enemy.currentFusionType, enemy.usedFusionLevel);
    }

    void EffectDamageActivate()
    {
        foreach (GameObject hero in CSM.Heroes)
        {
            if (hero.GetComponent<HeroStatemachine>().hero.beenEffected)
                hero.GetComponent<HeroStatemachine>().TakeEffectDamage();
        }
    }

    float EffectLVCalculation()
    {
        switch (enemy.EnergyLVEffected)
        {
            case EnergyLevel.I:
                if (enemy.effectDamageHold == 0)
                    enemy.effectDamageHold = 1;
                return 1;
            case EnergyLevel.II:
                if (enemy.effectDamageHold == 0)
                    enemy.effectDamageHold = 2;
                return 1.5f;
            case EnergyLevel.III:
                if (enemy.effectDamageHold == 0)
                    enemy.effectDamageHold = 3;
                return 2;
        }
        return 0;
    }
}
