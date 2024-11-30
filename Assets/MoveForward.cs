using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f; // Speed at which the object moves forward.
    [SerializeField] float wait_time = 10f;
    private bool canMove = false; // Flag to control movement.

    private void Start()
    {
        StartCoroutine(WaitToStart(wait_time));
    }

    void Update()
    {
        if (canMove)
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
    }

    IEnumerator WaitToStart(float wait_time)
    {
        yield return new WaitForSeconds(wait_time);
        canMove = true;
    }
}