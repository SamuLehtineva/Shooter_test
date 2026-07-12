using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBoost : MonoBehaviour
{
    [Header("RocketBoost")]
    public float maxFuel = 100;
    public float reFuelRate;
    public float reFuelDelay;
    private float reFuelDelayTimer;

    public float boostPower;
    public float boostDrainRate;
    public float airJumpForce;
    public float airJumpFuelDrain = 25f;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown = 0.5f;
    public float dashFuelDrain = 25f;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    
    private float currentFuel;

    private float boostInput;
    private bool isFlying = false;

    [Header("References")]
    public Transform cameraHolder;
    private Rigidbody rigid;
    private PlayerMovement pm;
    private PlayerInput input;

    [Header("Misc")]
    public UIBar fuelBar;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();

        input = pm.playerInput;
        input.Player.Boost.Enable();
        input.Player.Dash.Enable();
        input.Player.Dash.performed += StartDash;
        input.Player.Jump.performed += AirJump;

        currentFuel = maxFuel;
        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        fuelBar.SetValue(currentFuel / maxFuel);
    }

    void FixedUpdate()
    {
        boostInput = input.Player.Boost.ReadValue<float>();

        if (dashTimer < dashDuration)
        {
            DashMove();
        }

        DashCooldown();

        if (boostInput > 0 && currentFuel > 0)
        {
            if (!isFlying)
            {
                currentFuel -= 10;
                isFlying = true;
            }
            else
            {
                currentFuel -= boostDrainRate * Time.deltaTime;
            }

            pm.isBoosting = true;
            BoostMovement();

        }
        else
        {
            isFlying = false;
            pm.isBoosting = false;
        }
        
        if (pm.isGrounded && currentFuel < maxFuel)
        {
            ReFuel();
        }
    }

    void ReFuel()
    {
        if (reFuelDelayTimer >= reFuelDelay)
        {
            currentFuel += reFuelRate * Time.deltaTime;
        }
        else
        {
            reFuelDelayTimer += Time.fixedDeltaTime;
        }
    }

    public bool UseFuel(float fuelUsed)
    {
        if(currentFuel >= fuelUsed)
        {
            currentFuel -= fuelUsed;
            reFuelDelayTimer = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AirJump(InputAction.CallbackContext context)
    {
        if (!pm.isGrounded && !pm.isGrappling && !pm.isWallRunning)
        {
            if (UseFuel(airJumpFuelDrain))
            {
                rigid.velocity = new Vector3(rigid.velocity.x, airJumpForce, rigid.velocity.z);
            }
        }
       
    }

    void BoostMovement()
    {
        //rigid.AddForce(cameraHolder.forward * boostPower, ForceMode.Force);
        rigid.velocity = cameraHolder.forward * boostPower;
    }

    void StartDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTimer >= dashCooldown && UseFuel(dashFuelDrain))
        {
            Debug.Log("Start");
            if (pm.OnSlope())
            {
                dashDirection = pm.GetSlopeMoveDirection();
            }
            else
            {
                dashDirection = pm.moveDirection;
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
            if (dashDirection.y < 0.1f && pm.isGrounded)
            {
                rigid.AddForce(Vector3.down * 120f, ForceMode.Force);
            }
        }
    }

    void DashCooldown()
    {
        if (dashCooldownTimer < dashCooldown)
        {
            dashCooldownTimer += Time.fixedDeltaTime;
        }
    }

    void OnDisable()
    {
        input.Player.Fly.Disable();
        input.Player.Boost.Disable();
        input.Player.Dash.Disable();
    }
}
