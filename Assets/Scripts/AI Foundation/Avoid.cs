using UnityEngine;

namespace AI_Foundation
{
    public class Avoid : AIMovement
    {
        public override SteeringOutput GetSteering(IndividualAI agent)
        {
            SteeringOutput output = base.GetSteering(agent);
            
            float avoidDistanceSquared = agent.avoidanceRadius * agent.avoidanceRadius;
            
            Vector3 force = Vector3.zero;
            foreach (Transform obstacle in agent.obstacles)
            {
                Collider obstacleCollider = obstacle.GetComponent<Collider>();
                
                if (obstacleCollider == null) {
                    Debug.LogWarning("No collider");
                    continue; 
                }
                
                Vector3 closestPoint = obstacleCollider.ClosestPoint(agent.transform.position);
                Vector3 directionToObstacle = closestPoint - agent.transform.position;

                if (directionToObstacle.sqrMagnitude < avoidDistanceSquared)
                {
                    Vector3 perpendicularDirection = Vector3.Cross(directionToObstacle, Vector3.up).normalized;
                    if (Vector3.Dot(perpendicularDirection, agent.transform.forward) < 0) {
                        perpendicularDirection *= -1;
                    }

                    if (directionToObstacle.sqrMagnitude < 2f)
                    {
                        perpendicularDirection *= 10;
                    }
                    
                    force += perpendicularDirection / (directionToObstacle.sqrMagnitude + 0.01f) * agent.speed;
                }
            }

            output.linear = force - agent.Velocity;
            
            return output;
        }
    }
}
