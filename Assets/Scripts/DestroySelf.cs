using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public GameObject post_process_volume;
    public void DestroyS()
    {
        Destroy(gameObject);
	}
    public void ShowPostProcess()
    {
        post_process_volume.SetActive(true);
	}
	public void HidePostProcess()
	{
		post_process_volume.SetActive(false);
	}
}
