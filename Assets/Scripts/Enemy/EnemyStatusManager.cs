using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStatusManager : MonoBehaviour {

    // Enemy types: Bandit, RedSoldier, BlackSoldier, SWAT, Sniper, Boss
    public string enemyType;

    // Script reference
    private MobileNavigator mobileAI;
    private StationarySeeker stationaryAI;

    // Enemy health
    public int enemyHealth = 1;

    // Random spawn range
    public bool isRandomlySpawned;
    public float randomRadius;

    // Pickups: health, armour, ammo
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
        else if (enemyType == "RedSoldier")
        {
            //enemyHealth = 105;
            enemyHealth = 1;
        }
        else if (enemyType == "BlackSoldier")
        {
            enemyHealth = 120;
        }
        else if (enemyType == "SWAT")
        {
            enemyHealth = 200;
        }
        else if (enemyType == "Sniper")
        {
            //enemyHealth = 80;
            enemyHealth = 1000;
        }
        else if (enemyType == "Boss")
        {
            enemyHealth = 3000;
        }

        if (enemyType == "Sniper" || enemyType == "Boss")
        {
            stationaryAI = gameObject.GetComponent<StationarySeeker>();
        }
        else
        {
            mobileAI = gameObject.GetComponent<MobileNavigator>();
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
    public void EnemyTakeDamage(int damage, int type, Vector3 hurtRotation)
    {
        enemyHealth -= damage;

        if(enemyHealth <= 0)
        {
            EnemyOnDeath(type);
        }
        else
        {
            if (enemyType == "Sniper" || enemyType == "Boss")
            {
                stationaryAI.state = "Hurt";
                stationaryAI.hurtRotation = hurtRotation;
            }
            else
            {
                mobileAI.state = "Hurt";
                mobileAI.hurtRotation = hurtRotation;
            }
        }
    }

    // On enemy death
    private void EnemyOnDeath(int type)
    {
        GameObject.Find("EGO Spawn Manager").GetComponent<SpawnManager>().CallCountEnemies();

        // Drop pickups if not stationary
        if(enemyType != "Sniper" || enemyType != "Boss")
        {
            CalculateEnemyDrop(type);
        }

        // Enemy explosion effect on death


        // temporary. disappear on death
        Destroy(gameObject);
    }

    // Calculate enemy drop type and value
    void CalculateEnemyDrop(int type)
    {
        PlayerStatusManager statusManager = GameObject.FindObjectOfType<PlayerStatusManager>();

        float distanceToPlayer = Vector3.Distance(transform.position, statusManager.gameObject.GetComponent<Transform>().position);

        float dropValue;

        if (distanceToPlayer <= 5)
        {
            dropValue = 1;
        }
        else if (distanceToPlayer >= 30)
        {
            dropValue = 0;
        }
        else
        {
            dropValue = (30 - distanceToPlayer) / 25;
        }
        
        if(dropValue > 0)
        {
            int dropChance = Random.Range(0, 1000);

            int healthDropChance = Mathf.Clamp(statusManager.playerHealth, 500, 1000);

            if (dropChance > healthDropChance)
            {
                GameObject newDrop = Instantiate(pickups[0], transform.position, transform.rotation);

                newDrop.GetComponent<PickupManager>().value = Mathf.FloorToInt(25 * dropValue);
            }
            else
            {
                dropChance = Random.Range(0, 1000);

                int armourDropChance = Mathf.Clamp(statusManager.playerArmour, 625, 1000);

                if (dropChance > armourDropChance)
                {
                    GameObject newDrop = Instantiate(pickups[1], transform.position, transform.rotation);

                    newDrop.GetComponent<PickupManager>().value = Mathf.FloorToInt(20 * dropValue);
                }
                else
                {
                    dropChance = Random.Range(0, 1000);

                    int[] ammoMax = { 30, 4, 4, 50 };

                    int bulletType = Mathf.Clamp(type, 1, 4);
                    
                    if (dropChance < 400)
                    {
                    }
                    else if (dropChance < 600)
                    {
                        bulletType += 1;

                        if(bulletType > 4)
                        {
                            bulletType -= 4;
                        }
                    }
                    else if (dropChance < 800)
                    {
                        bulletType += 2;

                        if (bulletType > 4)
                        {
                            bulletType -= 4;
                        }
                    }
                    else
                    {
                        bulletType += 3;

                        if (bulletType > 4)
                        {
                            bulletType -= 4;
                        }
                    }

                    GameObject newDrop = Instantiate(pickups[2], transform.position, transform.rotation);

                    newDrop.GetComponent<PickupManager>().ammoType = bulletType;

                    newDrop.GetComponent<PickupManager>().value = Mathf.FloorToInt(ammoMax[bulletType - 1] * dropValue);
                }
            }
        }
    }
}
