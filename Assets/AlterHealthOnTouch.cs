using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterHealthOnTouch : MonoBehaviour
{
    [SerializeField] int alter_health;

    void OnCollisionEnter(Collision collision)
    {
        // should have check to see if collision was player

        print("Marmot Collision");

        // Check if the collided object has a Health component
        HasHealth target_health = gameObject.GetComponent<HasHealth>();
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (target_health != null && rb != null)
        {
            target_health.AlterHealth(CalculateDamage(rb.velocity.magnitude)); // Alter the object's health
        }
        else
        {
            print("Null");
        }
    }
    float CalculateDamage(float speed)
    {
        float k = 0.2f; // Scaling factor for speed's influence
        float p = 1.3f; // Power factor

        float final_damage = alter_health * (1 + k * Mathf.Pow(speed, p));
        return final_damage;
    }
}
