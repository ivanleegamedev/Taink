using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tank_Ranged : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] private int maxHealth;
    private int currentHealth;

    // Patrolling
    [SerializeField] private Vector3 walkPoint;
    [SerializeField] private bool walkPointSet;
    [SerializeField] private float walkPointRange;
    [SerializeField] private float stuckThreshold = 3.0f;
    [SerializeField] private float timeSinceLastMove = 0.0f;
    [SerializeField] private Vector3 lastPosition;

    // Attacking
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool alreadyAttacked;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootVelocity;

    // States
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSightRange;
    [SerializeField] private bool playerInAttackRange;

    private void Awake()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        lastPosition = transform.position;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

        CheckIfStuck();
    }

    private void Patrolling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void CheckIfStuck()
    {
        if (Vector3.Distance(transform.position, lastPosition) < 0.075f)
        {
            timeSinceLastMove += Time.deltaTime;

            if (timeSinceLastMove >= stuckThreshold)
            {
                walkPointSet = false;
                timeSinceLastMove = 0f;
            }
        }
        else
        {
            timeSinceLastMove = 0f;
        }

        lastPosition = transform.position;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            GameObject projectileInstance = Instantiate(projectile, firePoint.position + transform.forward * 1, transform.rotation);
            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * shootVelocity, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0) Invoke(nameof(DestroyEnemy), 0.0f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
