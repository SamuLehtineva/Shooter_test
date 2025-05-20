using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RocketBoost : MonoBehaviour
{
    [Header("RocketBoost")]
    public float boostPower;
    private float boostInput;

    [Header("References")]
    private Rigidbody rigid;
    private PlayerMovement pm;
    private PlayerInput input;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        input = pm.playerInput;
        input.Player.Boost.Enable();
    }

    void Update()
    {
        boostInput = input.Player.Boost.ReadValue<float>();
        if (boostInput > 0)
        {
            BoostMovement();
        }
    }

    void BoostMovement()
    {
        rigid.velocity = new Vector3(rigid.velocity.x, boostPower, rigid.velocity.z);
    }

    void OnDisable()
    {
        input.Player.Boost.Disable();
    }
}
