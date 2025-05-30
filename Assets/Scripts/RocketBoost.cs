using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBoost : MonoBehaviour
{
    [Header("RocketBoost")]
    public float maxFuel = 100;
    public float flyPower;
    public float flyDrain;
    private float currentFuel;
    private float flyInput;

    [Header("References")]
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
        currentFuel = maxFuel;
    }

    void Update()
    {
        fuelBar.SetValue(currentFuel / maxFuel);
    }

    void FixedUpdate()
    {
        flyInput = input.Player.Boost.ReadValue<float>();
        if (flyInput > 0 && currentFuel > 0)
        {
            currentFuel -= flyDrain * Time.deltaTime;
            BoostMovement();
        }
    }

    void BoostMovement()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, flyPower, rigid.velocity.z);
        
    }

    void OnDisable()
    {
        input.Player.Boost.Disable();
    }
}
