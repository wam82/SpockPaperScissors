using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{
    public enum State
    {
        Idle, Fleeing, Seeking
    }
    [Header("Agent Settings")] 
    public float lookAhead;
    public float avoidDistance;
    public float speed;
    public CollisionDetector detector;
    public State currentState = State.Idle;
    
    [Header("Target Information")]
    [SerializeField] private Transform trackedTarget;
    [SerializeField] private Vector3 targetPosition;
    
    public Transform TrackedTarget
    {
        get => trackedTarget;
    }
    
    public Vector3 TargetPosition
    {
        get => trackedTarget != null ? trackedTarget.position : targetPosition;
    }
    public Vector3 Velocity { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Move()
    {
        GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation);
        Velocity += steeringForceSum * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, speed);
        transform.position += Velocity * Time.deltaTime;
        transform.rotation *= rotation;
    }
    
    private void GetSteeringSum(out Vector3 steering, out Quaternion rotation)
    {
        steering = Vector3.zero;
        rotation = Quaternion.identity;
        AIMovement[] movements = GetComponents<AIMovement>();
        
        // Filter which movements

        foreach (AIMovement movement in movements)
        {
            steering += movement.GetSteering(this).Linear;
            rotation *= movement.GetSteering(this).Angular;
        }
    }
}
