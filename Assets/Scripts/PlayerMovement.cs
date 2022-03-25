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
    public InputAction slideAction;
    public InputAction sprintAction;

    public Animator thirdPersonAnimator;
    public CharacterController controller;
    public Transform groundCheck;
    public Transform body;
    public float groundDistance;
    public LayerMask groundMask;

    

    public float moveSpeed;
    public float sprintMoveSpeed;
    public float crouchMoveSpeed;
    public float unarmedModifier;
    public float jumpHeight;
    public float initialSlideSpeed;
    public float maxSlideDuration;
    Vector3 slideDirection;
    public float slideTimer;
    public float gravity;

    public Vector3 velocity;
    public Vector3 animationVelocity;
    bool isGrounded;

    Equipment equip;

    bool hasDoubleJumped;
    bool canDoubleJump = false;

    public AudioClip jumpSound, landSound;
    public AudioClip[] footstepSounds;
    public AudioSource jumpLandSource;

    public Transform cameraTransform;

    float jumpcoolDown = 0.1f;
    float jumpTimer = 0.0f;
    float airTime = 0.0f;

    float distanceSinceLastFootstep = 0.0f;


    private void Start() {
        controller = GetComponent<CharacterController>();
        moveAction.Enable();
        jumpAction.Enable();
        slideAction.Enable();
        sprintAction.Enable();
        equip = GetComponent<Equipment>();
        hasDoubleJumped = false;
    }

    
    

    // Update is called once per frame
    void Update()
    {
        //only if local player
        if (isLocalPlayer)
        {
            bool wasGrounded = isGrounded;
            // if (Input.GetKeyDown(KeyCode.Escape))
            // {
            //     Application.Quit();

            //     connectionToServer.Disconnect();
            //     connectionToClient.Disconnect();
            // }

            isGrounded = false;
            Vector3 positionCache = transform.position;

            Collider[] hits = Physics.OverlapSphere(groundCheck.position, groundDistance, groundMask);
            
            foreach (Collider _hit in hits)
            {
                if (_hit.transform.root == gameObject.transform)
                {
                    continue;
                }
                
                isGrounded = true;
                hasDoubleJumped = false;
                break;
            }
            if(!wasGrounded && isGrounded && airTime > 0.2f)
            {
                jumpLandSource.PlayOneShot(landSound);
            }
            if(!isGrounded)
            {
                airTime += Time.deltaTime;
            }
            else
            {
                airTime = 0.0f;
            }

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                
            }

            thirdPersonAnimator.SetBool("Jump", !isGrounded);

            Vector2 moveInput = moveAction.ReadValue<Vector2>();
            float x = moveInput.x;
            float z = moveInput.y;

            Vector3 move = Vector3.ClampMagnitude((body.right * x) + (body.forward * z), 1.0f);
            animationVelocity = Vector3.Lerp(animationVelocity, moveInput, Time.deltaTime * 10.0f);

            
            
            
            if(slideAction.ReadValue<float>() > 0.0f)
            {
                equip.SetSprinting(false);
                if(slideTimer == 0.0f)
                {
                    slideDirection = move;
                    if(move.magnitude == 0.0f)
                    {
                        slideTimer = maxSlideDuration;
                    }
                    //thirdPersonAnimator.SetTrigger("Slide");
                }
                if(slideTimer > maxSlideDuration)
                {
                    velocity.x = 0.0f;
                    velocity.z = 0.0f;
                    controller.Move(move * Time.deltaTime * crouchMoveSpeed);
                }
                else
                {
                    float velY = velocity.y;
                    velocity = (1.0f - Mathf.Clamp(slideTimer, 0, maxSlideDuration) / maxSlideDuration) * initialSlideSpeed * slideDirection;
                    velocity.y = velY;
                    slideTimer += Time.deltaTime;
                    
                }
                thirdPersonAnimator.SetBool("Crouching", true);
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + (transform.up * 0.1f), Time.deltaTime * 10.0f);
            }
            else
            {

                slideTimer = 0.0f;
                velocity.x = 0.0f;
                velocity.z = 0.0f;
                if(equip.currentWeapon == null) move *= unarmedModifier;
                if(move.magnitude > 0.0f)
                {
                    if(sprintAction.ReadValue<float>() > 0)
                    {
                        
                        controller.Move(move * Time.deltaTime * sprintMoveSpeed);
                        equip.SetSprinting(true);
                        //tilt camera based on movement
                        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(moveInput.y * 0.25f, 0.0f, moveInput.x * -1.5f), Time.deltaTime * 10.0f);
                    }
                    else
                    {
                        //tilt camera based on movement
                        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(0.0f, 0.0f, moveInput.x * -0.75f), Time.deltaTime * 10.0f);
                        controller.Move(move * Time.deltaTime * moveSpeed);
                        equip.SetSprinting(false);
                    }
                }
                
                thirdPersonAnimator.SetFloat("MovementForwardBack", animationVelocity.y);
                thirdPersonAnimator.SetFloat("MovementLeftRight", animationVelocity.x);
                thirdPersonAnimator.SetBool("Crouching", false);
                
                cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + (transform.up * 1.0f), Time.deltaTime * 10.0f);
            }

            
            jumpTimer -= Time.deltaTime;
            if (jumpAction.ReadValue<float>() > 0 && (isGrounded || (!hasDoubleJumped && canDoubleJump)) && jumpTimer <= 0.0f)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTimer = jumpcoolDown;
                jumpLandSource.PlayOneShot(jumpSound);
                if (!isGrounded)
                {
                    hasDoubleJumped = true;
                }
            }
            if (jumpAction.ReadValue<float>() <= 0)
            {
                canDoubleJump = true;
            }
            else{
                canDoubleJump = false;
            }
            
            

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
            if(isGrounded && slideAction.ReadValue<float>() <= 0.0f) distanceSinceLastFootstep += Vector3.Distance(positionCache, transform.position);
            if(isGrounded && slideAction.ReadValue<float>() <= 0.0f && distanceSinceLastFootstep > 4.0f)
            {
                jumpLandSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);
                distanceSinceLastFootstep = 0.0f;
            }
            
        }
    }
}
