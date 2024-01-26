using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public QuestInfoSO info;

    public QuestState state;

    private int currentQuestStepIndex;

    public Quest(QuestInfoSO questInfo)
    {
        this.info = questInfo;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.QuestStepPrerfabs.Length);
    }

    public void InstantiatecurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();
            questStep.InitializeQuestStep(info.Id);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject currentQuestStepPrefab = null;
        if (CurrentStepExists())
        {
            currentQuestStepPrefab = info.QuestStepPrerfabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("Index Out of range");
        }
        return currentQuestStepPrefab;
    }
}
