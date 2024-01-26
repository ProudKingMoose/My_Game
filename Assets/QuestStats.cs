using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestStats : MonoBehaviour
{
    public Quest quest;
    public GameObject QuestManager;
    public GameObject MoreInfoPanel;

    public void SelectQuest()
    {
        foreach (Transform mission in QuestManager.transform)
        {
            if (mission.name == quest.info.name)
            {
                mission.GetComponent<QuestPoint>().selected = true;
                Text Title = MoreInfoPanel.transform.Find("MissionName").GetComponent<Text>();
                Title.text = quest.info.name;
            }
        }
    }
}
