using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Misc")]
    public Transform orientation;
    public Grappling grappling;

    [Header("Movement")]
    public float walkSpeed;
    public float slideSpeed;
    public float wallSpeed;
    public float groundDrag;
    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;
    private float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool canJump;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    public bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    
    
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector3 moveDirection;
    public PlayerInput playerInput;

    private Rigidbody rigid;

    [Header("Misc")]
    public TextMeshProUGUI velocityText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI desiredText;
    public TextMeshProUGUI momentumText;

    public bool isWallRunning;
    public bool isBoosting;
    public bool isGrappling;
    public bool isSliding;

    public MovementState state;
    public enum MovementState
    {
        walking,
        sliding,
        wallrunning,
        boosting
    }

    void Awake()
    {
        playerInput = new PlayerInput();
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;

        canJump = true;
    }

    void StateHandler()
    {
        if (isWallRunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallSpeed;
        }
        else if (isGrounded && isSliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rigid.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                desiredMoveSpeed = walkSpeed * 1.5f;
            }
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.walking;
        }
        moveSpeed = desiredMoveSpeed;
    }

    void Update()
    {
        UpdateText();
    }

    void FixedUpdate() {
        MyInput();

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        if (isGrounded && !isGrappling)
        {
            rigid.drag = groundDrag;
            if (!isSliding)
            {
                //momentum = false;
            }
        }
        else
        {
            rigid.drag = 0;
        }

        StateHandler();

        if (!isBoosting && !isGrappling)
        {
            MovePlayer();
            SpeedControl();
        }
        else
        {
            rigid.useGravity = false;
        }
    }

    void MyInput()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        if (state == MovementState.walking)
        {
            if (OnSlope())
		    {
                rigid.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
                    
                if (canJump)
                {
                    if (GetSlopeMoveDirection().y < 0.1f)
                    {
                        rigid.AddForce(Vector3.down * 80f, ForceMode.Force);
                    }
                }
            }
            else if (isGrounded)
            {
                rigid.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }
            else if (!isGrounded)
            {
                rigid.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

            if (OnSlope() || isWallRunning || isBoosting || isGrappling)
            {
                rigid.useGravity = false;
            }
            else
            {
                rigid.useGravity = true;
            }
        }
        
        //rigid.useGravity = !OnSlope();
    }

    void SpeedControl()
    {
        if (OnSlope() && canJump)
        {
            if (rigid.velocity.magnitude > moveSpeed)
            {
                rigid.velocity = rigid.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rigid.velocity = new Vector3(limitedVel.x, rigid.velocity.y, limitedVel.z);
            }
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (isGrappling)
        {
            grappling.StopGrapple();
        }
        if (canJump && isGrounded)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
            rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            if (isSliding)
            {
                rigid.AddForce(transform.forward * 250f);
            }
            canJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        canJump = true;
    }

    public bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (2 * 0.5f + 0.2f), Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }


    void UpdateText()
    {
        //speedText.text = "Speed: " + rigid.velocity.magnitude;
        velocityText.text = "Velocity: " + new Vector2(rigid.velocity.x, rigid.velocity.z).magnitude;
        speedText.text = "Speed: " + moveSpeed;
        desiredText.text = "Desired Speed: " + desiredMoveSpeed;
        momentumText.text = "";
    }

    private void OnEnable()
    {
        playerInput.Player.Move.Enable();

        playerInput.Player.Jump.performed += Jump;
        playerInput.Player.Jump.Enable();

        //playerInput.Player.Slide.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Move.Disable();

        playerInput.Player.Jump.performed -= Jump;
        playerInput.Player.Jump.Disable();

        //playerInput.Player.Slide.Disable();
    }
}
