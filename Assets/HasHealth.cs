using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasHealth : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int max_health = 500;
    [SerializeField] int health;

    void Start()
    {
        health = max_health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetHealth()
    {
        return health;
    }

    public void AlterHealth(float amount)
    {
        health = Mathf.Clamp(health + (int)amount, 0, max_health);

        if (health <= 0)
        {
            Die();
        }

        EventBus.Publish<AlterHealthEvent>(new AlterHealthEvent(health, max_health));
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}

public class AlterHealthEvent
{
    public int current_health;
    public int max_health;

    public AlterHealthEvent(int _health, int _max_health) { current_health -= _health; max_health = _max_health; }
}


