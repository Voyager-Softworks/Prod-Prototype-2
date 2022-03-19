using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFX : MonoBehaviour
{
    public ParticleSystem muzzleFlash, flourishParticles;

    public void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }

    public void SetFlourishActive(bool active)
    {
        if(active)
        {
            flourishParticles.Play();
        }
        else
        {
            flourishParticles.Stop();
        }
    }
}
