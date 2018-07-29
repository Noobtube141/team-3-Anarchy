using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour {

    // Enemies the spawner will spawn
    public GameObject[] enemiesToSpawn;
    public int[] numberToSpawn;

    // Range from the EGO the enemies can spawn within
    public float spawnRange;
    
    // Spawn enemies and set their randomised spawn range
    public void SpawnEnemies()
    {
        int i = 0;

        foreach (int number in numberToSpawn)
        {
            for(int j = 0; j < numberToSpawn[i]; j++)
            {
                GameObject enemy = Instantiate(enemiesToSpawn[i], transform.position, transform.rotation);

                enemy.GetComponent<EnemyStatusManager>().isRandomlySpawned = true;
                enemy.GetComponent<EnemyStatusManager>().randomRadius = spawnRange;
            }

            i++;
        }
    }
}
