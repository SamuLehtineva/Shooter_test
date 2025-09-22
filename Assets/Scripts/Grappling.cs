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
    public GameObject grappleHook;
    public PlayerLook playerLook;
    private GameObject current;

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
            if (current != null)
            {
                lr.SetPosition(1, current.transform.position);
            }
        }
    }

    void StartGrapple(InputAction.CallbackContext context)
    {
        if (grappleTimer > 0)
        {
            return;
        }

        if (grappling)
        {
            StopGrapple();
        }
        else
        {
            grappling = true;
            lr.enabled = true;

            current = Instantiate(grappleHook, gunTip.position, cam.rotation);
            current.transform.LookAt(playerLook.GetTarget());
            current.GetComponent<GrappleBullet>().grappling = this;
        }

    }

    public void GrappeHookHit(Vector3 hitPoint)
    {
        Debug.Log("HIt");
        grapplePoint = hitPoint;
        pulling = true;
        rigid.velocity = Vector3.zero;
        rigid.AddForce(grappleDir * grapplePullSpeed, ForceMode.Force);
        pm.isGrappling = true;
    }

    void GrapplePull()
    {
        grappleDir = (grapplePoint - transform.position).normalized;
        rigid.AddForce(grappleDir * grapplePullSpeed, ForceMode.Force);
        Debug.Log(rigid.velocity);
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
        pm.isGrappling = false;
    }

    private void OnDisable()
    {
        input.playerActions.Grapple.performed -= StartGrapple;
        input.playerActions.Grapple.Disable();
    }
}
