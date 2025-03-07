using UnityEngine;

public class Avoid : AIMovement
{
    public override SteeringOutput GetSteering(AIAgent agent)
    {
        SteeringOutput output = new SteeringOutput();

        Vector3 ray = agent.Velocity.normalized * agent.lookAhead;
        
        Collision collision = agent.detector.GetCollision(agent.transform.position, ray);

        if (collision != null)
        {
            Vector3 avoidanceTarget = collision.Position + collision.Normal * agent.avoidDistance;
            
            Vector3 futurePosition = avoidanceTarget;

            Vector3 desiredVelocity = futurePosition - agent.transform.position;
            desiredVelocity = desiredVelocity.normalized * agent.speed;

            output.Linear = desiredVelocity - agent.Velocity;
        }
        
        return output;
    }
}