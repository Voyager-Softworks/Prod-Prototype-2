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
                    Debug.Log("Picking up: " + interactable.weaponData);
                    GetComponent<Equipment>().CmdTryEquipWeapon(hit.transform.gameObject);
                }
                else if(interactable.interactionType == Interactible.InteractionType.Use)
                {
                    
                }
                
            }
        }
    }

    void OnDropItem()
    {
        Debug.Log("Press DropItem");
        GetComponent<Equipment>().CmdDropWeapon();
    }
}
