using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStatusManager : MonoBehaviour {

    // Enemy types: bandit, redSoldier, blackSoldier, swat, sniper, boss
    public string enemyType;

    // Enemy health
    public int enemyHealth = 1;

    // Random spawn range
    public bool isRandomlySpawned;
    public float randomRadius;

    // Pickups
    public GameObject[] pickups;

	// Set initial stats and spawn with specified randimised range
	void Start ()
    {
        if (isRandomlySpawned)
        {
            RandomiseSpawn();
        }

        if (enemyType == "Bandit")
        {
            enemyHealth = 60;
        }
        if (enemyType == "RedSoldier")
        {
            //enemyHealth = 105;
            enemyHealth = 1;
        }
        if (enemyType == "BlackSoldier")
        {
            enemyHealth = 120;
        }
        if (enemyType == "SWAT")
        {
            enemyHealth = 200;
        }
        if (enemyType == "Sniper")
        {
            //enemyHealth = 80;
            enemyHealth = 1;
        }
        if (enemyType == "Boss")
        {
            enemyHealth = 3000;
        }
    }
	
    void RandomiseSpawn()
    {
        Vector3 centre = transform.position;

        for (int i = 0; i < 15; i++)
        {
            Vector3 rand = centre + Random.insideUnitSphere * randomRadius;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(rand, out hit, 1.0f, NavMesh.AllAreas))
            {
                transform.position = rand;
            }
        }
    }

    // Called by player damage sources. Receive damage and detect death
    public void EnemyTakeDamage(int damage)
    {
        enemyHealth -= damage;

        if(enemyHealth <= 0)
        {
            EnemyOnDeath();
        }
    }

    // On enemy death
    private void EnemyOnDeath()
    {
        GameObject.Find("EGO Spawn Manager").GetComponent<SpawnManager>().CallCountEnemies();

        // Drop pickups
        Instantiate(pickups[Random.Range(0, 3)], transform.position, transform.rotation);

        // Enemy explosion effect on death


        // temporary. disappear on death
        Destroy(gameObject);
    }
}
