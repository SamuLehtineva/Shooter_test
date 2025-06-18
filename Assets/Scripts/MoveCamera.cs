using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform target;
    public Transform cameraCopy;
    void Update()
    {
        transform.position = target.position;
        cameraCopy.rotation = transform.rotation;
    }
}
