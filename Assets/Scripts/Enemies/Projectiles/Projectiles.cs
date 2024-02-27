using System.Collections;
using System.Collections.Generic;
using Taink;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2.0f;
    [SerializeField] private int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var controller = collision.gameObject.GetComponent<Tank_Controller>();
            if (controller != null)
            {
                controller.TakeDamage(damage);
            }
            Destroy(gameObject);
            return;
        }
        if (collision.gameObject.tag == "Enemy")
        {
            var ranged = collision.gameObject.GetComponent<Tank_Ranged>();
            if (ranged != null)
            {
                ranged.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            var patrolBomb = collision.gameObject.GetComponent<Tank_Patrol_Bomb>();
            if (patrolBomb != null)
            {
                patrolBomb.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            var turret = collision.gameObject.GetComponent<Turret_Basic>();
            if (turret != null)
            {
                turret.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        Destroy(gameObject);
    }

}
