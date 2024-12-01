using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public Transform tnt;
    public SphereCollider sphere_collider;
    public MeshRenderer mesh_renderer;
	public ParticleSystem particle_system;
	private void Start()
	{
		StartCoroutine(Explode());
	}
	IEnumerator Explode()
	{
		float start_time = Time.time;
		float duration = 2.5f;
		float start_y = tnt.localPosition.y;
		while (Time.time - start_time < duration)
		{
			Color color = mesh_renderer.material.color;
			color.a = Mathf.Lerp(0.0f, 1.0f, (Time.time - start_time) / duration);
			mesh_renderer.material.color = color;
			float y = Mathf.Lerp(start_y, 0.0f, (Time.time - start_time) / duration);
			tnt.localPosition = new Vector3(0, y, 0);
			yield return null;
		}
		sphere_collider.enabled = true;
		tnt.gameObject.SetActive(false);
		particle_system.Play();
		mesh_renderer.enabled = false;
		yield return new WaitForSeconds(0.5f);
		Destroy(gameObject);
	}
}
