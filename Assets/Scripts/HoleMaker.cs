using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleMaker : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody tnt;
    public SphereCollider sphere_collider;
    public ParticleSystem particle_system;
    public GameObject cylinder;

    void Start()
    {
        cylinder.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
		{
			Trigger();
		}
	}
    IEnumerator TriggerHelper()
    {
		tnt.gameObject.SetActive(true);
		tnt.AddForce(new Vector3(0, 0, 230.0f), ForceMode.Impulse);
        yield return new WaitForSeconds(2.0f);
        particle_system.Play();
        tnt.gameObject.SetActive(false);
        sphere_collider.enabled = true;
		yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);

	}
    bool triggered = false;
	public void Trigger()
    {
        if (!triggered)
        {
			triggered = true;
			StartCoroutine(TriggerHelper());
		}
	}
}
