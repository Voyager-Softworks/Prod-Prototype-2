using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource, ambientSource;
    public AudioClip[] musicClips;
    public AudioClip ambientClip;


    // Start is called before the first frame update
    void Start()
    {
        ambientSource.clip = ambientClip;
        ambientSource.Play();
        musicSource.clip = musicClips[Random.Range(0, musicClips.Length)];
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(!musicSource.isPlaying)
        {
            int randpick = Random.Range(0, musicClips.Length);
            while (musicClips[randpick] == musicSource.clip)
            {
                randpick = Random.Range(0, musicClips.Length);
            }
            musicSource.clip = musicClips[randpick];
            musicSource.Play();
        }
    }
}
