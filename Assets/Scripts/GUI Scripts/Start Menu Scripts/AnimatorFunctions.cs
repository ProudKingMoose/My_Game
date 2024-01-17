using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] StartMenuButtonController startMenuButtonController;
    public bool disableOnce;

    void PlaySound(AudioClip clip)
    {
        if (!disableOnce)
        {
            startMenuButtonController.audioSource.PlayOneShot(clip);
        }
        else
            disableOnce = false;
    }
}
