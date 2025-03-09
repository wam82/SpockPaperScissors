using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndividualAI : MonoBehaviour
{
    public enum State
    {
        Idle, Fleeing, Seeking
    }
    
    [Header("Agent Information")] 
    public State currentState = State.Idle;
    [SerializeField] private GroupAI groupAI;
    public CollisionDetector detector;
    public LayerMask enemyLayer;
    public List<string> validTargetTags = new();
    
    [Header("Agent Settings")]
    public float lookAhead;
    public float avoidDistance;
    
    public float fleeTriggerDistance;
    public float chaseResumeDistance;
    
    public float speed;
    
    [Header("Boost Settings")]
    public float speedBoost;
    public float boostDuration;
    public float boostCooldown;

    private float boostTime = 0f;
    private float cooldownTime = 0f;
    private bool isBoosting = false;
    
    public Vector3 Velocity { get; set; }
    
    [Header("Target Information")]
    public Transform trackedTarget;
    private Vector3 targetPosition;
    private Transform holder;
    private Transform closestTarget;
    
    public Vector3 TargetPosition
    {
        get => trackedTarget != null ? trackedTarget.position : targetPosition;
    }
    
    public Vector3 TargetForward
    {
        get => trackedTarget != null ? trackedTarget.forward : Vector3.forward;
    }
    
    public void TrackTarget(Transform targetTransform)
    {
        holder = trackedTarget;
        trackedTarget = targetTransform;
    }

    public void RestoreTarget()
    {
        trackedTarget = holder;
        holder = null;
    }

    public void UnTrackTarget()
    {
        trackedTarget = null;
    }
    
    private void Start()
    {
        currentState = State.Idle;
        trackedTarget = FindClosestTarget();
        closestTarget = trackedTarget;
        currentState = State.Seeking;
    }

    private void Update()
    {
        float currentTime = Time.time;
        
        if (!isBoosting)
        {
            speed = groupAI.aggressiveness;
        }
        
        if ((holder = TargetToFlee()) != null && currentState != State.Fleeing)
        {
            currentState = State.Fleeing;
            PursuitRegistry.Instance.RegisterPursuit(transform, holder);
            trackedTarget = holder;
            holder = null;
        }

        if (currentState == State.Fleeing && HasFled())
        {
            currentState = State.Seeking;
            PursuitRegistry.Instance.RemovePursuit(transform);
            trackedTarget = FindClosestTarget();
            closestTarget = trackedTarget;
            holder = null;
        }
        
        if (currentState == State.Seeking)
        {
            if (isBoosting)
            {
                Cooldown();
            }

            if (PursuitRegistry.Instance.WasPursuing(transform))
            {
                ChangeTarget();
                PursuitRegistry.Instance.ClearPursuer(transform);
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
        speed += speedBoost;
    }
    
    private void Cooldown()
    {
        isBoosting = false;
        speed -= speedBoost;
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
    
    private Transform ChangeTarget()
    {
        float minDistance = Mathf.Infinity;
        Transform newClosest = null;

        foreach (string tag in validTargetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                // Skip if this target is the same as the previous closest target
                if (target.transform == closestTarget)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    newClosest = target.transform;
                }
            }
        }
    
        return newClosest;
    }
    
    private void Move()
    {
        GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation);
        Velocity += steeringForceSum * Time.deltaTime;
        Velocity = Vector3.ClampMagnitude(Velocity, speed);
        transform.position += Velocity * Time.deltaTime;
        transform.rotation *= rotation;
    }
    
    private void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
    {
        steeringForceSum = Vector3.zero;
        rotation = Quaternion.identity;
        AIMovement[] movements = GetComponents<AIMovement>();
        
        // Filter which movements
        if (currentState == State.Seeking)
        {
            movements = movements.Where(m => m is Seek || m is FaceDirection).ToArray();
        }
        
        if (currentState == State.Fleeing)
        {
            movements = movements.Where(m => m is Flee || m is FaceDirection).ToArray();
        }

        foreach (AIMovement movement in movements)
        {
            steeringForceSum += movement.GetSteering(this).linear;
            rotation *= movement.GetSteering(this).angular;
        }
    }
}
