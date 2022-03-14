using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Interact : NetworkBehaviour
{
    public InputAction interactAction;

    void Start()
    {
        interactAction.performed += ctx => OnInteract();
    }

    void OnDestroy()
    {
        interactAction.performed -= ctx => OnInteract();
    }

    void OnInteract()
    {
        if (interactAction.ReadValue<float>() == 0)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2))
        {
            Interactible interactable = hit.transform.GetComponent<Interactible>();
            if (interactable != null)
            {
                if(interactable.interactionType == Interactible.InteractionType.Pickup)
                {
                    GetComponent<Equipment>().EquipWeapon(interactable.weaponData);
                    if(interactable.destroyOnInteract)
                    {
                        Destroy(interactable.gameObject);
                    }
                }
                else if(interactable.interactionType == Interactible.InteractionType.Use)
                {
                    
                }
                
            }
        }
    }
}
