using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Basic : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsPlayer;

    [SerializeField] AudioClip deathClip;
    [SerializeField] FloatingHealthBar healthBar;
    [SerializeField] private int maxHealth;
    private int currentHealth;

    [SerializeField] private Transform turretTransform;
    [SerializeField] private float rotationSpeed;
    //[SerializeField] private int turretDamage = 10;

    // States
    [SerializeField] private float sightRange;
    [SerializeField] private bool playerInSightRange;

    // Attacking
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private bool alreadyAttacked;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootVelocity;

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<FloatingHealthBar>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        RaycastHit hit;
        playerInSightRange = false;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, sightRange, whatIsGround | whatIsWall | whatIsPlayer))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                playerInSightRange = true;
            }
        }

        if (!playerInSightRange)
        {
            Scanning();
        }
        if (playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void Scanning()
    {
        turretTransform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    private void AttackPlayer()
    {
        Vector3 direction = (player.position - turretTransform.position).normalized;
        direction.y = 0;

        if (!alreadyAttacked)
        {
            GameObject projectileInstance = Instantiate(projectile, firePoint.position + firePoint.forward * 1, firePoint.rotation);
            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * shootVelocity, ForceMode.Impulse);

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
        AudioSource.PlayClipAtPoint(deathClip, Camera.main.transform.position);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(firePoint.position, firePoint.forward * sightRange);
    }
}
