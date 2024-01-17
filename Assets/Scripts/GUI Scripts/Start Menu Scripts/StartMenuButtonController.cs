using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuButtonController : MonoBehaviour
{
    public int index;
    [SerializeField] bool keyDown;
    [SerializeField] int maxIndex;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            if (!keyDown)
            {
                if (Input.GetAxis("Vertical") < 0)
                {
                    if (index < maxIndex)
                        index++;
                    else
                        index = 1;
                }
                if (Input.GetAxis("Vertical") > 0)
                {
                    if (index > 1)
                        index--;
                    else
                        index = maxIndex;
                }
            }
            keyDown = true;
        }
        else
            keyDown = false;
    }
}
