using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI_Foundation
{
    public class IndividualAI : MonoBehaviour
    {
        public enum State
        {
            Idle, Seeking, Arriving, Fleeing
        }
        [Header("Agent Settings")]
        public State currentState = State.Idle;
        public float speed;
        public float avoidanceRadius;
        public float fleeTriggerDistance;
        public float seekResumeDistance;
        public float targetLockDuration;
        public Vector3 Velocity { get; set; }

        private float targetLockTime;
        
        [Header("Boost Settings")]
        public float speedBoost;
        public float boostDuration;
        public float boostCooldown;

        private float endOfBoostTime;
        private float cooldownTime;
        private bool isBoosting;
        
        [Header("Agent Parameters")]
        [SerializeField] private GroupAI groupAI;
        public List<string> targetTags = new List<string>();
        [SerializeField] public List<GameObject> obstacles = new List<GameObject>();
        public LayerMask enemyLayer;

        [Header("Target Parameters")] 
        [SerializeField] private Transform trackedTarget;
        [SerializeField] private Vector3 targetPosition;
        private Transform closestTarget;
        private Transform holder;

        public Vector3 TargetPosition
        {
            get => trackedTarget != null ? trackedTarget.position : targetPosition;
        }

        public Vector3 TargetForward
        {
            get => trackedTarget != null ? trackedTarget.forward : Vector3.forward;
        }

        private void Boost()
        {
            isBoosting = true;
            endOfBoostTime = Time.time + boostDuration;
            speed += speedBoost;
        }

        private void Cooldown()
        {
            isBoosting = false;
            cooldownTime = Time.time + boostCooldown;
            speed -= speedBoost;
        }

        private bool HasFled()
        {
            float distance = Vector3.Distance(transform.position, TargetPosition);
            return distance > seekResumeDistance;
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
            float closestDistance = Vector3.Distance(transform.position, closest.position);

            foreach (Collider col in enemies)
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closest = col.transform;
                    closestDistance = distance;
                }
            }
            
            return closest;
        }
        
        private Transform FindClosestTarget()
        {
            float minDistance = Mathf.Infinity;
            Transform closest = null;

            foreach (string targetTag in targetTags)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
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

        private Transform FindRandomTarget()
        {
            List<Transform> validTargets = new List<Transform>();

            foreach (string targetTag in targetTags)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);
                foreach (GameObject target in targets)
                {
                    validTargets.Add(target.transform);
                }
            }

            if (validTargets.Count == 0)
            {
                return null;
            }
            
            int randomIndex = Random.Range(0, validTargets.Count);
            return validTargets[randomIndex];
        }

        private Transform ChangeTarget()
        {
            float minDistance = Mathf.Infinity;
            Transform closest = null;

            foreach (string targetTag in targetTags)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

                foreach (GameObject target in targets)
                {
                    if (target.transform == closestTarget)
                    {
                        continue;
                    }
                    
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
            Vector3 position = steeringForceSum;
            position.y = 0;
            Velocity += position * Time.deltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, speed);
            transform.position += Velocity * Time.deltaTime;
            // Vector3 resetY = transform.position;
            // resetY.y = 0;
            // transform.position = resetY;
            transform.rotation *= rotation;
        }
        
        private void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
        {
            steeringForceSum = Vector3.zero;
            rotation = Quaternion.identity;
            AIMovement[] movements = GetComponents<AIMovement>();
            
            // Filter behaviours
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
                steeringForceSum += movement.GetSteering(this).linear;
                rotation *= movement.GetSteering(this).angular;
            }
        }

        private void Start()
        {
            GameObject[] foundObstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            obstacles.AddRange(foundObstacles);
                
            trackedTarget = FindRandomTarget();
            closestTarget = trackedTarget;
            currentState = State.Seeking;
        }

        private void Update()
        {
            if (GameManager.instance.gameState == GameManager.GameState.Pause)
            {
                return;
            }
            float currentTime = Time.time;

            if (!isBoosting)
            {
                speed = groupAI.aggressiveness;
            }

            if (trackedTarget == null)
            {
                trackedTarget = FindRandomTarget();
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

                if (trackedTarget != null && currentTime - targetLockTime >= targetLockDuration)
                {
                    /* Retargeting if it's been a certain amount of time that unit has been following
                        same target. This value changes depending on which faction the unit belongs to.*/
                    trackedTarget = FindRandomTarget();
                    closestTarget = trackedTarget;
                    holder = null;
                    targetLockTime = Time.time;
                }
                
                Move();
            }

            if (currentState == State.Fleeing)
            {
                if (!isBoosting && currentTime >= cooldownTime)
                {
                    Boost();
                }

                if (isBoosting && currentTime >= endOfBoostTime)
                {
                    Cooldown();
                }
                
                Move();
            }
        }
    }
}
