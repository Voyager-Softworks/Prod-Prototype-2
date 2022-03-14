using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class Equipment : NetworkBehaviour
{
    [SyncVar]
    public WeaponData currentWeapon;
    public Animator firstPersonAnimator;
    public Animator thirdPersonAnimator;

    public GameObject firstPerson;
    public GameObject thirdPerson;
    public  GameObject currentWeaponObject;
    public Transform firstPersonWeaponAnchor;
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
    
    public void SetTrigger(string[] args)
    {
        string paramStr = currentWeapon.weaponName.ToUpper();
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
        if (currentWeapon != null)
        {
            DropWeapon();
        }

        

        currentWeapon = weapon;

        SetTrigger(new string[] { "p" });
    }

    

    public void DropWeapon()
    {
        if (currentWeapon != null)
        {
            if (isLocalPlayer)
            {
                if (currentWeapon.weaponDropPrefab != null)
                {
                    GameObject weaponDrop = Instantiate(currentWeapon.weaponDropPrefab, firstPersonWeaponAnchor.position, firstPersonWeaponAnchor.rotation);
                    weaponDrop.GetComponent<Rigidbody>().velocity = transform.forward * 1;
                }
            }
            SetTrigger(new string[] { "d" });
            currentWeapon = null;
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
        }
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            if(currentWeapon != null && currentWeaponObject == null)
            {

            }
        }
    }
}
