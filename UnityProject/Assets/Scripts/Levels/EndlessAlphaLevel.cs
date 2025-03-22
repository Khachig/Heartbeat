using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EndlessAlphaLevel : Level
{
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = 1;

    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, GameObject tut1P, GameObject tut2P)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;

        Invoke("SpawnWave", 5f);
    }

    void SpawnWave()
    {
        if (wave <= 3){
            SpawnTutorialWave(wave);
        }
        else if (wave < 10){
            SpawnEasyWave();
            }
        else if (wave % 10 == 0)
            SpawnBossWave();
        else
            SpawnHardWave();
    } 

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            if (wave == 3){
                maxEnemyCount = 2;
            }
            if (wave == 4){
                maxEnemyCount = 1;
            }
            if (5 < wave && wave < 10)
                maxEnemyCount++;
            Invoke("SpawnWave", 3);
        }
    }

    void SpawnEnemy(int lane)
    {
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = lane,
        });

        enemyRhythmManager.AddEnemy(enemy);
    }

    void SpawnTutorialWave(int index)
    {
        SpawnEnemy(index);
        
        enemyRhythmManager.InitNewSequence();
    }
    
    void SpawnEasyWave()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            SpawnEnemy(i);
            enemyCount++;
        }
        enemyRhythmManager.InitNewSequence();
    }

    void SpawnHardWave()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            SpawnEnemy(i);
            enemyCount++;
        }
        enemyRhythmManager.InitNewSequence();
    }

    void SpawnBossWave()
    {
        enemyCount++;
        GameObject enemy = enemyManager.spawnBoss(new EnemyManager.SpawnParameters {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = 1,
        });
        enemyRhythmManager.AddEnemy(enemy);
        enemyRhythmManager.InitNewSequence();
    }
}
