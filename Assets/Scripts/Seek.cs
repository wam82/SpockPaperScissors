using UnityEngine;

public class Seek : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = base.GetSteering(agent);
            
        float distance = Vector3.Distance(agent.TargetPosition, agent.transform.position);
        float ahead = distance / 10;
        Vector3 futurePosition = agent.TargetPosition + agent.TrackedTarget.forward * ahead;

        Vector3 desiredVelocity = futurePosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * agent.baseSpeed;
        Vector3 steering = desiredVelocity - agent.Velocity;
        output.Linear = steering;
            
        return output;
    }
}