using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuButton : MonoBehaviour
{
    public enum buttonType
    {
        STARTGAME,
        OPTIONS,
        QUIT
    }
    public buttonType type;
    [SerializeField] StartMenuButtonController startMenuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] int thisIndex;
    public AudioClip hover;
    public AudioClip pressed;
    private bool audioPlayed = false;


    void Update()
    {
        Hover();
    }

    private void Hover()
    {
        if (startMenuButtonController.index == thisIndex)
        {
            if (!audioPlayed)
            {
                startMenuButtonController.audioSource.PlayOneShot(hover);
                audioPlayed = true;
            }

            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
            {
                Click();
            }
        }
        else
        {
            animator.SetBool("selected", false);
            audioPlayed = false;
        }

    }

    public void MouseHover()
    {
        startMenuButtonController.index = thisIndex;
    }

    public void MouseLeave()
    {
        startMenuButtonController.index = default;
    }

    public void Click()
    {
        startMenuButtonController.audioSource.PlayOneShot(pressed);
        animator.SetBool("pressed", true);
        TypeCheck();
    }

    private void TypeCheck()
    {
        switch (type)
        {
            case buttonType.STARTGAME:
                StartGame();
                break;

            case buttonType.OPTIONS://Complete this if you have time 
                
                break;

            case buttonType.QUIT:
                UnityEditor.EditorApplication.isPlaying = false;
                Application.Quit();
                break;
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("AdventureScene");
    }
}
