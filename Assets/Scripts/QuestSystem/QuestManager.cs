using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private int currentPlayerLevel;

    private static QuestManager ThisGameObject;

    public GameObject QuestNote;
    public Transform MissionSpacer;

    private void Awake()
    {
        questMap = CreateQuestMap();
        if (ThisGameObject == null)
            ThisGameObject = this;
        else if (ThisGameObject != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        GameManager.instance.questEvents.onStartQuest += StartQuest;
        GameManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameManager.instance.questEvents.onFinishQuest += FinishQuest;

        GameManager.instance.LevelChange += PlayerLevelChange;
    }

    private void OnDisable()
    {
        GameManager.instance.questEvents.onStartQuest -= StartQuest;
        GameManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameManager.instance.questEvents.onFinishQuest -= FinishQuest;

        GameManager.instance.LevelChange -= PlayerLevelChange;
    }

    private void Start()
    {
        foreach (Quest quest in questMap.Values)
        {
            GameManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameManager.instance.questEvents.QuestStateChange(quest);
        Debug.Log("SuccefrullyChanged");
    }

    private void PlayerLevelChange(int level)
    {
        currentPlayerLevel = level;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        if (currentPlayerLevel < quest.info.levelRequirement)
            meetsRequirements = false;

        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.Id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }
        Debug.Log(meetsRequirements);
        return meetsRequirements;
    }

    private void Update()
    {
        foreach (Quest quest in questMap.Values)
        {
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.Id, QuestState.CAN_START);
                CreateQuestNote(quest);
            }
        }
    }

    private void StartQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.InstantiatecurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.Id, QuestState.IN_PROGRESS);
        //change quest state to qesut under progress
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();

        if (quest.CurrentStepExists())
        {
            quest.InstantiatecurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.Id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {

        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.Id, QuestState.FINISHED);

    }

    private void ClaimRewards(Quest quest)
    {
        GameManager.instance.Coins += quest.info.coins;
        GameManager.instance.inventory.AddItems(quest.info.Items[0], 1);
        foreach (HeroStatStorage hero in GameManager.instance.StatStorage)
        {
            LevelSystem levelsystem;
            //Take the LevelSystem from each hero also
        }
    }

    private void CreateQuestNote(Quest quest)
    {
        GameObject note = Instantiate(QuestNote) as GameObject;
        Text Title = note.transform.Find("QuestName").GetComponent<Text>();
        Text Coins = note.transform.Find("Coins").GetComponent<Text>();
        Text XP = note.transform.Find("XP").GetComponent<Text>();
        Text Item = note.transform.Find("Item").GetComponent<Text>();
        Title.text = quest.info.title;
        Coins.text = quest.info.coins + " Coins";
        XP.text = quest.info.Xp + "Xp";
        Item.text = quest.info.Items[0].ToString();
        QuestStats stats = note.GetComponent<QuestStats>();
        stats.quest = quest;
        stats.QuestManager = this.gameObject;

        note.transform.SetParent(MissionSpacer, false);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.Id))
            {
                Debug.LogWarning("Found Quests with duplicate ID");
            }
            idToQuestMap.Add(questInfo.Id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogWarning("Id not found in quest map: " + id);
        }
        return quest;
    }
}
