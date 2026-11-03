
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    [Header("Flock")]
    public GameObject agentPrefab;
    public int agentCount = 20;
    public int leaderCount = 2;
    public float spawnRadius = 8f;

    [Header("Target")]
    public Transform target;

    [Header("Neighbourhood")]
    public float neighbourDistance = 5f;
    public float separationDistance = 1.5f;

    [Header("Flocking Weights")]
    public float cohesionWeight = 1f;
    public float alignmentWeight = 1f;
    public float separationWeight = 1.5f;
    public float leaderWeight = 2f;

    [Header("Movement")]
    public float maxSpeed = 4f;
    public float rotationSpeed = 5f;
    public float updateInterval = 0.2f;
    public float destinationLookAhead = 3f;

    [Header("Obstacle Avoidance")]
    public float obstacleLookAhead = 2f;
    public float obstacleAvoidanceWeight = 3f;
    public LayerMask obstacleLayers = ~0;

    [HideInInspector]
    public List<FlockAgent> agents = new List<FlockAgent>();

    void Start()
    {
        CreateFlock();
    }

    public void CreateFlock()
    {
        ClearFlock();

        if (agentPrefab == null)
        {
            Debug.LogWarning("FlockManager needs an agent prefab.");
            return;
        }

        int totalAgents = Mathf.Max(0, agentCount);
        int totalLeaders = Mathf.Clamp(leaderCount, 0, totalAgents);

        for (int i = 0; i < totalAgents; i++)
        {
            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomPosition.x, 0f, randomPosition.y);

            GameObject newAgent = Instantiate(
                agentPrefab,
                spawnPosition,
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
            );

            FlockAgent flockAgent = newAgent.GetComponent<FlockAgent>();

            if (flockAgent == null)
            {
                Debug.LogError( "The prefab needs a FlockAgent component.");

                Destroy(newAgent);
                continue;
            }

            flockAgent.manager = this;
            flockAgent.isLeader = i < totalLeaders;

            agents.Add(flockAgent);
        }
    }

    void ClearFlock()
    {
        foreach (FlockAgent agent in agents)
        {
            if (agent != null)
            {
                Destroy(agent.gameObject);
            }
        }

        agents.Clear();
    }
}