using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleBullet : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    public LayerMask layerMask;
    [HideInInspector]
    public Grappling grappling;

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        CheckHit();
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            grappling.StopGrapple();
            Destroy(this.gameObject);
        }
    }

    void CheckHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.forward * speed * Time.deltaTime, transform.forward, out hit, 0.12f, ~layerMask))
        {
            Debug.Log("Bullet: " + hit.point);
            grappling.GrappeHookHit(hit.point);
            Destroy(this.gameObject);
        }
    }
}
