using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterOnTouch : MonoBehaviour
{
	public GameObject fragments;
	[SerializeField] AudioClip rubble_audio_clip;
	private void OnCollisionEnter(Collision collision)
	{

		gameObject.SetActive(false);
		fragments.SetActive(true);

		GameObject mainCamera = Camera.main.gameObject;
        AudioSource.PlayClipAtPoint(rubble_audio_clip, mainCamera.transform.position);
    }
}
