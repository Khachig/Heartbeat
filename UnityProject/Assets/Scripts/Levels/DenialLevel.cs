using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using FMODUnity;

public class DenialLevel : Level
{
    public GameObject levelCompleteScreen;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject tut3Panel;
    [EventRef] public string levelTrack; // A reference to the FMOD event we want to use
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = -2;

    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, EasyRhythmAudioManager aManager, PulsableManager pulsableManager)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;
        wave = 1;
        enemyRhythmManager.Reset();
        enemyRhythmManager.SetDifficulty(2);
        enemyRhythmManager.SetWave(wave);
        enemyManager.enableEnemyMovement();
        levelCompleteScreen.SetActive(false);
        // aManager.ChangeTrack(levelTrack);
        pulsableManager.Reset();
        Invoke("SpawnWave", 5f);
    }

    void OnDisable()
    {
        if (enemyManager != null)
            enemyManager.onEnemyDeath -= OnEnemyDeath;
    }

    void SpawnWave()
    {
        if (wave == 1){
            tut3Panel.SetActive(true);
            ScoreManager.Instance.SetScoreLevel(1);
            // enemyRhythmManager.SetDifficulty(6);
            // SpawnBossWave();
            enemyRhythmManager.SetDifficulty(-1);
        }
        if (wave > 10)
            LevelComplete();
        else if (wave == 10){
            enemyRhythmManager.SetDifficulty(6);
            SpawnBossWave();}
        else
            SpawnRegWave();
    } 

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            
            wave++;
            if (wave == 2){
                // LevelComplete();
                //////
                tut3Panel.SetActive(false);
                enemyRhythmManager.SetDifficulty(2);

            }
            
            if (wave == 3){
                maxEnemyCount = 2;
                enemyRhythmManager.SetDifficulty(3);
            }
            
            
            if (wave == 5){
                maxEnemyCount = 3;
                enemyRhythmManager.SetDifficulty(4);
            }
            if (wave == 8){
                // 4 enemies
                maxEnemyCount = 4;
                enemyRhythmManager.SetDifficulty(5);
            }
            if (wave == 10)
                {enemyRhythmManager.SetDifficulty(6);}
            if (maxEnemyCount > 4)
                {maxEnemyCount = 4;}
            Invoke("SpawnWave", 3);
            
        }
    }

    void SpawnEnemy(int lane)
    {
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            prefab = enemyPrefab,
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = lane,
            isBoss = false
        });
        enemyRhythmManager.AddEnemy(enemy);
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
        GameObject enemy = enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
            prefab = bossPrefab,
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = 1,
            isBoss = true
        });
        enemyRhythmManager.AddEnemy(enemy);
        enemyRhythmManager.InitNewSequence();
    }

    void LevelComplete()
    {
        enemyManager.onEnemyDeath -= OnEnemyDeath;
        levelCompleteScreen.SetActive(true);
        onLevelComplete?.Invoke();
        Destroy(gameObject);
    }
}
