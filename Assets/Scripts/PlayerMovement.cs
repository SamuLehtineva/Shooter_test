using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Misc")]
    public Transform orientation;
    public UIBar dashBar;

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
    private bool canDoubleJump;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    public float slideCooldown;
    public float slideYScale;
    private float slideTimer;
    private bool sliding;
    private float startYScale;
    private float slideInput;
    private bool canSlide;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown = 1.5f;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask groundMask;
    private bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    
    private Vector2 moveInput;
    private Vector3 moveDirection;
    public PlayerInput playerInput;

    private Rigidbody rigid;

    public TextMeshProUGUI speedText;
    public TextMeshProUGUI desiredText;

    public bool isWallRunning;

    public MovementState state;
    public enum MovementState
    {
        walking,
        air,
        sliding,
        wallrunning
    }

    public bool momentum;

    void Awake()
    {
        playerInput = new PlayerInput();
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true;

        canJump = true;
        canSlide = true;

        startYScale = transform.localScale.y;
        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        MyInput();
        UpdateText();
        dashBar.SetValue(dashCooldownTimer / dashCooldown);
    }

    void FixedUpdate() {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        if (isGrounded)
        {
            rigid.drag = groundDrag;
            ResetDoubleJump();
        } else 
        {
            rigid.drag = 0;
        }
        StateHandler();
        MovePlayer();
        DashMove();
        DashCooldown();
        if (OnSlope())
        {
            //Debug.Log("ONSLOpe");
        }
        SpeedControl();
        if (sliding)
        {
            SlidingMovement();
        }
        
    }

    void StateHandler()
    {
        if (isWallRunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallSpeed;
        }
        else if (isGrounded && sliding)
        {
            state = MovementState.sliding;

            if (OnSlope() && rigid.velocity.y < 0.1f)
            {
                //Debug.Log("jee");
                desiredMoveSpeed = slideSpeed;
            }
            else
            {
                //Debug.Log("not jee");
                desiredMoveSpeed = walkSpeed * 1.5f;
            }
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
            /*if (moveInput != Vector2.zero)
            {
                desiredMoveSpeed = walkSpeed;
            }
            else
            {
                desiredMoveSpeed = 0;
            }*/
        }
        else
        {
            state = MovementState.air;
        }


        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 2f && moveSpeed != 0)
        {
            StopCoroutine(LerpMoveSpeed());
            StartCoroutine(LerpMoveSpeed());
        }
        else if (isGrounded && !isWallRunning && !sliding)
        {
            moveSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    void MyInput()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        slideInput = playerInput.Player.Slide.ReadValue<float>();

        if (slideInput > 0 && !sliding && canSlide)
        {
            StartSlide();
        }
        else if (slideInput == 0 && sliding)
        {
            StopSlide();
        }
    }

    void MovePlayer()
    {
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (OnSlope())
		{
            rigid.AddForce(GetSlopeMoveDirection() * moveSpeed * 10f, ForceMode.Force);
                
            if (canJump)
			{
                if (GetSlopeMoveDirection().y < 0.1f)
                {
                    rigid.AddForce(Vector3.down * 200f, ForceMode.Force);
                    momentum = true;
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
        
        rigid.useGravity = !OnSlope();
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

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (canJump && isGrounded)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
            rigid.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            canJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        else if (canDoubleJump && !isWallRunning)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, jumpForce, rigid.velocity.z);
            canDoubleJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void ResetJump()
    {
        canJump = true;
    }

    public void ResetDoubleJump()
    {
        canDoubleJump = true;
    }

    void ResetSlide()
    {
        canSlide = true;
    }

    bool OnSlope()
    {
        Debug.DrawRay(transform.position, Vector3.down * (2 * 0.5f + 0.2f), Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void StartDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTimer >= dashCooldown)
        {
            if (OnSlope())
            {
                dashDirection = GetSlopeMoveDirection();
            }
            else
            {
                dashDirection = moveDirection;
            }
            dashTimer = 0f;
            dashCooldownTimer = 0f;
        }
    }

    void DashMove()
    {
        if (dashTimer < dashDuration)
        {
            rigid.drag = 0;
            rigid.AddForce(dashDirection.normalized * dashSpeed * 10f, ForceMode.Force);
            rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
            dashTimer += Time.fixedDeltaTime;
            if (OnSlope() && canJump)
            {
                if (GetSlopeMoveDirection().y < 0.1f)
				{
                    rigid.AddForce(Vector3.down * 120f, ForceMode.Force);
                }
            }
        }
    }

    void DashCooldown()
    {
        if (dashCooldownTimer < dashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;
        }
    }

    void StartSlide()
    {
        sliding = true;
        canSlide = false;

        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        slideTimer = maxSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        if (!OnSlope() || rigid.velocity.y > -0.1f)
        {
            rigid.AddForce(moveDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rigid.AddForce(GetSlopeMoveDirection() * slideForce * 1.5f, ForceMode.Force);
            
            if (GetSlopeMoveDirection().y < 0.1f)
			{
                rigid.AddForce(Vector3.down * 200f, ForceMode.Force);
            }
        }


        if (slideTimer <= 0)
        {
            StopSlide();
        }
    }

    void StopSlide()
    {
        sliding = false;
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        Invoke(nameof(ResetSlide), slideCooldown);
    }

    void UpdateText()
    {
        speedText.text = "Speed: " + rigid.velocity.magnitude;
        desiredText.text = "Desired Speed: " + desiredMoveSpeed;
    }

    private void OnEnable()
    {
        playerInput.Player.Move.Enable();

        playerInput.Player.Jump.performed += Jump;
        playerInput.Player.Jump.Enable();

        playerInput.Player.Dash.Enable();
        playerInput.Player.Dash.performed += StartDash;

        playerInput.Player.Slide.Enable();
    }

    private void OnDisable()
    {
        playerInput.Player.Move.Disable();

        playerInput.Player.Jump.performed -= Jump;
        playerInput.Player.Jump.Disable();

        playerInput.Player.Dash.Disable();
        playerInput.Player.Dash.performed -= StartDash;

        playerInput.Player.Slide.Disable();
    }
}
