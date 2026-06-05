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
    private float flyPowerCurrent;
    public float flyPowerMax;
    public float flyPowerIncrease;
    public float flyDrainRate;

    public float boostPower;
    public float boostDrainRate;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown = 0.5f;
    public float dashFuelDrain = 25f;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    
    private float currentFuel;

    private float flyInput;
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
        input.Player.Fly.Enable();
        input.Player.Boost.Enable();
        input.Player.Dash.Enable();
        input.Player.Dash.performed += StartDash;

        currentFuel = maxFuel;
        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        fuelBar.SetValue(currentFuel / maxFuel);
    }

    void FixedUpdate()
    {
        flyInput = input.Player.Fly.ReadValue<float>();
        boostInput = input.Player.Boost.ReadValue<float>();

        if (dashTimer < dashDuration)
        {
            DashMove();
        }

        DashCooldown();

        if (flyInput > 0 && currentFuel > 0)
        {
            if (!isFlying)
            {
                isFlying = true;
            }
            else
            {
                currentFuel -= flyDrainRate * Time.deltaTime;
            }

            FlyMovement();

            if (flyPowerCurrent < flyPowerMax)
            {
                flyPowerCurrent += flyPowerIncrease * Time.fixedDeltaTime;
            }
            if (flyPowerCurrent > flyPowerMax)
            {
                flyPowerCurrent = flyPowerMax;
            }

        }
        else if (boostInput > 0 && currentFuel > 0)
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

            if (flyPowerCurrent < flyPowerMax)
            {
                flyPowerCurrent += flyPowerIncrease * Time.fixedDeltaTime;
            }
            if (flyPowerCurrent > flyPowerMax)
            {
                flyPowerCurrent = flyPowerMax;
            }

        }
        else
        {
            isFlying = false;
            flyPowerCurrent = 0;
            pm.isBoosting = false;

            if (flyPowerCurrent > 0)
            {
                flyPowerCurrent -= 7 * Time.fixedDeltaTime;
            }
            
            if (flyPowerCurrent < 0)
            {
                flyPowerCurrent = 0;
            }
        }
        reFuelDelayTimer += Time.fixedDeltaTime;
        ReFuel();
    }

    void ReFuel()
    {
        if (pm.isGrounded && currentFuel < maxFuel && reFuelDelayTimer >= reFuelDelay)
        {
            currentFuel += reFuelRate * Time.deltaTime;
        }
    }

    void DrainFuel(float fuelUsed)
    {
        currentFuel -= fuelUsed;
        reFuelDelayTimer = 0;
    }

    void FlyMovement()
    {
        if (rigid.velocity.y < 0)
        {
            rigid.velocity = new Vector3(rigid.velocity.x, flyPowerCurrent, rigid.velocity.z);
        }
        else
        {
            rigid.velocity = new Vector3(rigid.velocity.x, flyPowerCurrent, rigid.velocity.z);
        }
    }

    void BoostMovement()
    {
        //rigid.AddForce(cameraHolder.forward * boostPower, ForceMode.Force);
        rigid.velocity = cameraHolder.forward * boostPower;
    }

    void StartDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTimer >= dashCooldown && currentFuel >= dashFuelDrain)
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
            DrainFuel(dashFuelDrain);
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
