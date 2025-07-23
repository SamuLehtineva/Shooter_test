using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBullet : MonoBehaviour
{
    public float speed = 10;
    public float damage = 10;
    public float lifeTime = 5;

    private float timer = 0f;
    private Vector3 lastPos;
    private int layerMask = (1 << 7) | (1 << 8);

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        CheckHit();
    }

    void FixedUpdate()
    {
        //transform.position += transform.forward * speed * Time.fixedDeltaTime;

        timer += Time.fixedDeltaTime;
        if (timer > lifeTime)
        {
            Destroy(this.gameObject);
        }
        //CheckHit();
    }

    void CheckHit()
    {
        RaycastHit hit;

        //if (Physics.Raycast(transform.position, transform.forward, out hit, 0.15f))
        if (Physics.Raycast(transform.position + transform.forward * speed * Time.fixedDeltaTime, transform.forward, out hit, 0.12f, ~layerMask))
        {
            Destroy(this.gameObject);
        }
    }
}
