using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestStats : MonoBehaviour
{
    public Quest quest;
    public GameObject QuestManager;

    public void SelectQuest()
    {
        foreach (Transform mission in QuestManager.transform)
        {
            Debug.Log(mission.name);
            Debug.Log(quest.info.name);
            if (mission.name == quest.info.name)
            {
                Debug.Log("This is clicked");
                mission.GetComponent<QuestPoint>().selected = true;
            }
            else
            {
                mission.GetComponent<QuestPoint>().selected = false;
            }
        }
    }
    public void EndQuest()
    {
        foreach (Transform mission in QuestManager.transform)
        {
            if (mission.name == quest.info.name && quest.state == QuestState.CAN_FINISH)
            {
                Debug.Log("This is clicked");
                mission.GetComponent<QuestPoint>().selected = true;
            }
        }
    }
}
