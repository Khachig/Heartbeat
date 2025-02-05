using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManager", menuName = "Scriptable Objects/EnemyManager")]
public class EnemyManager : ScriptableObject
{
    [SerializeField] private GameObject enemyPrefab;

    public struct SpawnParameters
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    public void init()
    {
        Debug.Log("Scriptable Object Enemy Manager");
    }

    public void spawnEnemy(SpawnParameters parameters)
    {
        GameObject enemy = Instantiate(enemyPrefab, parameters.position, Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            parameters.rotation.x,
            enemy.transform.eulerAngles.y,
            parameters.rotation.z
        );
        EnemyBehaviour script = enemy.GetComponent<EnemyBehaviour>();
        script.init();
    }
}
