using UnityEngine;

public class FaceDirection : AIMovement
{
    public override SteeringOutput GetSteering(IndividualAI agent)
    {
        SteeringOutput output = new SteeringOutput();
        Quaternion angular = output.Angular;

        if (agent.Velocity == Vector3.zero)
        {
            angular = output.Angular;
        }

        if (agent.Velocity != Vector3.zero)
        {
            angular = Quaternion.LookRotation(agent.Velocity);
        }
        
        Vector3 from = Vector3.ProjectOnPlane(agent.transform.forward, Vector3.up);
        Vector3 to = angular * Vector3.forward;
        float angle = Vector3.SignedAngle(from, to, Vector3.up);
        output.Angular = Quaternion.AngleAxis(angle, Vector3.up);
        
        return output;
    }
}