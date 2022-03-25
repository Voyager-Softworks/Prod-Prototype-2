using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkGunAudio : NetworkBehaviour
{

    public AudioSource audioSource;
    public AudioClip shootShotgun, shootRifle, swingBottle;

    
    [Command(requiresAuthority = false)]
    public void CmdPlayShoot(string weaponname)
    {
        RpcPlayShoot(weaponname);
    }

    [ClientRpc]
    public void RpcPlayShoot(string weaponname)
    {
        if(isLocalPlayer) return;
        switch (weaponname.ToUpper())
        {
            case "HANDSHOTGUNS":
                audioSource.PlayOneShot(shootShotgun);
                break;
            case "ASSAULTRIFLE":
                audioSource.PlayOneShot(shootRifle);
                break;
            case "BOTTLE":
                audioSource.PlayOneShot(swingBottle);
                break;
        }
    }
    
    

}
