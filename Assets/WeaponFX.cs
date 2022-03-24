using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFX : MonoBehaviour
{
    public ParticleSystem flourishParticles;
    public ParticleSystem[] muzzleParticles;
    public AudioSource flourishLoopSource;
    public AudioSource weaponSoundSource;

    public AudioClip flourishLoopClip, fireClip, reloadClip, equipClip, hitClip;
    public AudioClip[] flourishClips;

    public void PlayMuzzleFlash(int index = 0)
    {
        muzzleParticles[index].Play();
    }

    public void PlayHit()
    {
        weaponSoundSource.PlayOneShot(hitClip);
    }

    void Start()
    {
        weaponSoundSource.clip = equipClip;
        weaponSoundSource.Play();
    }

    public void SetFlourishActive(bool active)
    {
        if (flourishParticles == null) return;

        if(active)
        {
            flourishParticles.Play();
            flourishLoopSource.clip = flourishLoopClip;
            flourishLoopSource.Play();
        }
        else
        {
            flourishParticles.Stop();
            flourishLoopSource.Stop();
        }
    }

    public void PlayFire()
    {
        weaponSoundSource.clip = fireClip;
        weaponSoundSource.Play();
    }

    public void PlayReload()
    {
        weaponSoundSource.clip = reloadClip;
        weaponSoundSource.Play();
    }

    public void PlayFlourish(int index)
    {
        weaponSoundSource.clip = flourishClips[index];
        weaponSoundSource.Play();
    }
}
