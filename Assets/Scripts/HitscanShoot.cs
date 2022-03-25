using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class HitscanShoot : NetworkBehaviour
{
    public GameObject _playerRoot;
    Equipment equip;

    float fireDelayTimer;

    public bool canFire = true;
    public bool isFlourishing = false;

    bool altAnim = false;

    public TMP_Text _ammoText;

    public Image _hitmarker;

    public AudioClip _hitSound;
    public float hitmarkerFadeTime = 1.25f;
    private float hitmarkerFadeTimer = 0;

    public GameObject hitParticleEffect;

    

    

    // Start is called before the first frame update
    void Start()
    {
        equip = GetComponent<Equipment>();
    }

    // Update is called once per frame
    void Update()
    {
        //locally do the raycast
        if (!isLocalPlayer) return;
        fireDelayTimer -= Time.deltaTime;
        if(_ammoText != null)
        {
            _ammoText.text = equip.currentWeapon.clipSize.ToString();
        }

        if (_hitmarker != null){
            if (hitmarkerFadeTimer > 0)
            {
                hitmarkerFadeTimer -= Time.deltaTime;
                _hitmarker.color = new Color(_hitmarker.color.r, _hitmarker.color.g, _hitmarker.color.b, hitmarkerFadeTimer / hitmarkerFadeTime);
            }
            else
            {
                _hitmarker.color = new Color(_hitmarker.color.r, _hitmarker.color.g, _hitmarker.color.b, 0);
            }
        }

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
            equip.currentWeaponObject.GetComponent<WeaponFX>().PlayFire();
            if(equip.currentWeapon.cycleAnimations)
            {
                if(altAnim) 
                {
                    altAnim = false;
                    equip.SetTrigger(new string[]{"s", "0"}, equip.currentWeapon);
                    equip.currentWeaponObject.GetComponent<WeaponFX>().PlayMuzzleFlash(1);
                }
                else
                {
                    altAnim = true;
                    equip.SetTrigger(new string[]{"s", "1"}, equip.currentWeapon);
                    equip.currentWeaponObject.GetComponent<WeaponFX>().PlayMuzzleFlash(0);
                }
            }
            else
            {
                equip.SetTrigger(new string[]{"s", Random.Range(0,1).ToString()}, equip.currentWeapon);
                equip.currentWeaponObject.GetComponent<WeaponFX>().PlayMuzzleFlash(0);
            }
            Debug.Log("Pew!");
            if(equip.currentWeapon.isShotgun)
            {
                ShotgunRaycast(equip.currentWeapon.range, new Vector2(equip.currentWeapon.accuracyJitter, equip.currentWeapon.accuracyJitter), Camera.main.transform.position, Camera.main.transform.forward, 25);
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
                        DoHitMarker();
                    }
                    if(equip.currentWeapon.isMelee && hit.distance < equip.currentWeapon.range)
                    {
                        equip.SetTrigger(new string[]{"h"}, equip.currentWeapon);
                        equip.currentWeaponObject.GetComponent<WeaponFX>().PlayHit();
                        equip.DumpAmmo();
                    }
                    else if(equip.currentWeapon.isMelee)
                    {
                        equip.SetTrigger(new string[]{"m"}, equip.currentWeapon);
                        equip.AddAmmo(1);
                    }
                    if(hit.distance < equip.currentWeapon.range) Destroy(Instantiate(hitParticleEffect, hit.point, Quaternion.LookRotation(hit.normal)), 10);
                }
                else if(equip.currentWeapon.isMelee)
                {
                    equip.SetTrigger(new string[]{"m"}, equip.currentWeapon);
                    equip.AddAmmo(1);
                }
            }
        }
    }

    //ConeCast
    // public RaycastHit[] ConeCast(float range, float angle, Vector3 origin, Vector3 direction, int layerMask)
    // {
    //     //Cursor.lockState = CursorLockMode.None;
    //     //Cursor.visible = true;
    //     RaycastHit[] sphereHits = Physics.SphereCastAll(origin, range, Vector3.up, range, layerMask);
    //     List<RaycastHit> hits = new List<RaycastHit>();
    //     foreach (RaycastHit hit in sphereHits)
    //     {
    //         if (Vector3.Angle(hit.point - origin, direction) < angle / 2.0f)
    //         {
    //             if(!Physics.Raycast(origin, hit.point - origin, out RaycastHit rayHit, range, LayerMask.GetMask("Default")))
    //             {
    //                 if(hit.transform != transform)
    //                 {
    //                     hits.Add(hit);
    //                 }
                    
                    
    //             }
    //         }
    //     }
    //     return hits.ToArray();
    // }

    public void ShotgunRaycast(float range, Vector2 jitter, Vector3 origin, Vector3 direction, int pellets)
    {
        bool didHit = false;
        for(int i = 0; i < pellets; i++)
        {
            float jitterX = Random.Range(-jitter.x, jitter.x);
            float jitterY = Random.Range(-jitter.y, jitter.y);
            Vector3 jitterVector = new Vector3(jitterX, jitterY, 0);
            Vector3 jitteredDirection = direction + jitterVector;
            RaycastHit hit;
            if (Physics.Raycast(origin, jitteredDirection, out hit, range))
            {
                Debug.Log("Hit: " + hit.transform.name);
                if (hit.transform.tag == "Player" && hit.distance < range)
                {
                    PlayerHealth.Damage dmg = new PlayerHealth.Damage(equip.currentWeapon.damage, equip.currentWeapon.weaponName, Time.time, hit.distance, Vector3.zero, transform.position, hit.point - transform.position, hit.point, hit.normal, GetComponentInParent<NetworkIdentity>());

                    GameObject player = hit.transform.root.gameObject;
                    if (player && player.GetComponent<PlayerHealth>()) player.GetComponent<PlayerHealth>().CmdTakeDamage(dmg);
                    didHit = true;
                }
                Destroy(Instantiate(hitParticleEffect, hit.point, Quaternion.LookRotation(hit.normal)), 10);
            }
        }
        if (didHit){
            DoHitMarker();
        }
    }

    private void DoHitMarker(){
        GetComponent<AudioSource>().PlayOneShot(_hitSound);
        if (_hitmarker) _hitmarker.color = new Color(1, 1, 1, 1);
        hitmarkerFadeTimer = hitmarkerFadeTime;
    }

    void OnDrawGizmosSelected()
    {
        if(equip && equip.currentWeapon)
        {
            if(equip.currentWeapon.isShotgun)
            {
                Gizmos.color = Color.red;
                ConeGizmo(equip.currentWeapon.range, equip.currentWeapon.accuracyJitter, Camera.main.transform.position, Camera.main.transform.forward);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * equip.currentWeapon.range);
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



