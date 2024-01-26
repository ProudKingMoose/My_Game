using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForNote;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    public bool selected = false;
    public bool started = false;
    private string questId;
    private QuestState currentQuestState;

    private void Awake()
    {
        questId = questInfoForNote.Id;
    }

    private void OnEnable()
    {
        GameManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameManager.instance.StartQuest += SubmitPressed;
    }

    private void OnDisable()
    {
        GameManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameManager.instance.StartQuest -= SubmitPressed;
    }

    private void SubmitPressed()
    {
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint && selected)
        {
            GameManager.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.Id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("Quest with id: " + questId + "updated to state: " + currentQuestState);
        }
    }
}
