// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class Avoid : AIMovement
// {
//     public override SteeringOutput GetSteering(IndividualAI agent)
//     {
//         SteeringOutput output = base.GetSteering(agent);
//
//         Vector3 ray = agent.Velocity.normalized * agent.lookAhead;
//         
//         PotentialCollision collision = agent.detector.GetCollision(agent.transform.position, ray);
//
//         if (collision != null && !agent.isAvoiding)
//         {
//             agent.isAvoiding = true;
//             Vector3 avoidanceTarget = collision.Position + collision.Normal * agent.avoidDistance;
//
//             agent.TrackTarget(avoidanceTarget);
//
//             // Vector3 futurePosition = avoidanceTarget;
//             //
//             // Vector3 desiredVelocity = futurePosition - agent.transform.position;
//             // desiredVelocity = desiredVelocity.normalized * agent.speed;
//             //
//             // output.linear = desiredVelocity - agent.Velocity;
//         }
//
//         if (agent.isAvoiding && collision == null)
//         {
//             Debug.LogWarning("Done Avoiding");
//             agent.isAvoiding = false;
//             agent.RestoreTarget();
//         }
//         
//         return output;
//     }
// }
