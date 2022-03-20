using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public GameObject weaponPrefabFirstPerson;
    public GameObject weaponDropPrefab;

    public string weaponName;

    public float fireDelay = 0.0f;

    public bool automatic = false;
    public bool cycleAnimations = true;

    public int damage = 10;

    public float range = 100.0f;

    public float accuracyJitter = 0.0f;

    public int clipSize;

    public Flourish flourish;

    

    
}
