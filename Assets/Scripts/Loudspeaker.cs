using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loudspeaker : MonoBehaviour
{
    AudioSource audioSource;

    Animator animator;

    bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponentInChildren<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        
        
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
        if(!initialized)
        {
            if(GameObject.FindGameObjectWithTag("Announcer"))
            {
                GameObject.FindGameObjectWithTag("Announcer").GetComponent<LoudspeakerController>().RegisterLoudspeaker(this);
                initialized = true;
            }
        }
        if(animator)
        {
            animator.SetBool("isPlaying", audioSource.isPlaying);
        }
    }


}
