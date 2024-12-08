using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunawaySidequest : MonoBehaviour
{
    public GameObject collection_prefab;
    GameObject current_collection;
	Subscription<RetryEvent> retry_event;
	private void OnEnable()
	{
		retry_event = EventBus.Subscribe<RetryEvent>(ResetLevel);
		ResetLevel(null);
	}
	private void OnDisable()
	{
		EventBus.Unsubscribe<RetryEvent>(retry_event);
	}
	public void ResetLevel(RetryEvent e)
    {
        if (current_collection != null)
        {
			Destroy(current_collection);
		}
		current_collection = Instantiate(collection_prefab, transform);
	}
}
