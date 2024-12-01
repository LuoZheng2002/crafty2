using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMotor : MonoBehaviour
{
	WheelCollider wheelCollider;
	public float torque_factor = 5.0f;
	Rigidbody rb;
	// Start is called before the first frame update
	void Start()
    {
		wheelCollider = GetComponent<WheelCollider>();
		rb = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
    {
		float v_input = Input.GetAxis("Vertical");  // "Vertical" corresponds to W/S or Up/Down keys

		// Apply motor torque based on input
		float torque = v_input * torque_factor / (1.0f + CarCore.Inst.RB.velocity.magnitude / 5.0f);  // Adjust multiplier for more/less power
		wheelCollider.motorTorque = torque;
	}
}
