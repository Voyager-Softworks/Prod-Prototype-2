using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanShoot : NetworkBehaviour
{
    public GameObject _playerRoot;
    Equipment equip;

    float fireDelayTimer;

    public bool canFire = true;
    public bool isFlourishing = false;

    bool altAnim = false;

    

    // Start is called before the first frame update
    void Start()
    {
        equip = GetComponent<Equipment>();
    }

    // Update is called once per frame
    async void Update()
    {
        //locally do the raycast
        if (!isLocalPlayer) return;
        fireDelayTimer -= Time.deltaTime;

        //mouse click + weapon equipped
        if (((Mouse.current.leftButton.isPressed && equip.currentWeapon && equip.currentWeapon.automatic ) || Mouse.current.leftButton.wasPressedThisFrame) && fireDelayTimer <= 0.0f && canFire)
        {
            if(equip.currentWeapon == null) return;
            
            if(isFlourishing)
            {
                fireDelayTimer = equip.currentWeapon.fireDelay + equip.currentWeapon.flourish.flourishFireDelayMod;
            }
            else
            {
                if(!equip.TryFire()) return;
                fireDelayTimer = equip.currentWeapon.fireDelay;
            }
            if(equip.currentWeapon.cycleAnimations)
            {
                if(altAnim) 
                {
                    altAnim = false;
                    equip.SetTrigger(new string[]{"s", "0"}, equip.currentWeapon);
                }
                else
                {
                    altAnim = true;
                    equip.SetTrigger(new string[]{"s", "1"}, equip.currentWeapon);
                }
            }
            else
            {
                equip.SetTrigger(new string[]{"s", Random.Range(0,1).ToString()}, equip.currentWeapon);
            }
            Debug.Log("Pew!");
            if(equip.currentWeapon.isShotgun)
            {
                RaycastHit[] hits = ConeCast(equip.currentWeapon.range, equip.currentWeapon.accuracyJitter, transform.position, Camera.main.transform.forward, LayerMask.GetMask("Default"));
                foreach(RaycastHit hit in hits)
                {
                    if (hit.transform.tag == "Player" && hit.distance < equip.currentWeapon.range)
                    {
                        PlayerHealth.Damage dmg = new PlayerHealth.Damage(equip.currentWeapon.damage, equip.currentWeapon.weaponName, Time.time, hit.distance, Vector3.zero, transform.position, hit.point - transform.position, hit.point, hit.normal, GetComponentInParent<NetworkIdentity>());

                        GameObject player = hit.transform.root.gameObject;
                        if (player && player.GetComponent<PlayerHealth>()) player.GetComponent<PlayerHealth>().CmdTakeDamage(dmg);
                    }
                }
            }
            else
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log("Hit: " + hit.transform.name);

                    if (hit.transform.tag == "Player" && hit.distance < equip.currentWeapon.range)
                    {
                        PlayerHealth.Damage dmg = new PlayerHealth.Damage(equip.currentWeapon.damage, equip.currentWeapon.weaponName, Time.time, hit.distance, Vector3.zero, transform.position, hit.point - transform.position, hit.point, hit.normal, GetComponentInParent<NetworkIdentity>());

                        GameObject player = hit.transform.root.gameObject;
                        if (player && player.GetComponent<PlayerHealth>()) player.GetComponent<PlayerHealth>().CmdTakeDamage(dmg);
                    }
                }
            }
        }
    }

    //ConeCast
    public RaycastHit[] ConeCast(float range, float angle, Vector3 origin, Vector3 direction, int layerMask)
    {
        RaycastHit[] sphereHits = Physics.SphereCastAll(origin, range, direction, angle, layerMask);
        List<RaycastHit> hits = new List<RaycastHit>();
        foreach (RaycastHit hit in sphereHits)
        {
            if (Vector3.Angle(hit.normal, direction) < angle / 2.0f)
            {
                if(Physics.Raycast(hit.point, transform.position - hit.point, range, layerMask))
                {
                    hits.Add(hit);
                }
            }
        }
        return hits.ToArray();
    }

    void OnDrawGizmosSelected()
    {
        if(equip.currentWeapon)
        {
            if(equip.currentWeapon.isShotgun)
            {
                Gizmos.color = Color.red;
                ConeGizmo(equip.currentWeapon.range, equip.currentWeapon.accuracyJitter, transform.position, Camera.main.transform.forward);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, Camera.main.transform.forward * equip.currentWeapon.range);
            }
        }
    }

    void ConeGizmo(float range, float angle, Vector3 origin, Vector3 direction)
    {
        Vector3 coneRay1 = Quaternion.AngleAxis(angle/2, Vector3.up) * direction;
        Vector3 coneRay2 = Quaternion.AngleAxis(angle/3, Vector3.up) * direction;
        Vector3 coneRay3 = Quaternion.AngleAxis(angle/4, Vector3.up) * direction;
        Vector3 coneRay4 = Quaternion.AngleAxis(angle/5, Vector3.up) * direction;
        Vector3 coneRay5 = Quaternion.AngleAxis(angle/6, Vector3.up) * direction;
        Vector3 forwardRay = origin + (direction.normalized * range);
        for (int i = 0; i < 10; i++)
        {
            Gizmos.DrawRay(origin, coneRay1.normalized * range);
            Gizmos.DrawLine((coneRay1.normalized * range) + origin, (coneRay2.normalized * range) + origin);
            Gizmos.DrawLine((coneRay2.normalized * range) + origin, (coneRay3.normalized * range) + origin);
            Gizmos.DrawLine((coneRay3.normalized * range) + origin, (coneRay4.normalized * range) + origin);
            Gizmos.DrawLine((coneRay4.normalized * range) + origin, (coneRay5.normalized * range) + origin);
            Gizmos.DrawLine((coneRay5.normalized * range) + origin, forwardRay);
            Vector3 temp = coneRay1;
            coneRay1 = Quaternion.AngleAxis(36.0f, direction) * coneRay1;
            coneRay2 = Quaternion.AngleAxis(36.0f, direction) * coneRay2;
            coneRay3 = Quaternion.AngleAxis(36.0f, direction) * coneRay3;
            coneRay4 = Quaternion.AngleAxis(36.0f, direction) * coneRay4;
            coneRay5 = Quaternion.AngleAxis(36.0f, direction) * coneRay5;
            Gizmos.DrawLine((temp.normalized * range) + origin, (coneRay1.normalized * range) + origin);
        }

    }
}



