using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDown.Camera
{
    public class TopDown_Camera : MonoBehaviour
    {

        #region Variables
        [SerializeField] private Transform m_target;
        [SerializeField] private float m_Height = 20.0f;
        [SerializeField] private float m_Distance = 20.0f;
        [SerializeField] private float m_Angle = 0.0f;
        [SerializeField] private float m_SmoothSpeed = 0.0f;

        private Vector3 refVelocity;
        #endregion



        #region Main Methods
        void Start()
        {
            HandleCamera();
        }

        void Update()
        {
            HandleCamera();
        }
        #endregion



        #region Helper Methods
        protected virtual void HandleCamera()
        {
            if (!m_target)
            {
                return;
            }

            // Build World Position Vector
            Vector3 worldPos = (Vector3.forward * -m_Distance) + (Vector3.up * m_Height);

            // Build our Rotated Vector
            Vector3 rotatedVector = Quaternion.AngleAxis(m_Angle, Vector3.up) * worldPos;

            // Move our position
            Vector3 flatTargetPos = m_target.position;
            flatTargetPos.y = 0.0f;
            Vector3 finalPos = flatTargetPos + rotatedVector;

            transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref refVelocity, m_SmoothSpeed);
            transform.LookAt(flatTargetPos);
        }

        private void OnDrawGizmos()
        {
            if (m_target)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.25f); 
                Gizmos.DrawLine(transform.position, m_target.position);
                Gizmos.DrawSphere(m_target.position, 1.5f);
            }
            Gizmos.DrawSphere(transform.position, 1.5f);
        }
        #endregion
    }
}
