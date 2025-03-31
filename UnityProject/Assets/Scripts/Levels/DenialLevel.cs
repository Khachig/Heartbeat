using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using FMODUnity;

public class DenialLevel : Level
{
    public GameObject levelCompleteScreen;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
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
        enemyRhythmManager.SetDifficulty(1);
        enemyRhythmManager.SetWave(wave);
        levelCompleteScreen.SetActive(false);
        aManager.ChangeTrack(levelTrack);
        pulsableManager.Reset();
        Invoke("SpawnWave", 5f);
    }

    void OnDisable()
    {
        enemyManager.onEnemyDeath -= OnEnemyDeath;
    }

    void SpawnWave()
    {
        if (wave > 5)
            LevelComplete();
        else if (wave % 5 == 0)
            SpawnBossWave();
        else
            SpawnRegWave();
    } 

    void OnEnemyDeath()
    {
        enemyCount--;
        if (enemyCount == 0) {
            wave++;
            maxEnemyCount++;
            if (maxEnemyCount > 4)
                maxEnemyCount = 4;
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
