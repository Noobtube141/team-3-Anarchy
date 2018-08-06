using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {
    
    // Type of bullet. Initialiased upon instantiation. Types: Pistol, AR, Shotgun, Sniper, SMG, Bandit, Red, Black, EnemySniper, Boss
    public int type;

    // Damage value
    public int[] damage;

    // Speed of the bullet
    public float[] magnitude;

    // Lifetime of the bullet. Just in case
    public float[] lifetime;

    // Was the bullet fired by an enemy and can hurt players?
    public bool canHurtPlayer = false;

    // Add force and propel the bullet and set auto delete
    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward * magnitude[type]);

        Destroy(gameObject, lifetime[type]);
    }
    
    // Inflict damage on 'collision' and delete
    private void OnTriggerEnter(Collider other)
    {
        // Player bullet greatly hurts enemy weakness
        if (!canHurtPlayer && other.gameObject.tag == "Enemy Weakness")
        {
            other.gameObject.GetComponentInParent<EnemyStatusManager>().EnemyTakeDamage(Mathf.FloorToInt(damage[type] * 1.5f), type, -transform.forward);
        }
        // Player bullet hurts the enemy
        else if (!canHurtPlayer && other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<EnemyStatusManager>().EnemyTakeDamage(damage[type], type, -transform.forward);
        }
        // Enemy bullet hurts the player
        else if (canHurtPlayer && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerStatusManager>().TakeDamage(damage[type]);
        }

        if(other.tag != "Bullet")
        {
            Destroy(gameObject);
        }
    }
}
