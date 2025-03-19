using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "EnemyManager", menuName = "Scriptable Objects/EnemyManager")]
public class EnemyManager : ScriptableObject
{
    public delegate void OnEnemyDeath();
    // To be invoked whenever a single enemy is destroyed/killed
    public OnEnemyDeath onEnemyDeath;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject bossPrefab;

    private List<GameObject> enemies;

    private EnemyMovement enemyMovement;
    private EasyRhythmAudioManager audioManager;
    private EnemyRhythmManager enemyRhythmManager;

    public struct SpawnParameters
    {
        public Vector3 position;
        public Quaternion rotation;
        public Stage stage;
        public ArrowDirection[] arrowArrangement;
        public int enemyLane;
        public float fireRate;
        public float lastFireTime;
    }

    public void init(EnemyMovement eMovement, EasyRhythmAudioManager aManager, EnemyRhythmManager erManager)
    {
        Debug.Log("Scriptable Object Enemy Manager");
        enemyMovement = eMovement;
        audioManager = aManager;
        enemyRhythmManager = erManager;
        enemies = new List<GameObject>();
    }

    public GameObject spawnEnemy(SpawnParameters parameters)
    {
        GameObject enemy = spawnEnemyHelper(parameters, enemyPrefab); 
        enemies.Add(enemy);
        enemyMovement.addEnemy(parameters.enemyLane, enemy);
        return enemy;
    }
    
    public GameObject spawnBoss(SpawnParameters parameters)
    {
        GameObject enemy = spawnEnemyHelper(parameters, bossPrefab);
        enemies.Add(enemy);
        return enemy;
    }

    private GameObject spawnEnemyHelper(SpawnParameters parameters, GameObject prefab)
    {
        GameObject enemy = Instantiate(
            prefab,
            parameters.position,
            Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            parameters.rotation.x,
            parameters.rotation.y,
            parameters.rotation.z
        );
        enemy.transform.parent = parameters.stage.transform;

        InitEnemyData(enemy, parameters.arrowArrangement);
        InitEnemyBehaviour(enemy, audioManager, enemyRhythmManager, parameters.fireRate);
        InitEnemyPulsable(enemy, audioManager);

        return enemy;
    }

    private void InitEnemyBehaviour(GameObject enemy,
                                    EasyRhythmAudioManager audioManager,
                                    EnemyRhythmManager enemyRhythmManager)
    {
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.Init(audioManager, enemyRhythmManager);
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;
    }

    private void InitEnemyBehaviour(GameObject enemy,
                                    EasyRhythmAudioManager audioManager,
                                    EnemyRhythmManager enemyRhythmManager,
                                    float fireRate)
    {
        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.Init(audioManager, enemyRhythmManager, fireRate);
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;
    }

    private void InitEnemyPulsable(GameObject enemy, EasyRhythmAudioManager audioManager)
    {
        // For enemy prefab, pulsable is in its first child: default
        Pulsable pulsable = enemy.transform.GetChild(0).GetComponent<Pulsable>();
        float bpm = audioManager.myAudioEvent.CurrentTempo / 2f;
        pulsable.Init(bpm, audioManager);
    }

    private void InitEnemyData(GameObject enemy, ArrowDirection[] arrowDirections)
    {
        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters {
            arrowArrangement = arrowDirections,
        });
    }

    // Returns whether any enemy was hit by the player attack
    public bool handlePlayerAttack(Vector2 input)
    {
        bool match = false;
        foreach(GameObject enemy in enemies.ToList())
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
                continue;
            }

            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour.IsDead())
                continue;

            match |= enemyBehaviour.HandlePlayerAttack(input);
        }
        return match;
    }

    public void setFireRateMultForAllEnemies(float mult)
    {
        foreach(GameObject enemy in enemies.ToList())
        {
            if (enemy == null)
            {
                enemies.Remove(enemy);
                continue;
            }

            EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
            if (enemyBehaviour.IsDead())
                continue;

            enemyBehaviour.SetFireRateMultiplier(mult);
        }
    }

    void OnEnemyDestroy()
    {
        ScoreManager.Instance.AddScore(250);
        onEnemyDeath.Invoke();
    }

    public void disableEnemyMovement()
    {
        enemyMovement.disableEnemyMovement();
    }
    
    public void enableEnemyMovement()
    {
        enemyMovement.enableEnemyMovement();
    }
}
