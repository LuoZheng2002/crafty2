using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSink : MonoBehaviour
{
    static LavaSink inst;
    public static LavaSink Inst
    {
        get { return inst; }
    }
	private void Start()
	{
        inst = this;
	}
    IEnumerator SinkHelper()
    {
        float start_time = Time.time;
        while(Time.time - start_time < 2.0f)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y - 5.0f*Time.deltaTime, transform.position.z);
            yield return null;
		}
		Destroy(gameObject);
	}
	private void OnDestroy()
	{
		inst = null;
	}
	public void Sink()
    {
        StartCoroutine(SinkHelper());
	}
}
