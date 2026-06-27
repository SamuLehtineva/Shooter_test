using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    private Rigidbody rigid;
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private Vector3 slideDirection;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    private bool sliding;

    public float slideYScale;
    private float startYScale;
    private Vector2 moveInput;
    private float slideInput;

    void Awake()
    {
        playerInput = new PlayerInput();
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = transform.localScale.y;
    }

    void FixedUpdate()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
        if (sliding)
        {
            SlidingMovement();
        }
    }

    void StartSlide(InputAction.CallbackContext context)
    {
        Debug.Log("Started Sliding");
        sliding = true;

        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
        rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        slideTimer = maxSlideTime;
        slideDirection = playerMovement.moveDirection;
        
    }

    void SlidingMovement()
    {   
        if(!playerMovement.OnSlope() || rigid.velocity.y > -0.1f)
        {
            rigid.AddForce(slideDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }
        else
        {
            rigid.AddForce(playerMovement.GetSlopeMoveDirection() * slideForce * 1.5f, ForceMode.Force);

            if (playerMovement.GetSlopeMoveDirection().y < 0.1f)
            {
                rigid.AddForce(Vector3.down * 200f, ForceMode.Force);
            }
        }
        
        if (slideTimer <= 0)
        {
            StopSlide(new InputAction.CallbackContext());
        }
    }

    void StopSlide(InputAction.CallbackContext context)
    {
        if (sliding)
        {
            Debug.Log("Stopped sliding");
            sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }


    void OnEnable()
    {
        playerInput.Player.Slide.Enable();
        playerInput.Player.Slide.started += StartSlide;
        playerInput.Player.Slide.canceled += StopSlide;
    }

    void OnDisable()
    {
        playerInput.Player.Slide.Disable();
        playerInput.Player.Slide.started -= StartSlide;
        playerInput.Player.Slide.canceled -= StopSlide;
    }

}
