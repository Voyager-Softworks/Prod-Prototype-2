using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class Interact : NetworkBehaviour
{
    public InputAction interactAction;
    public InputAction dropItemAction;
    public Transform camTransform;

    void Start()
    {
        interactAction.performed += ctx => OnInteract();
        interactAction.Enable();
        dropItemAction.performed += ctx => OnDropItem();
        dropItemAction.Enable();
    }

    void OnDestroy()
    {
        interactAction.performed -= ctx => OnInteract();
        interactAction.Disable();
        dropItemAction.performed -= ctx => OnDropItem();
        dropItemAction.Disable();
    }

    void OnInteract()
    {
        

        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 2))
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

    void OnDropItem()
    {
        GetComponent<Equipment>().DropWeapon();
    }
}
