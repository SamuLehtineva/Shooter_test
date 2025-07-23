using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public List<GameObject> guns;
    public int currentGun = 0;
    public InputManager input;
    private float scrollInput;
    private GameObject currentGunObj;

    void Start()
    {
        input.playerActions.NextWeapon.Enable();

        currentGunObj = guns[currentGun];
        currentGunObj.SetActive(true);
    }

    void Update()
    {
        ChangeGun();
    }

    void ChangeGun()
    {
        guns[currentGun].SetActive(false);

        scrollInput = input.playerActions.NextWeapon.ReadValue<Vector2>().y;

        if (scrollInput > 0)
        {
            currentGun++;
            if (currentGun > guns.Capacity - 1)
            {
                currentGun = 0;
            }
        }
        else if (scrollInput < 0)
        {
            currentGun--;
            if (currentGun < 0)
            {
                currentGun = guns.Capacity - 1;
            }
        }

        guns[currentGun].SetActive(true);
    }
}
