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

        [Header("Turret Properties")]
        public Transform turretTransform;
        public float turretLagSpeed = 8.0f;

        [Header("Reticle Properties")]
        public Transform reticleTransform;

        private Rigidbody rb;
        private Tank_Inputs input;
        private Vector3 finalTurretLookDir;
        #endregion



        #region Main Methods
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            input = GetComponent<Tank_Inputs>();
        }

        void FixedUpdate()
        {
            if (rb && input)
            {
                HandleMovement();
                HandleTurret();
                HandleReticle();
            }
        }
        #endregion



        #region Custom Methods
        protected virtual void HandleMovement()
        {
            // Handle Forward/Backward Movement
            Vector3 wantedPos = transform.position + (transform.forward * input.ForwardInput * moveSpeed * Time.deltaTime);
            rb.MovePosition(wantedPos);

            // Handle Rotation
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
        #endregion
    }
}
