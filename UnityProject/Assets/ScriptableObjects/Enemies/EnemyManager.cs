using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManager", menuName = "Scriptable Objects/EnemyManager")]
public class EnemyManager : ScriptableObject
{
    public delegate void OnEnemyDeath();
    public OnEnemyDeath onEnemyDeath;
    public float spawnForwardOffset = 20f;
    [SerializeField] private GameObject enemyPrefab;

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
        enemyBehaviour.enemyLane = parameters.enemyLane;
        enemyBehaviour.onEnemyDestroy += OnEnemyDestroy;
        EnemyData enemyData = enemy.GetComponent<EnemyData>();
        enemyData.init(new EnemyData.EnemyParameters {
            arrowArrangement = parameters.arrowArrangement,
        });
    }

    void OnEnemyDestroy()
    {
        onEnemyDeath.Invoke();
    }
}
