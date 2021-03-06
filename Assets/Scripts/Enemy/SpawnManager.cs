﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    // Array holding all EGO Spawners
    public GameObject[] spawners;

    // Number of enemies
    public int enemyCount;

    // Lower bound of enemyCount to initiate a new wave
    public int lowerEnemyBound;
    
    // Tracks the wave number
    public int waveNumber = 0;

    // The number of waves until completion
    public int waveMax;
    
    // Start the coroutine. Called by enemies on death
    public void CallCountEnemies()
    {
        StartCoroutine(CountEnemies());
    }

    // Count the number of enemies
    public IEnumerator CountEnemies()
    {
        yield return new WaitForSeconds(0.1f);
        
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        
        if (enemyCount <= lowerEnemyBound)
        {
            print(enemyCount);

            if (GameObject.FindGameObjectWithTag("Boss") == null)
            {
                waveNumber++;
            }

            if (waveNumber <= waveMax)
            {
                foreach (GameObject spawner in spawners)
                {
                    spawner.GetComponent<SpawnerController>().SpawnEnemies();
                }
            }
            else
            {
                if (enemyCount == 0 && waveNumber > waveMax)
                {
                    GameObject.FindGameObjectWithTag("Player").GetComponent<CombatController>().EnableExiting();

                    GameObject.FindGameObjectWithTag("Music Player").SendMessage("CrossFade", "EnemiesEliminated");
                }
            }
        }
    }
}
