using System;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public static PlayAudio instance;

    private AudioSource As;

    public AudioClip jump;
    public AudioClip walk1;
    public AudioClip walk2;
    public AudioClip pickup;
    public AudioClip stone;
    public AudioClip lighting;
    public AudioClip openBox;
    public AudioClip fire;
    public AudioClip death;
    public AudioClip puzzleTrigger;

    private void Awake()
    {
        instance = this;
        As = GetComponent<AudioSource>();
    }

    public void PlayJump()
    {
        As.PlayOneShot(jump, 0.7f);
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
    public void PlayPickup()
    {
        As.PlayOneShot(pickup);
    }
    public void PlayStone()
    {
        As.PlayOneShot(stone);
    }
    public void PlayLighting()
    {
        As.PlayOneShot(lighting);
    }
    public void PlayOpenBox()
    {
        As.PlayOneShot(openBox);
    }
    public void PlayFire()
    {
        As.PlayOneShot(fire);
    }
    public void PlayDeath()
    {
        As.PlayOneShot(death);
    }
    public void PlayPuzzleTrigger()
    {
        As.PlayOneShot(puzzleTrigger);
    }
}
