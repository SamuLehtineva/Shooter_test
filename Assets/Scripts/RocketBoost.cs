using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBoost : MonoBehaviour
{
    [Header("RocketBoost")]
    public float maxFuel = 100;
    public float flyPower;
    public float flyDrainRate;
    public float boostPower;
    public float boostDrainRate;
    public float reFuelRate;
    private float currentFuel;
    private float flyInput;
    private float boostInput;

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
            currentFuel -= flyDrainRate * Time.deltaTime;
            FlyMovement();
        }
        else if (boostInput > 0 && currentFuel > 0)
        {
            currentFuel -= boostDrainRate * Time.deltaTime;
            pm.isBoosting = true;
            BoostMovement();
        }
        else
        {
            pm.isBoosting = false;
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
        rigid.velocity = new Vector3(rigid.velocity.x, flyPower, rigid.velocity.z);
        
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
