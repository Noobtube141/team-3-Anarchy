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

    // Control movement
    private void Update()
    {
        CharacterController characterController = GetComponent<CharacterController>();

        if (characterController.isGrounded)
        {
            movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            movement = transform.TransformDirection(movement);

            movement *= movementSpeed;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSprinting = true;

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
        }

        movement.y -= characterGravity * Time.deltaTime;

        characterController.Move(movement * Time.deltaTime);
    }
}
