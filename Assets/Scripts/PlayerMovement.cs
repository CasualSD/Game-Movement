using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float crouchYSize;
    bool isCrouching;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    public Transform playerObject;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    public float playerVelocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        isCrouching = false;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        playerVelocity = rb.velocity.x;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && grounded)
        {
            Jump();
        }

        if (Input.GetKey(crouchKey))
        {
            Crouch();
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedGroundedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedGroundedVel.x, rb.velocity.y, limitedGroundedVel.z);

            if (!grounded && flatVel.magnitude > moveSpeed * 0.8f)
            {
                Vector3 limitedNotGroundedVel = flatVel.normalized * moveSpeed * 0.8f;
                rb.velocity = new Vector3(limitedNotGroundedVel.x, rb.velocity.y, limitedNotGroundedVel.z);
            }
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void Crouch()
    {

        Vector3 currentSize = playerObject.transform.localScale;
        if (isCrouching)
        {
            playerObject.transform.localScale = new Vector3(currentSize.x, currentSize.y * crouchYSize, currentSize.z);
        }
        else
        {
            playerObject.transform.localScale = new Vector3(currentSize.x, currentSize.y, currentSize.z);
        }
    }
}