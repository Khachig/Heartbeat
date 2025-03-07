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
        if (wave <= 3){
            SpawnTutorialWave(wave, 20/wave, 30);
        }
        else if (wave % 10 == 0)
            SpawnBossWave();
        else
            SpawnRegWave();
    } 

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            Debug.Log($"wave++ {wave}");
            if (wave == 3){
                maxEnemyCount = 2;
            }
            if (wave == 4){
                maxEnemyCount = 1;
            }
            else if (5 < wave && wave < 10)
                if (wave % 3 == 0){
                    maxEnemyCount++;
                }
                
            else if (wave >= 10)
            {
                fireRateMult *= 0.75f;
                enemyManager.setFireRateMultForAllEnemies(fireRateMult);
            }
            Invoke("SpawnWave", 3);
        }
    }

    void SpawnEnemy(int lane, ArrowDirection[] arrowDirections, float fireRate=0, float lastFireTime=0)
    {
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            arrowArrangement = arrowDirections,
            enemyLane = lane,
            fireRate = fireRate,
            lastFireTime = lastFireTime
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

    ArrowDirection[] GetArrowList(int lane)
    {
        ArrowDirection[] newList;
        if (wave == 1)
        {
            newList = new ArrowDirection[] {ArrowDirection.RANDOM};
        }
        else if (wave == 2)
        {
            newList = new ArrowDirection[] {ArrowDirection.UP, ArrowDirection.DOWN};
        }
        else{
            newList = new ArrowDirection[maxEnemyCount];
            for (int i = 0; i < maxEnemyCount; i++)
            {
                newList[i] = ArrowDirection.RANDOM;
            }
        }
        return newList;
    }
    
    void SpawnTutorialWave(int index, float fireRate = 15, float lastFireTime = 15)
    {
        ArrowDirection[] arrowDirections = GetArrowList(index);
        SpawnEnemy(index, arrowDirections, fireRate, lastFireTime);
        enemyCount++;
        enemyRhythmManager.InitNewSequence();
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
