using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Test2 : MonoBehaviour
{
    public Transform cameraHolder;
    public Transform bulletSpawn;
    public float fireDelay;
    public float maxRange;
    public float bulletSpead = 0.1f;
    public LayerMask mask;

    private float timer;
    private float lastShotTime;
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
        //GameObject current = Instantiate(bullet, bulletSpawn.position, cameraHolder.rotation);
        //RaycastHit hit = Physics.Raycast(bulletSpawn.position, cameraHolder.forward, maxRange);
        if (Physics.Raycast(bulletSpawn.position, cameraHolder.forward + new Vector3(0.4f, 0), out RaycastHit hit, maxRange, mask))
        {
            Debug.DrawLine(bulletSpawn.position, hit.point, Color.green, 3);
            Debug.Log(hit);
        }
    }
}
