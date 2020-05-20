using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{

    public SteamVR_Action_Vector2 input;
    public SteamVR_Action_Boolean jump;

    public float joystickDeadZone = 0.1f;

    public float speed = 12f;
    public float jumpHeight = 3f;
    public float gravity = 1.62f; // Moon Gravity
    //public float gravity = 9.81f; // Earth Gravity

    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    private CharacterController playerController;

    private Vector3 currentVelocity;
    private bool isGrounded;

    private void Start()
    {
        playerController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckDistance, groundMask);
        if (isGrounded && currentVelocity.y < 0)
        {
            currentVelocity.y = -1f;
        }

        // X Z Movement
        if (input.axis.magnitude > joystickDeadZone)
        {
            Vector3 direction = Player.instance.hmdTransform.TransformDirection(new Vector3(input.axis.x, 0, input.axis.y));
            // Vector3 gravityVector = new Vector3(0, gravity, 0) * Time.deltaTime;
            Vector3 movementProjection = Vector3.ProjectOnPlane(direction, Vector3.up);
            Vector3 movementVector = (movementProjection * speed * Time.deltaTime);
            playerController.Move(movementVector);
        }

        // Jumping
        if (jump.stateDown && isGrounded)
        {
            currentVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        currentVelocity.y += gravity * Time.deltaTime;
        playerController.Move(currentVelocity * Time.deltaTime);
    }
}
