using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayCanvas : MonoBehaviour
{
    static PlayCanvas inst;
    public GameObject rocket;
	public GameObject umbrella;

    public Image death_image;
    public Text death_text;
    public float RocketFuel { get; set; } = 1;
    public float rocket_consumption_speed = 0.2f;
    public ButtonScale button_scale;
    public bool AutoFill { get; set; } = false;
    Subscription<RetryEvent> retry_subscription;
    int death_count = 0;
	public void ShowAndResetDeathCount()
    {
        death_image.gameObject.SetActive(true);
        death_count = 0;
        death_text.text = "0";
        retry_subscription = EventBus.Subscribe<RetryEvent>(OnRetry);
	}
    void OnRetry(RetryEvent e)
    {
        death_count++;
        death_text.text = death_count.ToString();
	}
    public void HideDeathCount()
    {
        death_image.gameObject.SetActive(false);
        EventBus.Unsubscribe(retry_subscription);
	}
    public void ScaleStory()
    {
        button_scale.ScaleStart();
        Debug.Log("Scale start called");
    }
	public static PlayCanvas Inst
    {
        get { Debug.Assert(inst != null, "Play Canvas not set"); return inst; }
    }
	private void Start()
	{
        Debug.Assert(inst == null, "Play canvas already set");
        inst = this;
        Util.Delay(this, () =>
        {
            gameObject.SetActive(false);
        });
	}
	private void OnEnable()
	{
		RocketFuel = 1;
		SetRocketFill(RocketFuel);
	}
	public void ShowRocket()
    {
        rocket.SetActive(true);
	}
    public void HideRocket()
    {
        rocket.SetActive(false);
	}
    public Image rocket_fuel_image;
    public void SetRocketFill(float amount)
    {
        // Debug.Log($"Rocket fill amount: {amount}");
		rocket_fuel_image.fillAmount = amount;
	}
    public void ShowUmbrella()
    {
        umbrella.SetActive(true);
	}
    public void HideUmbrella()
    {
        umbrella.SetActive(false);
	}
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
	private void Update()
	{
		if (Input.GetMouseButton(1))
        {
			RocketFuel -= rocket_consumption_speed * Time.deltaTime;
			SetRocketFill(RocketFuel);

		}
        if (AutoFill && RocketFuel < 1.0f)
        {
            RocketFuel += rocket_consumption_speed*1.0f/3.0f * Time.deltaTime;
            SetRocketFill(RocketFuel);
		}
	}
	public void OnStoryClicked()
	{
        StoryCanvas.Inst.Show();
        button_scale.ScaleStop();
		Debug.Log("Scale stop called");
	}
    public GameObject story;
    public void ShowStory()
    {
        story.SetActive(true);
    }
    public Text story_text;
    public void SetStoryText(string text)
    {
        story_text.text = text;
	}
	public void HideStory()
	{
		story.SetActive(false);
	}
}
