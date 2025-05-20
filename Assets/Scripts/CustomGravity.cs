using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    public float gravityScale = 1.0f;
    public static float globalGravity = -25f;

    private Rigidbody rigid;

    void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.useGravity = false;
    }

    void FixedUpdate()
    {
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rigid.AddForce(gravity, ForceMode.Acceleration);
    }
}
