using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBoost : MonoBehaviour
{
    [Header("RocketBoost")]
    public float maxFuel = 100;
    private float flyPowerCurrent;
    public float flyPowerMax;
    public float flyPowerIncrease;
    public float flyDrainRate;
    public float boostPower;
    public float boostDrainRate;
    public float reFuelRate;
    private float currentFuel;
    private float flyInput;
    private float boostInput;
    private bool isDoing = false;

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

        currentFuel = maxFuel;
    }

    void Update()
    {
        fuelBar.SetValue(currentFuel / maxFuel);
    }

    void FixedUpdate()
    {
        flyInput = input.Player.Fly.ReadValue<float>();
        boostInput = input.Player.Boost.ReadValue<float>();

        if (flyInput > 0 && currentFuel > 0)
        {
            if (!isDoing)
            {
                isDoing = true;
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
            if (!isDoing)
            {
                currentFuel -= 10;
                isDoing = true;
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
            isDoing = false;
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

        ReFuel();
    }

    void ReFuel()
    {
        if (pm.isGrounded && currentFuel < maxFuel)
        {
            currentFuel += reFuelRate * Time.deltaTime;
        }
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

    void OnDisable()
    {
        input.Player.Fly.Disable();
        input.Player.Boost.Disable();
    }
}
