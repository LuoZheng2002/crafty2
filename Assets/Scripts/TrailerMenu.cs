using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrailerMenu : MonoBehaviour
{
    public void BlackIn()
    {
        BlackoutCanvas.Inst.BlackoutAsync(0.5f, 1.0f, 0.0f);
    }
	public void BlackOut()
	{
		BlackoutCanvas.Inst.BlackoutAsync(0.5f, 0.0f, 1.0f);
	}
	public Image title;
	public Text text1;
	public Text text2;
	public void ShowTitle()
	{
		StartCoroutine(ShowTitleHelper());
	}
	IEnumerator ShowTitleHelper()
	{
		float duration = 1.0f;
		float start_time = Time.time;
		Color color;
		while(Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			color = title.color;
			color.a = t;
			title.color = color;
			yield return null;
		}
		color = title.color;
		color.a = 1;
		title.color = color;
	}
	public void ShowText1()
	{
		StartCoroutine(ShowText1Helper());
	}
	IEnumerator ShowText1Helper()
	{
		float duration = 1.0f;
		float start_time = Time.time;
		Color color;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			color = text1.color;
			color.a = t;
			text1.color = color;
			yield return null;
		}
		color = text1.color;
		color.a = 1;
		text1.color = color;
	}
	public void ShowText2()
	{
		StartCoroutine(ShowText2Helper());
	}
	IEnumerator ShowText2Helper()
	{
		float duration = 1.0f;
		float start_time = Time.time;
		Color color;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			color = text2.color;
			color.a = t;
			text2.color = color;
			yield return null;
		}
		color = text2.color;
		color.a = 1;
		text2.color = color;
	}
	public void HideAll()
	{
		StartCoroutine(HideAllHelper());
	}
	IEnumerator HideAllHelper()
	{
		float duration = 1.0f;
		float start_time = Time.time;
		Color color;
		while (Time.time - start_time < duration)
		{
			float t = (Time.time - start_time) / duration;
			color = text1.color;
			color.a = 1-t;
			text1.color = color;
			color = text2.color;
			color.a = 1-t;
			text2.color = color;
			color = title.color;
			color.a = 1 - t;
			title.color = color;
			yield return null;
		}
	}
}
