using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSignScript : MonoBehaviour, IInteractable
{
    public GameObject QuestPanel;

    private void Start()
    {
        QuestPanel.SetActive(false);
    }

    private void Awake()
    {
        foreach (Transform quest in transform.Find("QuestManager"))
        {

            quest.GetComponent<QuestPoint>().selected = true;
        }
    }

    public void Interact()
    {
        if (!PauseMenu.IsPaused)
        {
            QuestPanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !PauseMenu.IsPaused)
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
