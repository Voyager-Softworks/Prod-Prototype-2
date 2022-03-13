using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "WeaponData", order = 1)]
public class WeaponData : ScriptableObject
{
    public GameObject weaponPrefabFirstPerson;
    public GameObject weaponPrefabThirdPerson;
    public GameObject weaponDropPrefab;
}
