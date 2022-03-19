using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class Equipment : NetworkBehaviour
{
    [SyncVar (hook = "OnChangeWeapon")]
    public WeaponData currentWeapon;
    public Animator firstPersonAnimator;
    public Animator thirdPersonAnimator;

    public GameObject firstPerson;
    public GameObject thirdPerson;
    public  GameObject currentWeaponObject;
    public Transform firstPersonWeaponAnchor;
    public FlourishHandler flourishHandler;

    //Weapon Vars

    int currentAmmo;

    public bool TryFire()
    {
        if(currentAmmo > 0)
        {
            currentAmmo--;
            return true;
        }
        return false;
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
    }

    public void ReloadAmmo()
    {
        currentAmmo = currentWeapon.clipSize;
    }
    private bool ArgsContain(string[] args, string arg)
    {
        foreach (string s in args)
        {
            if (s == arg)
            {
                return true;
            }
        }
        return false;
    }

    void Start()
    {
        if(isLocalPlayer)
        {
            firstPerson.SetActive(true);
            thirdPerson.SetActive(false);
        }
        else
        {
            firstPerson.SetActive(false);
            thirdPerson.SetActive(true);
        }
    }

    void OnChangeWeapon(WeaponData oldValue, WeaponData newValue)
    {
        if(currentWeapon != null)
        {
            SetTrigger(new string[] {"p"}, currentWeapon);
        }
        else
        {
            SetTrigger(new string[] {"d"}, oldValue);
        }
    }
    
    [Command]
    public void SetTrigger(string[] args, WeaponData dat)
    {
        string paramStr = dat.weaponName.ToUpper();
        if(ArgsContain(args, "r"))
        {
            paramStr += "_RELOAD";
            if(ArgsContain(args,"0"))
            {
                paramStr += "_0";
            }
            else if(ArgsContain(args,"1"))
            {
                paramStr += "_1";
            }
            else if(ArgsContain(args,"2"))
            {
                paramStr += "_2";
            }
            else if(ArgsContain(args,"3"))
            {
                paramStr += "_3";
            }
            else if(ArgsContain(args,"4"))
            {
                paramStr += "_4";
            }
            else if(ArgsContain(args,"5"))
            {
                paramStr += "_5";
            }
            else if(ArgsContain(args,"6"))
            {
                paramStr += "_6";
            }
            
            
        }
        else if(ArgsContain(args, "s"))
        {
            paramStr += "_SHOOT";
        }
        else if(ArgsContain(args, "d"))
        {
            paramStr += "_DROP";
        }
        else if(ArgsContain(args, "p"))
        {
            paramStr += "_PICKUP";
        }


        RpcSetTrigger(paramStr);
    }

    [ClientRpc]
    void RpcSetTrigger(string paramStr)
    {
        if(isLocalPlayer)
        {
            firstPersonAnimator.SetTrigger(paramStr);
        }
        else
        {
            thirdPersonAnimator.SetTrigger(paramStr);
        }
    }


    public void EquipWeapon(WeaponData weapon)
    {
        if (currentWeapon != null && isLocalPlayer)
        {
            DropWeapon();
        }

        currentAmmo = weapon.clipSize;
        currentWeaponObject = Instantiate(weapon.weaponPrefabFirstPerson, firstPersonWeaponAnchor);
        firstPersonAnimator = currentWeaponObject.GetComponentInChildren<Animator>();
        flourishHandler.reloadAnimator = currentWeaponObject.GetComponentInChildren<Animator>();
        flourishHandler.currentFlourish = weapon.flourish;
        currentWeapon = weapon;

        
    }

    

    
    [Command]
    public void DropWeapon()
    {
        if (currentWeapon != null)
        {
            
                if (currentWeapon.weaponDropPrefab != null)
                {
                    GameObject weaponDrop = Instantiate(currentWeapon.weaponDropPrefab, firstPersonWeaponAnchor.position, firstPersonWeaponAnchor.rotation);
                    weaponDrop.GetComponent<Rigidbody>().velocity = transform.forward * 1;
                    NetworkServer.Spawn(weaponDrop);
                }
            
            currentAmmo = 0;
            currentWeapon = null;
            Destroy(currentWeaponObject);
            flourishHandler.reloadAnimator = null;
            flourishHandler.currentFlourish = null;
            currentWeaponObject = null;
        }
    }

    void Update()
    {
        
    }
}
