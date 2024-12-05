using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleMakerTrigger : MonoBehaviour
{
	public HoleMaker hole_maker;
	private void OnTriggerEnter(Collider other)
	{
		hole_maker.Trigger();
	}
}
