using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] Text timer_text;
    [SerializeField] float remaining_time;
    bool timer_start = false;
    float original_time;

    private void Start()
    {
        EventBus.Subscribe<ConfirmSuccessEvent>(OnConfirmClicked);
        original_time = remaining_time;
        int minutes = Mathf.FloorToInt(remaining_time / 60);
        int seconds = Mathf.FloorToInt(remaining_time % 60);
        timer_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnConfirmClicked(ConfirmSuccessEvent e)
    {
        timer_start = true;
        remaining_time = original_time;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer_start)
        {
            remaining_time -= Time.deltaTime;

            if (remaining_time > 0 && remaining_time < 5)
            {
                timer_text.color = Color.red;
            }
            else if (remaining_time <= 0)
            {
                remaining_time = 0;
                timer_start = false;
            }

            int minutes = Mathf.FloorToInt(remaining_time / 60);
            int seconds = Mathf.FloorToInt(remaining_time % 60);
            timer_text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
