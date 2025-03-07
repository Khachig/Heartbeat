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
            if (wave >= 5)
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
        else if (wave == 3)
        {
            if (lane == 0)
                newList = new ArrowDirection[] {ArrowDirection.UP};
            if (lane == 1)
                newList = new ArrowDirection[] {ArrowDirection.DOWN, ArrowDirection.DOWN};
            else
                newList = new ArrowDirection[] {ArrowDirection.LEFT, ArrowDirection.RIGHT};
        }
        else
        {
            int r = Random.Range(0, 2);
            if (r == 0)
                newList = new ArrowDirection[] {ArrowDirection.RANDOM};
            else
                newList = new ArrowDirection[] {ArrowDirection.RANDOM, ArrowDirection.RANDOM};
        }
        return newList;
    }

    void SpawnTutorialWave(int index, float fireRate = 15, float lastFireTime = 15)
    {
        ArrowDirection[] arrowDirections = GetArrowList(index);
        SpawnEnemy(index, arrowDirections, fireRate, lastFireTime);
        
        enemyRhythmManager.InitNewSequence();
    }
    
    void SpawnEasyWave()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            ArrowDirection[] arrowDirections = GetArrowList(i);
            SpawnEnemy(i, arrowDirections);
            enemyCount++;
        }
        enemyRhythmManager.InitNewSequence();
    }

    void SpawnHardWave()
    {
        List<ArrowDirection[]> possibleArrangements = new List<ArrowDirection[]>
            {
                new ArrowDirection[] {ArrowDirection.UP},
                new ArrowDirection[] {ArrowDirection.DOWN},
                new ArrowDirection[] {ArrowDirection.LEFT},
                new ArrowDirection[] {ArrowDirection.RIGHT},
                new ArrowDirection[] {ArrowDirection.UP, ArrowDirection.UP},
                new ArrowDirection[] {ArrowDirection.DOWN, ArrowDirection.DOWN},
                new ArrowDirection[] {ArrowDirection.LEFT, ArrowDirection.LEFT},
                new ArrowDirection[] {ArrowDirection.RIGHT, ArrowDirection.RIGHT},
                new ArrowDirection[] {ArrowDirection.UP, ArrowDirection.DOWN},
                new ArrowDirection[] {ArrowDirection.LEFT, ArrowDirection.RIGHT},
                new ArrowDirection[] {ArrowDirection.LEFT, ArrowDirection.DOWN},
                new ArrowDirection[] {ArrowDirection.UP, ArrowDirection.RIGHT},
            };

        for (int i = 0; i < maxEnemyCount; i++)
        {
            if (possibleArrangements.Count == 0)
                break;

            int idx = Random.Range(0, possibleArrangements.Count);
            ArrowDirection[] chosenDirections = possibleArrangements[idx];

            foreach (ArrowDirection[] arrangement in possibleArrangements.ToList())
            {
                foreach (ArrowDirection direction in chosenDirections)
                {
                    if (arrangement.Contains(direction))
                    {
                        possibleArrangements.Remove(arrangement);
                        break;
                    }
                }

            }
            SpawnEnemy(i, chosenDirections);
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
