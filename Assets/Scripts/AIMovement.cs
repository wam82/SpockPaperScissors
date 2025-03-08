using UnityEngine;

public abstract class AIMovement : MonoBehaviour
{
    public virtual SteeringOutput GetKinematic(IndividualAI agent)
    {
        return new SteeringOutput { Angular = agent.transform.rotation };
    }

    public virtual SteeringOutput GetSteering(IndividualAI agent)
    {
        return new SteeringOutput { Angular = Quaternion.identity };
    }
}