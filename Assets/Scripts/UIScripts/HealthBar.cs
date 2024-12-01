using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    static HealthBar inst;
	float health = 1.0f;
	public Image image;
	public float Health { get { return health; }set { health = value; image.fillAmount = value; if (value <= 0.0f) Hide(); } }
    public static HealthBar Inst
	{
		get { Debug.Assert(inst != null, "Health Bar not set"); return inst; }
	}
	private void Start()
	{
		Debug.Assert(inst == null, "Health bar already set");
		inst = this;
		gameObject.SetActive(false);
	}
	public void Show()
	{
		gameObject.SetActive(true);
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}
