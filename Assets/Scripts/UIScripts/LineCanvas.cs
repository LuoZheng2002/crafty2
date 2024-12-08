using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using static Util;

public class LineCanvas : MonoBehaviour
{
	public int frames_per_char = 2;
	static LineCanvas topCanvas;
	static LineCanvas bottomCanvas;
	public bool top;
	public static LineCanvas Top
	{
		get { Debug.Assert(topCanvas != null, "Top Canvas not set"); return topCanvas; }
	}
	public static LineCanvas Bottom
	{
		get { Debug.Assert(bottomCanvas != null, "Bottom Canvas not set"); return bottomCanvas; }
	}
	Text line;
	Text n;
	ButtonScale buttonScale;
	private void Start()
	{
		if (top)
		{
			Debug.Assert(topCanvas == null, "Top canvas already set");
			topCanvas = this;
		}
		else
		{
			Debug.Assert(bottomCanvas == null, "Bottom canvas already set");
			bottomCanvas = this;
		}
		line = transform.Find("Panel").Find("Line").GetComponent<Text>();
		n = transform.Find("Panel").Find("Name").GetComponent<Text>();
		buttonScale = transform.Find("Panel").Find("Continue").GetComponent<ButtonScale>();
		Debug.Assert(line != null);
		Debug.Assert(n != null);
		Debug.Assert(buttonScale != null);
		buttonScale.gameObject.SetActive(false);
		Hide();
	}
	private void OnDestroy()
	{
		topCanvas = null;
	}
	public IEnumerator DisplayLine(string name_str, string line_str, Character talking_char, Util.VoiceLine voice_line)
	{
		if (talking_char != null)
		{
			talking_char.StartTalking();
		}
		VA.Inst.PlayAudio(voice_line);
		gameObject.SetActive(true);
		n.text = name_str;
		line.text = "";
		int frame_index = 0;
		int char_index = 0;
		while (char_index < line_str.Length)
		{
			if (frame_index == 0)
			{
				line.text += line_str[char_index++];
			}
			frame_index = (frame_index + 1) % frames_per_char;
			yield return null;
		}
		if (talking_char != null)
		{
			talking_char.StopTalking();
		}
		line.text = line_str;
	}
	IEnumerator AsyncHelper(string name_str, string line_str, float seconds, Util.VoiceLine voice_line)
	{
		yield return DisplayLine(name_str, line_str, null, voice_line);
		yield return new WaitForSeconds(seconds);
		Hide();
	}
	public void DisplayLineAsync(string name_str, string line_str, float seconds, Util.VoiceLine voice_line)
	{
		gameObject.SetActive(true);		
		StartCoroutine(AsyncHelper(name_str, line_str, seconds, voice_line));
	}
	public IEnumerator DisplayLineAndWaitForClick(string name_str, string line_str, Character talking_char, Util.VoiceLine voice_line)
	{
		gameObject.SetActive(true);
		VA.Inst.PlayAudio(voice_line);
		n.text = name_str;
		line.text = "";
		if (talking_char != null)
		{
			talking_char.StartTalking();
		}
		int frame_index = 0;
		int char_index = 0;
		while(char_index < line_str.Length)
		{
			if (Input.GetMouseButtonDown(0))
			{
				break;
			}
			if (frame_index == 0)
			{
				line.text += line_str[char_index++];
			}
			frame_index = (frame_index + 1)%frames_per_char;
			yield return null;
		}
		ShowContinue();
		line.text = line_str;
		if (talking_char != null)
		{
			talking_char.StopTalking();
		}
		yield return null;
		while(!Input.GetMouseButtonDown(0))
		{
			yield return null;
		}
		HideContinue();
		yield return null;
	}

	public IEnumerator DisplayLineAndWaitForEvent<Event_>(string name_str, string line_str, Func<Event_, bool> handler, Util.VoiceLine voice_line)
	{
		gameObject.SetActive(true);
		VA.Inst.PlayAudio(voice_line);
		n.text = name_str;
		line.text = "";
		int frame_index = 0;
		int char_index = 0;
		bool criteria_met = false;
		var subscription = EventBus.Subscribe<Event_>((Event_ e) =>
		{
			if (handler(e))
			{
				criteria_met = true;
			}
		});
		while (char_index < line_str.Length)
		{
			if (criteria_met || Input.GetMouseButtonDown(0))
			{
				break;
			}
			if (frame_index == 0)
			{
				line.text += line_str[char_index++];
			}
			frame_index = (frame_index + 1) % frames_per_char;
			yield return null;
		}
		line.text = line_str;
		while (!criteria_met)
		{
			yield return null;
		}
		EventBus.Unsubscribe(subscription);
	}
	public IEnumerator WaitForEvent<Event_>(Func<Event_, bool> handler)
	{
		bool criteria_met = false;
		var subscription = EventBus.Subscribe<Event_>((Event_ e) =>
		{
			if (handler(e))
			{
				criteria_met = true;
			}
		});
		while (!criteria_met)
		{
			yield return null;
		}
		EventBus.Unsubscribe(subscription);
	}
		public void Hide()
	{
		gameObject.SetActive(false);
	}
	public void ShowContinue()
	{
		buttonScale.gameObject.SetActive(true);
		buttonScale.ScaleStart();
	}
	public void HideContinue()
	{
		buttonScale.gameObject.SetActive(false);
	}
}
