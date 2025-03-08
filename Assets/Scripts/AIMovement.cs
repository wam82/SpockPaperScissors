using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIMovement : MonoBehaviour
{
    public virtual SteeringOutput GetSteering(IndividualAI agent)
    {
        return new SteeringOutput { angular = Quaternion.identity };
    }
}
