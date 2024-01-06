using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BaseClass;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        FUSING,
        BUFFING,
        CHECKTURN,
        DEAD,
    }

    public States currentstate;

    private float coldownLimit;
    private float currentColdown;
    private bool cameraMovement;
    public GameObject Selector;

    private float animationsSpeed = 10;
    private Vector3 startPosition;
    public GameObject EnemyTargeted;

    private bool alive = true;
    private bool actionStarted = false;
    private bool effectStart = false;

    private HeroPanelStats Stats;
    private Transform HeroPanelSpacer;
    public GameObject HeroPanel;

    public Transform AttackNameSpacer;
    public GameObject AttackNamePanel;
    private List<GameObject> attackNames = new List<GameObject>();
    private string attackName;
    private string itemName;

    public GameObject DamageNummerPanel;
    public Transform BattleUI;
    private List<GameObject> DMGText = new List<GameObject>();
    private float DMGTDuration = 1f;
    private float CDMGTDuration = 0;


    void Start()
    {
        hero.Level = new LevelSystem(OnLevelUp);

        StatCorrector();
        if (hero.Level.currentLV == 0)
            hero.Level.currentLV = 1;

        coldownLimit = 1;
        currentColdown = 0;

        hero.animator = GetComponent<Animator>();

        AttackNameSpacer = GameObject.Find("AttackNameSpacer").GetComponent<Transform>();
        BattleUI = GameObject.Find("Battle UI").GetComponent<Transform>();

        HeroPanelSpacer = GameObject.Find("Battle UI").transform.Find("HeroPanel").transform.Find("HeroPanelSpacer");
        CreateHeroPanel();

        Selector.SetActive(false);
        CSM = GameObject.Find("CombatManager").GetComponent<CombatStateMachine>();
        currentstate = States.CHECKTURN;
        startPosition = transform.position;
        cameraMovement = true;
    }

    void StatCorrector()
    {
        HeroStatStorage storedData = GameManager.instance.GetPlayerStats(hero.theName);
        hero.Level.currentLV = storedData.level;
        hero.Level.currentXP = storedData.XP;
        hero.Level.XPToNextLevel = hero.Level.GetXPForLevel(hero.Level.currentLV);

        hero.currentHP = storedData.currentHP;
        hero.baseHP = storedData.baseHP;
        hero.currentEnergy = storedData.currentEnergy;
        hero.baseEnergy = storedData.baseEnergy;

        hero.baseDefence = storedData.baseDefence;
        hero.baseAttackPower = storedData.baseAttackPower;
        hero.baseEDefence = storedData.baseEDefence;
        hero.baseEAttackPower = storedData.baseEAttackPower;
        hero.currentDefence = hero.baseDefence;
        hero.currentAttackPower = hero.baseAttackPower;
        hero.currentEDefence = hero.baseEDefence;
        hero.currentEAttackPower = hero.baseEAttackPower;

        hero.Type1Level = storedData.Type1Level;
        hero.Type2Level = storedData.Type2Level;

        hero.EnergyAttacks = storedData.EnergyAttacks;
        Debug.Log("This code is runned one time");
    }

    public void OnLevelUp()
    {
        Debug.Log("I Level Up");
    }

    // Update is called once per frame
    void Update()
    {
        if (CDMGTDuration > 0)
            CDMGTDuration -= Time.deltaTime;
        if (CDMGTDuration <= 0)
            RemoveDMGText();
        cameraMovement = CSM.cameraMove;
        if (!cameraMovement)
        {
            CameraSystem.instance.ReturnCamera();
        }

        switch (currentstate)
        {
            case (States.CHECKTURN):
                if (CSM.turn == CombatStateMachine.Turn.HEROTURN)
                    currentstate = States.PROCESSING;
                break;

            case (States.PROCESSING):
                ProcessingProgress();
            break;
            case (States.ADDTOLIST):
                CSM.HerosReadyToAttack.Add (this.gameObject);
                currentstate = States.WAITING;
            break;

            case (States.WAITING):
                if (CSM.HerosReadyToAttack.Count == 0)
                {
                    if (effectStart)
                        EffectDamageActivate();
                    CSM.turn = CombatStateMachine.Turn.ENEMYTURN;
                    currentstate = States.CHECKTURN;
                }
            break;

            case (States.ATTACKING):
                StartCoroutine(HeroMeleeAttack());
            break;

            case (States.FUSING):
                StartCoroutine(FusionStance());
                break;
            case (States.BUFFING):
                StartCoroutine(HeroBuffHero());
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

                    CSM.DeadHeroes.Add(this.gameObject);

                    Selector.SetActive(false);

                    CSM.ActionPanel.SetActive(false);

                    for (int i = 0; i < CSM.HandlerList.Count; i++)
                    {
                        if (i != 0)
                        {
                            if (CSM.HandlerList[i].AttackersGameObject == this.gameObject)
                                CSM.HandlerList.Remove(CSM.HandlerList[i]);
                            if (CSM.HandlerList[i].AttackTarget == this.gameObject)
                                CSM.HandlerList[i].AttackTarget = CSM.Heroes[UnityEngine.Random.Range(0, CSM.Heroes.Count)];
                        }
                    }

                    //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(100, 100, 100, 255);

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

        ANamePanel('A');
        Debug.Log(CSM.HandlerList[0].AttackersGameObject);
        if (cameraMovement)
            CameraSystem.instance.FirstCameraFix(CSM.HandlerList[0].AttackersGameObject, 'H');

        yield return new WaitForSeconds(1.5f);

        if (cameraMovement)
            CameraSystem.instance.FollowAttackerStep(CSM.HandlerList[0].AttackersGameObject);
        hero.animator.SetBool("Running", true);
        Vector3 enemyPos = new Vector3(EnemyTargeted.transform.position.x + 1.5f, EnemyTargeted.transform.position.y, EnemyTargeted.transform.position.z);
        while (MoveToEnemy(enemyPos)) { yield return null; }

        hero.animator.SetBool("Attack", true);
        yield return new WaitForSeconds(3.5f);
        //GetCurrentAnimatorStateInfo(layer).length

        hero.animator.SetBool("Attack", false);

        if (cameraMovement)
            CameraSystem.instance.CameraOnTargetTacker(CSM.HandlerList[0].AttackTarget, CSM.HandlerList[0].BuffTarget);

        DoDamage();

        RemoveAttackText();

        Vector3 startPos = startPosition;
        while (MoveToStartPos(startPos)) { yield return null; }
        hero.animator.SetBool("Running", false);

        CameraSystem.instance.ReturnCamera();

        CSM.HandlerList.RemoveAt(0);

        if (CSM.HerosReadyToAttack.Count == 0)
            effectStart = true;

        if(CSM.battleState != CombatStateMachine.Action.WIN && CSM.battleState != CombatStateMachine.Action.LOSE)
        {
            CSM.battleState = CombatStateMachine.Action.WAIT;

            currentColdown = 0;
            currentstate = States.WAITING;
        }

        if (hero.FusionUses == 0)
        {
            hero.currentFusionType = EnergyType1.None;
            //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 200, 255);
        }
        actionStarted = false;
    }

    private IEnumerator FusionStance()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        CSM.FusionPanel.SetActive(false);

        FusionEnhancement();

        if (cameraMovement)
            CameraSystem.instance.FirstCameraFix(CSM.HandlerList[0].AttackersGameObject, 'H');

        yield return new WaitForSeconds(2.5f);

        CSM.ActionPanel.SetActive(true);

        CameraSystem.instance.ReturnCamera();

        currentstate = States.WAITING;

        actionStarted = false;
    }

    private IEnumerator HeroBuffHero()
    {
        if (actionStarted)
        {
            yield break;
        }

        actionStarted = true;

        itemName = CSM.HandlerList[0].choosenItem.name;

        if (cameraMovement)
            CameraSystem.instance.FirstCameraFix(CSM.HandlerList[0].AttackersGameObject, 'H');

        ANamePanel('B');

        yield return new WaitForSeconds(1.5f);

        if (cameraMovement)
            CameraSystem.instance.CameraOnTargetTacker(CSM.HandlerList[0].AttackTarget, CSM.HandlerList[0].BuffTarget);

        ItemUsage();

        yield return new WaitForSeconds(1.5f);

        CameraSystem.instance.ReturnCamera();

        RemoveAttackText();
        this.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
        Debug.Log("This code is runned or else im angry");
        //CSM.HandlerList[0].BuffTarget.GetComponent<HeroStatemachine>().GetComponent<MeshRenderer>().material.color = Color.blue;

        CSM.HandlerList.RemoveAt(0);

        if (CSM.HerosReadyToAttack.Count == 0)
            effectStart = true;

        if (CSM.battleState != CombatStateMachine.Action.WIN && CSM.battleState != CombatStateMachine.Action.LOSE)
        {
            CSM.battleState = CombatStateMachine.Action.WAIT;

            currentColdown = 0;
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

    void ANamePanel(char type)
    {
        GameObject ANamePanel = Instantiate(AttackNamePanel) as GameObject;
        Text AttackNamePanelText = ANamePanel.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        if (hero.currentFusionType != EnergyType1.None )
        {
            FindEnergyCurrentLV();
            AttackNamePanelText.text = hero.currentFusionType + " " + hero.usedFusionLevel + " " + attackName;
        }
        else if (type == 'A')
            AttackNamePanelText.text = attackName;
        else if (type == 'B')
            AttackNamePanelText.text = itemName;
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

    void DamageNamePanel(float DMG)
    {
        GameObject DNamePanel = Instantiate(DamageNummerPanel) as GameObject;
        Text DamageNamePanelText = DNamePanel.transform.Find("Text (Legacy)").gameObject.GetComponent<Text>();
        DamageNamePanelText.text = DMG.ToString();
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


    public void TakeDamage(float damageAmount, EnergyType1 effect, EnergyLevel level)
    {
        float totDMG = damageAmount;
        DamageNamePanel(totDMG);

        hero.currentHP -= totDMG;
        if (hero.currentHP <= 0)
        {
            hero.currentHP = 0;
            currentstate = States.DEAD;
        }
        if (effect != EnergyType1.None)
        {
            hero.EnergyEffectedType = effect;
            hero.EnergyLVEffected = level;
        }
        HeroPanelUpdate();
    }

    public void TakeEffectDamage()
    {

    }

    void FindEnergyCurrentLV()
    {
        if (hero.currentFusionType == hero.Type1)
            hero.usedFusionLevel = hero.Type1Level;
        else if (hero.currentFusionType == hero.Type2)
            hero.usedFusionLevel = hero.Type2Level;
    }
    void DoDamage()
    {
        float calculatedDamage = hero.currentAttackPower + CSM.HandlerList[0].choosenAttack.attackDamage;

        if (hero.FusionUses > 0)
        {
            if(hero.FusionPower == 1)
                calculatedDamage *= 1.2f;
            if (hero.FusionPower == 2)
                calculatedDamage *= 1.4f;
            if (hero.FusionPower == 3)
                calculatedDamage *= 1.7f;
            hero.FusionUses -= 1;
        }

        Debug.Log(calculatedDamage);

        FindEnergyCurrentLV();
        EnemyTargeted.GetComponent<EnemyStateMachine>().TakeDamage(calculatedDamage, hero.currentFusionType, CSM.HandlerList[0].Abilitytype, CSM.HandlerList[0].AbilityLV, hero.usedFusionLevel);
        hero.currentEnergy -= (CSM.HandlerList[0].choosenAttack.energyCost);
        if (hero.currentEnergy <= 0)
            hero.currentEnergy = 0;
        HeroPanelUpdate();
    }

    void ItemUsage()
    {
        GameManager.instance.inventory.RemoveItems(CSM.HandlerList[0].choosenItem, 1);

        CSM.HandlerList[0].BuffTarget.GetComponent<HeroStatemachine>().ItemEffects(CSM.HandlerList[0].choosenItem);
    }

    void ItemEffects(RestoreObject item)
    {
        if (item.energyRestore > 0)
            hero.currentEnergy += item.energyRestore;
        if (item.healthRestore > 0)
            hero.currentHP += item.healthRestore;
        if (hero.currentHP > hero.baseHP)
            hero.currentHP = hero.baseHP;
        if (hero.currentEnergy > hero.baseEnergy)
            hero.currentEnergy = hero.baseEnergy;

        //this.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

        HeroPanelUpdate();
    }

    void FusionEnhancement()
    {
        FusionTypeLevelCheck();
    }

    void FusionTypeLevelCheck()
    {
        if (hero.currentFusionType == hero.Type1)
        {
            if (hero.Type1Level == EnergyLevel.I)
            {
                hero.FusionUses = 1;
                hero.FusionPower = 1;
                hero.currentEnergy -= 5;
            }
            else if (hero.Type1Level == EnergyLevel.II)
            {
                hero.FusionUses = 2;
                hero.FusionPower = 2;
                hero.currentEnergy -= 10;
            }
            else if (hero.Type1Level == EnergyLevel.III)
            {
                hero.FusionUses = 3;
                hero.FusionPower = 3;
                hero.currentEnergy -= 15;
            }
        }
        else if (hero.currentFusionType == hero.Type2)
        {
            if (hero.Type2Level == EnergyLevel.I)
            {
                hero.FusionUses = 1;
                hero.FusionPower = 1;
                hero.currentEnergy -= 5;
            }
            else if (hero.Type2Level == EnergyLevel.II)
            {
                hero.FusionUses = 2;
                hero.FusionPower = 2;
                hero.currentEnergy -= 10;
            }
            else if (hero.Type2Level == EnergyLevel.III)
            {
                hero.FusionUses = 3;
                hero.FusionPower = 3;
                hero.currentEnergy -= 15;
            }
        }
        switch (hero.currentFusionType)
        {
            case (EnergyType1.Heat):
                //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(200, 0, 0, 255);
                break;

            case (EnergyType1.Chill):
                //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 200, 255);
                break;

            case (EnergyType1.Zapp):
                //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 200, 200, 255);
                break;

            case (EnergyType1.Light):
                //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 255);
                break;

            case (EnergyType1.Darkness):
                //this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 0, 0, 255);
                break;
        }
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

    void EffectDamageActivate()
    {
        effectStart = false;
        foreach (GameObject enemy in CSM.Enemies)
        {
            if (enemy.GetComponent<EnemyStateMachine>().enemy.beenEffected)
                enemy.GetComponent<EnemyStateMachine>().TakeEffectDamage(hero.currentEAttackPower);
        }
    }

    void EffectLVCalculation(float damageMult)
    {

        switch (hero.EnergyLVEffected)
        {
            case EnergyLevel.I:
                if (hero.effectDamageHold == 0)
                    hero.effectDamageHold = 1;
                damageMult = 1;
                break;
            case EnergyLevel.II:
                if (hero.effectDamageHold == 0)
                    hero.effectDamageHold = 2;
                damageMult = 1.5f;
                break;
            case EnergyLevel.III:
                if (hero.effectDamageHold == 0)
                    hero.effectDamageHold = 3;
                damageMult = 2;
                break;
        }
    }

    void LevelStatsAlgorithm()
    {

    }
}
