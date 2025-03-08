using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class IndividualAI : MonoBehaviour
{
    public enum State
    {
        Idle, Fleeing, Seeking
    }
    [Header("Agent Settings")] 
    public State currentState = State.Idle;
    public CollisionDetector detector;
    public LayerMask enemyLayer;
    public List<string> validTargetTags = new List<string>();
    
    public float lookAhead;
    public float avoidDistance;
    
    public float fleeTriggerDistance;
    public float chaseResumeDistance;
    
    public float baseSpeed;
    
    [Header("Boost Settings")]
    public float speedBoost;
    public float boostDuration;
    public float boostCooldown;

    private float boostTime = 0f;
    private float cooldownTime = 0f;
    private bool isBoosting = false;
    
    [Header("Target Information")]
    [SerializeField] private Transform trackedTarget;
    [SerializeField] private Vector3 targetPosition;
    
    private Transform holder;
    
    public Transform TrackedTarget
    {
        get => trackedTarget;
    }
    
    public Vector3 TargetPosition
    {
        get => trackedTarget != null ? trackedTarget.position : targetPosition;
    }
    public Vector3 Velocity { get; set; }
    
    void Start()
    {
        currentState = State.Idle;
        trackedTarget = FindClosestTarget();
        currentState = State.Seeking;
    }
    
    void Update()
    {
        float currentTime = Time.time;
        if ((holder = TargetToFlee()) != null && currentState != State.Fleeing)
        {
            currentState = State.Fleeing;
            trackedTarget = holder;
            holder = null;
        }

        if (currentState == State.Fleeing && HasFled())
        {
            currentState = State.Seeking;
            trackedTarget = FindClosestTarget();
            holder = null;
        }

        if (currentState == State.Seeking)
        {
            // DebugUtils.DrawCircle(transform.position, Vector3.up, Color.red, fleeTriggerDistance);
            // DebugUtils.DrawCircle(transform.position, Vector3.up, Color.yellow, chaseResumeDistance);
            if (isBoosting)
            {
                Cooldown();
            }
            Move();
        }

        if (currentState == State.Fleeing)
        {
            if (!isBoosting && currentTime >= cooldownTime)
            {
                Boost();
            }

            if (isBoosting && currentTime >= boostTime)
            {
                Cooldown();
            }
            
            Move();
        }
    }

    private void Boost()
    { 
       isBoosting = true;
       boostTime = Time.time + boostDuration;
       baseSpeed += speedBoost;
    }

    private void Cooldown()
    {
        isBoosting = false;
        baseSpeed -= speedBoost;
        cooldownTime = Time.time + boostCooldown;
    }

    private bool HasFled()
    {
        float distance = Vector3.Distance(transform.position, TargetPosition);
        return distance > chaseResumeDistance;
    }

    private Transform TargetToFlee()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, fleeTriggerDistance, enemyLayer);

        if (nearbyEnemies.Length > 0)
        {
            Transform closestEnemy = FindClosestEnemy(nearbyEnemies);
            return closestEnemy;
        }
        return null;
    }
    
    private Transform FindClosestEnemy(Collider[] enemies)
    {
        Transform closest = enemies[0].transform;
        float minDistance = Vector3.Distance(transform.position, closest.position);

        foreach (Collider col in enemies)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                closest = col.transform;
                minDistance = distance;
            }
        }
        return closest;
    }

    private Transform FindClosestTarget()
    {
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (string tag in validTargetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = target.transform;
                }
            }
        }
        
        return closest;
    }

    private void Move()
    {
        GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation);
        Velocity += steeringForceSum * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, baseSpeed);
        transform.position += Velocity * Time.deltaTime;
        transform.rotation *= rotation;
    }
    
    private void GetSteeringSum(out Vector3 steering, out Quaternion rotation)
    {
        steering = Vector3.zero;
        rotation = Quaternion.identity;
        AIMovement[] movements = GetComponents<AIMovement>();
        
        // Filter which movements
        if (currentState == State.Seeking)
        {
            movements = movements.Where(m => m is Seek || m is FaceDirection || m is Avoid).ToArray();
        }
        
        if (currentState == State.Fleeing)
        {
            movements = movements.Where(m => m is Flee || m is FaceDirection || m is Avoid).ToArray();
        }

        foreach (AIMovement movement in movements)
        {
            steering += movement.GetSteering(this).Linear;
            rotation *= movement.GetSteering(this).Angular;
        }
    }
}
