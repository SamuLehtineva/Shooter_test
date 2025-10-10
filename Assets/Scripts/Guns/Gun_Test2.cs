using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_Test2 : MonoBehaviour
{
    public Transform cameraHolder;
    public Transform bulletSpawn;
    public float fireDelay;
    public float maxRange;
    public float bulletSpread = 0.5f;
    public int bulletCount = 5;
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
        int count = 0;
        while (count < bulletCount)
        {
            if (Physics.Raycast(bulletSpawn.position, GetRandomDirection(), out RaycastHit hit, maxRange, mask))
            {
                Debug.DrawLine(bulletSpawn.position, hit.point, Color.green, 3);
            }
            count++;
        }

        
    }

    Vector3 GetRandomDirection()
    {
        Vector3 point = Random.insideUnitCircle * bulletSpread;
        point.z = 1;
        Vector3 direction = (point - Vector3.zero).normalized;
        direction = cameraHolder.rotation * direction;
        Debug.Log(direction);
        return direction;
    }
}
