using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple player movement script
/// </summary>
public class PlayerMovement : NetworkBehaviour
{
    public InputAction moveAction;
    public InputAction jumpAction;

    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance;
    public LayerMask groundMask;

    public float moveSpeed;
    public float jumpHeight;

    public float gravity;

    public Vector3 velocity;
    bool isGrounded;

    private void Start() {
        controller = GetComponent<CharacterController>();
        moveAction.Enable();
        jumpAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //only if local player
        if (isLocalPlayer)
        {
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     Application.Quit();

            //     connectionToServer.Disconnect();
            //     connectionToClient.Disconnect();
            // }

            isGrounded = false;

            Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);

            foreach (Collider _hit in hits)
            {
                if (_hit.transform.root == gameObject.transform)
                {
                    continue;
                }

                isGrounded = true;
                break;
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            float x = moveInput.x;
            float z = moveInput.y;

            Vector3 move = Vector3.ClampMagnitude((transform.right * x) + (transform.forward * z), 1.0f);

            controller.Move(move * Time.deltaTime * moveSpeed);

            if (jumpAction.ReadValue<float>() > 0 && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            
        }
    }
}
