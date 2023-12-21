using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static BaseClass;

public class EnemyStateMachine : MonoBehaviour
{
    private CombatStateMachine CSM;
    public BaseEnemy enemy;
    private TypeEffectivness calculate;

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

    public GameObject DamageNummerPanel;
    public Transform BattleUI;
    private List<GameObject> DMGText = new List<GameObject>();

    private float DMGTDuration = 1f;
    private float CDMGTDuration = 0;

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
        BattleUI = GameObject.Find("Battle UI").GetComponent<Transform>();
        CSM = GameObject.Find("CombatManager").GetComponent<CombatStateMachine>();
        calculate = GameObject.Find("CombatManager").GetComponent<TypeEffectivness>();
        startPosition = transform.position;
        alive = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (CDMGTDuration > 0)
            CDMGTDuration -= Time.deltaTime;
        if (CDMGTDuration <= 0)
            RemoveDMGText();



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

                    CSM.DeadEnemies.Add(this.gameObject);

                    Selector.SetActive(false);

                    CSM.ActionPanel.SetActive(false);

                    for (int i = 0; i < CSM.HandlerList.Count; i++)
                    {
                        if (i != 0)
                        {
                            if (CSM.HandlerList[i].AttackersGameObject == this.gameObject)
                                CSM.HandlerList.Remove(CSM.HandlerList[i]);
                            if (CSM.HandlerList[i].AttackTarget == this.gameObject)
                                CSM.HandlerList[i].AttackTarget = CSM.Enemies[UnityEngine.Random.Range(0, CSM.Enemies.Count)];
                        }
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

    void DamageNamePanel(float DMG, float Vantage, float FVantage)
    {
        GameObject DNamePanel = Instantiate(DamageNummerPanel) as GameObject;
        Text DamageNamePanelText = DNamePanel.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        DamageNamePanelText.text = DMG.ToString();
        if (Vantage > 1.0)
            DamageNamePanelText.color = Color.red;
        else if (Vantage < 1.0)
            DamageNamePanelText.color= Color.blue;
        if (FVantage > 1.0)
            DamageNamePanelText.color = Color.red;
        else if (FVantage < 1.0)
            DamageNamePanelText.color = Color.blue;

        DNamePanel.transform.SetParent(BattleUI, false);

        float offsetPosY = this.gameObject.transform.position.y + 1.5f;
        Vector3 offsetPos = new Vector3(this.gameObject.transform.position.x, offsetPosY, this.gameObject.transform.position.z);

        Vector2 canvasPos;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)BattleUI, screenPoint, null, out canvasPos);

        DNamePanel.transform.localPosition = canvasPos;

        CDMGTDuration = DMGTDuration;

        DMGText.Add(DNamePanel);

    }

    void RemoveDMGText()
    {
        foreach (GameObject name in DMGText)
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

    public void TakeDamage(float damageAmount, EnergyType1 fusionEffect, EnergyType1 AbilityType, EnergyLevel AbiilityLV, EnergyLevel FusionLV)
    {
        float typeVantage = calculate.GetEffectivness(AbilityType, enemy.Type1, AbiilityLV) * calculate.GetEffectivness(AbilityType, enemy.Type2, AbiilityLV);

        float FusionVantage = calculate.GetEffectivness(fusionEffect, enemy.Type1, FusionLV) * calculate.GetEffectivness(fusionEffect, enemy.Type2, FusionLV);

        float totDMG = damageAmount * typeVantage * FusionVantage;
        enemy.currentHP -= totDMG;

        DamageNamePanel(totDMG, typeVantage, FusionVantage);
        if (fusionEffect != EnergyType1.None)
        {
            enemy.EnergyEffectedType = fusionEffect;
            enemy.EnergyLVEffected = FusionLV;
            enemy.beenEffected = true;
            Debug.Log(enemy.beenEffected + "effected by" + enemy.EnergyEffectedType + " " + enemy.EnergyLVEffected);
        }
        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            currentstate = States.DEAD;
        }
    }

    public void TakeEffectDamage(float heroEPower)
    {
        float DMGMult = EffectLVCalculation();

        float totDMG = (heroEPower - enemy.currentEDefence) * DMGMult;
        if (totDMG < 0)
            totDMG = 0;
        DamageNamePanel(totDMG, 1.0f, 1.0f);
        enemy.currentHP -= totDMG;
        enemy.effectDamageHold--;

        Debug.Log(totDMG + " Damage was done to " + enemy.theName);
        if (enemy.effectDamageHold == 0)
            enemy.beenEffected = false;

        if (enemy.currentHP < 0)
        {
            enemy.currentHP = 0;
            currentstate = States.DEAD;
        }
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
