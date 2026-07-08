using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask wallMask;
    public LayerMask groundMask;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallJumpForwardForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    public float exitWallCoolDown;
    private bool exitingWall;
    private float exitWallTimer;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rigid;
    public PlayerLook cam;

    private PlayerInput playerInput;
    private Vector2 moveInput;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        playerInput = pm.playerInput;
        playerInput.Player.Jump.performed += WallJump;
    }

    void StateMachine()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        if ((wallLeft || wallRight) && moveInput.y > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.isWallRunning)
            {
                StartWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && pm.isWallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallCoolDown;
            }
        }
        else if (exitingWall)
        {
            StopWallRun();

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            StopWallRun();
        }
    }

    void Update()
    {
        WallCheck();
        StateMachine();
    }

    void FixedUpdate()
    {
        if (pm.isWallRunning)
        {
            WallRunningMovement();
        }
    }

    void WallCheck()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, wallMask);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, wallMask);
    }
    
    bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, groundMask);
    }

    void StartWallRun()
    {
        pm.isWallRunning = true;
        pm.ResetDoubleJump();
        wallRunTimer = maxWallRunTime;
        rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
        //cam.DoFov(110f);
        if (wallLeft)
        {
            cam.DoTilt(-5f);
        }
        else
        {
            cam.DoTilt(5f);
        }
    }
    
    void WallRunningMovement()
    {
        rigid.useGravity = false;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rigid.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // push to wall
        if (!(wallLeft && moveInput.y > 0) && !(wallRight && moveInput.y < 0))
        {
            rigid.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    void StopWallRun()
    {
        pm.isWallRunning = false;
        //cam.DoFov(80f);
        cam.DoTilt(0f);
    }

    void WallJump(InputAction.CallbackContext context)
    {
        if (pm.isWallRunning)
        {
            exitingWall = true;
            exitWallTimer = exitWallCoolDown;

            Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

            Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce + orientation.forward * wallJumpForwardForce;

            rigid.velocity = new Vector3(rigid.velocity.x, 0f, rigid.velocity.z);
            rigid.AddForce(forceToApply, ForceMode.Impulse);
        }
    }

    void StartWallJump()
    {
        if (pm.isWallRunning)
        {
            exitingWall = true;
            exitWallTimer = exitWallCoolDown;
        }
    }


    void OnDisable()
    {
        playerInput.Player.Jump.performed -= WallJump;
    }
}
