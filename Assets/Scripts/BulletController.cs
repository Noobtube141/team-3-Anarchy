using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    // Damage value
    public int damage = 1;

    // Speed of the bullet
    public float magnitude = 1000.0f;

    // Lifetime of the bullet. Just in case
    private float lifetime = 20.0f;

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
        // check player versus enemy + inflict damage

        Destroy(gameObject);
    }
}
