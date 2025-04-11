using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using FMODUnity;

public class AngerLevel : Level
{
    public GameObject tut1Panel;
    public GameObject tut2Panel;
    public GameObject tutBossPanel;
    public GameObject levelCompleteScreen;
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    [EventRef] public string levelTrack; // A reference to the FMOD event we want to use
    public GameObject firelanePrefab;
    private int enemyCount = 0;
    private int maxEnemyCount = 1;
    private int wave = -2;

    private Stage stage;
    private EnemyManager enemyManager;
    private EnemyRhythmManager enemyRhythmManager; 
    private float tut2time;

    public override void Load(Stage stg, EnemyManager eManager, EnemyRhythmManager erManager, EasyRhythmAudioManager aManager, PulsableManager pulsableManager)
    {
        stage = stg;
        enemyRhythmManager = erManager;
        enemyManager = eManager;
        enemyManager.onEnemyDeath += OnEnemyDeath;
        enemyRhythmManager.Reset();
        enemyRhythmManager.SetDifficulty(-1);
        wave = -2;
        enemyRhythmManager.SetWave(wave);
        tut1Panel.SetActive(false);
        tut2Panel.SetActive(false);
        tutBossPanel.SetActive(false);
        levelCompleteScreen.SetActive(false);
        Stage.Lanes.SetOffLimitLane(firelanePrefab);
        aManager.ChangeTrack(levelTrack);
        pulsableManager.Reset();
        Effects.SpecialEffects.ResetComboText();
        Invoke("SpawnWave", 5f);
    }

    void OnDisable()
    {
        if (enemyManager != null)
            enemyManager.onEnemyDeath -= OnEnemyDeath;
    }

    void SpawnWave()
    {
        enemyRhythmManager.SetWave(wave);
        if (wave == -2){
            enemyManager.enableEnemyMovement();
            tut2Panel.SetActive(true);
            SpawnTutorialWave(2); // arrows only, 1 direction
            
        }
        else if (wave == -1){
            // enemyRhythmManager.SetDifficulty(5);
            // SpawnBossWave();
            tut1Panel.SetActive(true); 
            enemyManager.disableEnemyMovement();
            SpawnTutorialWave(0); // arrows only, 2 directions
            
            // LevelComplete();
        }
        else if (wave == 0){
            tut2time = Time.time;
            
            enemyRhythmManager.SetDifficulty(2);
            SpawnTutorialWave(0); // projectiles only
        }
        // else if (wave == 1){
        //     enemyRhythmManager.SetDifficulty(5);
        //     SpawnBossWave();
        // }
        else if (wave % 5 == 0)
        {
            tutBossPanel.SetActive(true);
            SpawnBossWave();
            Invoke(nameof(DeactivateBossTutorialPanel), 5f);
        }
        else if (wave > 5)
            LevelComplete();
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
            if (wave == -1){
                tut2Panel.SetActive(false);  
            }
            if (wave == 0){
                tut1Panel.SetActive(false);
            }
            else if (wave == 1){
                
                enemyManager.enableEnemyMovement();
                // enemyRhythmManager.SetDifficulty(-1);
            }
            else if (wave == 2){
                enemyRhythmManager.SetDifficulty(3);
            }
            else if (wave == 4)
                enemyRhythmManager.SetDifficulty(4); // Set to normal difficulty (harder rhythms)
            // else if (wave >= 6)
            //     enemyRhythmManager.SetDifficulty(4); // Set to normal difficulty (harder rhythms)
            else if (wave == 5)
                enemyRhythmManager.SetDifficulty(5); // Set to difficulty 5 for boss
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
            prefab = enemyPrefab,
            position = Vector3.zero,
            rotation = Quaternion.identity,
            stage = stage,
            enemyLane = lane,
            isBoss = false
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
        GameObject playerObject = GameObject.FindWithTag("Player");
        PlayerMovement playerMovement = playerObject.GetComponent<PlayerMovement>();
        playerMovement.SetInLevelEndScreen(true);
        onLevelComplete?.Invoke();
        Destroy(gameObject);
    }

    private void DeactivateBossTutorialPanel()
    {
        tutBossPanel.SetActive(false);
    }
}
