using UnityEngine;

namespace AI_Foundation
{
    public abstract class AIMovement : MonoBehaviour
    {
        public virtual SteeringOutput GetSteering(IndividualAI agent)
        {
            return new SteeringOutput { angular = Quaternion.identity };
        }
    }
}