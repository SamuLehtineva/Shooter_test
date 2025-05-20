using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rigid;
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;

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

        startYScale = playerObj.localScale.y;
    }

    void Update()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        if (sliding)
        {
            SlidingMovement();
        }
    }

    void StartSlide(InputAction.CallbackContext context)
    {
        Debug.Log("Sliding");
        sliding = true;

        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rigid.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        
        slideTimer = maxSlideTime;
    }

    void SlidingMovement()
    {
        Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        
        rigid.AddForce(moveDirection.normalized * slideForce, ForceMode.Force);

        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0)
        {
            StopSlide(new InputAction.CallbackContext());
        }
    }

    void StopSlide(InputAction.CallbackContext context)
    {
        Debug.Log("Stopped sliding");
        sliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);
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
