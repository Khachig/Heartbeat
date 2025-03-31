using UnityEngine;

public class DemoLevel : Level
{
    public Stage stage;
    private int enemyCount = 0;
    private int maxEnemyCount = 2;

    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private string nextWave = "SpawnWave1";

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, EasyRhythmAudioManager aManager, PulsableManager pulsableManager)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;

        // Invoke("SpawnWave", 3);
        Invoke(nextWave, 3);
    }


    void SpawnWave1()
    {
            Vector3 enemyPosition = GetPositionForLane(0, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e1 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 0,
            });
            enemyPosition = GetPositionForLane(1, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e2 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 1,
            });
            nextWave = "SpawnWave2";
            enemyCount = 2;
            enemyRhythmManager.AddEnemy(e1);
            enemyRhythmManager.AddEnemy(e2);
            enemyRhythmManager.InitNewSequence();
    }
    void SpawnWave2()
    {
            Vector3 enemyPosition = GetPositionForLane(0, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e1 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 0,
            });
            enemyPosition = GetPositionForLane(1, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e2 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 1,
            });
            enemyPosition = GetPositionForLane(2, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e3 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 2,
            });
            nextWave = "SpawnWave3";
            enemyCount = 3;
            enemyRhythmManager.AddEnemy(e1);
            enemyRhythmManager.AddEnemy(e2);
            enemyRhythmManager.AddEnemy(e3);
            enemyRhythmManager.InitNewSequence();
    }

    void SpawnWave3()
    {
            Vector3 enemyPosition = GetPositionForLane(0, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e1 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 0,
            });
            enemyPosition = GetPositionForLane(1, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e2 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 1,
            });
            enemyPosition = GetPositionForLane(2, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e3 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 2,
            });
            enemyPosition = GetPositionForLane(3, stage.tunnelRadius, stage.numLanes, 20);
            GameObject e4 = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = 3,
            });
            nextWave = "Done";
            enemyCount = 4;
            enemyRhythmManager.AddEnemy(e1);
            enemyRhythmManager.AddEnemy(e2);
            enemyRhythmManager.AddEnemy(e3);
            enemyRhythmManager.AddEnemy(e4);
            enemyRhythmManager.InitNewSequence();
    }

    void SpawnWave()
    {
        enemyCount = maxEnemyCount;

        for (int i = 0; i < maxEnemyCount; i++)
        {
            Vector3 enemyPosition = GetPositionForLane(i, stage.tunnelRadius, stage.numLanes, 20);
            enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                stage = stage,
                enemyLane = i,
            });
        }
    }

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            maxEnemyCount++;
            if (nextWave == "Done") {
                onLevelComplete?.Invoke();
                Destroy(gameObject);
            }
            Invoke(nextWave, 3);
        }
    }

    Vector3 GetPositionForLane(int laneIndex, float tunnelRadius, int numLanes, float offset)
    {
        float angleStep = 360f / numLanes;
        float angle = angleStep * laneIndex * Mathf.Deg2Rad;

        Vector3 spawnCenter = stage.transform.position + stage.transform.forward * offset;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = Vector3.ProjectOnPlane(newposition, stage.transform.forward).normalized * tunnelRadius + spawnCenter;
        return newposition;
    }
}
