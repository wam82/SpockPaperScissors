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
    public CollisionDetector detector;
    public LayerMask enemyLayer;
    public List<string> validTargetTags = new List<string>();
    
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
        currentState = State.Seeking;
    }

    private void Update()
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
        float secondMinDistance = Mathf.Infinity;
        Transform closest = null;
        Transform secondClosest = null;

        foreach (string tag in validTargetTags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                Transform targetTransform = target.transform;
                float distance = Vector3.Distance(transform.position, targetTransform.position);

                if (targetTransform == trackedTarget)
                    continue; // Ignore current target if possible

                if (distance < minDistance)
                {
                    secondMinDistance = minDistance;
                    secondClosest = closest;

                    minDistance = distance;
                    closest = targetTransform;
                }
                else if (distance < secondMinDistance)
                {
                    secondMinDistance = distance;
                    secondClosest = targetTransform;
                }
            }
        }

        // If closest is the same as the current target, return the second closest instead
        return (closest == trackedTarget) ? secondClosest : closest;
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
