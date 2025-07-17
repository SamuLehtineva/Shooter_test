using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_1 : MonoBehaviour
{
    public GameObject bullet;
    public Transform cameraHolder;
    public float fireDelay;

    private float timer;
    private PlayerInput input;
    private PlayerMovement pm;

    void Start()
    {
        pm = GetComponentInParent<PlayerMovement>();

        input = pm.playerInput;
        input.Player.PrimaryFire.Enable();

        timer = fireDelay;
    }

    void FixedUpdate()
    {
        if (timer < fireDelay)
        {
            timer += Time.fixedDeltaTime;
        }

        if (timer >= fireDelay && input.Player.PrimaryFire.ReadValue<float>() > 0)
        {
            timer = 0;
            PrimaryFire();
        }
    }
    void PrimaryFire()
    {
        GameObject current = Instantiate(bullet, transform.position, cameraHolder.rotation);
    }
}
