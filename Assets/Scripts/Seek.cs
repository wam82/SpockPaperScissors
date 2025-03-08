using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = base.GetSteering(agent);
            
        float distance = Vector3.Distance(agent.TargetPosition, agent.transform.position);
        float ahead = distance / 10;
        Vector3 futurePosition = agent.TargetPosition + agent.TargetForward * ahead;

        Vector3 desiredVelocity = futurePosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * agent.speed;
        Vector3 steering = desiredVelocity - agent.Velocity;
        output.linear = steering;
            
        return output;
    }
}
