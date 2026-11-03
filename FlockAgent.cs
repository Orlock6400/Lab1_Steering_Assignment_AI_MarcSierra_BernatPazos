
using UnityEngine;
using UnityEngine.AI;

public class FlockAgent : MonoBehaviour
{
    [HideInInspector]
    public FlockManager manager;

    [HideInInspector]
    public bool isLeader;

    private NavMeshAgent navAgent;
    private float updateTimer;

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (manager != null)
        {
            navAgent.speed = manager.maxSpeed;
            navAgent.angularSpeed = manager.rotationSpeed * 60f;
            navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

            if (isLeader)
            {
            navAgent.avoidancePriority = 10;
            }
            else
            {
            navAgent.avoidancePriority = Random.Range(30, 90);
            }

            updateTimer = Random.Range(0f, manager.updateInterval);
        }
    }

    private void Update()
    {
        if (manager == null || !navAgent.isOnNavMesh)
        {
            return;
        }

        updateTimer += Time.deltaTime;

        if (updateTimer >= manager.updateInterval)
        {
            updateTimer = 0f;
            UpdateSteering();
        }
    }

    private void UpdateSteering()
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 separation = Vector3.zero;

        int neighbourCount = 0;

        foreach (FlockAgent other in manager.agents)
        {
            if (other == null || other == this)
            {
                continue;
            }

            Vector3 offset = other.transform.position - transform.position;

            offset.y = 0f;

            float distance = offset.magnitude;

            if (distance > manager.neighbourDistance ||
                distance <= 0.001f)
            {
                continue;
            }

            cohesion += other.transform.position;

            Vector3 neighbourDirection = other.navAgent.velocity;
            neighbourDirection.y = 0f;

            if (neighbourDirection.sqrMagnitude > 0.001f)
            {
                alignment += neighbourDirection.normalized;
            }

            if (distance < manager.separationDistance)
            {
                //separation -= offset / (distance * distance);

                float strength = 1f - distance / manager.separationDistance;
                separation -= offset.normalized * strength;
            }

            neighbourCount++;
        }

        if (neighbourCount > 0)
        {
            cohesion = cohesion / neighbourCount - transform.position;

            cohesion.y = 0f;

            if (cohesion.sqrMagnitude > 0.001f)
            {
                cohesion.Normalize();
            }

            if (alignment.sqrMagnitude > 0.001f)
            {
                alignment.Normalize();
            }

            separation = Vector3.ClampMagnitude(separation, 1f);
        }

        Vector3 leaderFollowing = Vector3.zero;

        if (!isLeader)
        {
            FlockAgent leader = FindNearestLeader();

            if (leader != null)
            {
                Vector3 leaderOffset = leader.transform.position - transform.position;

                leaderOffset.y = 0f;

                if (leaderOffset.sqrMagnitude > 0.001f)
                {
                    leaderFollowing = leaderOffset.normalized;
                }
            }
        }

        Vector3 obstacleAvoidance = CalculateObstacleAvoidance();

        Vector3 desiredDirection =
            cohesion * manager.cohesionWeight
            + alignment * manager.alignmentWeight
            + separation * manager.separationWeight
            + leaderFollowing * manager.leaderWeight
            + obstacleAvoidance * manager.obstacleAvoidanceWeight;

        desiredDirection.y = 0f;

        if (isLeader && manager.target != null)
        {
            Vector3 targetDirection =
                manager.target.position - transform.position;

            targetDirection.y = 0f;

            if (targetDirection.sqrMagnitude > 0.001f)
            {
                desiredDirection += targetDirection.normalized
                    * manager.leaderWeight;
            }
        }

        if (desiredDirection.sqrMagnitude > 0.001f)
        {
            desiredDirection.Normalize();

            Vector3 destination = transform.position + desiredDirection * manager.destinationLookAhead;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                destination, out hit, 2f, navAgent.areaMask))
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(hit.position);
            }
        }
        else
        {
            if (!isLeader && leaderFollowing.sqrMagnitude > 0.001f)
            {
                navAgent.isStopped = false;
                navAgent.SetDestination(transform.position + leaderFollowing * manager.destinationLookAhead);
            }
            else
            {
                navAgent.isStopped = true;
            }
        }
    }

    private FlockAgent FindNearestLeader()
    {
        FlockAgent nearestLeader = null;
        float nearestDistance = float.MaxValue;

        foreach (FlockAgent other in manager.agents)
        {
            if (other == null || !other.isLeader)
            {
                continue;
            }

            float distance = Vector3.Distance(
                transform.position, other.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestLeader = other;
            }
        }

        return nearestLeader;
    }

    private Vector3 CalculateObstacleAvoidance()
    {
        Vector3 direction = navAgent.velocity;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        direction.Normalize();

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(
            origin,
            navAgent.radius,
            direction,
            out RaycastHit hit,
            manager.obstacleLookAhead,
            manager.obstacleLayers,
            QueryTriggerInteraction.Ignore))
        {
            Vector3 away = transform.position - hit.point;
            away.y = 0f;

            if (away.sqrMagnitude > 0.001f)
            {
                return away.normalized;
            }
        }

        return Vector3.zero;
    }
}
