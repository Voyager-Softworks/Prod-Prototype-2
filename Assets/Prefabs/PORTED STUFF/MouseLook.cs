using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity;

    public Transform playerBody;
    public Transform playerTorso;
    public GameObject head;
    public GameObject playerCanvas;

    public GameObject player;

    float xRoation;

    private void OnLevelWasLoaded(int level)
    {
        GameObject mc = GameObject.Find("MenuCamera");
        if (mc)
        {
            mc.SetActive(false);
        }
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
            playerCanvas.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;

        //If loaded in, disable menu camera
        GameObject mc = GameObject.Find("MenuCamera");

        if (mc)
        {
            mc.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Tab to hide/show cursor, mouse move while hidden to move cam
        if (player.GetComponent<PlayerMovement>().isLocalPlayer)
        {

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.lockState = (Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None);
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.visible = false;
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

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
