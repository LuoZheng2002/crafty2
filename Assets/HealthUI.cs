using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    Subscription<AlterHealthEvent> health_event_subscription;
    RectTransform health_bar_rect;
    float health_bar_x_scale;

    void Start()
    {
        health_event_subscription = EventBus.Subscribe<AlterHealthEvent>(_OnHealthUpdated);
        health_bar_rect = GetComponent<RectTransform>();
        health_bar_x_scale = health_bar_rect.localScale.x;
    }

    void _OnHealthUpdated(AlterHealthEvent e)
    {
        AlterHealthBar(e.current_health, e.max_health);
    }
    public void AlterHealthBar(int health, int max_health)
    {
        // Update the scale of the health bar
        // Not sure why x scale becomes negative, negating it temporarily
        health_bar_rect.localScale = new Vector3(((float)health / max_health) * health_bar_x_scale * -1, health_bar_rect.localScale.y, health_bar_rect.localScale.z);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(health_event_subscription);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AlterHealthBar(Random.Range(0, 500), 500); // Test with random health values
        }
    }

}
