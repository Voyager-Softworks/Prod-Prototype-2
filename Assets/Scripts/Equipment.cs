using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Equipment : NetworkBehaviour
{
    public WeaponData currentWeapon;
    private GameObject tempSpawnPrefab = null;
    public Animator firstPersonAnimator;
    public Animator thirdPersonAnimator;
    public NetworkAnimator networkAnimator;

    public GameObject firstPerson;
    public GameObject thirdPerson;
    public  GameObject currentWeaponObject;
    public Transform firstPersonWeaponAnchor;
    public FlourishHandler flourishHandler;

    public TMP_Text ammoText;

    //Weapon Vars

    [SyncVar] int currentAmmo;

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

    public void DumpAmmo()
    {
        currentAmmo = 0;
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
            thirdPerson.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        else
        {
            firstPerson.SetActive(false);
        }
    }

    void OnChangeWeapon(WeaponData oldValue, WeaponData newValue)
    {
        if(currentWeapon != null)
        {
            SetTrigger(new string[] {"p"}, currentWeapon);
        }
        
    }
    [Command]
    public void SetSprinting(bool value)
    {
        SetSprintingRPC(value);
    }

    [ClientRpc]
    public void SetSprintingRPC(bool value)
    {
        if(isLocalPlayer && firstPersonAnimator)
        {
            if(currentWeapon != null)
            {
                firstPersonAnimator.SetBool("SPRINTING", value);
            }
        }
        else if (thirdPersonAnimator)
        {
            thirdPersonAnimator.SetBool("SPRINTING", value);
        }
    }

    public bool IsWaiting()
    {
        if(!isLocalPlayer) return false;

        if(!currentWeapon) return false;

        return firstPersonAnimator.GetCurrentAnimatorStateInfo(0).loop;
    }

    
    public void SetTrigger(string[] args, WeaponData dat)
    {
        if (dat == null) return;
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
        else if(ArgsContain(args, "d"))
        {
            paramStr += "_DROP";
        }
        else if(ArgsContain(args, "p"))
        {
            paramStr += "_PICKUP";
        }
        else if(ArgsContain(args, "c"))
        {
            paramStr += "_CANCEL";
        }
        else if(ArgsContain(args, "h"))
        {
            paramStr += "_HIT";
        }
        else if(ArgsContain(args, "m"))
        {
            paramStr += "_MISS";
        }
        


        CmdSetTrigger(paramStr);
        networkAnimator.SetTrigger(paramStr);
        
    }

    [Command]
    void CmdSetTrigger(string paramStr)
    {
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
            //thirdPersonAnimator.SetTrigger(paramStr);
            
        }
    }

    [Command]
    public void CmdTryEquipWeapon(GameObject _weapon)
    {
        Debug.Log("Equipping " + _weapon);

        RpcEquipWeapon(_weapon);
    }

    [ClientRpc]
    private void RpcEquipWeapon(GameObject _weapon)
    {
        EquipWeapon(_weapon);

        if (isServerOnly) EquipWeapon(_weapon);
    }

    private void EquipWeapon(GameObject _weapon)
    {
        if (_weapon == null) return;
        Interactible interactible = _weapon.GetComponent<Interactible>();
        if (interactible == null) return;
        WeaponData weaponData = interactible.weaponData;
        if (weaponData == null) return;

        if (currentWeapon != null)
        {
            if (isLocalPlayer){
                CmdDropWeapon();
                CmdTryEquipWeapon(_weapon);
            }
            
            return;
        }

        currentWeapon = weaponData;
        currentAmmo = weaponData.clipSize;

        if (isLocalPlayer) {
            currentWeaponObject = Instantiate(weaponData.weaponPrefabFirstPerson, firstPersonWeaponAnchor);
            firstPersonAnimator = currentWeaponObject.GetComponentInChildren<Animator>();
            flourishHandler.reloadAnimator = currentWeaponObject.GetComponentInChildren<Animator>();
            flourishHandler.currentFlourish = weaponData.flourish;
        }

        if(interactible.destroyOnInteract)
        {
            interactible.CmdDestroy();
        }

        SetTrigger(new string[] {"p"}, currentWeapon);
    }

    [Command(requiresAuthority = false)]
    private void CmdDoBoth(GameObject _weapon){
        RpcDropWeapon();
        RpcEquipWeapon(_weapon);
    }

    [Command(requiresAuthority = false)]
    public void CmdDropWeapon()
    {
        Debug.Log("CMD Dropping Weapon");
        RpcDropWeapon();
    }

    [ClientRpc]
    private void RpcDropWeapon()
    {
        DropWeapon();

        if (isServerOnly) DropWeapon();
    }

    private void DropWeapon()
    {
        Debug.Log("DropWeapon client");

        if (currentWeapon == null) return;

        tempSpawnPrefab = currentWeapon.weaponDropPrefab;

        if (isLocalPlayer)
        {
            if (currentWeapon.weaponDropPrefab != null){
                Debug.Log("Dropping weapon: " + currentWeapon.weaponDropPrefab);
                Vector3 forwardDir = Vector3.zero;
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    forwardDir = cam.transform.forward;
                }

                CmdSpawnCurrentWeapon(firstPersonWeaponAnchor.position + forwardDir * 2.0f, firstPersonWeaponAnchor.rotation, forwardDir);

                // Debug.Log("Spawning Weapon: " + currentWeapon.weaponDropPrefab);
                // GameObject weaponDrop = Instantiate(currentWeapon.weaponDropPrefab, firstPersonWeaponAnchor.position, firstPersonWeaponAnchor.rotation);
                // weaponDrop.GetComponent<Rigidbody>().velocity = transform.forward;
                // NetworkServer.Spawn(weaponDrop);
            }
            

            Destroy(currentWeaponObject);
            flourishHandler.reloadAnimator = null;
            flourishHandler.currentFlourish = null;
            currentWeaponObject = null;
        }

        currentAmmo = 0;
        currentWeapon = null;

        SetTrigger(new string[] {"p"}, currentWeapon);
    }

    //drop weapon on server
    [Command]
    public void CmdSpawnCurrentWeapon(Vector3 _position, Quaternion _rotation, Vector3 _velocity)
    {
        if (tempSpawnPrefab == null) return;

        Debug.Log("Spawning Weapon: " + tempSpawnPrefab);
        GameObject weaponDrop = Instantiate(tempSpawnPrefab, _position, Quaternion.identity);
        weaponDrop.GetComponent<Rigidbody>().velocity = _velocity;
        NetworkServer.Spawn(weaponDrop);
    }

    void Update()
    {
        if(ammoText != null)
        {
            if(currentWeapon != null)
            {
                ammoText.text = currentAmmo.ToString() + " / " + currentWeapon.clipSize.ToString();
            }
            else
            {
                ammoText.text = "";
            }
        }
    }
}