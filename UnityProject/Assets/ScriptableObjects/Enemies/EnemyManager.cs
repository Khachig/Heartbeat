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
    List<GameObject> enemies;

    public EnemyMovement enemyMovement;

    public struct SpawnParameters
    {
        public Vector3 position;
        public Quaternion rotation;
        public Stage stage;
        public ArrowDirection[] arrowArrangement;
        public int enemyLane;
    }

    public void init()
    {
        Debug.Log("Scriptable Object Enemy Manager");
        enemies = new List<GameObject>();
    }

    public void spawnEnemy(SpawnParameters parameters)
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
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;
        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters {
            arrowArrangement = parameters.arrowArrangement,
        });
        enemies.Add(enemy);
        enemyMovement.addEnemy(parameters.enemyLane, enemy);
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

    void OnEnemyDestroy()
    {
        onEnemyDeath.Invoke();
    }
}
