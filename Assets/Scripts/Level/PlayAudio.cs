using System;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public static PlayAudio instance;

    private AudioSource As;

    public AudioClip jump;
    public AudioClip walk1;
    public AudioClip walk2;

    private void Awake()
    {
        instance = this;
        As = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        As.PlayOneShot(jump);
    }
    public void PlayWalk()
    {
        if(UnityEngine.Random.Range(0,2)==1)
        {
            As.PlayOneShot(walk1);
        }
        else
        {
            As.PlayOneShot(walk2);
        }
    }
}
