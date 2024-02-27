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
            collision.gameObject.GetComponent<Tank_Controller>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Tank_Ranged>().TakeDamage(damage);
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
