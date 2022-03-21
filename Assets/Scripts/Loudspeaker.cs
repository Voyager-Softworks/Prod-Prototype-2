using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loudspeaker : MonoBehaviour
{
    AudioSource audioSource;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        FindObjectOfType<LoudspeakerController>().RegisterLoudspeaker(this);
    }

    public void Play(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    void Update()
    {
        if(animator)
        {
            animator.SetBool("isPlaying", audioSource.isPlaying);
        }
    }


}
