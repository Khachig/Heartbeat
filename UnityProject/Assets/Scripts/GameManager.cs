using UnityEngine;

public class GameManager : MonoBehaviour
{
    // TODO: include PlayerManager, LevelManager, whatever is needed etc.
    [SerializeField] private EnemyManager enemyManager;

    void Start()
    {
        enemyManager.init();
        // enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
        //     position = new Vector3(2, -1, 5),
        //     rotation = new Quaternion(0, 180, 0, 1),
        // });
        // enemyManager.spawnEnemy(new EnemyManager.SpawnParameters {
        //     position = new Vector3(-2, -1, 3),
        //     rotation = new Quaternion(0, 180, 0, 1),
        // });
    }
}
