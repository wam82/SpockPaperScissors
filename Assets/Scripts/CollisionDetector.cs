using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [Header("Collision Settings")]
    [Tooltip("Configure which layers to check against. For example, obstacles and (if needed) friendly characters.")]
    public LayerMask collisionMask;
    
    public PotentialCollision GetCollision(Vector3 position, Vector3 moveAmount)
    {
        // Calculate the direction (normalized) and the distance to check
        Vector3 direction = moveAmount.normalized;
        float distance = moveAmount.magnitude;

        RaycastHit hit;
        // Perform a raycast using the provided collision mask.
        if (Physics.Raycast(position, direction, out hit, distance, collisionMask))
        {
            // (Optional) Additional filtering based on tag can be done here if needed.
            // For example, if blue players should avoid objects tagged "Blue" or "Obstacle":
            // if (hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Blue"))
            // {
            //     return new Collision(hit.point, hit.normal);
            // }

            Debug.Log("Hit");
            return new PotentialCollision(hit.point, hit.normal);
        }

        return null;
    }
}
