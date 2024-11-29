using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f; // Speed at which the object moves forward.

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }
}