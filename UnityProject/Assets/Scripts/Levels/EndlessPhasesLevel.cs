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
    private GameObject tut1Panel;
    private GameObject tut2Panel;
    private float tut2time;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, GameObject tut1P, GameObject tut2P)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;
        enemyRhythmManager.SetDifficulty(-1);
        wave = -2;
        tut1Panel = tut1P;
        tut2Panel = tut2P;
        Invoke("SpawnWave", 5f);
    }

    void OnDisable()
    {
        enemyManager.onEnemyDeath -= OnEnemyDeath;
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
                enemyRhythmManager.SetDifficulty(-1);
            }
            else if (wave == 2){
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

    void SpawnEnemy(int lane)
    {
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = lane,
        });
        enemyRhythmManager.AddEnemy(enemy);
    }

    void SpawnTutorialWave(int index)
    {
        SpawnEnemy(index);
        enemyCount++;
        enemyRhythmManager.InitNewSequence();
    }
    
    void SpawnRegWave()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            SpawnEnemy(i);
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
            enemyLane = 1,
        });
        enemyRhythmManager.AddEnemy(enemy);
        enemyRhythmManager.InitNewSequence();
    }
}
