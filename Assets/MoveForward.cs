using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f; // Speed at which the object moves forward.
    [SerializeField] float wait_time = 1.5f;
    private bool canMove = false; // Flag to control movement.

    private void Start()
    {
        EventBus.Subscribe<TrackStartEvent>(OnConfirmClicked);
    }
    void OnConfirmClicked(TrackStartEvent e)
    {
		canMove = true;
	}

    void Update()
    {
        if (canMove)
        {
            transform.position += Vector3.forward * speed * Time.deltaTime;
        }
    }
}