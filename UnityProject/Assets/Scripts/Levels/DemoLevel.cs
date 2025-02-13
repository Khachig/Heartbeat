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
        float angleStep = 360f / maxEnemyCount;
        float spawnRadius = 2.5f;

        for (int i = 0; i < maxEnemyCount; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector3 enemyPosition = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), Random.Range(2,4)) * spawnRadius + mainCamera.transform.position;
            enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
                position = enemyPosition,
                rotation = new Quaternion(0, 180, 0, 1),
                camera = mainCamera,
                arrowArrangement = GetRandomArrowList(maxEnemyCount - 1),
            });
        }
    }

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            maxEnemyCount++;
            if (maxEnemyCount == 6) {
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
}
