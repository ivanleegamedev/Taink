using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Taink
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Tank_Inputs))]
    public class Tank_Controller : MonoBehaviour
    {
        #region Variables
        [Header("Movement Properties")]
        [SerializeField] private float moveSpeed = 15.0f;
        [SerializeField] private float rotationSpeed = 60.0f;

        [Header("Health Properties")]
        [SerializeField] AudioClip deathClip;
        [SerializeField] private FloatingHealthBar healthBar;
        [SerializeField] private int maxHealth = 100;
        private int currentHealth;

        [Header("Turret Properties")]
        public Transform turretTransform;
        public float turretLagSpeed = 8.0f;

        [Header("Shooting Properties")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private float fireRate = 0.5f;
        [SerializeField] private float bulletSpeed = 1000.0f;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip shootClip;
        private bool bCanShoot = true;

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        private Rigidbody rb;
        private Tank_Inputs input;
        private Vector3 finalTurretLookDir;
        #endregion



        #region Main Methods
        void Start()
        {
            currentHealth = maxHealth;
            healthBar = GetComponentInChildren<FloatingHealthBar>();

            rb = GetComponent<Rigidbody>();
            input = GetComponent<Tank_Inputs>();

            bCanShoot = true;
        }

        private void Update()
        {
            if (rb && input)
            {
                HandlePausing();
            }
        }

        void FixedUpdate()
        {
            if (rb && input)
            {
                HandleMovement();
                HandleTurret();
                HandleReticle();
                HandleShooting();
            }
        }
        #endregion



        #region Custom Methods
        protected virtual void HandleMovement()
        {
            Vector3 wantedPos = transform.forward * input.ForwardInput * moveSpeed * Time.deltaTime;
            rb.velocity = wantedPos;

            Quaternion wantedRot = transform.rotation * Quaternion.Euler(Vector3.up * (input.RotationInput * rotationSpeed * Time.deltaTime));
            rb.MoveRotation(wantedRot);
        }

        protected virtual void HandleTurret()
        {
            if (turretTransform)
            {
                Vector3 turretLookDir = input.ReticlePos - turretTransform.position;
                turretLookDir.y = 0.0f;

                finalTurretLookDir = Vector3.Lerp(finalTurretLookDir, turretLookDir, turretLagSpeed * Time.deltaTime);
                turretTransform.rotation = Quaternion.LookRotation(finalTurretLookDir);
            }
        }

        protected virtual void HandleReticle()
        {
            if (reticleTransform)
            {
                reticleTransform.position = input.ReticlePos;
            }
        }

        protected virtual void HandleShooting()
        {
            if (input.FireInput > 0.0f && bCanShoot)
            {
                ShootBullet();
                audioSource.PlayOneShot(shootClip);
                bCanShoot = false;
                Invoke(nameof(ResetShoot), fireRate);                
            }
        }

        private void HandlePausing()
        {
            if (input.PauseInput > 0.0f)
            {
                GameManager.Instance.ChangeGameState(GameState.PAUSE);
            }
        }

        private void ShootBullet()
        {
            if (!bulletPrefab || !bulletSpawnPoint) return;

            GameObject bulletInstance = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody bulletRb = bulletInstance.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(bulletSpawnPoint.forward * bulletSpeed);
            }
        }

        private void ResetShoot()
        {
            bCanShoot = true;
        }

        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
            Debug.Log($"Tank took {damage} damage! Current Health: {currentHealth}");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log("Tank is Dead!");
            AudioSource.PlayClipAtPoint(deathClip, Camera.main.transform.position);
            GameManager.Instance.ChangeGameState(GameState.GAMEOVER);
            Destroy(gameObject);
        }
        #endregion
    }
}
