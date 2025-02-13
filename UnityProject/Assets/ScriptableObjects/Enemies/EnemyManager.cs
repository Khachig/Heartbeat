using UnityEngine;

[CreateAssetMenu(fileName = "EnemyManager", menuName = "Scriptable Objects/EnemyManager")]
public class EnemyManager : ScriptableObject
{
    public float spawnForwardOffset = 20f;
    [SerializeField] private GameObject enemyPrefab;

    public struct SpawnParameters
    {
        public Vector3 position;
        public Quaternion rotation;
        public Camera camera;
    }

    public void init()
    {
        Debug.Log("Scriptable Object Enemy Manager");
    }

    public void spawnEnemy(SpawnParameters parameters)
    {
        GameObject enemy = Instantiate(enemyPrefab, parameters.position + parameters.camera.transform.forward * spawnForwardOffset, Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            parameters.rotation.x,
            parameters.rotation.y,
            parameters.rotation.z
        );
        enemy.transform.parent = parameters.camera.transform;
        /* EnemyBehaviour script = enemy.GetComponent<EnemyBehaviour>(); */
        /* script.init(); */
    }
}
