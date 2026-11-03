using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SeekFleeSteering : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance")]
    public float safetyDistance = 5f;
    public float stopDistance = 0.5f;
    public float fleeDistance = 6f;
    public float heightTolerance = 2f;

    [Header("Steering")]
    public float acceleration = 8f;
    public float maxSpeed = 5f;
    public float turnSpeed = 360f;
    public float updateInterval = 0.1f;

    private NavMeshAgent agent;
    private float updateTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.speed = maxSpeed;
        agent.acceleration = acceleration;
        agent.angularSpeed = turnSpeed;
        agent.stoppingDistance = stopDistance;
    }

    private void Update()
    {
        if (player == null || !agent.isOnNavMesh)
        {
            return;
        }

        updateTimer += Time.deltaTime;

        if (updateTimer < updateInterval)
        {
            return;
        }

        updateTimer = 0f;

        Vector3 offset = player.position - transform.position;
        float heightDifference = Mathf.Abs(offset.y);

        offset.y = 0f;

        float distance = offset.magnitude;

        bool onSameLevel = heightDifference <= heightTolerance;

        if (distance < safetyDistance && onSameLevel)
        {
            Flee();
        }
        else
        {
            Seek();
        }
    }

    private void Seek()
    {
        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            player.position, out hit, 3f, NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
    }

    private void Flee()
    {
        Vector3 away = transform.position - player.position;
        away.y = 0f;

        if (away.sqrMagnitude < 0.001f)
        {
            away = transform.forward;
        }

        away.Normalize();

        float[] angles = { 0f, 45f, -45f, 90f, -90f, 135f, -135f };

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 direction = Quaternion.Euler(0f, angles[i], 0f) * away;
            Vector3 candidate = transform.position + direction * fleeDistance;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                candidate, out hit, 2f, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
                return;
            }
        }

        agent.isStopped = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, safetyDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
