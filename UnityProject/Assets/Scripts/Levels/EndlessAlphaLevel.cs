using UnityEngine;

public class EndlessAlphaLevel : Level
{
    public Stage stage;
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = 1;

    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;

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
        enemyCount = maxEnemyCount;

        for (int i = 0; i < maxEnemyCount; i++)
        {
            SpawnEnemy(i);
        }
        enemyRhythmManager.InitNewSequence();
    }

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            if (maxEnemyCount < 4)
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
            arrowArrangement = GetArrowList(lane),
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
}
