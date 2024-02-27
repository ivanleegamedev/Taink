using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Taink
{
    public class Tank_Inputs : MonoBehaviour
    {
        #region Variables
        [Header("Input Properties")]
        public Camera cam;
        #endregion



        #region Properties
        private Vector3 reticlePos;
        public Vector3 ReticlePos
        {
            get { return reticlePos; }
        }

        private Vector3 reticleNormal;
        public Vector3 ReticleNormal
        {
            get { return reticleNormal; }
        }

        private float forwardInput;
        public float ForwardInput
        {
            get { return forwardInput; }
        }

        private float rotationInput;
        public float RotationInput
        {
            get { return rotationInput; }
        }

        private float fireInput;
        public float FireInput
        {
            get { return fireInput; }
        }
        #endregion



        #region Main Methods
        void Update()
        {
            if (cam)
            {
                HandleInputs();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(reticlePos, 0.5f);
        }
        #endregion



        #region Custom Methods
        protected virtual void HandleInputs()
        {
            Ray screenRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(screenRay, out hit))
            {
                reticlePos = hit.point;
                reticleNormal = hit.normal;
            }

            forwardInput = Input.GetAxis("Vertical");
            rotationInput = Input.GetAxis("Horizontal");

            fireInput = Input.GetAxis("Fire1");
        }
        #endregion
    }
}
