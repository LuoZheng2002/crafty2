using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // The object to shoot.
    [SerializeField] private float launchForce = 10f;     // The initial force applied to the projectile.
    [SerializeField] private float y_axis_addition = 60f;
    [SerializeField] private float shootInterval = 5f;    // Time between each shot.
    [SerializeField] private Vector3[] targetPositions;   // Array of target positions.
    [SerializeField] float wait_time = 10f;

    private int currentTargetIndex = 0; // Keeps track of the current target position.

    void Start()
    {
        StartCoroutine(WaitToStart(wait_time));
    }

    // Call this method to shoot a projectile toward a target position.
    public void Shoot(Vector3 targetPosition)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab or Spawn Point is not assigned.");
            return;
        }

        Vector3 spawn_position = transform.position + new Vector3(0, y_axis_addition, 0);

        // Instantiate the projectile at the spawn point.
        GameObject projectile = Instantiate(projectilePrefab, spawn_position, Quaternion.identity);

        // Get the Rigidbody component to apply force.
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Projectile prefab must have a Rigidbody component.");
            Destroy(projectile); // Destroy the projectile if it doesn't have a Rigidbody.
            return;
        }

        // Calculate the direction toward the target position.
        Vector3 direction = (targetPosition - spawn_position).normalized;

        // Apply force to the projectile.
        rb.AddForce(direction * launchForce, ForceMode.Impulse);
    }

    // Coroutine to shoot projectiles at intervals.
    private IEnumerator ShootAtTargets()
    {
        while (true)
        {
            if (targetPositions.Length == 0)
            {
                Debug.LogWarning("No target positions defined.");
                yield break;
            }

            // Shoot at the current target position.
            Shoot(targetPositions[currentTargetIndex]);
            Debug.Log("Object shot");

            // Move to the next target position in the array.
            currentTargetIndex = (currentTargetIndex + 1) % targetPositions.Length;

            // Wait for the defined interval before shooting again.
            yield return new WaitForSeconds(shootInterval);
        }
    }

    IEnumerator WaitToStart(float wait_time)
    {
        yield return new WaitForSeconds(wait_time);

        // Start the coroutine to shoot projectiles periodically.
        StartCoroutine(ShootAtTargets());
    }
}