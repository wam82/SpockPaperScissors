using UnityEngine;

public class Flee : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = new SteeringOutput();
        
        Vector3 desiredVelocity = agent.TargetPosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * agent.baseSpeed;
        Vector3 steering = desiredVelocity - agent.Velocity;
        output.Linear = -steering;
        
        return output;
    }
}