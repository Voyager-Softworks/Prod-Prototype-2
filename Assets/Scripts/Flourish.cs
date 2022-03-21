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

    
    public float flourishActivateDelay = 0.0f;
    public float effectDuration = 2.0f;

    public float flourishFireDelayMod = 0.0f;
    public float flourishMoveSpeedMod = 0.0f;

    


    


}
