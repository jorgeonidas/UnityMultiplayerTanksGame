using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Networking;

namespace Project.Player
{
    public class PlayerManager : MonoBehaviour
    {
        const float BARREL_PIVO_OFFSET = 90.0f;

        [Header("Data")]
        [SerializeField]
        private float speed = 2f;
        [SerializeField]
        private float rotation = 60f;
        [Header("object references")]
        [SerializeField]
        private Transform barrelPivot;

        private float lastRotation;

        [Header("class references")]
        [SerializeField]
        private NetworkIdentity networkIdentity;

        // Update is called once per frame
        void Update()
        {
            if (networkIdentity.IsControlling())
            {
                CheckMovement();
                CheckAiming();
            }
        }

        public float GetLastRotation()
        {
            return lastRotation;
        }

        public void SetBarrelRotation(float value)
        {
            barrelPivot.rotation = Quaternion.Euler(0, 0, value + BARREL_PIVO_OFFSET);
        }

        private void CheckMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            //con el eje vertical acelero y detrocedo
            transform.position += -transform.up* vertical * speed * Time.deltaTime;
            //con el eje horizontal roto el cuerpo del tanque
            transform.Rotate(new Vector3(0,0,-horizontal * rotation * Time.deltaTime));
        }

        private void CheckAiming()
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 diference = mousePosition - transform.position;
            diference.Normalize();
            float rotation = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;

            lastRotation = rotation;

            barrelPivot.rotation = Quaternion.Euler(0, 0, rotation + BARREL_PIVO_OFFSET);
        }
    }
}

