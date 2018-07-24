using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusManager : MonoBehaviour {

    // Enemy types: bandit, redSoldier, blackSoldier, swat, sniper, boss
    public string enemyType;

    // Enemy health
    private int enemyHealth = 1;

	// Set initial stats
	void Start ()
    {
		if(enemyType == "Bandit")
        {
            enemyHealth = 60;
        }
        if (enemyType == "RedSoldier")
        {
            enemyHealth = 105;
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
            enemyHealth = 80;
        }
        if (enemyType == "Boss")
        {
            enemyHealth = 3000;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
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
        // Drop pickups


        // Enemy explosion effect on death


        // temporary. disappear on death
        Destroy(gameObject);
    }
}
