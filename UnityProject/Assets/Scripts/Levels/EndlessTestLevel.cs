using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EndlessTestLevel : Level
{
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = 1;

    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private float fireRateMult = 1f;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;

        Invoke("SpawnWave", 5f);
    }

    void SpawnWave()
    {
        if (wave % 5 == 0)
            SpawnBossWave();
        else
            SpawnRegWave();
    } 

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            if (2 < wave && wave < 5)
                maxEnemyCount++;
            if (wave >= 5)
            {
                fireRateMult *= 0.75f;
                enemyManager.setFireRateMultForAllEnemies(fireRateMult);
            }
            Invoke("SpawnWave", 3);
        }
    }

    void SpawnEnemy(int lane, ArrowDirection[] arrowDirections)
    {
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            arrowArrangement = arrowDirections,
            enemyLane = lane,
        });
        enemyRhythmManager.AddEnemy(enemy);
    }

    ArrowDirection[] GetArrowList()
    {
        ArrowDirection[] newList = new ArrowDirection[maxEnemyCount];
        for (int i = 0; i < maxEnemyCount; i++)
        {
            newList[i] = ArrowDirection.RANDOM;
        }
        return newList;
    }
    
    void SpawnRegWave()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            ArrowDirection[] arrowDirections = GetArrowList();
            SpawnEnemy(i, arrowDirections);
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
            arrowArrangement = new ArrowDirection[] {ArrowDirection.RANDOM,
                                                     ArrowDirection.RANDOM,
                                                     ArrowDirection.RANDOM,
                                                     ArrowDirection.RANDOM},
            enemyLane = 1,
        });
        enemyRhythmManager.AddEnemy(enemy);
        enemyRhythmManager.InitNewSequence();
    }
}
