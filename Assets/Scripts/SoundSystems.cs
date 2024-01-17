using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSystems : MonoBehaviour
{
    [SerializeField]
    private AudioClip hitSound;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
    void HitSound()
    {
        source.Play();
    }
}
