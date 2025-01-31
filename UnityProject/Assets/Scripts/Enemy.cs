using UnityEngine;
using System.Collections.Generic;
using System;

public class Enemy : MonoBehaviour
{
    public bool[] enemyInUsed = new bool[4]; // 0: Enemy1, 1: Enemy2, 2: Enemy3, 3: Enemy4
    [SerializeField] private GameObject[] enemyPrefabs = new GameObject[4]; // 0: Enemy1, 1: Enemy2, 2: Enemy3, 3: Enemy4

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Enemy Start");
    }

    // Update is called once per frame
    void Update()
    {
        // for testing purpose, press the space key to spawn an enemy
        if (Input.GetKeyDown(KeyCode.Space)) { SpawnEnemy(); }

        // for testing purpose, press the enter key to kill an enemy
        if (Input.GetKeyDown(KeyCode.Return)) {
            List<int> avaliableIndex = new List<int>();
            for (int i = 0; i < enemyInUsed.Length; i++)
            {
                if (enemyInUsed[i])
                {
                    avaliableIndex.Add(i);
                }
            }

            if (avaliableIndex.Count != 0)
            {
                KillEnemy(avaliableIndex[UnityEngine.Random.Range(0, avaliableIndex.Count)]);
            }
        }
    }

    public void SpawnEnemy()
    {   
        // Randomly select an enemy to spawn from the available enemies
        List<int> avaliableIndex = new List<int>();
        for (int i = 0; i < enemyInUsed.Length; i++)
        {
            if (!enemyInUsed[i])
            {
                avaliableIndex.Add(i);
            }
        }

        if (avaliableIndex.Count == 0)
        {
            return;
        }

        int enemyIndex = avaliableIndex[UnityEngine.Random.Range(0, avaliableIndex.Count)];

        // update the enemy availability
        enemyInUsed[enemyIndex] = true;

        // display the enemy
        enemyPrefabs[enemyIndex].SetActive(true);
    }

    public void KillEnemy(int enemyIndex)
    {
        // check if the enemyIndex is valid
        if (enemyIndex < 0 || enemyIndex >= enemyInUsed.Length || !enemyInUsed[enemyIndex])
        {
            return;
        }

        // update the enemy availability
        enemyInUsed[enemyIndex] = false;

        // hide the enemy (disable the game object)
        enemyPrefabs[enemyIndex].SetActive(false);
    }
}
