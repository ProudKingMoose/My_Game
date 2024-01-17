using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuButton : MonoBehaviour
{
    [SerializeField] StartMenuButtonController startMenuButtonController;
    [SerializeField] Animator animator;
    [SerializeField] AnimatorFunctions animatorfunctions;
    [SerializeField] int thisIndex;

    void Update()
    {
        if (startMenuButtonController.index == thisIndex)
        {
            animator.SetBool("selected", true);
            if (Input.GetAxis("Submit") == 1)
                animator.SetBool("pressed", true);
            else if (animator.GetBool("pressed"))
            {
                animator.SetBool("pressed", false);
                animatorfunctions.disableOnce = true;
            }
        }
        else
            animator.SetBool("selected", false);
    }

}
