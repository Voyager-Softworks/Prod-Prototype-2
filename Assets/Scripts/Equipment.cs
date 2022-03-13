using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class Equipment : NetworkBehaviour
{
    [SyncVar]
    public WeaponData currentWeapon;
    public  GameObject currentWeaponObject;
    public Transform firstPersonWeaponAnchor;

    


    public void EquipWeapon(WeaponData weapon)
    {
        if (currentWeapon != null)
        {
            DropWeapon();
        }

        currentWeapon = weapon;

        if (isLocalPlayer)
        {
            if (weapon.weaponPrefabFirstPerson != null)
            {
                currentWeaponObject = Instantiate(weapon.weaponPrefabFirstPerson, transform.position, transform.rotation);
                currentWeaponObject.transform.SetParent(firstPersonWeaponAnchor);
            }
        }
    }

    public void DropWeapon()
    {
        if (currentWeapon != null)
        {
            if (isLocalPlayer)
            {
                if (currentWeapon.weaponDropPrefab != null)
                {
                    GameObject weaponDrop = Instantiate(currentWeapon.weaponDropPrefab, transform.position, transform.rotation);
                    weaponDrop.GetComponent<Rigidbody>().velocity = transform.forward * 5;
                }
            }

            currentWeapon = null;
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
        }
    }
}
