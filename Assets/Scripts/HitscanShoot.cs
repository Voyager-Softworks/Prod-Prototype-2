using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanShoot : NetworkBehaviour
{
    public GameObject _playerRoot;
    Equipment equip;

    // Start is called before the first frame update
    void Start()
    {
        equip = GetComponent<Equipment>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(equip.currentWeapon == null) return;
            if(!equip.TryFire()) return;
            equip.SetTrigger(new string[]{"s", Random.Range(0,1).ToString()}, equip.currentWeapon);
            Debug.Log("Pew!");

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.transform.name);

                if (hit.transform.tag == "Player" && hit.distance < equip.currentWeapon.range)
                {
                    PlayerHealth.Damage dmg = new PlayerHealth.Damage(equip.currentWeapon.damage, equip.currentWeapon.name, Time.time, hit.distance, Vector3.zero, transform.position, hit.point - transform.position, hit.point, hit.normal, GetComponentInParent<NetworkIdentity>());

                    GameObject player = hit.transform.root.gameObject;
                    if (player && player.GetComponent<PlayerHealth>()) player.GetComponent<PlayerHealth>().CmdTakeDamage(dmg);
                }
            }
        }
    }
}