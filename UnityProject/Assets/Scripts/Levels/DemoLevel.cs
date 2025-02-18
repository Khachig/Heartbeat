using UnityEngine;

public class DemoLevel : Level
{
    public Stage stage;
    private int enemyCount = 0;
    private int maxEnemyCount = 2;

    private EnemyManager enemyManager;

    public override void Load(EnemyManager eManager)
    {
        if (!stage)
            stage = GameObject.Find("Stage").GetComponent<Stage>();
        enemyManager = eManager;
        enemyManager.init();
        enemyManager.onEnemyDeath += OnEnemyDeath;

        Invoke("SpawnWave", 3);
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
                arrowArrangement = GetRandomArrowList(maxEnemyCount),
                enemyLane = i
            });
        }
    }

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            maxEnemyCount++;
            if (maxEnemyCount == 5) {
                onLevelComplete?.Invoke();
                Destroy(gameObject);
            }
            Invoke("SpawnWave", 3);
        }
    }

    ArrowDirection[] GetRandomArrowList(int numArrows)
    {
        ArrowDirection[] newList = new ArrowDirection[numArrows];
        for (int i = 0; i < numArrows; i++)
            newList[i] = ArrowDirection.RANDOM;
        return newList;
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
