using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class RangeEnemy : MonoBehaviour
{
    public float stoppingDistance;
    public float retreatDistance;
    public float speed;
    public float startTimeBtwShoots;
    public MoveForwardProjectile  projectile;

    public int numPatrolPoints = 3; // Количество случайных точек патрулирования
    public float patrolRadius = 3f; // Радиус для генерации случайных точек
    public float chaseDistance;
    private bool playerInRange;

    private Transform player;
    private NavMeshAgent agent;
    private float timeBtwShoots;

    private List<Vector3> randomPatrolPoints = new List<Vector3>();


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        GenerateRandomPatrolPoints();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (distanceToPlayer < retreatDistance)
        {
            Vector3 retreatPoint = transform.position - (player.position - transform.position).normalized * retreatDistance;
            agent.SetDestination(retreatPoint);
        }
        else if (distanceToPlayer <= stoppingDistance)
        {
            agent.SetDestination(transform.position);
        }
        else if (playerInRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }

        if (timeBtwShoots <= 0 && playerInRange)
        {
            // Определяем направление полета снаряда
            Vector2 shootDirection = (player.position - transform.position).normalized;
            // Создаем снаряд
            Instantiate(projectile, transform.position, Quaternion.identity).SetTargetDirection(shootDirection);// Передаем направление снаряда скрипту MoveForwardProjectile

            timeBtwShoots = startTimeBtwShoots; 
        }
        else
        {
            timeBtwShoots -= Time.deltaTime;
        }
    }

    void GenerateRandomPatrolPoints()
    {
        for (int i = 0; i < numPatrolPoints; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius * 2;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, 1))
            {
                randomPatrolPoints.Add(hit.position);
            }
        }
    }

    void Patrol()
    {
        if (randomPatrolPoints.Count > 0)
        {
            agent.SetDestination(randomPatrolPoints[0]);
            if (Vector3.Distance(transform.position, randomPatrolPoints[0]) < 1f)
            {
                randomPatrolPoints.Add(randomPatrolPoints[0]);
                randomPatrolPoints.RemoveAt(0);
            }
        }
        if (agent.velocity.magnitude < 0.1f)
        {
            GenerateRandomPatrolPoints();
        }
    }
}
