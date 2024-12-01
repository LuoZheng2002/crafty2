using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WallRetryEvent
{

}
public class WallWarpFinishEvent
{

}
public class Walls : MonoBehaviour
{
	static Walls inst;
	public static Walls Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}

	public bool TryingWall { get; set; } = false;

	Coroutine coroutine;
	Vector3 startPos;
	Vector3 endPos;
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		startPos = start.position;
		endPos = end.position;
		EventBus.Subscribe<WallRetryEvent>(OnWallRetry);
		
	}
	public Transform start;
	public Transform end;
	public void StartWarp()
	{
		coroutine = StartCoroutine(WarpHelper());
	}
	public float warp_duration = 20.0f;
	IEnumerator WarpHelper()
	{
		float start_time = Time.time;
		while(Time.time - start_time < warp_duration)
		{
			transform.position = Vector3.Lerp(startPos, endPos, (Time.time - start_time) / warp_duration);
			yield return null;
		}
		EventBus.Publish(new WallWarpFinishEvent());
	}
	public void WarpImmediately()
	{
		transform.position = endPos;
	}
	void OnWallRetry(WallRetryEvent e)
	{
		if (coroutine != null)
		{
			StopCoroutine(coroutine);
			transform.position = startPos;
		}
		GameState.Inst.UpdateRetryAndGoal(Util.WaypointName.CaveRoom2, Util.WaypointName.CaveTrap);
	}
}
