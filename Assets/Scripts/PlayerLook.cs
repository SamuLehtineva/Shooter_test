using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerLook : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;
    public Transform camHolder;

    float xRotation;
    float yRotation;

    private PlayerInput playerLook;

    void Awake()
    {
        playerLook = new PlayerInput();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
    }

    void Look()
    {
        Vector2 lookDirection = playerLook.Player.Look.ReadValue<Vector2>();

        float mouseX = lookDirection.x * Time.fixedDeltaTime * sensX;
        float mouseY = lookDirection.y * Time.fixedDeltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
    
    public Vector3 GetTarget()
	{
        RaycastHit hit;
        int layerMask = (1 << 3) | (1 << 7);
        Physics.Raycast(transform.position, transform.forward, out hit, 150f, ~layerMask);
        Debug.DrawRay(transform.position, transform.forward, Color.red, 10f);
        return hit.point;
	}

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.3f);
    }

    private void OnEnable()
    {
        playerLook.Player.Look.Enable();
    }

    private void OnDisable()
    {
       playerLook.Player.Look.Disable(); 
    }
}
