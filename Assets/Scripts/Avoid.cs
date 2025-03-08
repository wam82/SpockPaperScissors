using UnityEngine;

public class Avoid : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = base.GetSteering(agent);

        Vector3 ray = agent.Velocity.normalized * agent.lookAhead;
        
        Collision collision = agent.detector.GetCollision(agent.transform.position, ray);

        if (collision != null)
        {
            Vector3 avoidanceTarget = collision.Position + collision.Normal * agent.avoidDistance;
            
            Vector3 futurePosition = avoidanceTarget;

            Vector3 desiredVelocity = futurePosition - agent.transform.position;
            desiredVelocity = desiredVelocity.normalized * agent.baseSpeed;

            output.Linear = desiredVelocity - agent.Velocity;
        }
        
        return output;
    }
}