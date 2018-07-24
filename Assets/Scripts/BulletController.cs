﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    // Damage value
    public int damage = 1;

    // Speed of the bullet
    public float magnitude = 1000.0f;

    // Lifetime of the bullet. Just in case
    public float lifetime = 20.0f;

    // Was the bullet fired by an enemy and can hurt players?
    public bool canHurtPlayer = false;

    // Add force and propel the bullet and set auto delete
    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * magnitude);

        Destroy(gameObject, lifetime);
    }
    
    // Inflict damage on collision and delete
    private void OnCollisionEnter(Collision collision)
    {
        // Player bullet hurts the enemy
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyStatusManager>().EnemyTakeDamage(damage);
        }
        // Enemy bullet hurts the player
        else if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerStatusManager>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
