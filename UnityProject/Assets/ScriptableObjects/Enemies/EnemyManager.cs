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

    private List<GameObject> enemies;

    private EnemyMovement enemyMovement;
    private EasyRhythmAudioManager audioManager;
    private EnemyRhythmManager enemyRhythmManager;

    public struct SpawnParameters
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
        public Stage stage;
        public int enemyLane;
    }

    public void init(EnemyMovement eMovement, EasyRhythmAudioManager aManager, EnemyRhythmManager erManager)
    {
        enemyMovement = eMovement;
        audioManager = aManager;
        enemyRhythmManager = erManager;
        enemies = new List<GameObject>();
    }

    public GameObject spawnEnemy(SpawnParameters parameters)
    {
        GameObject enemy = spawnEnemyHelper(parameters, parameters.prefab); 
        enemies.Add(enemy);
        enemyMovement.addEnemy(parameters.enemyLane, enemy);
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

        InitEnemyData(enemy);
        InitEnemyBehaviour(enemy, audioManager, enemyRhythmManager);
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

    private void InitEnemyPulsable(GameObject enemy, EasyRhythmAudioManager audioManager)
    {
        // For enemy prefab, pulsable is in its first child: default
        Pulsable pulsable = enemy.transform.GetChild(0).GetComponent<Pulsable>();
        float bpm = audioManager.myAudioEvent.CurrentTempo / 2f;
        pulsable.Init(bpm, audioManager);
    }

    private void InitEnemyData(GameObject enemy)
    {
        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters());
    }

    void OnEnemyDestroy()
    {
        ScoreManager.Instance.AddScore(250);
        onEnemyDeath?.Invoke();
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
