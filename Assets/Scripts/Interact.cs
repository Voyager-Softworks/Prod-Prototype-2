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
        interactAction.performed += ctx => { if(isLocalPlayer)OnInteract(); };
        interactAction.Enable();
        dropItemAction.performed += ctx => {if(isLocalPlayer)OnDropItem();};
        dropItemAction.Enable();
    }

    void OnDestroy()
    {
        interactAction.performed -= ctx => OnInteract();
        interactAction.Disable();
        dropItemAction.performed -= ctx => OnDropItem();
        dropItemAction.Disable();
    }

    [Command]
    void OnInteract()
    {
        Debug.Log("Interact");

        RaycastHit hit;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 3))
        {
            Debug.Log("Hit: " + hit.transform.name);
            
            Interactible interactable = hit.transform.GetComponent<Interactible>();
            if (interactable != null)
            {
                if(interactable.interactionType == Interactible.InteractionType.Pickup)
                {
                    GetComponent<Equipment>().EquipWeapon(Instantiate(interactable.weaponData));
                    if(interactable.destroyOnInteract)
                    {
                        NetworkServer.Destroy(interactable.gameObject);
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
