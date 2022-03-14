using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Interactible : NetworkBehaviour
{
    public enum InteractionType
    {
        None,
        Pickup,
        Use
    }

    public InteractionType interactionType;
    public WeaponData weaponData;
    public UnityEvent onInteract;

    public bool destroyOnInteract = true;

    public void Interact()
    {
        
    }

    

    public void OnHoverEnter()
    {
        
    }
    public void OnHoverExit()
    {
        
    }

    
}
