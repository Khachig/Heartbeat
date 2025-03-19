using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class EndlessPhasesLevel : Level
{
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = -2;

    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;
    private float fireRateMult = 1f;
    private GameObject tut1Panel;
    private GameObject tut2Panel;
    private float tut2time;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, GameObject tut1P, GameObject tut2P)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;
        wave = -2;
        tut1Panel = tut1P;
        tut2Panel = tut2P;

        Invoke("SpawnWave", 5f);
    }

    void SpawnWave()
    {
        if (wave == -2){
            SpawnTutorialWave(2); // arrows only, 1 direction
            tut1Panel.SetActive(true);
        }
        else if (wave == -1){
            SpawnTutorialWave(0); // arrows only, 2 directions
            tut1Panel.SetActive(false);   
        }
        else if (wave == 0){
            // enable movement
            tut2Panel.SetActive(true);
            tut2time = Time.time;
            enemyManager.enableEnemyMovement();
            SpawnTutorialWave(0); // projectiles only
        }
        else if (wave % 5 == 0)
            SpawnBossWave();
        else
            SpawnRegWave();
    } 
    void Update(){
        if (tut2time > 0 && Time.time - tut2time > 7.0f){
            tut2Panel.SetActive(false);   
        }
    }

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            if (wave < 1){
                tut1Panel.SetActive(false);
                tut2Panel.SetActive(false);
                enemyRhythmManager.SetDifficulty(-1);
                enemyRhythmManager.SetWave(wave);
            }
            else if (wave == 1){
                tut2Panel.SetActive(false);
                enemyRhythmManager.SetDifficulty(0);
            }
            else if (wave == 4)
                enemyRhythmManager.SetDifficulty(1); // Set to normal difficulty (harder rhythms)


            if (wave <= 3)
                maxEnemyCount = 1;
            else if (maxEnemyCount < 4)
                maxEnemyCount++;
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
    
    void SpawnTutorialWave(int index)
    {
        ArrowDirection[] arrowDirections = GetArrowList();
        SpawnEnemy(index, arrowDirections);
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
