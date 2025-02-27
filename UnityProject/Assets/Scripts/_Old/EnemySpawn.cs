using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    
    void Start()
    {
        Vector3 position = new Vector3(
            0f,
            0f,
            0f
        );

        /* enemyPrefab.SetActive(false); */
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        enemy.transform.eulerAngles = new Vector3(
            45f,
            enemy.transform.eulerAngles.y,
            45f
        );

        EnemyManager script = enemy.GetComponent<EnemyManager>();
        // script.init();

        /* enemyPrefab.SetActive(true); */
    }
}
