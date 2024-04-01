using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;

public class QuestSignScript : MonoBehaviour, IInteractable
{
    public GameObject QuestPanel;
    public GameObject QuestNote;
    public Transform MissionSpacer;

    private void Start()
    {
        QuestPanel.SetActive(false);
    }

    private void Awake()
    {
    }

    public void Interact()
    {
        if (!PauseMenu.IsPaused)
        {
            QuestPanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (QuestManager.QuestsThatCanFinish != null)
            {
                foreach (Quest s in QuestManager.QuestsThatCanFinish)
                {
                    if (s.state == QuestState.CAN_FINISH)
                        CreateQuestNote(s);
                }
            }
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

        note.transform.SetParent(MissionSpacer, false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !PauseMenu.IsPaused)
            ExitMenu();
    }

    private void ExitMenu()
    {
        QuestPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
