using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Movement values
    public float movementSpeed = 10.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpSpeed = 10.0f;
    public float characterGravity = 25.0f;

    // Movement booleans
    public bool isSprinting = false;

    // Movement expressed as vector3
    private Vector3 movement = Vector3.zero;

    // Check for jump (mobile)
    private bool toJump = false;

    // Joystick for control
    public JoystickController joystick;

    // Control movement
    private void Update()
    {
        CharacterController characterController = GetComponent<CharacterController>();
        
        if (characterController.isGrounded)
        {
            // Check if running in the editor or standalone build
            #if UNITY_STANDALONE || UNITY_WEBPLAYER

            movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            movement = transform.TransformDirection(movement);

            movement *= movementSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    isSprinting = true;
                }

                movement *= sprintMultiplier;
            }
            else
            {
                isSprinting = false;
            }

            if (Input.GetButtonDown("Jump"))
            {
                movement *= 0.75f;

                movement.y = jumpSpeed;
            }

            // Check if running on mobile
            #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

            if(joystick.detectedInputDirection.magnitude != 0)
            {
                movement = new Vector3(joystick.detectedInputDirection.x, 0, joystick.detectedInputDirection.y);

                movement = transform.TransformDirection(movement);

                movement *= movementSpeed;

                if(joystick.detectedInputDirection.magnitude >= 0.95f)
                {
                    isSprinting = true;
                    
                    movement *= sprintMultiplier;
                }
                else
                {
                    isSprinting = false;
                }
            }
            else
            {
                movement = Vector3.zero;
            }

            if (toJump)
            {
                toJump = false;

                movement *= 0.75f;

                movement.y = jumpSpeed;
            }
            
            #endif
        }

        movement.y -= characterGravity * Time.deltaTime;

        characterController.Move(movement * Time.deltaTime);
    }

    // Jump on click
    public void JumpOnClick()
    {
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController.isGrounded)
        {
            toJump = true;
        }
    }
}
