using UnityEngine;

public class DemoLevel : Level
{
    private int enemyCount = 0;
    private int maxEnemyCount = 2;

    private EnemyManager enemyManager;
    private Camera mainCamera;

    public override void Load(EnemyManager eManager, Camera mCamera)
    {
        enemyManager = eManager;
        mainCamera = mCamera;
        enemyManager.init();
        enemyManager.onEnemyDeath += OnEnemyDeath;

        Invoke("SpawnWave", 3);
    }

    void SpawnWave()
    {
        enemyCount = maxEnemyCount;
        float spawnRadius = 3f;
        int numLanes = 4;

        for (int i = 0; i < maxEnemyCount; i++)
        {
            Vector3 enemyPosition = GetPositionForLane(i, spawnRadius, numLanes);

            enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                camera = mainCamera,
                arrowArrangement = GetRandomArrowList(maxEnemyCount),
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
    Vector3 GetPositionForLane(int laneIndex, float tunnelRadius, int numLanes)
    {
        float angleStep = 360f / numLanes;
        float angle = angleStep * laneIndex * Mathf.Deg2Rad;
        Vector3 spawnCenter = mainCamera.transform.position + mainCamera.transform.forward * 6;
        Vector3 newposition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
        newposition = Vector3.ProjectOnPlane(newposition, mainCamera.transform.forward).normalized * tunnelRadius + spawnCenter;
        return newposition;
    }
}
