using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HitscanShoot : NetworkBehaviour
{
    public GameObject _playerRoot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("HitscanShoot");

        if (!isLocalPlayer) return;

        if (Mouse.current.leftButton.isPressed)
        {
            GetComponent<PlayerMovement>().velocity.y = 1.0f;

            Debug.Log("Pew!");

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.transform.name);

                if (hit.transform.tag == "Player")
                {
                    hit.transform.root.GetComponent<PlayerHealth>().TakeDamage(10);
                }
            }
        }
    }
}
