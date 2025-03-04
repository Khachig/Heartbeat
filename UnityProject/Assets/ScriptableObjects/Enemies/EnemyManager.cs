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
    public float spawnForwardOffset = 20f;
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
        GameObject enemy = Instantiate(
            enemyPrefab,
            parameters.position + parameters.stage.transform.forward * spawnForwardOffset,
            Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            parameters.rotation.x,
            parameters.rotation.y,
            parameters.rotation.z
        );
        enemy.transform.parent = parameters.stage.transform;

        EnemyBehaviour enemyBehaviour = enemy.GetComponent<EnemyBehaviour>();
        enemyBehaviour.Init(parameters.stage, audioManager, enemyRhythmManager);
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;

        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters {
            arrowArrangement = parameters.arrowArrangement,
        });

        enemies.Add(enemy);
        enemyMovement.addEnemy(parameters.enemyLane, enemy);
        return enemy;
    }
    
    public GameObject spawnBoss(SpawnParameters parameters)
    {
        GameObject enemy = Instantiate(
            bossPrefab,
            parameters.position + parameters.stage.transform.forward * spawnForwardOffset,
            Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            parameters.rotation.x,
            parameters.rotation.y,
            parameters.rotation.z
        );
        enemy.transform.parent = parameters.stage.transform;

        BossBehaviour enemyBehaviour = enemy.GetComponent<BossBehaviour>();
        enemyBehaviour.Init(parameters.stage, audioManager, enemyRhythmManager);
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;

        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters {
            arrowArrangement = parameters.arrowArrangement,
        });

        enemies.Add(enemy);
        return enemy;
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
            enemyBehaviour.SetFireRateMultiplier(mult);
        }
    }

    void OnEnemyDestroy()
    {
        onEnemyDeath.Invoke();
    }
}
