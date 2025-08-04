using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask grappleSurface;
    public LineRenderer lr;
    public InputManager input;
    public PlayerMovement pm;
    public Rigidbody rigid;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float grappleCooldown;
    public float grapplePullSpeed;
    public float stopDistance;
    private float grappleTimer;
    private Vector3 grapplePoint;
    private bool grappling;
    private bool pulling;
    private Vector3 grappleDir;

    void Start()
    {
        input.playerActions.Grapple.Enable();
        input.playerActions.Grapple.performed += StartGrapple;

        pm = GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        if (grappleTimer > 0)
        {
            grappleTimer -= Time.fixedDeltaTime;
        }

        if (pulling)
        {
            GrapplePull();
        }
    }

    void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    void StartGrapple(InputAction.CallbackContext context)
    {
        if (grappleTimer! > 0)
        {
            return;
        }

        grappling = true;
        lr.enabled = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappleSurface))
        {
            grapplePoint = hit.point;
            pulling = true;
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            StopGrapple();
        }

        
        lr.SetPosition(1, grapplePoint);
    }

    void GrapplePull()
    {
        grappleDir = (grapplePoint - transform.position).normalized;
        rigid.AddForce(grappleDir * grapplePullSpeed, ForceMode.Force);
        if (Vector3.Distance(transform.position, grapplePoint) <= stopDistance)
        {
            StopGrapple();
        }

    }

    void StopGrapple()
    {
        grappling = false;
        grappleTimer = grappleCooldown;
        lr.enabled = false;
        pulling = false;
    }

    private void OnDisable()
    {

    }
}
