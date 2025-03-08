using UnityEngine;

public class Flee : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = base.GetSteering(agent);
        
        Vector3 desiredVelocity = agent.TargetPosition - agent.transform.position;
        desiredVelocity = desiredVelocity.normalized * agent.speed;
        Vector3 steering = desiredVelocity - agent.Velocity;
        output.linear = -steering;
        
        return output;
    }
}
