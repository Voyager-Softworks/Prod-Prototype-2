using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;

    public Transform playerBody;
    public Transform playerTorso;
    public GameObject head;
    public PlayerCanvas playerCanvas;

    public GameObject player;

    float xRoation;

    private void OnLevelWasLoaded(int level)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //Disable eyes if client, otherwise disable camera and canvas
        if (player.GetComponent<PlayerMovement>().isLocalPlayer)
        {
            head.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
            playerCanvas.gameObject.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Tab to hide/show cursor, mouse move while hidden to move cam
        if (player.GetComponent<PlayerMovement>().isLocalPlayer)
        {

            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.visible = false;
                Vector2 mouseInput = Mouse.current.delta.ReadValue();
                float mouseX = mouseInput.x * mouseSensitivity;
                float mouseY = mouseInput.y * mouseSensitivity;

                xRoation -= mouseY;
                xRoation = Mathf.Clamp(xRoation, -90f, 90f);

                playerTorso.transform.localRotation = Quaternion.Euler(xRoation, 0f, 0f);

                playerBody.Rotate(playerBody.up * mouseX);
            }
            else
            {
                Cursor.visible = true;
            }
        }
    }
}
