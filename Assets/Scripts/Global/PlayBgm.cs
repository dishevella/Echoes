using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip bgmClip;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (audioSource == null || bgmClip == null) return;

        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}