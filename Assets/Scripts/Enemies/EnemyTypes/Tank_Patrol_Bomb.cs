using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Taink;

public class Tank_Patrol_Bomb : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] private int maxHealth;
    private int currentHealth;

    [SerializeField] private float chargeSpeed;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float patrolAccelSpeed;
    [SerializeField] private int explosionDamage = 50;
    //[SerializeField] private GameObject explosionEffect;

    // Patroling
    [SerializeField] Transform[] waypoints;
    [SerializeField] private int waypointIndex = 0;

    // States
    [SerializeField] private float sightRange;
    [SerializeField] private bool playerInSightRange;

    private void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        RaycastHit hit;
        playerInSightRange = false;

        if (Physics.Raycast(transform.position, transform.forward, out hit, sightRange, whatIsGround | whatIsWall | whatIsPlayer))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                playerInSightRange = true;
            }
        }

        if (!playerInSightRange)
        {
            Patrolling();
        }
        if (playerInSightRange)
        {
            ChasePlayer();
        }
    }

    private void Patrolling()
    {
        agent.speed = patrolSpeed;
        agent.acceleration = patrolAccelSpeed;
        agent.autoBraking = true;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            agent.SetDestination(waypoints[waypointIndex].position);
        }
        // If the agent has no path or is far from the destination, keep the current destination
        else if (agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.remainingDistance > agent.stoppingDistance)
        {
            agent.SetDestination(waypoints[waypointIndex].position);
        }
    }

    private void ChasePlayer()
    {
        transform.LookAt(player);
        agent.autoBraking = false;
        agent.speed = chargeSpeed;
        agent.acceleration = chargeSpeed;
        agent.SetDestination(player.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Tank_Controller playerController = collision.gameObject.GetComponent<Tank_Controller>();

            if (playerController != null)
            {
                playerController.TakeDamage(explosionDamage);
            }
            else
            {
                Debug.LogError("Tank_Controller component not found on collided player!");
            }

            Explode();
        }    
    }

    private void Explode()
    {
        //Instantiate(explosionEffect, transform.position, Quaternion.identity);
        DestroyEnemy();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Invoke(nameof(DestroyEnemy), 0.0f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * sightRange);    
    }
}
