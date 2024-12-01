using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLoader : MonoBehaviour
{
	public GameObject play_canvas;
	public GameObject build_canvas;
	public GameObject obtain_canvas;
	public GameObject story_canvas;
	public GameObject map_canvas;
	public GameObject line_canvas_top;
	public GameObject line_canvas_bottom;
	public GameObject chapter_canvas;
	private void Start()
	{
		play_canvas.SetActive(true);
		build_canvas.SetActive(true);
		obtain_canvas.SetActive(true);
		story_canvas.SetActive(true);
		map_canvas.SetActive(true);
		line_canvas_top.SetActive(true);
		line_canvas_bottom.SetActive(true);
		chapter_canvas.SetActive(true);
	}
}
