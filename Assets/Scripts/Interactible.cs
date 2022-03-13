using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

public abstract class Interactible : NetworkBehaviour
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
        if (interactionType == InteractionType.Pickup)
        {
            Pickup();
        }
        else if (interactionType == InteractionType.Use)
        {
            Use();
        }
    }

    

    public void OnHoverEnter()
    {
        
    }
    public void OnHoverExit()
    {
        
    }

    public virtual void Pickup()
    {
        
    }

    public virtual void Use()
    {
        
    }
}
