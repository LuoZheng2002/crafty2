using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnTouch : MonoBehaviour
{
	public GameObject fragments;
	private void OnCollisionEnter(Collision collision)
	{
		gameObject.SetActive(false);
		fragments.SetActive(true);
	}
}
