using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flourish", menuName = "Weapons", order = 1)]
public class Flourish : ScriptableObject
{
    public enum FlourishDir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    public List<FlourishDir> Moves;
    


}
