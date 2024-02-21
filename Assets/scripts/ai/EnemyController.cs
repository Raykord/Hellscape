using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float stoppingDistance = 2.0f;
    [SerializeField] float chaseDistance = 5.0f;

    public int numPatrolPoints = 3; // Количество случайных точек патрулирования
    public float patrolRadius = 3f; // Радиус для генерации случайных точек
    public float patrolWaitTime = 3.0f; // Время ожидания на точке патрулирования

    private NavMeshAgent agent;
    private List<Vector3> randomPatrolPoints = new List<Vector3>();
    private bool playerInRange;
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;
    private float patrolTimer = 0.0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        GenerateRandomPatrolPoints();

        agent.stoppingDistance = stoppingDistance;
        playerInRange = false;
        patrolTimer = patrolWaitTime;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, target.position) <= chaseDistance)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (playerInRange)
        {
            agent.SetDestination(target.position);

            // Attack logic
        }
        else
        {
            Patrol();
        }
    }

    void GenerateRandomPatrolPoints()
    {
        for (int i = 0; i < numPatrolPoints; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius * 2;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, 3))
            {
                randomPatrolPoints.Add(hit.position);
            }
        }
    }

    void Patrol()
    {
        if (isPatrolling && randomPatrolPoints.Count > 0)
        {
            agent.SetDestination(randomPatrolPoints[currentPatrolIndex]);
            if (Vector3.Distance(transform.position, randomPatrolPoints[currentPatrolIndex]) < agent.stoppingDistance)
            {
                patrolTimer -= Time.deltaTime;
                if (patrolTimer <= 0.0f)
                {
                    currentPatrolIndex = (currentPatrolIndex + 1) % randomPatrolPoints.Count;
                    patrolTimer = patrolWaitTime;
                }
            }
            if (agent.velocity.magnitude < 0.1f)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % randomPatrolPoints.Count;
            }
        }
    }


}
