using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EndlessAlphaLevel : Level
{
    public Stage stage;
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = 1;

    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private float fireRateMult = 1f;

    public override void Load(EnemyManager eManager)
    {
        if (!stage)
            stage = GameObject.Find("Stage").GetComponent<Stage>();
        enemyManager = eManager;
        enemyManager.init();
        enemyManager.onEnemyDeath += OnEnemyDeath;
        enemyRhythmManager = GameObject.Find("EnemyRhythmManager").GetComponent<EnemyRhythmManager>();

        Invoke("SpawnWave", 3);
    }

    void SpawnWave()
    {
        if (wave <= 3)
            SpawnEasyWave();
        else if (wave % 5 == 0)
            SpawnBossWave();
        else
            SpawnHardWave();
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
