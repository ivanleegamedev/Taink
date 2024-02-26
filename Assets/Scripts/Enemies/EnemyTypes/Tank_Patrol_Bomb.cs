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

    [SerializeField] private int health;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float patrolAccelSpeed;
    [SerializeField] private float explosionDamage = 50f;
    //[SerializeField] private GameObject explosionEffect;

    // Patroling
    [SerializeField] Transform[] waypoints;
    [SerializeField] private int waypointIndex = 0;

    // States
    [SerializeField] private float sightRange;
    [SerializeField] private bool playerInSightRange;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check for sight using raycast
        RaycastHit hit;
        playerInSightRange = false;

        // Perform raycast forward from the enemy's position
        if (Physics.Raycast(transform.position, transform.forward, out hit, sightRange, whatIsGround | whatIsWall | whatIsPlayer))
        {
            // Check if the player is the object hit
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
            Explode();
        }
    }

    private void Explode()
    {
        //Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // TODO
        //player.GetComponent<Tank_Controller>().TakeDamage(explosionDamage);

        DestroyEnemy();
    }

    private void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
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
