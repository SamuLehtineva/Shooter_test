using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBullet : MonoBehaviour
{
    public float speed = 10;
    public float damage = 10;
    public float lifeTime = 10;

    private float timer = 0f;

    void Awake()
    {
        //GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer > lifeTime)
        {
            Destroy(this);
        }

        GetComponent<Rigidbody>().AddForce(transform.forward * speed);
    }
}
