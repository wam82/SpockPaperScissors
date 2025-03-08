using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirection : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = base.GetSteering(agent);
        Quaternion angular = output.angular;

        if (agent.Velocity == Vector3.zero) {
            angular = output.angular;
        }

        if (agent.Velocity != Vector3.zero)
            angular = Quaternion.LookRotation(agent.Velocity);

        Vector3 from = Vector3.ProjectOnPlane(agent.transform.forward, Vector3.up);
        Vector3 to = angular * Vector3.forward;
        float angleY = Vector3.SignedAngle(from, to, Vector3.up);
        output.angular = Quaternion.AngleAxis(angleY, Vector3.up);

        return output;
    }
}
