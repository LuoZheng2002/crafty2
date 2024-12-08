using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	static Door inst;
	public static Door Inst
	{
		get
		{
			Debug.Assert(inst != null);
			return inst;
		}
	}
	Vector3 start_pos;
	Vector3 end_pos;
	public Transform start;
	public Transform end;
	private void Start()
	{
		Debug.Assert(inst == null);
		inst = this;
		start_pos = start.position;
		end_pos = end.position;
		EventBus.Subscribe<WallWarpFinishEvent>(OnWallWarpFinish);
		EventBus.Subscribe<WallRetryEvent>(OnWallRetry);
	}
	void OnWallRetry(WallRetryEvent e)
	{
		StartCoroutine(Close());
	}
	void OnWallWarpFinish(WallWarpFinishEvent e)
	{
		StartCoroutine(Open());
	}
	public IEnumerator Open()
	{
		LineCanvas.Bottom.DisplayLineAsync("Shirley", "It's time for the ultimate combat!", 1.0f, Util.VoiceLine.ultimate);
		float start_time = Time.time;
		while (Time.time - start_time < 1.0f)
		{
			transform.position = Vector3.Lerp(start_pos, end_pos, (Time.time - start_time) / 1.0f);
			yield return null;
		}
	}
	public IEnumerator Close()
	{
		float start_time = Time.time;
		while (Time.time - start_time < 1.0f)
		{
			transform.position = Vector3.Lerp(end_pos, start_pos, (Time.time - start_time) / 1.0f);
			yield return null;
		}
	}
}
